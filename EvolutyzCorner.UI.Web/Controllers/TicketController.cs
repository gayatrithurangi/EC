using evolCorner.Models;
using Evolutyz.Data;
using Evolutyz.Entities;
using EvolutyzCorner.UI.Web.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EvolutyzCorner.UI.Web.Controllers
{
    public class TicketController : Controller
    {
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        SqlConnection Conn = new SqlConnection();
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        // GET: Ticket
        public ActionResult Index(int Tid)
        {
            ViewBag.Tid = Tid;
            return View();
        }



        public JsonResult ImageUpload(ProductViewModel model)
        {

            var file = model.ImageFile;
            string UniqueFileName = null;

            if (file != null)
            {

                UniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                var fileName = Path.GetFileName(file.FileName);
                var extention = Path.GetExtension(file.FileName);
                var filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                //file.SaveAs(Server.MapPath("/TicketUploadedImage/" + file.FileName));
                file.SaveAs(Server.MapPath("/TicketUploadedImage/" + UniqueFileName));

            }

            // return Json(file.FileName, JsonRequestBehavior.AllowGet);
            return Json(UniqueFileName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartment()
        {

            var DepObj = db.Departments.Where(x => x.Status == true).Select(D => new
            {
                D.Did,
                D.DepartmentName,
            }).ToList();
            return Json(DepObj);
        }

        public JsonResult IssueType(int Did)
        {

            var DepObj = db.Issuetypes.Where(x => x.Status == true && x.DepartmentID == Did).Select(D => new
            {
                D.ITID,
                D.Description,
            }).ToList();
            return Json(DepObj);
        }
        public JsonResult GetPriority()
        {

            var DepObj = db.TicketPriorities.Where(x => x.Status == true).Select(D => new
            {
                D.TPID,
                D.Description,
            }).ToList();
            return Json(DepObj);
        }


        public string createTicket(ProductViewModel entity, FormCollection form)
        {
            try
            {
                string strresponse = "";
                UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = Convert.ToInt32(objUserSession.UserId);
                var serializer = new JavaScriptSerializer();
                var gr = form["TImages"];
                var grs = serializer.Deserialize<List<TImages>>(gr);
                entity.TImages = grs;

                string host = System.Web.HttpContext.Current.Request.Url.Host;
                string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
                string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

                string UrlEmailAddress = string.Empty;
                var ccactualuserid = db.Users.Where(x => x.Usr_UserID == _userID).FirstOrDefault();

               

                if (host == "localhost")
                {
                    UrlEmailAddress = "http://" + host + ":" + port;
                }
                else
                {
                    UrlEmailAddress = "http://" + port1;
                }
                var emailcontent = "";


                if (entity.Tid == 0)
                {
                    db.Set<Ticket>().Add(new Ticket
                    {
                        DepartmentID = entity.departmentId,
                        Description = entity.TicketDescription,
                        TypeOfIssue = entity.IssueType,
                        Ticket_raise_date = DateTime.Now,
                        Ticket_Forecast_date = entity.forecastdate,
                        Priority = entity.priority,
                        CreatedDate = DateTime.Now,
                        CreatedBy = _userID,
                    });
                    db.SaveChanges();
                    var ticketid = db.Tickets.Where(x => x.DepartmentID == entity.departmentId && x.Description == entity.TicketDescription && x.TypeOfIssue == entity.IssueType && x.CreatedBy == _userID).Select(x => x.TID).FirstOrDefault();
                    foreach (var data in entity.TImages)
                    {
                        string s = data.Imagename;
                        string[] s1 = s.Split('/');

                        db.Set<TicketImage>().Add(new TicketImage
                        {
                            ImageName = s1[2],
                            TicketId = ticketid,
                        });
                        db.SaveChanges();
                    }


              




                    int? Departmentid = db.Tickets.Where(x => x.TID == ticketid).Select(x => x.DepartmentID).FirstOrDefault();

                  
                        var tomailid = (from a in db.TicketsAuthorities
                                        join b in db.Users on a.UserId equals b.Usr_UserID
                                        where a.DepartmentID == Departmentid
                                        select new
                                        {
                                            b.Usr_LoginId,
                                        }).ToList();

                       

                      



                    string issuetype = db.Issuetypes.Where(x => x.ITID == entity.IssueType).Select(x=> x.Description).FirstOrDefault();

                    var k = "";
                    var j = "";
                    var z = "";

                    string xys = UrlEmailAddress + "//TicketUploadedImage//";

                
                    if (entity.TImages.Count > 0)
                    {
                        foreach (var data in entity.TImages)
                        {
                            string s = data.Imagename;
                            z = "<li style='display: inline-block;margin-bottom: 5px;'>" +
                                   "<img src='" + UrlEmailAddress + "//TicketUploadedImage//" + s + "' style='display: inline-block;height: 150px;width: 200px;'> " +
                            "</li> ";
                        }
                    }

                    emailcontent = "<html>" +
                        "<head>" +
                        "<meta charset='UTF-8'>" +
                        "<title>Comments-gallery</title>" +
                        "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
                        "<meta content='width=device-width' name='viewport'>" +
                        "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
                        "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
                        "</head>" +
                        "<body style='background-color:#f4f4f5;'>" +
                        "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td style='text-align:center;'>" +
                        "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size: 100%; max-width:680px;'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td>" +
                        "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:95px; padding-right:95px;'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td style='padding-top:24px;'>" +
                        "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Ticket Raised</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Ticket Raised By : <b>" + ccactualuserid.Usr_Username + "</b></td>" +
                        "</tr>" +

                         "<tr>" +
                        "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Description : <b>" + entity.TicketDescription + "</b></td>" +
                        "</tr>" +

                          "<tr>" +
                        "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Issue Type : <b>" + issuetype + "</b></td>" +
                        "</tr>" +

                            "<tr>" +
                        "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> comments : <b>" + entity.comments + "</b></td>" +
                        "</tr>" +




                        "<tr>" +
                        "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + k + "" + j + "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td>" +
                        "<ul style='list-style: none; margin: 0; padding: 0;'>" + z + "</ul> " +
                        "</td> " +
                        "</tr> " +
                        "</tbody>" +
                        "</table>" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td>" +
                        "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:95px; padding-right:95px;'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
                        "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
                        "<tbody>" +
                        "<tr>" +
                        "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
                        "</td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td style='height:72px;'>" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "</td>" +
                        "</tr>" +
                        "</tbody>" +
                        "</table>" +
                        "</body>" +
                        "</html>";

                    var cc1 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == _userID && a.Is_L1_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();

                    var cc2 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == _userID && a.Is_L2_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();


                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        From = new EmailAddress("noreply@evolutyz.com"),
                        Subject = "Ticket Raised",
                        HtmlContent = emailcontent

                    };
                    foreach (var data in tomailid)
                    {
                        msgs.AddTo(new EmailAddress(data.Usr_LoginId));
                    }
                    msgs.AddCc(ccactualuserid.Usr_LoginId);
                    msgs.AddCc(ccactualuserid.Usr_LoginId);
                    if(cc1 != null)
                    msgs.AddCc(cc1.Usr_LoginId);
                    if(cc2 != null)
                    msgs.AddCc(cc2.Usr_LoginId);
                    var responses = client.SendEmailAsync(msgs);
                    strresponse = "1";
                }
                else
                {
                    string xys = UrlEmailAddress + "//TicketUploadedImage//";

                    var k = "";
                    var j = "";
                    var z = "";

                    (from p in db.Tickets
                     where p.TID == entity.Tid
                     select p).ToList().ForEach(x =>
                     {
                         x.Ticket_Forecast_date = entity.forecastdate;
                         x.Priority = entity.priority;
                         x.ModifiedDate = DateTime.Now;
                         x.ModifiedBy = _userID;
                         db.SaveChanges();
                     });



                    foreach (var data in entity.TImages)
                    {
                        string si = data.Imagename;
                        string[] s1 = si.Split('/');
                        var imagename = s1[2];

                        var isimage = db.TicketImages.Where(x => x.TicketId == entity.Tid && x.ImageName == imagename).Count();
                        if (isimage == 0)
                        {
                            db.Set<TicketImage>().Add(new TicketImage
                            {
                                ImageName = s1[2],
                                TicketId = entity.Tid,
                                CreatedDate = DateTime.Now,
                                CreatedBy = _userID,
                            });
                            db.SaveChanges();

                       
                                string s = s1[2];
                                z = "<li style='display: inline-block;margin-bottom: 5px;'>" +
                                       "<img src='" + UrlEmailAddress + "//TicketUploadedImage//" + s + "' style='display: inline-block;height: 150px;width: 200px;'> " +
                                "</li> ";
                    

                        }
                    }

                    bool isadmin = false;
                    var data1 = db.TicketsAuthorities.Where(x => x.UserId == _userID && x.DepartmentID == entity.departmentId).Count();
                    if(data1 > 0)
                    {
                        isadmin = true;
                    }

                    if (isadmin == true)
                    {
                        if (entity.forecastdate.ToString() != "" || entity.forecastdate != null)
                        {
                            k = "<b style='display: block;'>" + entity.forecastdate + "</b>";
                        }
                        if (entity.priority.ToString() != "")
                        {
                            j = "<b style='display: block;'>" + entity.priority + "</b> ";
                        }
                    }

                    if (k != "" || j != "" || z != "")
                    {

                        emailcontent = "<html>" +
                            "<head>" +
                            "<meta charset='UTF-8'>" +
                            "<title>Comments-gallery</title>" +
                            "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
                            "<meta content='width=device-width' name='viewport'>" +
                            "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
                            "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
                            "</head>" +
                            "<body style='background-color:#f4f4f5;'>" +
                            "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td style='text-align:center;'>" +
                            "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size: 100%; max-width:680px;'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td>" +
                            "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:95px; padding-right:95px;'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td style='padding-top:24px;'>" +
                            "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
                            "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Ticket Updated</ td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> updated by : <b>" + ccactualuserid.Usr_Username + "</b></td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + k + "" + j + "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td>" +
                            "<ul style='list-style: none; margin: 0; padding: 0;'>" + z + "</ul> " +
                            "</td> " +
                            "</tr> " +
                            "</tbody>" +
                            "</table>" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td>" +
                            "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:95px; padding-right:95px;'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
                            "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
                            "<tbody>" +
                            "<tr>" +
                            "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
                            "</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style='height:72px;'>" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "</td>" +
                            "</tr>" +
                            "</tbody>" +
                            "</table>" +
                            "</body>" +
                            "</html>";


                        var client = new SendGridClient("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");

                        var msgs = new SendGridMessage()
                        {
                            From = new EmailAddress("noreply@evolutyz.com"),
                            Subject = "Ticket Updated",
                            HtmlContent = emailcontent

                        };
                     
                        if(isadmin == true)
                        {
                            var Departmentid = db.Tickets.Where(x => x.TID == entity.Tid).FirstOrDefault();
                            var tomailid = db.Users.Where(x => x.Usr_UserID == Departmentid.CreatedBy).ToList().FirstOrDefault();

                            msgs.AddTo(new EmailAddress(tomailid.Usr_LoginId));

                            
                            var ccmailid = (from a in db.TicketsAuthorities
                                            join b in db.Users on a.UserId equals b.Usr_UserID
                                            where a.DepartmentID == Departmentid.DepartmentID
                                            select new
                                            {
                                                b.Usr_LoginId,
                                            }).ToList();
                            foreach (var data2 in ccmailid)
                            {
                                msgs.AddCc(data2.Usr_LoginId);
                            }


                            var cc1 = (from a in db.UserProjects
                                       join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                                       where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L1_Manager == true
                                       select new
                                       {
                                           b.Usr_LoginId,
                                       }).ToList().FirstOrDefault();

                            var cc2 = (from a in db.UserProjects
                                       join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                                       where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L2_Manager == true
                                       select new
                                       {
                                           b.Usr_LoginId,
                                       }).ToList().FirstOrDefault();


                            if (cc1 != null)
                                msgs.AddCc(cc1.Usr_LoginId);
                            if (cc2 != null)
                                msgs.AddCc(cc2.Usr_LoginId);

                        }
                        if(isadmin == false)
                        {
                            var Departmentid = db.Tickets.Where(x => x.TID == entity.Tid).FirstOrDefault();
                            var tomailid = db.Users.Where(x => x.Usr_UserID == Departmentid.CreatedBy).ToList().FirstOrDefault();

                           


                            var ccmailid = (from a in db.TicketsAuthorities
                                            join b in db.Users on a.UserId equals b.Usr_UserID
                                            where a.DepartmentID == Departmentid.DepartmentID
                                            select new
                                            {
                                                b.Usr_LoginId,
                                            }).ToList();
                            foreach (var data2 in ccmailid)
                            {
                                msgs.AddTo(new EmailAddress(data2.Usr_LoginId));
                               
                            }
                            msgs.AddCc(tomailid.Usr_LoginId);

                            var cc1 = (from a in db.UserProjects
                                       join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                                       where a.UProj_UserID == _userID && a.Is_L1_Manager == true
                                       select new
                                       {
                                           b.Usr_LoginId,
                                       }).ToList().FirstOrDefault();

                            var cc2 = (from a in db.UserProjects
                                       join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                                       where a.UProj_UserID == _userID && a.Is_L2_Manager == true
                                       select new
                                       {
                                           b.Usr_LoginId,
                                       }).ToList().FirstOrDefault();
                            if (cc1 != null)
                                msgs.AddCc(cc1.Usr_LoginId);
                            if (cc2 != null)
                                msgs.AddCc(cc2.Usr_LoginId);

                        }

                       
                        
                       

                        var responses = client.SendEmailAsync(msgs);
                    }
                    strresponse = "2";
                }


               
                return strresponse;


            }
            catch (Exception)
            {
                throw;
            }

        }


        public ActionResult GetllTickets()
        {
            return View();
        }

        public JsonResult GetTickets()
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                    int _userID = Convert.ToInt32(objUserSession.UserId);

                    var Role = db.TicketsAuthorities.Where(x => x.UserId == _userID).Count();
                    int? Depid = 0;
                    if (Role > 0)
                    {
                        Depid = db.TicketsAuthorities.Where(x => x.UserId == _userID).Select(x => x.DepartmentID).FirstOrDefault();
                    }



                    List<GetTickets> GetTickets = new List<GetTickets>();

                    Conn = new SqlConnection(str);
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand("GETALLTickets", Conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Userid", _userID);
                    cmd.Parameters.AddWithValue("@Depid", Depid);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        GetTickets.Add(new GetTickets
                        {
                            TID = Convert.ToInt32(dr["TID"]),
                            DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString() == "" ? 0 : dr["DepartmentID"]),
                            DepartmentName = dr["DepartmentName"].ToString(),
                            Description = dr["Description"].ToString(),
                            TypeOfIssue = Convert.ToInt32(dr["TypeOfIssue"].ToString() == "" ? 0 : dr["TypeOfIssue"]),
                            TypeOfIssueDescription = dr["TypeOfIssueDescription"].ToString(),
                            Ticket_raise_date = dr["Ticket_raise_date"].ToString(),
                            Ticket_Forecast_date = dr["Ticket_Forecast_date"].ToString(),
                            Ticket_Closed_date = dr["Ticket_Closed_date"].ToString(),
                            closedby = Convert.ToInt32(dr["ClosedBy"].ToString() == "" ? 0 : dr["ClosedBy"]),
                            closedbyName = dr["closedbyName"].ToString(),
                            Priority = Convert.ToInt32(dr["Priority"]),
                            Prioritydescription = dr["Prioritydescription"].ToString(),



                        });


                    }




                    return Json(GetTickets, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }



        }

        DataTable comment = new DataTable();
        DataTable image = new DataTable();
        public JsonResult GetTicketsByID(int Tid)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                    int _userID = Convert.ToInt32(objUserSession.UserId);
                    var department = db.Tickets.Where(x => x.TID == Tid).Select(x => x.DepartmentID).FirstOrDefault();
                    var ticketauth = db.TicketsAuthorities.Where(x => x.DepartmentID == department && x.UserId == _userID).Count();


                    List<GetTickets> GetTickets = new List<GetTickets>();
                    List<Comments> Comments = new List<Comments>();
                    List<TicketImage> ticketImage = new List<TicketImage>();
                    Conn = new SqlConnection(str);
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand("GetTicketsByID", Conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Ticketid", Tid);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);
                    comment = ds.Tables[1];
                    image = ds.Tables[2];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        GetTickets.Add(new GetTickets
                        {
                            TID = Convert.ToInt32(dr["TID"]),
                            DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString() == "" ? 0 : dr["DepartmentID"]),
                            DepartmentName = dr["DepartmentName"].ToString(),
                            Description = dr["Description"].ToString(),
                            TypeOfIssue = Convert.ToInt32(dr["TypeOfIssue"].ToString() == "" ? 0 : dr["TypeOfIssue"]),
                            TypeOfIssueDescription = dr["TypeOfIssueDescription"].ToString(),
                            Ticket_raise_date = dr["Ticket_raise_date"].ToString(),
                            Ticket_Forecast_date = dr["Ticket_Forecast_date"].ToString(),
                            Ticket_Closed_date = dr["Ticket_Closed_date"].ToString(),
                            closedby = Convert.ToInt32(dr["ClosedBy"].ToString() == "" ? 0 : dr["ClosedBy"]),
                            closedbyName = dr["closedbyName"].ToString(),
                            Priority = Convert.ToInt32(dr["Priority"]),
                            Prioritydescription = dr["Prioritydescription"].ToString(),
                            comments = GetAllCooments(),
                            ticketImages = GetAllImages(),
                            IsTicketsAuthority = ticketauth > 0 ? true : false,

                        });


                    }




                    return Json(GetTickets, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }


        }


        public List<Comments> GetAllCooments()
        {
            try
            {
                UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = Convert.ToInt32(objUserSession.UserId);
                List<Comments> comment1 = new List<Comments>();
                DataView dv = new DataView(comment);
                System.Data.DataTable _comm = dv.ToTable();
                foreach (DataRow dr in _comm.Rows)
                {
                    comment1.Add(new Comments
                    {
                        Commentid = Convert.ToInt32(dr["commentId"]),
                        Userid = Convert.ToInt32(dr["Userid"]),
                        UserName = dr["Usr_Username"].ToString(),
                        CommentName = dr["Description"].ToString(),
                        Ticketid = Convert.ToInt32(dr["TicketId"]),
                        Active = Convert.ToInt32(dr["Userid"]) == _userID ? true : false,
                    });
                }
                return comment1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TicketImages> GetAllImages()
        {
            try
            {
                List<TicketImages> TicketImages1 = new List<TicketImages>();
                DataView dv = new DataView(image);
                System.Data.DataTable _image = dv.ToTable();
                foreach (DataRow dr in _image.Rows)
                {
                    TicketImages1.Add(new TicketImages
                    {
                        Imageid = Convert.ToInt32(dr["TIID"]),
                        ImageName = dr["ImageName"].ToString(),
                        Ticketid = Convert.ToInt32(dr["TicketId"]),

                    });
                }
                return TicketImages1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Closeissue(int Tid)
        {
            try
            {
                UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = Convert.ToInt32(objUserSession.UserId);
                (from p in db.Tickets
                 where p.TID == Tid
                 select p).ToList().ForEach(x =>
                 {
                     x.Ticket_Closed_date = DateTime.Now;
                     x.ModifiedDate = DateTime.Now;
                     x.ModifiedBy = _userID;
                     db.SaveChanges();
                 });

                string host = System.Web.HttpContext.Current.Request.Url.Host;
                string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
                string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

                string UrlEmailAddress = string.Empty;

                if (host == "localhost")
                {
                    UrlEmailAddress = "http://" + host + ":" + port;
                }
                else
                {
                    UrlEmailAddress = "http://" + port1;
                }
                var emailcontent = "";


                string comment = db.comments.Where(x => x.Userid == _userID).Select(x => x.Description).FirstOrDefault();
                string username = db.Users.Where(x => x.Usr_UserID == _userID).Select(x => x.Usr_Username).FirstOrDefault();

                emailcontent = "<html>" +
                    "<head>" +
                    "<meta charset='UTF-8'>" +
                    "<title>Ticket Closed</title>" +
                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
                    "<meta content='width=device-width' name='viewport'>" +
                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
                    "</head>" +
                    "<body style='background-color:#f4f4f5;'>" +
                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='text-align:center;'>" +
                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size: 100%; max-width:680px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td>" +
                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:120px; padding-right:120px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='padding-top:24px;'>" +
                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Ticket Closed By : <b>" + username + "</b></td>" +
                    "</tr>" +
                  

                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td>" +
                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:120px; padding-right:120px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='height:72px;'>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</body>" +
                    "</html>";


                var client = new SendGridClient("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");

                var msgs = new SendGridMessage()
                {
                    From = new EmailAddress("noreply@evolutyz.com"),
                    Subject = "Ticket Closed",
                    HtmlContent = emailcontent

                };


                var ticketdata = db.Tickets.Where(x => x.TID == Tid).FirstOrDefault();
                bool isadmin = false;
                var data1 = db.TicketsAuthorities.Where(x => x.UserId == _userID && x.DepartmentID == ticketdata.DepartmentID).Count();
                if (data1 > 0)
                {
                    isadmin = true;
                }


                if (isadmin == true)
                {
                    var Departmentid = db.Tickets.Where(x => x.TID == Tid).FirstOrDefault();
                    var tomailid = db.Users.Where(x => x.Usr_UserID == Departmentid.CreatedBy).ToList().FirstOrDefault();

                    msgs.AddTo(new EmailAddress(tomailid.Usr_LoginId));


                    var ccmailid = (from a in db.TicketsAuthorities
                                    join b in db.Users on a.UserId equals b.Usr_UserID
                                    where a.DepartmentID == Departmentid.DepartmentID
                                    select new
                                    {
                                        b.Usr_LoginId,
                                    }).ToList();
                    foreach (var data2 in ccmailid)
                    {
                        msgs.AddCc(data2.Usr_LoginId);
                    }


                    var cc1 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L1_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();

                    var cc2 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L2_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();


                    if (cc1 != null)
                        msgs.AddCc(cc1.Usr_LoginId);
                    if (cc2 != null)
                        msgs.AddCc(cc2.Usr_LoginId);

                }


                var responses = client.SendEmailAsync(msgs);






                return "1";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string chatsendmsg(int Tid, string chat)
        {
            try
            {
                UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = Convert.ToInt32(objUserSession.UserId);
                db.Set<comment>().Add(new comment
                {
                    Userid = _userID,
                    Description = chat,
                    TicketId = Tid,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _userID,

                });
                db.SaveChanges();

                string host = System.Web.HttpContext.Current.Request.Url.Host;
                string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
                string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

                string UrlEmailAddress = string.Empty;

                if (host == "localhost")
                {
                    UrlEmailAddress = "http://" + host + ":" + port;
                }
                else
                {
                    UrlEmailAddress = "http://" + port1;
                }
                var emailcontent = "";



                string comment = db.comments.Where(x => x.Userid == _userID).Select(x => x.Description).FirstOrDefault();
                string username = db.Users.Where(x => x.Usr_UserID == _userID).Select(x => x.Usr_Username).FirstOrDefault();

                emailcontent = "<html>" +
                    "<head>" +
                    "<meta charset='UTF-8'>" +
                    "<title>Comments</title>" +
                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
                    "<meta content='width=device-width' name='viewport'>" +
                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
                    "</head>" +
                    "<body style='background-color:#f4f4f5;'>" +
                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='text-align:center;'>" +
                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size: 100%; max-width:680px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td>" +
                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:120px; padding-right:120px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='padding-top:24px;'>" +
                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Comment added by : <b>" + username + "</b></td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + comment + "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td>" +
                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:120px; padding-right:120px;'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td style='height:72px;'>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</body>" +
                    "</html>";


                var client = new SendGridClient("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");

                var msgs = new SendGridMessage()
                {
                    From = new EmailAddress("noreply@evolutyz.com"),
                    Subject = "Comment Added for Ticket",
                    HtmlContent = emailcontent

                };


                var ticketdata = db.Tickets.Where(x => x.TID == Tid).FirstOrDefault();
                bool isadmin = false;
                var data1 = db.TicketsAuthorities.Where(x => x.UserId == _userID && x.DepartmentID == ticketdata.DepartmentID).Count();
                if (data1 > 0)
                {
                    isadmin = true;
                }


                if (isadmin == true)
                {
                    var Departmentid = db.Tickets.Where(x => x.TID == Tid).FirstOrDefault();
                    var tomailid = db.Users.Where(x => x.Usr_UserID == Departmentid.CreatedBy).ToList().FirstOrDefault();

                    msgs.AddTo(new EmailAddress(tomailid.Usr_LoginId));


                    var ccmailid = (from a in db.TicketsAuthorities
                                    join b in db.Users on a.UserId equals b.Usr_UserID
                                    where a.DepartmentID == Departmentid.DepartmentID
                                    select new
                                    {
                                        b.Usr_LoginId,
                                    }).ToList();
                    foreach (var data2 in ccmailid)
                    {
                        msgs.AddCc(data2.Usr_LoginId);
                    }


                    var cc1 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L1_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();

                    var cc2 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L2_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();


                    if (cc1 != null)
                        msgs.AddCc(cc1.Usr_LoginId);
                    if (cc2 != null)
                        msgs.AddCc(cc2.Usr_LoginId);

                }
                if (isadmin == false)
                {
                    var Departmentid = db.Tickets.Where(x => x.TID == Tid).FirstOrDefault();
                    var tomailid = db.Users.Where(x => x.Usr_UserID == Departmentid.CreatedBy).ToList().FirstOrDefault();




                    var ccmailid = (from a in db.TicketsAuthorities
                                    join b in db.Users on a.UserId equals b.Usr_UserID
                                    where a.DepartmentID == Departmentid.DepartmentID
                                    select new
                                    {
                                        b.Usr_LoginId,
                                    }).ToList();
                    foreach (var data2 in ccmailid)
                    {
                        msgs.AddTo(new EmailAddress(data2.Usr_LoginId));

                    }
                    msgs.AddCc(tomailid.Usr_LoginId);

                    var cc1 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == _userID && a.Is_L1_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();

                    var cc2 = (from a in db.UserProjects
                               join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
                               where a.UProj_UserID == _userID && a.Is_L2_Manager == true
                               select new
                               {
                                   b.Usr_LoginId,
                               }).ToList().FirstOrDefault();
                    if (cc1 != null)
                        msgs.AddCc(cc1.Usr_LoginId);
                    if (cc2 != null)
                        msgs.AddCc(cc2.Usr_LoginId);

                }

                var responses = client.SendEmailAsync(msgs);
                return "1";
            }
            catch (Exception)
            {
                throw;
            }
        }



        //public string SendMailsforTickets(int Ticketid, int Userid, bool isfirsttimeticketrised, bool isupdatedfromuser, bool isUpdatedfromAdmin, bool iscommentaddedbyuser, bool iscommentaddedbyadmin)
        //{
        //    try
        //    {

        //        string host = System.Web.HttpContext.Current.Request.Url.Host;
        //        string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
        //        string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

        //        string UrlEmailAddress = string.Empty;

        //        if (host == "localhost")
        //        {
        //            UrlEmailAddress = "http://" + host + ":" + port;
        //        }
        //        else
        //        {
        //            UrlEmailAddress = "http://" + port1;
        //        }
        //        var emailcontent = "";




        //        int? Departmentid = db.Tickets.Where(x => x.TID == Ticketid).Select(x => x.DepartmentID).FirstOrDefault();

        //        int TicketsAuthorities = db.TicketsAuthorities.Where(x => x.DepartmentID == Departmentid && x.UserId == Userid).Count();
        //        if (TicketsAuthorities == 0)
        //        {
        //            var tomailid = (from a in db.TicketsAuthorities
        //                            join b in db.Users on a.UserId equals b.Usr_UserID
        //                            where a.DepartmentID == Departmentid
        //                            select new
        //                            {
        //                                b.Usr_LoginId,
        //                            }).ToList();
        //            var ccactualuserid = db.Users.Where(x => x.Usr_UserID == Userid).Select(x => x.Usr_LoginId).FirstOrDefault();

        //            var cc1 = (from a in db.UserProjects
        //                       join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
        //                       where a.UProj_UserID == Userid && a.Is_L1_Manager == true
        //                       select new
        //                       {
        //                           b.Usr_LoginId,
        //                       }).ToList();

        //            var cc2 = (from a in db.UserProjects
        //                       join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
        //                       where a.UProj_UserID == Userid && a.Is_L2_Manager == true
        //                       select new
        //                       {
        //                           b.Usr_LoginId,
        //                       }).ToList();

        //            if (isfirsttimeticketrised == true)
        //            {

        //            }
        //            else if (iscommentaddedbyuser == true)
        //            {

        //                string comment = db.comments.Where(x => x.Userid == Userid).Select(x => x.Description).FirstOrDefault();
        //                string username = db.Users.Where(x => x.Usr_UserID == Userid).Select(x => x.Usr_Username).FirstOrDefault();
        //                //http://evolutyz.in/img/hero-alt-long2.jpg min-height:100vh; height:100%;
        //                //https://www.evolutyz.com/wp-content/themes/evolutyz/img/logo.png
        //                //height:100%;
        //                emailcontent = "<html>" +
        //                    "<head>" +
        //                    "<meta charset='UTF-8'>" +
        //                    "<title>Comments</title>" +
        //                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
        //                    "<meta content='width=device-width' name='viewport'>" +
        //                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
        //                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
        //                    "</head>" +
        //                    "<body style='background-color:#f4f4f5;'>" +
        //                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='text-align:center;'>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/hero-alt-long2.jpg); background-position:80% top; background-repeat: no-repeat; background-size: cover; max-width:680px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:120px; padding-right:120px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:24px;'>" +
        //                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Comment added by : <b>" + username + "</b></td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + comment + "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:120px; padding-right:120px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
        //                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='height:72px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</body>" +
        //                    "</html>";
        //            }
        //            else if (isupdatedfromuser == true)
        //            {

        //                string username = db.Users.Where(x => x.Usr_UserID == Userid).Select(x => x.Usr_Username).FirstOrDefault();

        //                var images = db.TicketImages.Where(x => x.TIID == Ticketid).ToList();
        //                var TicketPrioritie = (from a in db.Tickets
        //                                       join b in db.TicketPriorities on a.Priority equals b.TPID
        //                                       where a.TID == Ticketid
        //                                       select new
        //                                       {
        //                                           b.Description
        //                                       }).FirstOrDefault();
        //                var Ticket_Forecast_date = db.Tickets.Where(x => x.TID == Ticketid).Select(x => x.Ticket_Forecast_date).FirstOrDefault();

        //                var k = "";
        //                var j = "";
        //                var z = "";

        //                string xys = UrlEmailAddress + "//TicketUploadedImage//";

        //                if (Ticket_Forecast_date.ToString() != "" || Ticket_Forecast_date != null)
        //                {
        //                    k = "<b style='display: block;'>" + Ticket_Forecast_date + "</b>";
        //                }
        //                if (TicketPrioritie.Description.ToString() != "")
        //                {
        //                    j = "<b style='display: block;'>" + TicketPrioritie.Description + "</b> ";
        //                }
        //                if (images.Count > 0)
        //                {
        //                    foreach (var data in images)
        //                    {
        //                        string s = data.ImageName;
        //                        z = "<li style='display: inline-block;margin-bottom: 5px;'>" +
        //                               "<img src='" + UrlEmailAddress + "//TicketUploadedImage//" + s + "' style='display: inline-block;height: 150px;width: 200px;'> " +
        //                        "</li> ";
        //                    }
        //                }

        //                emailcontent = "<html>" +
        //                    "<head>" +
        //                    "<meta charset='UTF-8'>" +
        //                    "<title>Comments-gallery</title>" +
        //                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
        //                    "<meta content='width=device-width' name='viewport'>" +
        //                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
        //                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
        //                    "</head>" +
        //                    "<body style='background-color:#f4f4f5;'>" +
        //                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='text-align:center;'>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:80% top; background-repeat: no-repeat; background-size: cover; max-width:680px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:95px; padding-right:95px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:24px;'>" +
        //                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> updated by : <b>" + username + "</b></td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + k + "" + j + "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<ul style='list-style: none; margin: 0; padding: 0;'>" + z + "</ul> " +
        //                    "</td> " +
        //                    "</tr> " +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:95px; padding-right:95px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
        //                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='height:72px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</body>" +
        //                    "</html>";
        //            }

        //        }
        //        else if (TicketsAuthorities > 0)
        //        {
        //            var tomailid = (from a in db.Tickets
        //                            join b in db.Users on a.CreatedBy equals b.Usr_UserID
        //                            where a.TID == Ticketid
        //                            select new
        //                            {
        //                                b.Usr_LoginId,
        //                                b.Usr_UserID,
        //                            }).ToList().FirstOrDefault();
        //            var ccactualuserid = (from a in db.TicketsAuthorities
        //                                  join b in db.Users on a.UserId equals b.Usr_UserID
        //                                  where a.DepartmentID == Departmentid
        //                                  select new
        //                                  {
        //                                      b.Usr_LoginId,
        //                                  }).ToList();

        //            var cc1 = (from a in db.UserProjects
        //                       join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
        //                       where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L1_Manager == true
        //                       select new
        //                       {
        //                           b.Usr_LoginId,
        //                       }).ToList();

        //            var cc2 = (from a in db.UserProjects
        //                       join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
        //                       where a.UProj_UserID == tomailid.Usr_UserID && a.Is_L2_Manager == true
        //                       select new
        //                       {
        //                           b.Usr_LoginId,
        //                       }).ToList();

        //            if (isUpdatedfromAdmin == true)
        //            {
        //                var images = db.TicketImages.Where(x => x.TIID == Ticketid).ToList();
        //                var TicketPrioritie = (from a in db.Tickets
        //                                       join b in db.TicketPriorities on a.Priority equals b.TPID
        //                                       where a.TID == Ticketid
        //                                       select new
        //                                       {
        //                                           b.Description
        //                                       }).FirstOrDefault();
        //                var Ticket_Forecast_date = db.Tickets.Where(x => x.TID == Ticketid).Select(x => x.Ticket_Forecast_date).FirstOrDefault();
        //                var k = "";
        //                var j = "";
        //                var z = "";
        //                string username = db.Users.Where(x => x.Usr_UserID == Userid).Select(x => x.Usr_Username).FirstOrDefault();


        //                if (Ticket_Forecast_date.ToString() != "" || Ticket_Forecast_date != null)
        //                {
        //                    k = "<b style='display: block;'>" + Ticket_Forecast_date + "</b>";
        //                }
        //                if (TicketPrioritie.Description != "")
        //                {
        //                    j = "<b style='display: block;'>" + TicketPrioritie.Description + "</b> ";
        //                }
        //                if (images.Count > 0)
        //                {
        //                    foreach (var data in images)
        //                    {
        //                        string s = data.ImageName;
        //                        z = "<li style='display: inline-block;margin-bottom: 5px;'>" +
        //                               "<img src='E:\\EvolutyzCornerNew\\EvolutyzCorner.UI.Web\\TicketUploadedImage\\" + s + "' style='display: inline-block;height: 150px;width: 200px;'> " +
        //                        "</li> ";
        //                    }
        //                }

        //                emailcontent = "<html>" +
        //                    "<head>" +
        //                    "<meta charset='UTF-8'>" +
        //                    "<title>Comments-gallery</title>" +
        //                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
        //                    "<meta content='width=device-width' name='viewport'>" +
        //                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
        //                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
        //                    "</head>" +
        //                    "<body style='background-color:#f4f4f5;'>" +
        //                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='text-align:center;'>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:80% top; background-repeat: no-repeat; background-size: cover; max-width:680px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:95px; padding-right:95px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:24px;'>" +
        //                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> updated by : <b>" + username + "</b></td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + k + "" + j + "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<ul style='list-style: none; margin: 0; padding: 0;'>" + z + "</ul> " +
        //                    "</td> " +
        //                    "</tr> " +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:95px; padding-right:95px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
        //                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='height:72px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</body>" +
        //                    "</html>";
        //            }
        //            else if (iscommentaddedbyadmin == true)
        //            {
        //                string comment = db.comments.Where(x => x.Userid == Userid).Select(x => x.Description).ToList().FirstOrDefault();
        //                string username = db.Users.Where(x => x.Usr_UserID == Userid).Select(x => x.Usr_Username).FirstOrDefault();
        //                //http://evolutyz.in/img/hero-alt-long2.jpg min-height:100vh; height:100%;
        //                //https://www.evolutyz.com/wp-content/themes/evolutyz/img/logo.png
        //                //height:100%;
        //                emailcontent = "<html>" +
        //                    "<head>" +
        //                    "<meta charset='UTF-8'>" +
        //                    "<title>Comments</title>" +
        //                    "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
        //                    "<meta content='width=device-width' name='viewport'>" +
        //                    "<style type='text/css'>@font-face{font-family:&#x27;Postmates Std&#x27;; font-weight:600; font-style:normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:500; font-style:normal; src:local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family:&#x27;Postmates Std&#x27;; font-weight:400; font-style:normal; src:local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); }</style>" +
        //                    "<style media='screen and (max-width: 680px)'> @media screen and (max-width:680px) { .page-center { padding-left:0 !important; padding-right:0 !important; } .footer-center { padding-left:20px !important; padding-right:20px !important; } }</style>" +
        //                    "</head>" +
        //                    "<body style='background-color:#f4f4f5;'>" +
        //                    "<table cellpadding='0' cellspacing='0' style='width:100%; height:100%; background-color:#f4f4f5; text-align:center;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='text-align:center;'>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%;  padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:80% top; background-repeat: no-repeat; background-size: cover; max-width:680px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:120px; padding-right:120px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:24px;'>" +
        //                    "<img src='https://ez-evolutyzcornerweb.azurewebsites.net/LoginCssNew/images/evolutyzcorplogo.png' style='width:100px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Comments</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style='width:100%; height:1px; max-height:1px; background-color: #d9dbe0; opacity: 0.81'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Comment added by : <b>" + username + "</b></td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' >" + comment + "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td>" +
        //                    "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:120px; padding-right:120px;'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style='padding-top:50px;padding-bottom:15px;width:100%;color: #f8c26c;font-size:40px;'> Evolutyz Corner</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td colspan = '2' style= 'padding-top:24px; padding-bottom:48px;' >" +
        //                    "<table cellpadding= '0' cellspacing= '0' style= 'width:100%'>" +
        //                    "<tbody>" +
        //                    "<tr>" +
        //                    "<td style= 'width:100%; height:1px; max-height:1px; background-color: #EAECF2; opacity: 0.19' >" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style= '-ms-text-size-adjust:100%; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size:15px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing: 0; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:helpdesk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight:500; color: #ffffff' target='_blank'>Help Center.</a>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "<tr>" +
        //                    "<td style='height:72px;'>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</td>" +
        //                    "</tr>" +
        //                    "</tbody>" +
        //                    "</table>" +
        //                    "</body>" +
        //                    "</html>";
        //            }

        //        }



        //        var tomailid = (from a in db.TicketsAuthorities
        //                        join b in db.Users on a.UserId equals b.Usr_UserID
        //                        where a.DepartmentID == Departmentid
        //                        select new
        //                        {
        //                            b.Usr_LoginId,
        //                        }).ToList();



        //        var cc1 = (from a in db.UserProjects
        //                   join b in db.Users on a.UProj_L1_ManagerId equals b.Usr_UserID
        //                   where a.UProj_UserID == _userID && a.Is_L1_Manager == true
        //                   select new
        //                   {
        //                       b.Usr_LoginId,
        //                   }).ToList().FirstOrDefault();

        //        var cc2 = (from a in db.UserProjects
        //                   join b in db.Users on a.UProj_L2_ManagerId equals b.Usr_UserID
        //                   where a.UProj_UserID == _userID && a.Is_L2_Manager == true
        //                   select new
        //                   {
        //                       b.Usr_LoginId,
        //                   }).ToList().FirstOrDefault();



        //        var client = new SendGridClient("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");

        //        var msgs = new SendGridMessage()
        //        {
        //            From = new EmailAddress("noreply@evolutyz.com"),
        //            Subject = "Request to reset password",
        //            HtmlContent = emailcontent

        //        };
        //        msgs.AddTo(new EmailAddress("ganga.kurada@evolutyz.com"));
        //        msgs.AddCc("kuradabhavani@gmail.com");

        //        var responses = client.SendEmailAsync(msgs);
        //        return null;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        public ActionResult GetllPositions(int pid)
        {
            try
            {
                TempData["pid"] = pid;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public JsonResult GEtInsertUpdatePositions(int IPID)
        {

            try
            {
                List<Position_TechnologyStack_List> li = new List<Position_TechnologyStack_List>();
                List<Position_TechnologyStack_List> position_TechnologyStack_List = new List<Position_TechnologyStack_List>();
                AssessmentForPositions AFP = new AssessmentForPositions();
                EvolutyzCornerDataEntities db1 = new EvolutyzCornerDataEntities();
                if (IPID == 0)
                {

                    AFP.InterviewForPositionId = 0;
                    AFP.InterviewForPositionname = "";
                    AFP.Description = "";
                    AFP.Position_TechnologyStack_List = (from a in db1.TechnologyStacks
                                                         select new Position_TechnologyStack_List
                                                         {
                                                             Technologyid = a.TID,
                                                             Title = a.Title,
                                                             Ischecked = false,
                                                             NoofQuestions = 0,
                                                             Timeinseconds = 0,
                                                         }).ToList();



                }
                else if (IPID > 0)
                {
                    var Positionid = db.Assessment_For_Position.Where(x => x.APID == IPID).FirstOrDefault();
                    AFP.InterviewForPositionId = Positionid.APID;
                    AFP.InterviewForPositionname = Positionid.InterviewForPositionname;
                    AFP.Description = Positionid.Description;

                    Conn = new SqlConnection(str);
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand("Assessmenttechnology", Conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Positionid", IPID);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        position_TechnologyStack_List.Add(new Position_TechnologyStack_List
                        {

                            Technologyid = Convert.ToInt32(dr["TID"]),
                            Title = dr["Title"].ToString(),
                            Ischecked = Convert.ToInt32(dr["ischecked"]) == 1 ? true : false,

                            NoofQuestions = Convert.ToInt32(dr["No_of_Questions"]),
                            Timeinseconds = Convert.ToInt32(dr["Assessment_TimePeriod_in_sec"]),

                        });
                    }
                    AFP.Position_TechnologyStack_List = position_TechnologyStack_List;

                }
                return Json(AFP, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public ActionResult GetllAssessment()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public JsonResult GetAllPosition()
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    List<GetallPositions> GetallPositions = new List<GetallPositions>();
                    GetallPositions = (from a in db.Assessment_For_Position
                                       select new GetallPositions
                                       {
                                           APID = a.APID,
                                           InterviewForPositionname = a.InterviewForPositionname,
                                           Description = a.Description,
                                       }).ToList();

                    return Json(GetallPositions, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }



        }

        public string AssessmentForPositions(AssessmentForPositions entity, FormCollection form)
        {
            try
            {

                string strresponse = "";
                UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = Convert.ToInt32(objUserSession.UserId);

                var serializer = new JavaScriptSerializer();
                var gr = form["Position_TechnologyStack_List"];
                var grs = serializer.Deserialize<List<Position_TechnologyStack_List>>(gr);
                entity.Position_TechnologyStack_List = grs;


                if (entity.InterviewForPositionId == 0)
                {

            
                    SqlConnection conn = new SqlConnection(str);
                    SqlCommand scmd = new SqlCommand();
                    scmd.CommandType = CommandType.StoredProcedure;
                    scmd.CommandText = "INSERTUPDATEAssessment_For_Position";
                    scmd.Parameters.Add("@InterviewForPositionname", SqlDbType.VarChar).Value = entity.InterviewForPositionname;
                    scmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = entity.Description;
                    scmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = _userID;

                    scmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    scmd.Connection = conn;
                    conn.Open();
                    scmd.ExecuteNonQuery();
                    int id = Convert.ToInt32(scmd.Parameters["@id"].Value.ToString());


                    foreach (var data in entity.Position_TechnologyStack_List)
                    {
                        if (data.Ischecked == true)
                        {
                            db.Set<Interview_Position_TechnologyStack>().Add(new Interview_Position_TechnologyStack
                            {
                                TechnologyStack_Id = data.Technologyid,
                                Assessment_For_Position_Positionid = id,
                                No_of_Questions = data.NoofQuestions,
                                Assessment_TimePeriod_in_sec = data.Timeinseconds,
                                CreatedDate = DateTime.Now,
                                CreatedBy = _userID,
                            });
                            db.SaveChanges();
                        }
                    }

                    strresponse = "1";
                }
                else if (entity.InterviewForPositionId > 0)
                {
                    (from p in db.Assessment_For_Position
                     where p.APID == entity.InterviewForPositionId
                     select p).ToList().ForEach(x =>
                     {
                         x.InterviewForPositionname = entity.InterviewForPositionname;
                         x.Description = entity.Description;
                         db.SaveChanges();
                     });


                    foreach(var data in entity.Position_TechnologyStack_List)
                    {
                        if(data.Ischecked == true)
                        {
                            var data1 = db.Interview_Position_TechnologyStack.Where(x => x.IPID == data.Technologyid && x.Assessment_For_Position_Positionid == entity.InterviewForPositionId).Count();
                            if(data1 == 0)
                            {
                                db.Set<Interview_Position_TechnologyStack>().Add(new Interview_Position_TechnologyStack
                                {
                                    TechnologyStack_Id = data.Technologyid,
                                    Assessment_For_Position_Positionid = entity.InterviewForPositionId,
                                    No_of_Questions = data.NoofQuestions,
                                    Assessment_TimePeriod_in_sec = data.Timeinseconds,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedBy = _userID,
                                });
                                db.SaveChanges();
                            }

                            if (data1 > 0)
                            {

                                (from p in db.Interview_Position_TechnologyStack
                                 where p.TechnologyStack_Id == data.Technologyid 
                                 && p.Assessment_For_Position_Positionid == entity.InterviewForPositionId
                                 select p).ToList().ForEach(x =>
                                 {
                                     x.TechnologyStack_Id = data.Technologyid;
                                     x.Assessment_For_Position_Positionid = entity.InterviewForPositionId;
                                     x.No_of_Questions = data.NoofQuestions;
                                     x.Assessment_TimePeriod_in_sec = data.Timeinseconds;
                                     db.SaveChanges();
                                 });

                            }

                        }


                        if (data.Ischecked == false)
                        {
                            var data1 = db.Interview_Position_TechnologyStack.Where(x => x.IPID == data.Technologyid && x.Assessment_For_Position_Positionid == entity.InterviewForPositionId).Count();
                            
                            if (data1 > 0)
                            {


                                Interview_Position_TechnologyStack ipt = null;

                                ipt = db.Set<Interview_Position_TechnologyStack>().Where(u => u.IPID == data.Technologyid && u.Assessment_For_Position_Positionid == entity.InterviewForPositionId).FirstOrDefault<Interview_Position_TechnologyStack>();
                                    if (ipt != null)
                                    {
                                        db.Interview_Position_TechnologyStack.Remove(ipt);
                                        db.SaveChanges();
                                    }
                
                            }

                        }

                    }

                    strresponse = "2";
                }

                return strresponse;


            }
            catch (Exception)
            {
                throw;
            }

        }


        public ActionResult Getallexamresult (int Userid)
        {
            try
            {
                TempData["Userid"] = Userid;
                
                return View();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public List<Questionsdata> getAllQuestionResult(int Userid)
        {

            try
            {


     //           UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
     //TempData["RoleidAdmin"] = Convert.ToInt32(objUserSession.RoleId);


                List<Questionsdata> Q = new List<Questionsdata>();
                List<Questionsdata> Q1 = new List<Questionsdata>();
                int i = 1;
                int j = 0;
                Q = (from a in db.CandidateInterviewResults
                     join b in db.QuestionBanks on a.Questionid equals b.QBID
                     where a.Candidateid == Userid
                     select new Questionsdata
                     {
                        
                         QBID = b.QBID,
                         TechnologyStackId = b.TechnologyStackId,
                         Question = b.Question,
                         Option1 = b.Option1,
                         Option2 = b.Option2,
                         Option3 = b.Option3,
                         Option4 = b.Option4,
                         Answer = b.Answer,
                         selectedanswer = a.SelectedOption,
                         isAttempted = a.Status == 2 ? false : true,
                         isCorrectanswer = a.Status == 1 ? true: false,
                         Descriptionforoptionchoosen = a.Descriptionforoptionchoosen,

                     }).ToList();

                foreach(var data in Q)
                {

                    Q1.Add(new Questionsdata
                    {
                        Number = "Q" + i++,
                        index = j++,
                        QBID =data.QBID,
                        TechnologyStackId = data.TechnologyStackId,
                        Question = data.Question,
                        Option1 = data.Option1,
                        Option2 = data.Option2,
                        Option3 = data.Option3,
                        Option4 = data.Option4,
                        Answer = data.Answer,
                        selectedanswer = data.selectedanswer,
                        isAttempted = data.isAttempted,
                        isCorrectanswer = data.isCorrectanswer,
                        Descriptionforoptionchoosen = data.Descriptionforoptionchoosen,
                    }); 

                    
                }

                return Q1;

            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}