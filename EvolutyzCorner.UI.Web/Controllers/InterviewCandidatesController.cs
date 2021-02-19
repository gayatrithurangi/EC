using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using EvolutyzCorner.UI.Web.Models;
using System.Configuration;
using evolCorner.Models;
using System.Data.Entity.SqlServer;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EvolutyzCorner.UI.Web.Controllers
{
    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class InterviewCandidatesController : Controller
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: InterviewCandidates
        public ActionResult Index()
        {
            UserSessionInfo sessId = new UserSessionInfo();
            int userID = sessId.UserId;
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
         
                if (item.ModuleName == "Add InterviewCandidate")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }


            }


            InterviewCandidateComponent compobj = new InterviewCandidateComponent();


                var ICPNames = compobj.GetAllInterviewPositionNames().Select(a => new SelectListItem()
                {
                    Value = a.APID.ToString(),
                    Text = a.InterviewForPositionname,

                });
                ViewBag.PositionNames = ICPNames;
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            int Roleid = Convert.ToInt32(objUserSession.RoleId);
            TempData["adminroleid"] = Roleid;



                ViewBag.RecUserId = userID;


            return View();
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                //lower  
                byte2String += targetData[i].ToString("x2");
                //upper  
                //byte2String += targetData[i].ToString("X2");  
            }
            return byte2String;
        }


        //[HttpPost]
        //public ActionResult CreateInterviewCandidate(string FirstName,string LastName, string Email, string MobileNumber,string Password, int InterviewForPositionname,string AssignmentDate,string AssignmentTime)
        //{
        //    using (var db = new EvolutyzCornerDataEntities())
        //    {

        //        try
        //        {

        //            InterviewCandidate nt = new InterviewCandidate();
        //            UserSessionInfo sessId = new UserSessionInfo();

        //            int IPID = Convert.ToInt32(InterviewForPositionname);
        //            int userID = sessId.UserId;
        //            int accid = sessId.AccountId;
        //            // DateTime date = Convert.ToDateTime(AssignmentDate);
        //           DateTime date = DateTime.ParseExact(AssignmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            string pswd = GetMD5(Password);

        //            EvolutyzCornerDataEntities evolutyzData = new EvolutyzCornerDataEntities();

        //            InterviewCandidate Depart = new InterviewCandidate();
        //            InterviewCandidate Departs = new InterviewCandidate();
        //            InterviewCandidate user = new InterviewCandidate();

        //                Depart.Assessment_For_Positionid = IPID;
        //                Depart.FirstName = FirstName;
        //                Depart.LastName = LastName;
        //                Depart.Emailid = Email;
        //                Depart.MobileNo = MobileNumber;
        //                Depart.Password = pswd;
        //                Depart.RecrutementUserid = userID;
        //                Depart.AssessmentDate = date;
        //                Depart.AssessmentTime = AssignmentTime;
        //                Depart.CreatedDate = System.DateTime.Now;
        //                Depart.status = true;

        //                evolutyzData.InterviewCandidates.Add(Depart);
        //           // }

        //            int response = evolutyzData.SaveChanges();

        //            if (response > 0)
        //            {
        //                EvolutyzCornerDataEntities en = new EvolutyzCornerDataEntities();
        //                return Json("Data Added Successfully", JsonRequestBehavior.AllowGet);
        //                string FName = FirstName;
        //                string LName = LastName;
        //                string Pswd = Password;
        //                var Recuiteremail = (from a in en.Users where a.Usr_UserID == userID select a.Usr_LoginId).FirstOrDefault();
        //                SendEmailToresetpassword(FName, LName, Pswd, Recuiteremail);
        //            }
        //            else
        //            {
        //                return Json("Try Again!!", JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        catch (Exception EX)
        //        {

        //            throw;
        //        }
        //        //return null;
        //    }
        //}

        public string CreateInterviewCandidate(InterviewCandidateEntity InterviewDtl)
        {
            string strResponse = string.Empty;
            UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = _objSessioninfo.UserId;
            InterviewDtl.Createdby = _userID;
            var Org = new InterviewCandidateComponent();
           
            string Password = InterviewDtl.Password;
            InterviewDtl.Password = GetMD5(InterviewDtl.Password);

            int r = Org.AddIntCandidate(InterviewDtl);
            if (r == 0)
            {
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                strResponse = "User created successfully";

                string FName = InterviewDtl.FirstName;
                string LName = InterviewDtl.LastName;
                int RecUserid = _userID;
                string password = Password;
                var RecuiterEmail = (from a in db.Users where a.Usr_UserID == RecUserid select a.Usr_LoginId).FirstOrDefault();

               SendEmailToresetpassword(FName, LName, RecUserid, password, RecuiterEmail);
            }
            else if (r == 1)
            {
                strResponse = "User already exists";
            }
            else if (r == -1)
            {
                strResponse = "Please Fill All Mandatory Fields";
            }
            else if (r == 2)
            {
                strResponse = "UserName already Exists";
            }
            else if (r == 3)
            {
                strResponse = "Loginid already Exists";
            }

            return strResponse;
        }

        public static IRestResponse SendEmailToresetpassword(string FName, string LName,int RecUserid,string password, string Recuiteremail)
        {

            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];



            string UrlEmailAddress = string.Empty;

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }
            var emailcontent = "";

            emailcontent = "<html>" +
                          "<head><meta charset='UTF-8'>" +
                          "<title>JobSeekerDetails</title>" +
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
                          //<!-- http://evolutyz.in/img/hero-alt-long2.jpg min-height:100vh;height:100%;-->
                          "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%; padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size: 100%; max-width:680px;'>" +
                         "<tbody>" +
                          "<tr>" +
                          "<td>" +
                          "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom:88px; width:100%; padding-left:90px; padding-right:90px;'>" +
                          "<tbody>" +
                          "<tr>" +
                          "<td style='padding-top:24px;'>" +
                          "<img src='https://media.glassdoor.com/sqll/1077141/evolutyz-squarelogo-1516869456062.png' style='border-radius: 100px;width: 155px;'>" +
                          "</td>" +
                          "</tr>" +
                          "<tr>" +
                          "<td colspan='2' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#5f5f5f; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Jobseeker Details</td>" +
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
                          //"<tr>"+
                          //"<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> Job seeker details to take assessment. </td>"+
                          //"</tr>"+
                          "<tr>" +
                          "<td colspan='2'>" +
                          "<table width=240 height=100 cellspacing=1 cellpadding=1 border=0 style='margin: auto; color: #585858;'>" +
                          "<tbody>" +
                           "<tr>" +
                          "<td><span style='font-size:18px; color: #2b2b2b;'><small>User name:</small></span></td>" +
                           "<td style='text-align: right;'><span><strong> "+ FName + "</strong></span></td>" +
                           "</tr> " +
                           "<tr> " +
                            "<td>" +
                            "<span style='font-size:18px; color: #2b2b2b;'><small>Password:</small>" +
                             "</td> " +
                            "<td style='text-align: right;'><span><strong>" + password + "</strong></span></td> " +
                           "</tr> " +
                           //"<tr> " +
                           //"<td><span style='font-size: 18px; color: #2b2b2b;'><small>Email:</small></td> " +
                           // "<td style='text-align: right;'><span><strong>abc@gmail.com</strong></span></td> " +
                           //"</tr>" +
                          "</tbody> " +
                         "</table>" +
                          "</td>" +
                          "</tr>" +
                          "<tr>" +
                          "<td style = 'padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to take assessment. </td>" +
                          "</tr>" +
                          "<tr>" +
                          "<td>" +
                          //"<a data-click-track-id ='37' href =' UrlEmailAddress + "/Home/ResetPassword?token=  "1234 "' style ='margin-top:36px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #ffffff; font-family:  sans-serif; font-size:12px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:0.7px; line-height:48px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:220px; background-color: #795548; border-radius:28px; display: block; text-align: center; text-transform: uppercase' target = '_blank' > Start Assessment </a>"+
                          "</td>" +
                          "</tr>" +
                          "</tbody>" +
                          "</table>" +
                          "</td>" +
                          "</tr>" +
                          "</tbody>" +
                          "</table>" +
                          //<!-- height:100%; -->
                          "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; '>" +
                          "<tbody>" +
                          "<tr>" +
                          "<td>" +
                          "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:90px; padding-right:90px;'>" +
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
                Subject = "Credentials for EvolutyzCorner Portal",
                //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                HtmlContent = emailcontent

            };
            msgs.AddTo(new EmailAddress(Recuiteremail));

            var responses = client.SendEmailAsync(msgs);
            return null;
        }



        public JsonResult GetCandidateByICID(int ICID)
        {
            InterviewCandidateEntity CandidateDetails = null;
            try
            {
                var TktDtl = new InterviewCandidateComponent();
                CandidateDetails = TktDtl.GetCandidateByICID(ICID);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(CandidateDetails, JsonRequestBehavior.AllowGet);
        }


        InterviewCandidateComponent _InterviewComp = new InterviewCandidateComponent();
        public string UpdateInterviewCandidates(string id, string FirstName, string LastName, string Email,string MobileNumber, string InterviewForPositionname, string AssignmentDate, string AssignmentTime)
        {
            string Interview = string.Empty;
            try
            {
                Interview = _InterviewComp.UpdateInterviewCandidates(id, FirstName, LastName, Email, MobileNumber, InterviewForPositionname, AssignmentDate, AssignmentTime);


            }

            catch (Exception ex)
            {
                return null;
            }
            return Interview;

        }

        public JsonResult GetInterviewCandidates()
        {
            UserSessionInfo sessId = new UserSessionInfo();
            int userID = sessId.UserId;
            List<InterviewCandidateEntity> AccDetails = null;
            try
            {
                var objDtl = new InterviewCandidateComponent();
                AccDetails = objDtl.GetInterviewCandidatesList();
                ViewBag.OrgAccDetails = AccDetails[0].ICID;
                ViewBag.Userid = userID;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(AccDetails, JsonRequestBehavior.AllowGet);
        }

        // GET: InterviewCandidates/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InterviewCandidates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InterviewCandidates/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InterviewCandidates/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InterviewCandidates/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InterviewCandidates/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        public ActionResult AddInterviewCandidate()
        {
            return View();
        }


        //public ActionResult GetInterviewCandidates()
        //{
        //    return View();

        //}

        //public JsonResult GetInterviewCandidate()
        //{
        //    //using (var db = new EvolutyzCornerDataEntities())
        //    //{
        //    try
        //    {

        //        List<InterviewCandidateEntity> entity = new List<InterviewCandidateEntity>();
        //        InterviewCandidateComponent component = new InterviewCandidateComponent();
        //        entity = component.GetInterviewCandidateDetails();


        //        return Json(entity, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    // }



        //}

        // POST: InterviewCandidates/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
