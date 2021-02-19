using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Evolutyz.Business;
using EvolutyzCorner.UI.Web.Models;
using Evolutyz.Entities;
using Evolutyz.Data;
using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Drawing;
using System.IO;

namespace EvolutyzCorner.UI.Web.Controllers.LeaveManagement
{
    public class WrkFrmHomeCommentsController : Controller
    {
        SqlConnection Conn = new SqlConnection();
        public static string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static string host = System.Web.HttpContext.Current.Request.Url.Host;
        public static string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
        public static string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
        public static string UrlEmailAddress = string.Empty;
        public static string UrlEmailImage = string.Empty;


        string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static string mail;
        static string newleavid;
        static int uid;
        static string mgrname;
        static string mgr2name;
        static string mgr2id;
        static string mgr1id;
        static string email;
        static string id;
        static string name;
        static string newuwfhid;
        static string newemail;
        static string levelofmanager;
        static string FromMailAddress = "noreply@evolutyz.com";


        static string accmail = string.Empty;

        public ActionResult AddWFHComments(string userid, string status, string userwfhId, string managermail, string managerid, string managername, string startDate, string endDate)
        {
            Decript objdecrypt = new Decript();
            var newemail = accmail;
            var useriddd = objdecrypt.Decryption(userid);
            var leavid = objdecrypt.Decryption(userwfhId);
            var mngermail = objdecrypt.Decryption(managermail);
            var mngerid = objdecrypt.Decryption(managerid);
            var mngername = objdecrypt.Decryption(managername);
            int Usrid = uid;
            uid =Convert.ToInt32(useriddd);

            ManagerWFHforApproval obj = new ManagerWFHforApproval();
            obj = new ManagerWFHforApproval()
            {
                Usrl_UserId = Convert.ToInt32(useriddd),
                UserwfhID = Convert.ToInt32(leavid),
                ManagerID1 = Convert.ToInt32(mngerid),
                ManagerName1 = mngername,
                Leavestatus = status.ToString(),

            };
            obj = CheckMailWFHApproval(obj);

            if (obj.Message == "Already Approved")
            {
                ViewBag.msg = "Already Approved";
                return View();
            }

            else if (obj.Message == "Already Rejected")
            {
                ViewBag.msg = "Already Rejected";
                return View();
            }
            else if (obj.Message == "On Hold")
            {
                ViewBag.msg = "Already On Hold";
                return View();
            }
            else
            {
                managerwfhcomments wfhcmt = new managerwfhcomments();
                wfhcmt.WfhUserid = userid;
                wfhcmt.WfhStatuses = status;
                wfhcmt.userWFHId = userwfhId;
                wfhcmt.WfhManagerMail = managermail;
                wfhcmt.WfhManagerId = managerid;
                wfhcmt.WfhManagerName = managername;
                wfhcmt.WfhStartDate = startDate;
                wfhcmt.WfhEndDate = endDate;

                ViewBag.WfhUserid = wfhcmt.WfhUserid;
                ViewBag.WfhStatuses = wfhcmt.WfhStatuses;
                ViewBag.userWFHId = wfhcmt.userWFHId;
                ViewBag.WfhManagerMail = wfhcmt.WfhManagerMail;
                ViewBag.WfhManagerId = wfhcmt.WfhManagerId;
                ViewBag.WfhManagerName = wfhcmt.WfhManagerName;
                ViewBag.WfhStartDate = wfhcmt.WfhStartDate;
                ViewBag.WfhEndDate = wfhcmt.WfhEndDate;
                return View(wfhcmt);

            }
        }


