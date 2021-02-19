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
//
///My Edit code
namespace EvolutyzCorner.UI.Web.Controllers
{

    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class UserController : Controller
    {
        List<SelectListItem> lsttimesheet = new List<SelectListItem>();
        List<SelectListItem> lstmonth = new List<SelectListItem>();

        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string htmlStr = "";
        public int timeSheetID = 0;
        SqlConnection Conn = new SqlConnection();
        SqlDataAdapter da = new SqlDataAdapter(); DataSet ds = new DataSet(); DataTable dt = new DataTable();
        DataSet ds1 = new DataSet(); DataTable dtheadings = new DataTable(); DataTable dtData = new DataTable();
        EmailFormats objemail = new EmailFormats(); string StatusMsg = string.Empty;
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        timesheet lstusers = new timesheet();

        public ActionResult Index(bool? pdf)
        {

            UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int UserId = _objSessioninfo.UserId;

            //int AccountId = _objSessioninfo.AccountId;
            ViewBag.UserId = UserId;
            string roleID = _objSessioninfo.RoleName;
            ViewBag.Roleid = _objSessioninfo.RoleName;
            int accountID = _objSessioninfo.AccountId;
            ViewBag.Accountid = _objSessioninfo.AccountId;
            LeaveSchemeComponent compobj = new LeaveSchemeComponent();
            var Accountname = compobj.GetallAccountnames(accountID, roleID).Select(a => new SelectListItem()
            {
                Value = a.Acc_AccountID.ToString(),
                Text = a.Acc_AccountName,
            });
            ClientComponent clientcomponent = new ClientComponent();
            ViewBag.usertitle = clientcomponent.GetTitle().Select(a => new SelectListItem()
            {
                Value = a.Usr_Titleid.ToString(),
                Text = a.TitlePrefix,

            });
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                //if (item.ModuleName == "Add User")
                //{
                //    var mk = item.ModuleAccessType;
                //    ViewBag.b = mk;
                //}
                if (item.ModuleName == "Add User")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }


            }

            ViewBag.Accountname = Accountname;
            return View();
            //if (!pdf.HasValue)
            //{
            //    #region to return UserList

            //    //objSessioninfo.UserId = 501;
            //    //Session["UserSessionInfo"] = objSessioninfo;

            //    List<LookupStatusDetail> objStatusList = new List<LookupStatusDetail>();
            //    objStatusList.Add(new LookupStatusDetail { StatusID = 1, Status = "Active" });
            //    objStatusList.Add(new LookupStatusDetail { StatusID = 0, Status = "InActive" });

            //    var objStList = from cl in objStatusList
            //                    orderby cl.StatusID
            //                    select new
            //                    {
            //                        value = cl.StatusID,
            //                        text = cl.Status
            //                    };
            //    ViewBag.Status = objStList;
            //    return View();
            //    #endregion
            //}
            //else
            //{
            //    string filename = "User.pdf";
            //    string filePath = Server.MapPath("~/Content/PDFs/" + filename);

            //    var objDtl = new UserComponent();
            //    IList<UserEntity> UserList = objDtl.GetUserDetail();

            //    ExportPDF(UserList, new string[] { "Usr_UserID", "AccountName", "UserType", "RoleName", "Usr_LoginId" }, filePath);

            //    return File(filePath, "application/pdf");
            //}
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

