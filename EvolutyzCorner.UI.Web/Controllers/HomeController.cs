using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using System.Web.Security;
using System.Data.Entity.SqlServer;
using evolCorner.Models;
using Microsoft.Ajax.Utilities;
using EvolutyzCorner.UI.Web.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Owin;

namespace EvolutyzCorner.UI.Web.Controllers
{
    

    public class HomeController : Controller
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        UserSessionInfo objUserSession;
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        public int timeSheetID = 0;
        string StatusMsg = string.Empty;
        SqlConnection Conn = new SqlConnection();
        SqlDataAdapter da = new SqlDataAdapter();
        timesheet lstusers = new timesheet();
        string _userID;

        string _paswd;
        string loginid;
        LoginComponent loginComponent = new LoginComponent();
        static int Userid;
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


        public ActionResult Index()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            loginid = Session["LoginId"].ToString();
            _userID = Session["LoginmemberID"].ToString();
            _paswd = (objUserSession.Password);
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();

            LoginComponent loginComponent = new LoginComponent();
           // TempData["Error"] = "Invalid Crediantials";
            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);
            if (usersprojects != null)

            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;

                //Session["projectName"] = objuser.projectName;
                //Session["ProjectClientName"] = objuser.ProjectClientName;
                //Session["username"] = objuser.Usr_Username;
                this.Session["TaskId"] = objuser;
            }
            return View(objuser);
        }


        [Authorize]
        [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
        public ActionResult Dashboard()
        {
            return View();
        }

        
        [AllowAnonymous]

        public ActionResult Login()
        {
            if (Session["LoginmemberID"] == null || Session["UserId"] == null)
            {
                objUserSession = new UserSessionInfo();
            }

            TempData["setpass"] = Request.QueryString["type"];

            return View();
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }



        #region Insertion (addSubmitTimeSheet)
        [HttpPost]
        public ActionResult AddUser(TotalTimeSheetTimeDetails sheetObj)
        {

            Conn = new SqlConnection(str);
            //  SendEmail objMail = new SendEmail();Admin@evolutyz.in

            int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[AddSubmitTimesheetWeb]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                objCommand.Parameters.AddWithValue("@TimesheetMonth", Convert.ToDateTime(sheetObj.timesheets.TimeSheetMonth).ToShortDateString());
                objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                objCommand.Parameters.AddWithValue("@ProjectID", sheetObj.timesheets.ProjectID);
                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
                objCommand.Parameters.AddWithValue("@L1ApproverStatus", false);
                objCommand.Parameters.AddWithValue("@L2ApproverStatus", false);
                objCommand.Parameters.Add("@TimesheetID", SqlDbType.Int);
                objCommand.Parameters["@TimesheetID"].Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                timeSheetID = int.Parse(objCommand.Parameters["@TimesheetID"].Value.ToString());
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());


                if (timeSheetID > 0)
                {
                    foreach (var item in sheetObj.listtimesheetdetails)
                    {

                        if ((Trans_Output == 105) || (Trans_Output == 106))
                        {
                            objCommand = new SqlCommand("[EditSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                            objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
                            objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
                            // objCommand.Parameters.AddWithValue("@TaskDate", Convert.ToDateTime(item.taskDate).ToShortDateString());
                            if (item.taskDate != null)
                            {
                                string dt = DateTime.Parse(item.taskDate.Trim())
     .ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                                objCommand.Parameters.AddWithValue("@TaskDate", dt);
                            }
                            else
                            {

                                // objCommand.Parameters.AddWithValue("@TaskDate", "");
                            }

                            objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                            objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                            objCommand.ExecuteNonQuery();
                            // Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                        }
                        else
                        {
                            objCommand = new SqlCommand("[AddSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                            objCommand.Parameters.AddWithValue("@TaskId", item.taskid);

                            objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
                            if (item.taskDate != null)
                            {
                                string dt = DateTime.Parse(item.taskDate.Trim())
     .ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                                objCommand.Parameters.AddWithValue("@TaskDate", dt);
                            }
                            else
                            {

                                // objCommand.Parameters.AddWithValue("@TaskDate", "");
                            }
                            objCommand.ExecuteNonQuery();
                        }

                    }

                }

                Conn.Close();
                if (Trans_Output == 1)
                {

                    if (sheetObj.timesheets.SubmittedType == "Submit")
                    {

                        StatusMsg = "TimeSheet Submitted Successfully..";
                    }

                    else
                    {
                        StatusMsg = "TimeSheet Saved Successfully..";

                    }
                }

                else
                {

                    if (Trans_Output == 104)
                    {

                        StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TimeSheetMonth + "' is already exists for this User";

                        //return StatusMsg;
                    }

                    else if (Trans_Output == 105)
                    {
                        StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TimeSheetMonth + "' is Saved Sucessfully";
                    }

                    else if (Trans_Output == 106)
                    {
                        StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TimeSheetMonth + "' is Submitted Sucessfully";
                    }

                    else if (Trans_Output == 111)
                    {
                        StatusMsg = "TimeSheet for this particular Month '" + sheetObj.timesheets.TimeSheetMonth + "' is Already  Submitted";
                    }

                    else
                    {
                        StatusMsg = "Precondition Failed";
                    }


                }

            }

            catch (Exception ex)
            {

                StatusMsg = ex.Message.ToString();


            }
            finally
            {
                if (Conn != null)
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }

            }
            //  return objmessageDetails;

            return Json(StatusMsg, JsonRequestBehavior.AllowGet);

        }
        #endregion

        public DateTime? GetLastVisitForUser(int id)

        {

            DateTime? dt = new DateTime();


            var item = (from x in db.Users where x.Usr_UserID == id select x.Usr_ModifiedDate).FirstOrDefault();

            if (item == null)
            {

                dt = null;
            }
            else
            {
                dt = Convert.ToDateTime(item);

            }
            return dt;
        }

        public void SetLastVisitForUser(int id, DateTime dateTime)

        {
            User u = new User();

            var item = (from x in db.Users where x.Usr_UserID == id select x).FirstOrDefault();

            if (item != null)
            {
                item.Usr_ModifiedDate = dateTime;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        public ActionResult SendUrl(System.Web.Mvc.FormCollection form)
        {
            string UserName = form["UserName"].ToString().Trim();
            LoginEntity logindetails = new LoginEntity();
            UserEntity objuser = new UserEntity();
            objuser = loginComponent.GerUserDetails(UserName);
            //string empid = objuser.UsrP_EmployeeID;
            if (objuser == null)
            {
                TempData["response"] = "Please Enter Registered EmailId";
                return View("ForgotPassword");
            }
            else
            {
                string fname = objuser.UsrP_FirstName;
                string lid = objuser.Usr_LoginId;
                int uid = objuser.Usr_UserID;
                string token = RandomString(20);
                SendEmailToresetpassword(fname, lid, uid, token);
                string response = loginComponent.SaveToken(token, uid);
                response = "Reset Link Sent to Your Mail";
                TempData["response"] = response;
                return View("Login");
            }

        }
        public static IRestResponse SendEmailToresetpassword(string fname, string lid, int uid, string token)
        {

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
            emailcontent = "<html>" +
               "<head>" +
               "<meta charset='UTF-8'>" +
               "<title>Reset your password</title>" +
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
               "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color:#fff; width:100%; min-height:100vh; padding:15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position:100% top; background-repeat: no-repeat; background-size:100%; max-width:680px; height:100%;'>" +
               "<tbody>" +
               "<tr>" +
               "<td>" +
               "<table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left;table-layout:fixed; padding-bottom:88px; width:100%; padding-left:40px; padding-right:40px;'>" +
               "<tbody>" +
               "<tr>" +
               "<td style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;font-weight: 600;height: 85px;vertical-align: middle;'>" +
               //"<img src='https://www.evolutyz.com/wp-content/themes/evolutyz/img/logo.png' style='width:100px;'>" +http://evolutyz.in/img/hero-alt-long2.jpg
               "<img src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/logowithoutTag.png' style='height: 50px;'>" +
               "</td>" +
               "<td style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;font-weight: 600;vertical-align: middle;height: 85px;text-align:right;font-size: 10px;'>" +
               "<div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;text-align: left;display: inline-block;'>" +
               "<p style='margin: 0 0 5px;color: #212121;'>An Award-Winning IT Solutions Firm</p>" +
               "<p style='color: #ffffff; margin: 0 0 5px;'>Together, the future is limitless.</p>" +
               "<p style='margin: 0 0 5px;color: #3c3c3c;'>Let's Innovate</p>" +
               "</div>" +
               "</td>" +
               "<td style='height: 85px;width: 85px;text-align: center;vertical-align: middle;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;'>" +
               "<img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc500.png' alt='logo' style='border:0;cursor: pointer;height: 60px;' onclick='window.open(\'https://www.inc.com/profile/evolutyz', \'_blank');'>" +
               "</td>" +
               "<td style='height: 85px;width: 65px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;'>" +
               "<img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc.png' alt='logo' style='border:0;cursor: pointer;background-color: #fff; border-radius: 50%; padding: 2px; border: 5px solid #212121; height: 56px;' onclick='window.open(\'https://www.inc.com/profile/evolutyz', \'_blank');'>" +
           "</td>" +
               "</tr>" +
               "<tr>" +
               "<td colspan='4' style='padding-top:72px; -ms-text-size-adjust:100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color:#000000; font-family:  sans-serif; font-size:48px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:-2.6px; line-height:52px; mso-line-height-rule: exactly; text-decoration: none;'>Reset your password</td>" +
               "</tr>" +
               "<tr>" +
               "<td colspan='4' style='padding-top:48px; padding-bottom:48px;'> <table cellpadding='0' cellspacing='0' style='width: 100%'>" +
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
               "<td colspan='4' style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;'> You are receiving this e - mail because you requested a password reset for your Evolutyz account. </td>" +
               "</tr>" +
               "<tr>" +
               "<td colspan='4' style='padding-top:24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size:16px; font-smoothing: always; font-style: normal; font-weight:400; letter-spacing:-0.18px; line-height:24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to choose a new password. </td>" +
               "</tr>" +
               "<tr>" +
               "<td colspan='4'>" +
               "<a data-click-track-id ='37' href ='" + UrlEmailAddress + "/Home/ResetPassword?token= " + token + "' style = 'margin-top:36px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #ffffff; font-family:  sans-serif; font-size:12px; font-smoothing: always; font-style: normal; font-weight:600; letter-spacing:0.7px; line-height:48px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width:220px; background-color: #795548; border-radius:28px; display: block; text-align: center; text-transform: uppercase' target = '_blank' > Reset Password </a></td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               "</td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               "<table align = 'center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width:100%; max-width:680px; height:100%;'>" +
               "<tbody>" +
               "<tr>" +
               "<td>" +
               "<table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left:40px; padding-right:40px;'>" +
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



            // RestClient client = new RestClient();
            //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
            //RestRequest request = new RestRequest();

            //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
            //request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");  
            //request.AddParameter("to", lid);
            //// request.AddParameter("to", manager2email);
            //request.AddParameter("subject", "Request to reset password");
            //request.AddParameter("html", emailcontent);
            //request.Method = Method.POST;
            //return client.Execute(request);

            var client = new SendGridClient("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");

            var msgs = new SendGridMessage()
            {
                From = new EmailAddress("noreply@evolutyz.com"),
                Subject = "Request to reset password",
                //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                HtmlContent = emailcontent

            };
            msgs.AddTo(new EmailAddress(lid));

            var responses = client.SendEmailAsync(msgs);
            return null;
        }


        static string Token;
        public ActionResult ResetPassword(string token)
        {
            Token = token;
            string response = loginComponent.gettoken(token);
            if (response == "")
            {
                TempData["setpass"] = "Password already reseted";
                return View("Login");
            }
            else
            {
                return View();

            }
        }
        [HttpPost]
        public ActionResult SetNewPassword(string Newpassword)
        {
            string NewPassword = GetMD5(Newpassword.Trim());

            string response = loginComponent.UpdatePasswordandtoken(Token, NewPassword);
            // TempData["setpass"] = "Password Reseted Successfully";
            return View("Login", new { type = response });



        }
        public ActionResult VerifyData()
        {
           return RedirectToAction("Login");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyData(System.Web.Mvc.FormCollection form)
        {
            bool bStatus = false;
            string UserName = form["txtuserName"].ToString().Trim();
            string Pass = GetMD5(form["txtUserPassword"].ToString().Trim());

            string Password = Pass;
            LoginEntity loginentity = new LoginEntity { UserName = UserName, Password = Password };


            this.Session["UserProfile"] = loginentity;
            UserEntity objuser = new UserEntity();
            ViewBag.Error = "Invalid Credentials";
            objuser = loginComponent.ValidateLogin(loginentity);
            if (objuser != null)
            {
                // 
                LoginEntity ent = new LoginEntity();
                // UserEntity userdtl = new UserEntity();

                var userdtl = (from u in db.Users
                               join upr in db.UserProjects on u.Usr_UserID equals upr.UProj_UserID
                               where u.Usr_LoginId == ent.UserName && u.Usr_Password == ent.Password ||
                                            u.Usr_Username == ent.UserName && u.Usr_Password == ent.Password
                               select new UserEntity
                               {
                                   Projectid = upr.UProj_ProjectID,
                                   TimesheetMode = upr.TimesheetMode,
                                   ClientprojID = upr.ClientprojID,

                               }).ToList();
                var userstatus = (from u in db.Users
                                  where u.Usr_LoginId == ent.UserName && u.Usr_Password == ent.Password ||
                                       u.Usr_Username == ent.UserName && u.Usr_Password == ent.Password
                                  select new UserEntity
                                  {
                                      Usr_ActiveStatus = u.Usr_ActiveStatus
                                  }).ToList();

                int? projectid = 0;
                int? clientproid = 0;
                int? timesheetmodeid = 0;
                bool? activestatus = userstatus[0].Usr_ActiveStatus;
                if (userdtl.Count > 0)
                {
                    projectid = userdtl[0].Projectid;
                    timesheetmodeid = userdtl[0].TimesheetMode;
                    clientproid = userdtl[0].ClientprojID;
                    Session["projectid"] = projectid;

                   
                }

                

                UserSessionInfo objSessioninfo = new UserSessionInfo();
                objSessioninfo.UserId = objuser.Usr_UserID;
                objSessioninfo.LoginId = objuser.Usr_LoginId;
                objSessioninfo.AccountId = objuser.Usr_AccountID;
                objSessioninfo.RoleName = objuser.RoleName;
                objSessioninfo.UserName = objuser.Usr_Username;
                objSessioninfo.FirstName = objuser.UsrP_FirstName;
                objSessioninfo.LastName = objuser.UsrP_LastName;
                objSessioninfo.Usrp_ProfilePicture = objuser.Usrp_ProfilePicture;
                objSessioninfo.Projectid = projectid;
                objSessioninfo.ClientprojID = clientproid;
                objSessioninfo.TimesheetMode = timesheetmodeid;
                objSessioninfo.status = activestatus;

                objSessioninfo.UsAccount = objuser.isusacc;
                Session["UsAccount"] = objuser.isusacc;
                ViewBag.isusacc = Convert.ToString(Session["UsAccount"]);
                Userid = objuser.Usr_UserID;
                ViewBag.userid = Userid;
                Session["Usrp_ProfilePicture"] = objuser.Usrp_ProfilePicture;
                Session["UserSessionInfo"] = objSessioninfo;
                Session["LoginId"] = objuser.Usr_LoginId;
                Session["userid"] = objuser.Usr_UserID;

                Session["username"] = objuser.Usr_LoginId;
                Session["Password"] = objuser.Usr_Password;
                Session["Role"] = objuser.RoleName;
                Session["Usr_UserTypeID"] = objuser.Usr_UserTypeID;
                Session["firstName"] = objuser.UsrP_FirstName;
                Session["lastName"] = objuser.UsrP_LastName;
                Session["roleName"] = objuser.RoleName;
                Session["roleid"] = objuser.RoleId;
                Session["Acountname"] = objuser.Usr_AccountID;

                ViewBag.userid = Convert.ToInt32(Session["UserId"]);
                ViewBag.firstName = Convert.ToString(Session["FirstName"]);
                ViewBag.lastName = Convert.ToString(Session["LastName"]);
                ViewBag.roleName = Convert.ToString(Session["RoleName"]);
                ViewBag.profilePicture = Convert.ToString(Session["Usrp_ProfilePicture"]);


                // ViewBag.RoleName = Session["RoleName"].ToString();//
                bStatus = true;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, objuser.Usr_LoginId, DateTime.Now, DateTime.Now.AddMinutes(2880), true, objuser.RoleName, FormsAuthentication.FormsCookiePath);
                string hash = FormsAuthentication.Encrypt(ticket);
                System.Web.HttpCookie cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, hash);

                if (ticket.IsPersistent)
                {
                    cookie.Expires = ticket.Expiration;
                }
                Response.Cookies.Add(cookie);
            }
            UserSessionInfo info = new UserSessionInfo();
            if (info.UsAccount == true)
            {
                DateTime? lastVisited = GetLastVisitForUser(Convert.ToInt32(Session["userid"]));
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select dbo.dReturnDate('" + lastVisited + "','Central Standard Time')", conn);
                //query[i].CreatedDate = Convert.ToDateTime(cmd.ExecuteScalar());
                TempData["lastVisited"] = Convert.ToDateTime(cmd.ExecuteScalar());
            }
            else
            {
                DateTime? lastVisited = GetLastVisitForUser(Convert.ToInt32(Session["userid"]));
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select dbo.dReturnDate('" + lastVisited + "','India Standard Time')", conn);
                //query[i].CreatedDate = Convert.ToDateTime(cmd.ExecuteScalar());
                TempData["lastVisited"] = Convert.ToDateTime(cmd.ExecuteScalar());
            }


            SetLastVisitForUser(Convert.ToInt32(Session["userid"]), DateTime.Now);

            if (bStatus)
            {
                Session["UN"] = HttpContext.User.Identity.Name;
                ViewBag.userid = Convert.ToInt32(Session["UserId"]);
                ViewBag.firstName = Convert.ToString(Session["FirstName"]);
                ViewBag.lastName = Convert.ToString(Session["LastName"]);
                ViewBag.roleName = Convert.ToString(Session["RoleName"]);
                return RedirectToAction("DashBoard1", "DashBoard");
            }



            return View("Login");
        }
        public List<Newmodel> GetAdminMenu()
        {

            // int Usid = Convert.ToInt32(Session["userid"]);

            //int a = ViewBag.userid;
            //if (Session["UserId"] ==a)
            //{
            //}
            UserSessionInfo info = new UserSessionInfo();
            int uid = info.UserId;
            var model1 = new Newmodel();

            var model = (from a in db.Accounts
                         join u in db.Users on a.Acc_AccountID equals u.Usr_AccountID
                         join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                         join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
                         join rm in db.RoleModules on r.Rol_RoleID equals rm.RMod_RoleID
                         join mm in db.Master_Sub_Module on rm.Sub_ModuleID equals mm.Sub_ModuleID
                         join mat in db.ModuleAccessTypes on rm.ModuleAccessTypeID equals mat.ModuleAccessTypeID
                         where u.Usr_UserID == uid && rm.RMod_isDeleted == false
                         select new Newmodel

                         {
                             UserID = u.Usr_UserID,
                             AccounId = a.Acc_AccountID,
                             RoleName = gr.Title,
                             ModuleAccessType = mat.ModuleAccessType1,
                             ModuleName = mm.Sub_ModuleName,
                             Mod_ModuleID = mm.Mod_ModuleID
                         }).Distinct().ToList();

            return model;
        }
        //public ActionResult GetUserProjectsInfo()
        //{

        //    string UserName = Session["UserName"].ToString();
        //    string Password = Session["Password"].ToString();
        //    LoginEntity loginentity = new LoginEntity { UserName = UserName, Password = Password };
        //    LoginComponent loginComponent = new LoginComponent();

        //   UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
        //    TempData["Error"] = "Event not raised";
        // var   usersprojects = loginComponent.getUserProjectsDetailsInfo(loginentity);
        //    if (usersprojects != null)
        //    {
        //        objuser.AccountName = usersprojects.AccountName;
        //        objuser.Usr_Username = usersprojects.Usr_Username;
        //        objuser.projectName = usersprojects.projectName;
        //        objuser.ProjectClientName = usersprojects.ProjectClientName;
        //        Session["projectName"] = objuser.projectName;
        //        Session["ProjectClientName"] = objuser.ProjectClientName;
        //        Session["username"] = objuser.Usr_Username;
        //        ViewData["UserProjectInfo"] = objuser;


        //    }
        //    return View("Index", ViewData["UserProjectInfo"]) ;


        //}
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
        public string ChangePassword(string Login_Password, string Login_NewPassword)
        {
            bool res = false;
            string strResponse = "";
            string _username = string.Empty;

            Session["UserName"] = ""; // Move this line to Login button success method

            if (Session["UserName"] != null)
                _username = "SuperAdmin";

            if (Login_Password != null && Login_NewPassword != null)
            {
                string password = FetchUserPassword(_username);

                if (Login_Password == password)
                {
                    //res = u.ChangePassword(Login_Password, Login_NewPassword);
                    //if (res)
                    //{
                    //    strResponse = "Password changed successfully.";
                    //    // Send Email to user to intimate the Password Change
                    //    #region Send Email (using Template)
                    //    bool sendMailToUser = false;
                    //    sendMailToUser = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailToUser"].ToString());

                    //    if (sendMailToUser)
                    //    {
                    //        byte[] buffer = null;
                    //        string sURL = Convert.ToString(WebConfigurationManager.AppSettings["WEBURL"]);
                    //        string securePasswordURL = Convert.ToString(WebConfigurationManager.AppSettings["securePasswordURL"]);
                    //        string pwdText = "<a href='" + securePasswordURL + "'>create a secure password</a>.";
                    //        string strSubject = Convert.ToString(WebConfigurationManager.AppSettings["ChangePasswordEmailSubject"]);
                    //        string strFromEmail = Convert.ToString(WebConfigurationManager.AppSettings["FromMailId"]);

                    //        string sendertemplateTEXT = EmailUtility.ReadTextFromFile(Server.MapPath("~/EmailTemplates/ChangePassword.txt"));
                    //        sendertemplateTEXT = EmailUtility.ReplaceSpecificPlaceholders(sendertemplateTEXT, "UserName", CommonReusableComponent.textINFO.ToTitleCase(_userEmail));
                    //        sendertemplateTEXT = EmailUtility.ReplaceSpecificPlaceholders(sendertemplateTEXT, "url", sURL);
                    //        sendertemplateTEXT = EmailUtility.ReplaceSpecificPlaceholders(sendertemplateTEXT, "securePasswordURL", pwdText);
                    //        EmailUtility.SendEmail(_userEmail, "", "", strFromEmail, strFromEmail, strSubject, "", sendertemplateTEXT, 1, "", buffer, "");
                    //    }
                    //    #endregion
                    //}
                    //else
                    //{
                    //    strResponse = "Error!";
                    //}
                }
                else
                {
                    strResponse = "Existing old password not matched, Please type again";
                }
            }
            return strResponse;
        }

        private string FetchUserPassword(string username)
        {
            try
            {
                var obj = new CommonRepostoryComponent();
                return obj.FetchfetchUserPassword(username);
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        [Authorize]
        [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
        public ActionResult ManageProfile()
        {
            return View();
        }

        [Authorize]
        public ActionResult SignOut()
        {
            this.Response.Cookies.Add(new System.Web.HttpCookie("ASP.NET_SessionId", ""));

            FormsAuthentication.SignOut();

            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-20));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();

            Session.Clear();

            Session.Abandon();

            Session.RemoveAll();

            HttpContext.Session.Clear();

            // Removing Cookies
            CookieOptions option = new CookieOptions();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                option.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                //Response.Cookies.Append(".AspNetCore.Session", "", option);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                option.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["AuthToken"].Value = string.Empty;
                //Response.Cookies.Append("AuthenticationToken", "", option);
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Usertimesheet()
        {
            //ViewBag.TotalStudents = Gettimesheet.Count();

            return View();
        }


        public ActionResult TaskLookups()
        {
            IEnumerable<SelectListItem> itemssss = db.GenericTasks.Select(c => new SelectListItem
            {

                Value = SqlFunctions.StringConvert((double)c.tsk_TaskID),
                Text = c.tsk_TaskName

            });

            ViewBag.taskname = itemssss;

            return View();
        }
       
        public ActionResult Authuser(string UserName, string Password)
        {
            bool bStatus = false;
            string Username = UserName.ToString().Trim();
            string Pass = GetMD5(Password.ToString().Trim());

            string password = Pass;
            LoginEntity loginentity = new LoginEntity { UserName = Username, Password = password };


            this.Session["UserProfile"] = loginentity;
            UserEntity objuser = new UserEntity();
            ViewBag.Error = "Invalid Credentials";
            objuser = loginComponent.ValidateLogin(loginentity);
            if (objuser != null)
            {
                // 
                LoginEntity ent = new LoginEntity();
                // UserEntity userdtl = new UserEntity();

                var userdtl = (from u in db.Users
                               join upr in db.UserProjects on u.Usr_UserID equals upr.UProj_UserID
                               where u.Usr_LoginId == ent.UserName && u.Usr_Password == ent.Password ||
                                            u.Usr_Username == ent.UserName && u.Usr_Password == ent.Password
                               select new UserEntity
                               {
                                   Projectid = upr.UProj_ProjectID,
                                   TimesheetMode = upr.TimesheetMode,
                                   ClientprojID = upr.ClientprojID,

                               }).ToList();
                var userstatus = (from u in db.Users
                                  where u.Usr_LoginId == ent.UserName && u.Usr_Password == ent.Password ||
                                       u.Usr_Username == ent.UserName && u.Usr_Password == ent.Password
                                  select new UserEntity
                                  {
                                      Usr_ActiveStatus = u.Usr_ActiveStatus
                                  }).ToList();

                int? projectid = 0;
                int? clientproid = 0;
                int? timesheetmodeid = 0;
                bool? activestatus = userstatus[0].Usr_ActiveStatus;
                if (userdtl.Count > 0)
                {
                    projectid = userdtl[0].Projectid;
                    timesheetmodeid = userdtl[0].TimesheetMode;
                    clientproid = userdtl[0].ClientprojID;
                    Session["projectid"] = projectid;
                }



                UserSessionInfo objSessioninfo = new UserSessionInfo();
                objSessioninfo.UserId = objuser.Usr_UserID;
                objSessioninfo.LoginId = objuser.Usr_LoginId;
                objSessioninfo.AccountId = objuser.Usr_AccountID;
                objSessioninfo.RoleName = objuser.RoleName;
                objSessioninfo.UserName = objuser.Usr_Username;
                objSessioninfo.FirstName = objuser.UsrP_FirstName;
                objSessioninfo.LastName = objuser.UsrP_LastName;
                objSessioninfo.Usrp_ProfilePicture = objuser.Usrp_ProfilePicture;
                objSessioninfo.Projectid = projectid;
                objSessioninfo.ClientprojID = clientproid;
                objSessioninfo.TimesheetMode = timesheetmodeid;
                objSessioninfo.status = activestatus;

                objSessioninfo.UsAccount = objuser.isusacc;
                Session["UsAccount"] = objuser.isusacc;
                ViewBag.isusacc = Convert.ToString(Session["UsAccount"]);
                Userid = objuser.Usr_UserID;
                ViewBag.userid = Userid;
                Session["Usrp_ProfilePicture"] = objuser.Usrp_ProfilePicture;
                Session["UserSessionInfo"] = objSessioninfo;
                Session["LoginId"] = objuser.Usr_LoginId;
                Session["userid"] = objuser.Usr_UserID;

                Session["username"] = objuser.Usr_LoginId;
                Session["Password"] = objuser.Usr_Password;
                Session["Role"] = objuser.RoleName;
                Session["Usr_UserTypeID"] = objuser.Usr_UserTypeID;
                Session["firstName"] = objuser.UsrP_FirstName;
                Session["lastName"] = objuser.UsrP_LastName;
                Session["roleName"] = objuser.RoleName;
                Session["roleid"] = objuser.RoleId;
                Session["Acountname"] = objuser.Usr_AccountID;

                ViewBag.userid = Convert.ToInt32(Session["UserId"]);
                ViewBag.firstName = Convert.ToString(Session["FirstName"]);
                ViewBag.lastName = Convert.ToString(Session["LastName"]);
                ViewBag.roleName = Convert.ToString(Session["RoleName"]);
                ViewBag.profilePicture = Convert.ToString(Session["Usrp_ProfilePicture"]);


                // ViewBag.RoleName = Session["RoleName"].ToString();//
                bStatus = true;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, objuser.Usr_LoginId, DateTime.Now, DateTime.Now.AddMinutes(2880), true, objuser.RoleName, FormsAuthentication.FormsCookiePath);
                string hash = FormsAuthentication.Encrypt(ticket);
                System.Web.HttpCookie cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, hash);

                if (ticket.IsPersistent)
                {
                    cookie.Expires = ticket.Expiration;
                }
                Response.Cookies.Add(cookie);
            }
            UserSessionInfo info = new UserSessionInfo();
            if (info.UsAccount == true)
            {
                DateTime? lastVisited = GetLastVisitForUser(Convert.ToInt32(Session["userid"]));
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select dbo.dReturnDate('" + lastVisited + "','Central Standard Time')", conn);
                //query[i].CreatedDate = Convert.ToDateTime(cmd.ExecuteScalar());
                TempData["lastVisited"] = Convert.ToDateTime(cmd.ExecuteScalar());
            }
            else
            {
                DateTime? lastVisited = GetLastVisitForUser(Convert.ToInt32(Session["userid"]));
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select dbo.dReturnDate('" + lastVisited + "','India Standard Time')", conn);
                //query[i].CreatedDate = Convert.ToDateTime(cmd.ExecuteScalar());
                TempData["lastVisited"] = Convert.ToDateTime(cmd.ExecuteScalar());
            }


            SetLastVisitForUser(Convert.ToInt32(Session["userid"]), DateTime.Now);

            if (bStatus)
            {
                Session["UN"] = HttpContext.User.Identity.Name;
                ViewBag.userid = Convert.ToInt32(Session["UserId"]);
                ViewBag.firstName = Convert.ToString(Session["FirstName"]);
                ViewBag.lastName = Convert.ToString(Session["LastName"]);
                ViewBag.roleName = Convert.ToString(Session["RoleName"]);
                return RedirectToAction("DashBoard1", "DashBoard");
            }
            else
            {
                return View("Login");
            }



           
        }

    }
}

//