        public ManagerWFHforApproval CheckMailWFHApproval(ManagerWFHforApproval objWFH)
        {
            ManagerWFHforApproval objLeaves = new ManagerWFHforApproval();
            int Trans_Output = 0;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            try
            {
                Conn = new SqlConnection(str);
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[UserWFHManagerActionsfromEmail]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@userid", objWFH.Usrl_UserId);
                objCommand.Parameters.AddWithValue("@WFHID", objWFH.UserwfhID);
                objCommand.Parameters.AddWithValue("@ManagerId", objWFH.ManagerID1);
                objCommand.Parameters.AddWithValue("@StatusType", objWFH.Leavestatus);
                objCommand.Parameters.AddWithValue("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                if (Trans_Output == 0)
                {
                    objWFH = new ManagerWFHforApproval()
                    {
                        Message = "Waiting for Approval",
                    };
                }

                if (Trans_Output == 111)
                {
                    objWFH = new ManagerWFHforApproval()
                    {
                        Message = "Already Approved",
                    };
                }
                if (Trans_Output == 112)
                {
                    objWFH = new ManagerWFHforApproval()
                    {
                        Message = "Already Rejected",
                    };
                }
                if (Trans_Output == 113)
                {
                    objWFH = new ManagerWFHforApproval()
                    {
                        Message = "On Hold",
                    };
                }
                //if (Trans_Output == 114)
                //{
                //    objWFH = new ManagerWFHforApproval()
                //    {
                //        Message = "Leave Already Approved"
                //    };
                //}
                //if (Trans_Output == 115)
                //{
                //    objWFH = new ManagerWFHforApproval()
                //    {
                //        Message = "Leave Already Rejected"
                //    };
                //}
            }
            catch (Exception ex)
            {
                throw ex;
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
            return objWFH;
        }

        public string WorkFromHomeStatus(string cmt, string userid, string status, string leaveid, string managermail, string managerid, string managername, string startDate, string endDate)
        {

            string LeavId = String.Empty, mangrmail = String.Empty, mangrname = String.Empty, mangrid = String.Empty;
            string UrlEmailImage = string.Empty;

            Decript objdecrypt = new Decript();
            var newemail = accmail;
            var useriddd = objdecrypt.Decryption(userid);
            var leavid = objdecrypt.Decryption(leaveid);
            var mngermail = objdecrypt.Decryption(managermail);
            var mngerid = objdecrypt.Decryption(managerid);
            var mngername = objdecrypt.Decryption(managername);
            int Usrid = uid;

            //LeavId = newuwfhid;
            //mangrmail = email;
            //mangrid = id;
            //mangrname = name;

            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var getusermailid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
            var getaccntid = (from u in db.Users where u.Usr_UserID == uid select u.Usr_AccountID).FirstOrDefault();
            var getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
            // UrlEmailImage = "<img alt='Company Logo' style='height:100px;display:block;margin:auto;max-width:100%;' src='" + "https://" + UrlEmailAddress + "/uploadimages/Images/" + getaccntlogo + "'";
            UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'";
            var emailcontent = "";

            ManagerWFHforApproval obj = new ManagerWFHforApproval();
            obj = new ManagerWFHforApproval()
            {
                Usrl_UserId = Convert.ToInt32(useriddd),
                UserwfhID = Convert.ToInt32(leavid),
                ManagerID1 = Convert.ToInt32(mngerid),
                ManagerName1 = mngername,
                Leavestatus = status.ToString(),

            };
            obj = CheckMailWFHApproval(obj);

            if (obj.Message == "Already approved")
            {
                return obj.Message;
            }

            else if (obj.Message == "Already Rejected")
            {
                return obj.Message;
            }
            else if (obj.Message == "On Hold")
            {
                return obj.Message;
            }
            //else if(obj.Message == "Leave Already Approved")
            //{
            //    return obj.Message;
            //}
            //else if(obj.Message == "Leave Already Rejected")
            //{
            //    return obj.Message;
            //}
            else
            {

                if (status == "1")
                {
                    emailcontent = "<html>" +
                    "<body bgcolor='#f6f6f6' style='-webkit-font-smoothing: antialiased; background:#f6f6f6; margin:0 auto; padding:0;'>" +
                    "<div style='background:#f4f4f4; border: 0px solid #f4f4f4; margin:0; padding:0'>" +
                    "<center>" +
                    "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding-bottom:0px; padding-top:0px valign='top'>" +
                    "<a href='javascript:;' style='color: #7DA33A;text-decoration: none;display: block;' target='_blank'>" +
                    //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                    UrlEmailImage +
                    "</a>" +
                    "</td>" +
                    " </tr>" +
                    " <tr>" +
                    "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 0px' valign='top'>" +
                    " </td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td align='center' bgcolor='#fffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 20px' valign=top>" +
                    " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Work From Home</h2>" +
                    "</td>" +
                    " </tr>" +
                    "</tbody>" +
                    "</table>" +
                    "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td align='center'>" +
                    "<table bgcolor='#ffffff' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; border: 1px solid #d1d2d1; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:96%'>" +
                    " <tbody>" +
                    " <tr>" +
                    "<td align=center bgcolor='#ffffff' style='background: #ffffff; padding-bottom:0px'; padding-top:0' valign='top'>" +
                    "<table align='center' border='0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:80%'>" +
                    " <tbody>" +
                    "<tr>" +
                    "<td align='left' bgcolor='#ffffff' style='background: #ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding:10px 0px 20px' valign='top'>" +
                    "<table align='center' border='0' cellpadding='0' cellspacing='0' style='border-collapse:collapse !important; max-width:600px; width:100%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td align='left' bgcolor='#ffffff' style=background:'#ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; max-width:80%; padding:0px; width:40%' valign='top'>" +
                    "<p align='left' style='color: #707070; font-size:14px; font-weight:400; line-height:22px; padding-bottom:0px; text-align:left'>" +
                    "<strong>" +
                    "Work From Home Approved By" +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 0px; width: 40%' valign='top'>" +
                    "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                     name +
                    "</p>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "<hr>" +
                    "<table align ='center' border = '0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                    " <tbody>" +
                    "<tr>" +
                    "<td align = 'left' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    "<p align = 'left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    "<strong>" +
                    " FromDate " +
                    " </strong>" +
                    "<br> " +
                    "</p>" +
                    " </td>" +
                    "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    "<strong>" +
                    startDate +
                    "</strong>" +
                    "</p>" +
                    "</td>" +
                    "</tr>" +
                    "<tr>" +
                    "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    "<strong> " +
                    " ToDate " +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    "<strong> " +
                    endDate +
                    "</strong>" +
                    "</p>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "<table align = center bgcolor = #ffffff border =0 cellpadding =0 cellspacing = 0 style = 'background: #ffffff; border-collapse: collapse !important; border-top-color: #D1D2D1; border-top-style: solid; border-top-width: 1px; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                    "<tbody>" +
                    " <tr>" +
                    "<td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding: 10x 0px 20px' valign='middle'>" +
                    "<table align = center border = 0 cellpadding = 0 cellspacing = 0 style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 80%'>" +
                    "<tbody>" +
                    "<tr>" +
                    " <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    "<strong> " +
                    "Comments " +
                    "</strong>" +
                    "</p>" +
                    "</td>" +
                    "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    "<strong>" +
                     cmt +
                    "</strong>" +
                    "</p>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    "</td>" +
                    "</tr>" +
                    "</tbody>" +
                    "</table>" +
                    " </td>" +
                    "</tr>" +
                    "</tbody>" +
                    " </table>" +
                    "<table bgcolor = #ffffff cellpadding =0 cellspacing =0 class=mobile style='background: #ffffff; border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 96%'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td align = center style='padding-bottom: 20px' valign=top>" +
                    "<table align = center border=0 cellpadding=0 cellspacing=0 style='border-collapse: collapse !important; mso-table-lspace: 0pt; mso-table-rspace: 0pt'>" +
                    "<tbody>" +
                    "<tr>" +
                    "<td align = center style='padding-bottom: 0px; padding-top: 30px' valign=top>" +
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
                    "</center>" +
                    "</div>" +
                    "</body>" +
                    "</html>";
                    //RestClient client = new RestClient();
                    //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                    //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
                    //RestRequest request = new RestRequest();
                    //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                    //request.Resource = "{domain}/messages";
                    //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");
                    //request.AddParameter("to", getusermailid);
                    //request.AddParameter("to", newemail);
                    //request.AddParameter("subject", "Work From Home Approved");
                    //request.AddParameter("html", emailcontent);
                    //request.Method = Method.POST;
                    //client.Execute(request);

                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        // From = new EmailAddress(mngermail),
                        From = new EmailAddress(FromMailAddress),
                        Subject = "Work From Home Approved",
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(getusermailid));

                    var responses = client.SendEmailAsync(msgs);
                }
                else
                {
                    if (status == "2")
                    {
                        emailcontent = "<html>" +
                   "<body bgcolor='#f6f6f6' style='-webkit-font-smoothing: antialiased; background:#f6f6f6; margin:0 auto; padding:0;'>" +
                   "<div style='background:#f4f4f4; border: 0px solid #f4f4f4; margin:0; padding:0'>" +
                   "<center>" +
                   "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                   "<tbody>" +
                   "<tr>" +
                   "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding-bottom:0px; padding-top:0px valign='top'>" +
                   "<a href='javascript:;' style='color: #7DA33A;text-decoration: none;display: block;' target='_blank'>" +
                    //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                    UrlEmailImage +
                   "</a>" +
                   "</td>" +
                   " </tr>" +
                   " <tr>" +
                   "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 0px' valign='top'>" +
                   " </td>" +
                   "</tr>" +
                   "<tr>" +
                   "<td align='center' bgcolor='#fffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 20px' valign=top>" +
                   " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Work From Home</h2>" +
                   "</td>" +
                   " </tr>" +
                   "</tbody>" +
                   "</table>" +
                   "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                   "<tbody>" +
                   "<tr>" +
                   "<td align='center'>" +
                   "<table bgcolor='#ffffff' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; border: 1px solid #d1d2d1; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:96%'>" +
                   " <tbody>" +
                   " <tr>" +
                   "<td align=center bgcolor='#ffffff' style='background: #ffffff; padding-bottom:0px'; padding-top:0' valign='top'>" +
                   "<table align='center' border='0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:80%'>" +
                   " <tbody>" +
                   "<tr>" +
                   "<td align='left' bgcolor='#ffffff' style='background: #ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding:10px 0px 20px' valign='top'>" +
                   "<table align='center' border='0' cellpadding='0' cellspacing='0' style='border-collapse:collapse !important; max-width:600px; width:100%'>" +
                   "<tbody>" +
                   "<tr>" +
                   "<td align='left' bgcolor='#ffffff' style=background:'#ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; max-width:80%; padding:0px; width:40%' valign='top'>" +
                   "<p align='left' style='color: #707070; font-size:14px; font-weight:400; line-height:22px; padding-bottom:0px; text-align:left'>" +
                   "<strong>" +
                   "Work From Home Rejected By" +
                   " </strong>" +
                   "<br>" +
                   "</p>" +
                   "</td>" +
                   "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width:80%; padding:0px; width:40%' valign='top'>" +
                   "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                    name +
                   "</p>" +
                   "</td>" +
                   "</tr>" +
                   "</tbody>" +
                   "</table>" +
                   "<hr>" +
                   "<table align ='center' border = '0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                   " <tbody>" +
                   "<tr>" +
                   "<td align = 'left' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                   "<p align = 'left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                   "<strong>" +
                   " FromDate " +
                   " </strong>" +
                   "<br> " +
                   "</p>" +
                   " </td>" +
                   "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                   "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                   "<strong>" +
                   startDate +
                   "</strong>" +
                   "</p>" +
                   "</td>" +
                   "</tr>" +
                   "<tr>" +
                   "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                   "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                   "<strong> " +
                   " ToDate " +
                   " </strong>" +
                   "<br>" +
                   "</p>" +
                   "</td>" +
                   "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                   "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                   "<strong> " +
                   endDate +
                   "</strong>" +
                   "</p>" +
                   "</td>" +
                   "</tr>" +
                   "</tbody>" +
                   "</table>" +
                   "</td>" +
                   "</tr>" +
                   "</tbody>" +
                   "</table>" +
                   "<table align = center bgcolor = #ffffff border =0 cellpadding =0 cellspacing = 0 style = 'background: #ffffff; border-collapse: collapse !important; border-top-color: #D1D2D1; border-top-style: solid; border-top-width: 1px; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                   "<tbody>" +
                   " <tr>" +
                   "<td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding: 10x 0px 20px' valign='middle'>" +
                   "<table align = center border = 0 cellpadding = 0 cellspacing = 0 style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 80%'>" +
                   "<tbody>" +
                   "<tr>" +
                   " <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                   "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                   "<strong> " +
                   "Comments " +
                   "</strong>" +
                   "</p>" +
                   "</td>" +
                   "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                   "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                   "<strong>" +
                    cmt +
                   "</strong>" +
                   "</p>" +
                   "</td>" +
                   "</tr>" +
                   "</tbody>" +
                   "</table>" +
                   "</td>" +
                   "</tr>" +
                   "</tbody>" +
                   "</table>" +
                   " </td>" +
                   "</tr>" +
                   "</tbody>" +
                   " </table>" +
                   "<table bgcolor = #ffffff cellpadding =0 cellspacing =0 class=mobile style='background: #ffffff; border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 96%'>" +
                   "<tbody>" +
                   "<tr>" +
                   "<td align = center style='padding-bottom: 20px' valign=top>" +
                   "<table align = center border=0 cellpadding=0 cellspacing=0 style='border-collapse: collapse !important; mso-table-lspace: 0pt; mso-table-rspace: 0pt'>" +
                   "<tbody>" +
                   "<tr>" +
                   "<td align = center style='padding-bottom: 0px; padding-top: 30px' valign=top>" +
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
                   "</center>" +
                   "</div>" +
                   "</body>" +
                   "</html>";
                        //RestClient client = new RestClient();
                        //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                        //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
                        //RestRequest request = new RestRequest();
                        //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                        //request.Resource = "{domain}/messages";
                        //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");
                        //request.AddParameter("to", getusermailid);
                        //request.AddParameter("to", newemail);
                        //request.AddParameter("subject", "Work From Home Rejected");
                        //request.AddParameter("html", emailcontent);
                        //request.Method = Method.POST;
                        //client.Execute(request);
                        var client = new SendGridClient("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");

                        var msgs = new SendGridMessage()
                        {
                            //From = new EmailAddress(mngermail),
                            From = new EmailAddress(FromMailAddress),
                            Subject = "Work From Home Rejected",
                            //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                            HtmlContent = emailcontent

                        };
                        msgs.AddTo(new EmailAddress(getusermailid));
                        var responses = client.SendEmailAsync(msgs);

                    }
                    else
                    {
                        emailcontent = "<html>" +
                "<body bgcolor='#f6f6f6' style='-webkit-font-smoothing: antialiased; background:#f6f6f6; margin:0 auto; padding:0;'>" +
                "<div style='background:#f4f4f4; border: 0px solid #f4f4f4; margin:0; padding:0'>" +
                "<center>" +
                "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding-bottom:0px; padding-top:0px valign='top'>" +
                "<a href='javascript:;' style='color: #7DA33A;text-decoration: none;display: block;' target='_blank'>" +
                //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                UrlEmailImage +
                "</a>" +
                "</td>" +
                " </tr>" +
                " <tr>" +
                "<td align='center' bgcolor='#ffffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 0px' valign='top'>" +
                " </td>" +
                "</tr>" +
                "<tr>" +
                "<td align='center' bgcolor='#fffff' style='background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding-bottom: 20px; padding-top: 20px' valign=top>" +
                " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Work From Home</h2>" +
                "</td>" +
                " </tr>" +
                "</tbody>" +
                "</table>" +
                "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td align='center'>" +
                "<table bgcolor='#ffffff' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; border: 1px solid #d1d2d1; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:96%'>" +
                " <tbody>" +
                " <tr>" +
                "<td align=center bgcolor='#ffffff' style='background: #ffffff; padding-bottom:0px'; padding-top:0' valign='top'>" +
                "<table align='center' border='0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:80%'>" +
                " <tbody>" +
                "<tr>" +
                "<td align='left' bgcolor='#ffffff' style='background: #ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding:10px 0px 20px' valign='top'>" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' style='border-collapse:collapse !important; max-width:600px; width:100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td align='left' bgcolor='#ffffff' style=background:'#ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; max-width:80%; padding:0px; width:40%' valign='top'>" +
                "<p align='left' style='color: #707070; font-size:14px; font-weight:400; line-height:22px; padding-bottom:0px; text-align:left'>" +
                "<strong>" +
                "Work From Home kept on hold by" +
                " </strong>" +
                "<br>" +
                "</p>" +
                "</td>" +
                "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width:80%; padding:0px; width:40%' valign='top'>" +
                "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                 name +
                 "</p>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "<hr>" +
                "<table align ='center' border = '0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                " <tbody>" +
                "<tr>" +
                "<td align = 'left' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                "<p align = 'left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                "<strong>" +
                " FromDate " +
                " </strong>" +
                "<br> " +
                "</p>" +
                " </td>" +
                "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                "<strong>" +
                startDate +
                "</strong>" +
                "</p>" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                "<strong> " +
                " ToDate " +
                " </strong>" +
                "<br>" +
                "</p>" +
                "</td>" +
                "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                "<strong> " +
                endDate +
                "</strong>" +
                "</p>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "<table align = center bgcolor = #ffffff border =0 cellpadding =0 cellspacing = 0 style = 'background: #ffffff; border-collapse: collapse !important; border-top-color: #D1D2D1; border-top-style: solid; border-top-width: 1px; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
                "<tbody>" +
                " <tr>" +
                "<td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding: 10x 0px 20px' valign='middle'>" +
                "<table align = center border = 0 cellpadding = 0 cellspacing = 0 style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 80%'>" +
                "<tbody>" +
                "<tr>" +
                " <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                "<strong> " +
                "Comments " +
                "</strong>" +
                "</p>" +
                "</td>" +
                "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                "<strong>" +
                cmt +
                "</strong>" +
                "</p>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                " </td>" +
                "</tr>" +
                "</tbody>" +
                " </table>" +
                "<table bgcolor = #ffffff cellpadding =0 cellspacing =0 class=mobile style='background: #ffffff; border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 96%'>" +
                "<tbody>" +
                "<tr>" +
                "<td align = center style='padding-bottom: 20px' valign=top>" +
                "<table align = center border=0 cellpadding=0 cellspacing=0 style='border-collapse: collapse !important; mso-table-lspace: 0pt; mso-table-rspace: 0pt'>" +
                "<tbody>" +
                "<tr>" +
                "<td align = center style='padding-bottom: 0px; padding-top: 30px' valign=top>" +
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
                "</center>" +
                "</div>" +
                "</body>" +
                "</html>";

                        //RestClient client = new RestClient();
                        //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                        //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
                        //RestRequest request = new RestRequest();
                        //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                        //request.Resource = "{domain}/messages";
                        //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");
                        //request.AddParameter("to", getusermailid);
                        //request.AddParameter("to", newemail);
                        //request.AddParameter("subject", "Work from Home OnHold");
                        //request.AddParameter("html", emailcontent);
                        //request.Method = Method.POST;
                        //client.Execute(request);

                        var client = new SendGridClient("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");

                        var msgs = new SendGridMessage()
                        {
                            //From = new EmailAddress(mngermail),
                            From = new EmailAddress(FromMailAddress),
                            Subject = "Work from Home OnHold",
                            //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                            HtmlContent = emailcontent

                        };
                        msgs.AddTo(new EmailAddress(getusermailid));
                        var responses = client.SendEmailAsync(msgs);
                    }
                }

                AdminComponent admc = new AdminComponent();

                //var res = admc.SaveWorkFromHomeStatus(newuwfhid, status, id);leavid, status, mngerid, useriddd
                var res = admc.SaveWorkFromHomeStatus(cmt, leavid, status, mngerid, useriddd);
                obj.Message = res;
            }

            // return null;
            return obj.Message;

        }
    }
}