        public JsonResult GetUserCollection(int acntID, string RoleId)
        {
            List<UserEntity> UserDetails = null;
            try
            {

                string accid = ViewBag.Accountid;
                // ViewBag.Accountid=
                var objDtl = new UserComponent();
                UserDetails = objDtl.GetUserDetail(acntID, RoleId);
                ViewBag.UserDetails = UserDetails[0].Usr_Version;

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByID(int catID)
        {
            UserEntity UserDetails = null;
            try
            {
                var objDtl = new UserComponent();
                UserDetails = objDtl.GetUserDetailByID(catID);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetL2ManagerNames(int AccountId)
        {
            List<UserEntity> l2Manger = null;
            try
            {
                var objDtl = new UserComponent();
                l2Manger = objDtl.GetManagerNames2(AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetL1ManagerNames(int AccountId)
        {
            List<UserEntity> l1Manger = null;
            try
            {
                var objDtl = new UserComponent();
                l1Manger = objDtl.GetManagerNames(AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l1Manger, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageProfile()
        {
            return View();
        }

        public JsonResult GetUserRoles(int AccountId, string RoleId)
        {
            List<UserEntity> UserRoles = null;
            try
            {
                var objDtl = new UserComponent();
                UserRoles = objDtl.GetUserRolenames(AccountId, RoleId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTaskNames(int AccountId)
        {
            List<UserEntity> TaskNames = null;
            try
            {
                var objDtl = new UserComponent();
                TaskNames = objDtl.GetTaskNames(AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(TaskNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserAccounts(int acntID, int userid)
        {
            List<UserEntity> AccountNames = null;
            try
            {
                var objDtl = new UserComponent();
                AccountNames = objDtl.GetAccounts(acntID, userid);

            }
            catch (Exception)
            {
                return null;
            }
            return Json(AccountNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserAccounts1(int acntID, string RoleId)
        {
            List<UserEntity> AccountNames = null;
            try
            {
                var objDtl = new UserComponent();
                AccountNames = objDtl.GetAccounts1(acntID, RoleId);

            }
            catch (Exception)
            {
                return null;
            }
            return Json(AccountNames, JsonRequestBehavior.AllowGet);
        }

        public string CreateUser([Bind(Exclude = "Usr_UserID")] UserEntity UserDtl)
        {
            int r = 0;
            string Password = UserDtl.Usr_Password;
            
            string strResponse = string.Empty;
            string fileExtension = string.Empty;
            UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = _objSessioninfo.UserId;
            UserDtl.Usr_CreatedBy = _userID;
            var Org = new UserComponent();
            string imagename = string.Empty;
            if (UserDtl.Usrp_ProfilePicture == "undefined")
            {
               
                UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);
                r = Org.AddUser(UserDtl);
            }
            else if (UserDtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(UserDtl.imgCropped.Split(',')[1]);
                string sourceFile = "";
                string filePath = String.Empty;
                string base64 = UserDtl.imgCropped;
                filePath = "/uploadimages/Images/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                sourceFile = filePath.Replace("/uploadimages/Images/", "").ToString();
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                string physicalPath = Server.MapPath("/uploadimages/Images/" + sourceFile);

                imagename = sourceFile;
                UserDtl.Usrp_ProfilePicture = imagename;
            }
            else if (Request.Files.Count > 0)
            {

                var file = Request.Files[0];
                var fileName = "/uploadimages/Images/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
                UserDtl.Usrp_ProfilePicture = imagename;

            }
            else
            {
                imagename = UserDtl.Usrp_ProfilePicture;
            }



            UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);
            r = Org.AddUser(UserDtl);
            #region old uplaod Region
            //if (Request.Files.Count > 0)
            //{
            //    var file = Request.Files[0];
            //    var fileName = "/uploadimages/Images/" + file.FileName;
            //    imagename = file.FileName;
            //    var imagepath = Server.MapPath(fileName);
            //    file.SaveAs(imagepath);
            //}
            //if (imagename == "")
            //{
            //    UserDtl.Usrp_ProfilePicture = "User.png";
            //}
            //else
            //{
            //    UserDtl.Usrp_ProfilePicture = imagename;
            //} 
            #endregion




            if (r == 1)
            {
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                strResponse = "User created successfully";

                string username = UserDtl.Usr_LoginId;
                string password = Password;
                string fname = UserDtl.UsrP_FirstName;
                int roleid = UserDtl.Usr_RoleID;
                int acid = UserDtl.Usr_AccountID;
                var Role_name = (from a in db.Roles where a.Rol_RoleID == roleid select a.Rol_RoleName).FirstOrDefault();
                var profileimage = (from a in db.Accounts where a.Acc_AccountID == acid select a.Acc_CompanyLogo).FirstOrDefault();
                int rolename = Convert.ToInt32(Role_name);
                string usrname = UserDtl.Usr_Username;
                SendEmailToresetpassword(username, password, fname, rolename, usrname, profileimage);
            }
            else if (r == 0)
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

        public static IRestResponse SendEmailToresetpassword(string username, string password, string fname, int rolename, string usrname, string profileimage)
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
            if (rolename == 1002)
            {
                emailcontent = "<html>" +
             "<head><meta charset='UTF-8'>" +
             "<title>Reset your password</title>" +
             "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
             "<meta content='width=device-width' name='viewport'>" +
             "<style type='text/css'> @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 600; font-style: normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 500; font-style: normal; src: local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 400; font-style: normal; src: local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); } </style> <style media='screen and (max-width: 680px)'> @media screen and (max-width: 680px) { .page-center { padding-left: 0 !important; padding-right: 0 !important; } .footer-center { padding-left: 20px !important; padding-right: 20px !important; } }</style>" +
             "</head>" +
             "<body style='background-color: #f4f4f5;'> " +
             "<table cellpadding='0' cellspacing='0' style='width: 100%; height: 100%; background-color: #f4f4f5; text-align: center;'>" +
             "<tbody><tr><td style='text-align: center;'> " +
             "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color: #fff; width: 100%; min-height: 100vh; padding: 15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position: 100% top; background-repeat: no-repeat; background-size: 100%; max-width: 680px; height: 100%;'> " +
              "<tbody> <tr> <td> <table align='center' cellpadding='0' cellspacing='0' class='page-center' style='table-layout: fixed;text-align: left; padding-bottom: 88px; width: 100%; padding-left: 40px; padding-right: 40px;'> <tbody> <tr>" +
              "<td style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;font-weight: 600;height: 85px;vertical-align: middle;'><img src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + profileimage + "' style='height: 50px;'> " +
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
             "<td colspan='4'>" +
             "<span style='padding-top:60px;color: rgba(29, 25, 25, 0.7098039215686275);font-family:sans-serif;font-size: 19px;font-style:normal;font-weight:600;letter-spacing:-0.25px;line-height:35px;text-decoration:none;'>Hi " + fname + " ,</span>" +
             "<p style='-ms-text-size-adjust: 100%;-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: 100%;color: rgba(0, 0, 0, 0.75);font-family: sans-serif;font-size: 16px;font-smoothing: always;font-style: normal;font-weight: 500;letter-spacing: -0.25px;line-height: 20px;mso-line-height-rule: exactly;text-decoration: none;margin: 50px 0 0;text-indent: 40px;'>Welcome to Evolutyz family, find your credentials</p>  " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan='4' style='padding-top:48px; padding-bottom:48px;'><table cellpadding='0' cellspacing='0' style='width: 100%'>" +
             "<tbody><tr><td style='width: 100%; height: 1px; max-height: 1px; background-color: #d9dbe0; opacity: 0.81'>" +
             "</td> " +
             "</tr> " +
             "</tbody> " +
             "</table> " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan='4' style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none;vertical-align:top;width: 100%;'>" +
             "<table style='width: 100%;table-layout:fixed;'>" +
              "<tr>" +
             "<td>Username:</td> " +
             "<td>" + usrname + "</td>" +
             " </tr>" +
             "<tr>" +
             "<tr>" +
             "<td>Login Id:</td> " +
             "<td>" + username + "</td>" +
             " </tr>" +
             "<tr>" +
             "<td>Password:</td>" +
             "<td>" + password + "</td>" +
             "</tr>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan='4' style = 'padding-top: 24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to redirect to Evolutyz Corner. " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan='4'>" +
             "<a data-click-track-id = '37' href = 'https://" + UrlEmailAddress + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Click Here to Login</a>" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "<table align='center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width: 100%; max-width: 680px; height: 100%;'><tbody><tr><td><table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left: 40px; padding-right: 40px;'>" +
             " <tbody>" +
             " <tr>" +
             " <td colspan = '2' style='padding-top: 50px;padding-bottom: 15px;width: 100%;color: #f8c26c;font-size: 40px;'> Evolutyz Corner</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan = '2' style= 'padding-top: 24px; padding-bottom: 48px;' >" +
             "<table cellpadding= '0' cellspacing= '0' style= 'width: 100%'>" +
             "<tbody>" +
             "<tr>" +
             "<td style= 'width: 100%; height: 1px; max-height: 1px; background-color: #EAECF2; opacity: 0.19' >" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style= '-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size: 15px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: 0; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:noreply@evolutyz.com' title='noreply@evolutyz.com' style='font-weight: 500; color: #ffffff' target='_blank'>Help Center</a>. " +
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
            }
            else
            {
                emailcontent = "<html>" +
             "<head><meta charset='UTF-8'>" +
             "<title>Reset your password</title>" +
             "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
             "<meta content='width=device-width' name='viewport'>" +
             "<style type='text/css'> @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 600; font-style: normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 500; font-style: normal; src: local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 400; font-style: normal; src: local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); } </style> <style media='screen and (max-width: 680px)'> @media screen and (max-width: 680px) { .page-center { padding-left: 0 !important; padding-right: 0 !important; } .footer-center { padding-left: 20px !important; padding-right: 20px !important; } }</style>" +
             "</head>" +
             "<body style='background-color: #f4f4f5;'> " +
             "<table cellpadding='0' cellspacing='0' style='width: 100%; height: 100%; background-color: #f4f4f5; text-align: center;'>" +
             "<tbody><tr><td style='text-align: center;'> " +
             "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color: #fff; width: 100%; min-height: 100vh; padding: 15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position: 100% top; background-repeat: no-repeat; background-size:100%; max-width: 680px; height: 100%;'> " +
             "<tbody> <tr> <td> <table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom: 88px; width: 100%; padding-left: 120px; padding-right: 120px;'> <tbody> <tr> <td style='padding-top: 24px;'>  <img src='https://" + UrlEmailAddress + "/Content/images/CompanyLogos/evolutyzcorplogo.png' style='width: 100px;'> " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan='2'>" +
             "<span style='padding-top:60px;color: rgba(29, 25, 25, 0.7098039215686275);font-family:sans-serif;font-size: 19px;font-style:normal;font-weight:600;letter-spacing:-0.25px;line-height:35px;text-decoration:none;'>Hi " + fname + " ,</span>" +
             "<p style='-ms-text-size-adjust: 100%;-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: 100%;color: rgba(0, 0, 0, 0.75);font-family: sans-serif;font-size: 16px;font-smoothing: always;font-style: normal;font-weight: 500;letter-spacing: -0.25px;line-height: 20px;mso-line-height-rule: exactly;text-decoration: none;margin: 50px 0 0;text-indent: 40px;'>Welcome to Evolutyz family, please submit your timesheet in the Evolutyz Corner Portal and find your credentials</p>  " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style='padding-top:48px; padding-bottom:48px;'><table cellpadding='0' cellspacing='0' style='width: 100%'>" +
             "<tbody><tr><td style='width: 100%; height: 1px; max-height: 1px; background-color: #d9dbe0; opacity: 0.81'>" +
             "</td> " +
             "</tr> " +
             "</tbody> " +
             "</table> " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none;vertical-align:top;width: 100%;'>" +
             "<table style='width: 100%;table-layout:fixed;'>" +
             "<tr>" +
             "<td>User name :</td> " +
             "<td>" + usrname + "</td>" +
             " </tr>" +
             "<tr>" +
             "<tr>" +
             "<td>Login Id:</td> " +
             "<td>" + username + "</td>" +
             " </tr>" +
             "<tr>" +
             "<td>Password :</td>" +
             "<td>" + password + "</td>" +
             "</tr>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style = 'padding-top: 24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to redirect to Evolutyz Corner. " +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td>" +
             "<a data-click-track-id = '37' href = 'https://" + UrlEmailAddress + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Click Here to Login</a>" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "<table align='center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width: 100%; max-width: 680px; height: 100%;'><tbody><tr><td><table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left: 120px; padding-right: 120px;'>" +
             " <tbody>" +
             " <tr>" +
             " <td colspan = '2' style='padding-top: 50px;padding-bottom: 15px;width: 100%;color: #f8c26c;font-size: 40px;'> Evolutyz Corner</td>" +
             "</tr>" +
             "<tr>" +
             "<td colspan = '2' style= 'padding-top: 24px; padding-bottom: 48px;' >" +
             "<table cellpadding= '0' cellspacing= '0' style= 'width: 100%'>" +
             "<tbody>" +
             "<tr>" +
             "<td style= 'width: 100%; height: 1px; max-height: 1px; background-color: #EAECF2; opacity: 0.19' >" +
             "</td>" +
             "</tr>" +
             "</tbody>" +
             "</table>" +
             "</td>" +
             "</tr>" +
             "<tr>" +
             "<td style= '-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size: 15px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: 0; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053'  title='noreply@evolutyz.com' style='font-weight: 500; color: #ffffff' target='_blank'>Help Center</a>. " +
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
            }




            //RestClient client = new RestClient();
            //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
            //RestRequest request = new RestRequest();

            //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
            //request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");
            //request.AddParameter("to", username);
            //// request.AddParameter("to", manager2email);
            //request.AddParameter("subject", "Credentials for EvolutyzCorner Portal");
            //request.AddParameter("html", emailcontent);
            //request.Method = Method.POST;
            //return client.Execute(request);

            var client = new SendGridClient("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");

            var msgs = new SendGridMessage()
            {
                From = new EmailAddress("noreply@evolutyz.com"),
                Subject = "Credentials for EvolutyzCorner Portal",
                //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                HtmlContent = emailcontent

            };
            msgs.AddTo(new EmailAddress(username));

            var responses = client.SendEmailAsync(msgs);
            return null;
        }



        [HttpPost]
        public string UpdateUser(UserEntity UserDtl)
        {
            string strResponse = string.Empty;
            short UsTCurrentVersion = 0;
            string imagename = string.Empty;

            if (UserDtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(UserDtl.imgCropped.Split(',')[1]);
                try
                {
                    string sourceFile = "";
                    string filePath = String.Empty;
                    string base64 = UserDtl.imgCropped;
                    filePath = "/uploadimages/Images/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                    sourceFile = filePath.Replace("/uploadimages/Images/", "").ToString();
                    using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }

                    imagename = sourceFile;
                    UserDtl.Usrp_ProfilePicture = imagename;
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            else
            {
                if (Request.Files.Count > 0)
                {
                    var mediaFile = Request.Files[0];



                    if (mediaFile != null)
                    {

                        var fileName = "/uploadimages/Images/" + mediaFile.FileName;
                        imagename = mediaFile.FileName;
                        var imagepath = Server.MapPath(fileName);
                        mediaFile.SaveAs(imagepath);
                        UserDtl.Usrp_ProfilePicture = imagename;
                    }



                }
                else
                {
                    imagename = UserDtl.Usrp_ProfilePicture;


                }
            }
            UserDtl.Usrp_ProfilePicture = imagename;
            var em = (from p in db.Users where p.Usr_UserID == UserDtl.Usr_UserID select p.Usr_Password).FirstOrDefault();

            if (UserDtl.Usr_Password == null)
            {
                UserDtl.Usr_Password = em;
            }
            else if (UserDtl.Usr_Password != em)
            {

                UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);

            }
            else
            {
                UserDtl.Usr_Password = em;
            }
            try
            {
                var UserComponent = new UserComponent();
                var currentUserDetails = UserComponent.GetUserDetailByID(UserDtl.Usr_UserID);
                int UserID = currentUserDetails.Usr_UserID;
                UsTCurrentVersion = Convert.ToInt16(currentUserDetails.Usr_Version);
                bool _currentStatus = false;

                _currentStatus = UserDtl.Usr_ActiveStatus == true;

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = _objSessioninfo.UserId;
                UserDtl.Usr_ModifiedBy = _userID;

                UserDtl.Usr_Version = ++UsTCurrentVersion;
                UserDtl.Usr_ActiveStatus = _currentStatus;
                var Org = new UserComponent();
                int r = Org.UpdateUserDetail(UserDtl);
                if (r == 1)
                {
                    strResponse = "User updated successfully";
                    _objSessioninfo.Usrp_ProfilePicture = imagename;
                }
                else if (r == 0)
                {
                    strResponse = "User does not exists";
                }
                else if (r == -1)
                {
                    strResponse = "Error occured in UpdateUser";
                }
                else if (r == 2)
                {
                    strResponse = "UserName already Exists";
                }
                else if (r == 3)
                {
                    strResponse = "Loginid already Exists";
                }
            }
            //  }
            catch (Exception ex)
            {
                return strResponse;
            }
            #region MyRegion
            //if (Request.Files.Count > 0)
            //{
            //    var em = (from p in db.Users where p.Usr_UserID == UserDtl.Usr_UserID select p.Usr_Password).FirstOrDefault();

            //    if (UserDtl.Usr_Password == null)
            //    {
            //        UserDtl.Usr_Password = em;
            //    }
            //    else if (UserDtl.Usr_Password != em)
            //    {

            //        UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);

            //    }
            //    else
            //    {
            //        UserDtl.Usr_Password = em;
            //    }

            //    var file = Request.Files[0];
            //    var fileName = "/uploadimages/Images/" + file.FileName;
            //    imagename = file.FileName;
            //    var imagepath = Server.MapPath(fileName);
            //    file.SaveAs(imagepath);
            //    UserDtl.Usrp_ProfilePicture = imagename;
            //    try
            //    {
            //        var UserComponent = new UserComponent();
            //        var currentUserDetails = UserComponent.GetUserDetailByID(UserDtl.Usr_UserID);
            //        int UserID = currentUserDetails.Usr_UserID;
            //        UsTCurrentVersion = Convert.ToInt16(currentUserDetails.Usr_Version);
            //        bool _currentStatus = false;

            //        _currentStatus = UserDtl.Usr_ActiveStatus == true;

            //        UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            //        int _userID = _objSessioninfo.UserId;
            //        UserDtl.Usr_ModifiedBy = _userID;

            //        UserDtl.Usr_Version = ++UsTCurrentVersion;
            //        UserDtl.Usr_ActiveStatus = _currentStatus;
            //        var Org = new UserComponent();
            //        int r = Org.UpdateUserDetail(UserDtl);
            //        if (r == 1)
            //        {
            //            strResponse = "User updated successfully";
            //            _objSessioninfo.Usrp_ProfilePicture = imagename;
            //        }
            //        else if (r == 0)
            //        {
            //            strResponse = "User does not exists";
            //        }
            //        else if (r == -1)
            //        {
            //            strResponse = "Error occured in UpdateUser";
            //        }
            //        else if (r == 2)
            //        {
            //            strResponse = "UserName already Exists";
            //        }
            //        else if (r == 3)
            //        {
            //            strResponse = "Loginid already Exists";
            //        }
            //    }
            //    //  }
            //    catch (Exception ex)
            //    {
            //        return strResponse;
            //    }
            //}
            //else
            //{

            //    try
            //    {

            //        var em = (from p in db.Users where p.Usr_UserID == UserDtl.Usr_UserID select p.Usr_Password).FirstOrDefault();



            //        if (UserDtl.Usr_Password == null)
            //        {
            //            UserDtl.Usr_Password = em;
            //        }
            //        else if (UserDtl.Usr_Password != em)
            //        {

            //            UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);

            //        }
            //        else
            //        {
            //            UserDtl.Usr_Password = em;
            //        }

            //        var UserComponent = new UserComponent();
            //        var currentUserDetails = UserComponent.GetUserDetailByID(UserDtl.Usr_UserID);
            //        int UserID = currentUserDetails.Usr_UserID;
            //        UsTCurrentVersion = Convert.ToInt16(currentUserDetails.Usr_Version);
            //        bool _currentStatus = false;

            //        ////check for version and active status
            //        //if (ModelState["Usr_ActiveStatus"].Value != null)
            //        //{
            //        _currentStatus = UserDtl.Usr_ActiveStatus == true;
            //        //}

            //        //if (ModelState.IsValid)
            //        //{
            //        UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            //        int _userID = _objSessioninfo.UserId;
            //        UserDtl.Usr_ModifiedBy = _userID;
            //        //while udating increment version by1
            //        UserDtl.Usr_Version = ++UsTCurrentVersion;
            //        UserDtl.Usr_ActiveStatus = _currentStatus;
            //        var Org = new UserComponent();
            //        // UserDtl.Usr_Password = GetMD5(UserDtl.Usr_Password);
            //        int r = Org.UpdateUserDetailByImage(UserDtl);

            //        if (r > 0)
            //        {
            //            strResponse = "User updated successfully";
            //        }
            //        else if (r == 0)
            //        {
            //            strResponse = "User does not exists";
            //        }
            //        else if (r < 0)
            //        {
            //            strResponse = "Error occured in UpdateUser";
            //        }
            //    }
            //    //  }
            //    catch (Exception ex)
            //    {
            //        return strResponse;
            //    }


            //} 
            #endregion





            return strResponse;
        }

        public ActionResult getUserTypes(int AccountId)
        {
            //UserSessionInfo infoobj = new UserSessionInfo();
            //int accountid = infoobj.AccountId;

            List<UserEntity> UserTypes = null;
            try
            {
                var objDtl = new UserComponent();
                UserTypes = objDtl.getUserTypes(AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserTypes, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetUserTypesByAccountid()
        {
            UserSessionInfo infoobj = new UserSessionInfo();
            int accountid = infoobj.AccountId;

            List<UserEntity> UserTypes = null;
            try
            {
                var objDtl = new UserComponent();
                UserTypes = objDtl.getUserTypes(accountid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserTypes, JsonRequestBehavior.AllowGet);


        }

        public string DeleteUser(int UserID)
        {
            string strResponse = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var Org = new UserComponent();
                    int r = Org.DeleteUserDetail(UserID);
                    if (r > 0)
                    {
                        strResponse = "User deleted successfully";
                    }
                    else if (r == 0)
                    {
                        strResponse = "User does not exists";
                    }
                    else if (r < 0)
                    {
                        strResponse = "Error occured in DeleteUser";
                    }
                }
            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }
        public string DeleteSkill(int skillid)
        {
            string strResponse = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var Org = new UserComponent();
                    int r = Org.DeleteSkill(skillid);
                    if (r > 0)
                    {
                        strResponse = "Skill deleted successfully";
                    }
                    else if (r == 0)
                    {
                        strResponse = "skill does not exists";
                    }
                    else if (r < 0)
                    {
                        strResponse = "Error occured in DeleteUser";
                    }
                }
            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }

        public string SkillChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            var objDtl = new UserComponent();
            strResponse = objDtl.SkillChangeStatus(id, status);

            return strResponse;
        }

        public JsonResult GetUserHistoryByID(int ID)
        {
            List<History_UsersEntity> HisUserDetails = null;
            try
            {
                var objDtl = new UserComponent();
                HisUserDetails = objDtl.GetHistoryUserDetailsByID(ID);
                ViewBag.UserDetails = HisUserDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(HisUserDetails, JsonRequestBehavior.AllowGet);
        }

        #region Export to PDF

        //private static void ExportPDF<TSource>(IList<TSource> UserList, string[] columns, string filePath)
        //{
        //    iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 10, iTextSharp.text.Color.WHITE);
        //    iTextSharp.text.Font rowfont = FontFactory.GetFont("Verdana", 10, iTextSharp.text.Color.BLUE);
        //    Document document = new Document(PageSize.A4);

        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate));
        //    document.Open();
        //    PdfPTable table = new PdfPTable(columns.Length);
        //    foreach (var column in columns)
        //    {
        //        PdfPCell cell = new PdfPCell(new Phrase(column, headerFont));
        //        cell.BackgroundColor = iTextSharp.text.Color.BLACK;
        //        table.AddCell(cell);
        //    }

        //    foreach (var item in UserList)
        //    {
        //        foreach (var column in columns)
        //        {
        //            string value = item.GetType().GetProperty(column).GetValue(item).ToString();
        //            PdfPCell cell5 = new PdfPCell(new Phrase(value, rowfont));
        //            table.AddCell(cell5);
        //        }
        //    }

        //    document.Add(table);
        //    document.Close();
        //}
        #endregion

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

        public JsonResult Months()
        {

            ViewBag.MonthNames = Month;
            return Months();
        }

        public IEnumerable<SelectListItem> Month
        {
            get
            {
                return DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((MonthNames, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = MonthNames
                       });
            }
        }

        public JsonResult GetTaskdropdown()
        {

            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();

            IEnumerable<SelectListItem> itemssss = db.GenericTasks.Select(c => new SelectListItem
            {

                Value = c.tsk_TaskDescription,
                Text = c.tsk_TaskDescription

            });

            ViewBag.taskname = itemssss;

            return Json(ViewBag.itemssss, JsonRequestBehavior.AllowGet);
        }

        #region Call GetGradesByID Method from Component
        public JsonResult Gettimesheet(int userid)
        {
            var admComp = new UserComponent();
            TimesheetEntity qec = admComp.Gettimesheet(userid);
            return Json(qec, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult Usertimesheet1()
        {
            // ViewBag.userid = Convert.ToInt32(Session["UserId"]);
            ViewBag.username = Session["UserName"].ToString();
            return View();
        }

        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            var objDtl = new UserComponent();
            strResponse = objDtl.ChangeStatus(id, status);

            return strResponse;
        }
    }

}
