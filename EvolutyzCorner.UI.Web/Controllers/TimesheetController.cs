using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using EvolutyzCorner.UI.Web.Models;
using System.Configuration;
using evolCorner.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Globalization;
using System.Drawing.Imaging;
using System.Text;
using System.Web.Script.Serialization;

namespace EvolutyzCorner.UI.Web.Controllers
{
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class TimesheetController : Controller
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string htmlStr = "";
        public int timeSheetID = 0;
        SqlConnection Conn = new SqlConnection();
        SqlDataAdapter da = new SqlDataAdapter(); DataSet ds = new DataSet(); DataTable dt = new DataTable();
        DataSet ds1 = new DataSet(); DataTable dtheadings = new DataTable(); DataTable dtData = new DataTable();
        EmailFormats objemail = new EmailFormats(); string StatusMsg = string.Empty;
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        timesheet lstusers = new timesheet();
        ClientComponent obj = new ClientComponent();
        static int UpId;
        public ActionResult Index()
        {
            return View();
        }


        //

        #region Insertion (addSubmitTimeSheet)
        [HttpPost]
        public ActionResult AddSubmitTimesheet(TotalTimeSheetTimeDetails sheetObj)
        {

            Conn = new SqlConnection(str);
            // var news = TempData["errorNotification"];
            List<string> data = TempData["errorNotification"] as List<string>;
            //  SendEmail objMail = new SendEmail();Admin@evolutyz.in
            UserSessionInfo info = new UserSessionInfo();
            int? userid = info.UserId;
            int Trans_Output = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand();


                if (sheetObj.timesheets.TimesheetMode == 2 || sheetObj.timesheets.TimesheetMode == 3 || sheetObj.timesheets.TimesheetMode == 4)
                {
                    objCommand = new SqlCommand("[AddSubmitTimesheetWebByWeekly]", Conn);
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                    objCommand.Parameters.AddWithValue("@TimesheetMonth", Convert.ToDateTime(sheetObj.timesheets.TimeSheetMonth).ToShortDateString());
                    objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                    objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
                    objCommand.Parameters.AddWithValue("@ByWeeklyStartDate", Convert.ToDateTime(sheetObj.timesheets.ByWeeklyStartDate).ToShortDateString());
                    objCommand.Parameters.AddWithValue("@ClientProjtId", sheetObj.timesheets.ClientProjectId);
                    objCommand.Parameters.AddWithValue("@ByWeeklyEndDate", Convert.ToDateTime(sheetObj.timesheets.ByWeeklyEndDate).ToShortDateString());
                    objCommand.Parameters.Add("@TimesheetID", SqlDbType.Int);
                    objCommand.Parameters["@TimesheetID"].Direction = ParameterDirection.Output;
                    objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                    objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                    objCommand.ExecuteNonQuery();
                    timeSheetID = int.Parse(objCommand.Parameters["@TimesheetID"].Value.ToString());
                    Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                }

                else
                {

                    objCommand = new SqlCommand("[AddSubmitTimesheetWeb]", Conn);
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                    objCommand.Parameters.AddWithValue("@TimesheetMonth", Convert.ToDateTime(sheetObj.timesheets.TimeSheetMonth).ToShortDateString());
                    objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                    //  objCommand.Parameters.AddWithValue("@ProjectID", sheetObj.timesheets.ProjectID);
                    objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
                    objCommand.Parameters.AddWithValue("@L1ApproverStatus", false);
                    objCommand.Parameters.AddWithValue("@L2ApproverStatus", false);
                    objCommand.Parameters.AddWithValue("@ClientProjtId", sheetObj.timesheets.ClientProjectId);
                    objCommand.Parameters.Add("@TimesheetID", SqlDbType.Int);
                    objCommand.Parameters["@TimesheetID"].Direction = ParameterDirection.Output;
                    objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                    objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                    objCommand.ExecuteNonQuery();
                    timeSheetID = int.Parse(objCommand.Parameters["@TimesheetID"].Value.ToString());
                    Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());

                }

                if (timeSheetID > 0)
                {
                    int dbprojectid = 0;
                    DataSet set = new DataSet();
                    using (SqlCommand cmd = new SqlCommand("GgetProjectIdByTimeshhet", Conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TimesheetID", timeSheetID);

                        using (SqlDataAdapter ADP = new SqlDataAdapter(cmd))
                        {
                            ADP.Fill(set);
                        }
                    }

                    DataTable dbdt = new DataTable();
                    if (set.Tables.Count > 0)
                    {
                        dbdt = set.Tables["Table"];
                        if (dbdt.Rows.Count >= 1)
                        {
                            foreach (DataRow dr in dbdt.Rows)
                            {
                                dbprojectid = Convert.ToInt32(dr["Proj_ProjectID"]);
                            }
                        }
                    }



                    foreach (var item in sheetObj.listtimesheetdetails)
                    {

                        if ((Trans_Output == 105) || (Trans_Output == 106))
                        {
                            objCommand = new SqlCommand("[EditSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid == 0 ? dbprojectid : item.projectid);
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


                            }

                            objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                            objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                            objCommand.ExecuteNonQuery();

                        }
                        else
                        {
                            objCommand = new SqlCommand("[AddSubmitTaskDetails]", Conn);
                            objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            objCommand.Parameters.AddWithValue("@TimesheetID", timeSheetID);
                            objCommand.Parameters.AddWithValue("@ProjectID", item.projectid == 0 ? dbprojectid : item.projectid);
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


                            }
                            objCommand.ExecuteNonQuery();
                        }

                    }


                }


                Conn.Close();
                if (Trans_Output == 1)
                {

                    var saveimages = obj.SavetimeSheetImages(timeSheetID, userid, data, UpId);
                    if (sheetObj.timesheets.SubmittedType == "Submit")
                    {
                        SendMailsForApprovals(sheetObj, timeSheetID, userid, data);
                        StatusMsg = "TimeSheet Submitted Successfully..";
                        // var saveimages = obj.SavetimeSheetImages(timeSheetID, userid, data);
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
                        SendMailsForApprovals(sheetObj, timeSheetID, userid, data);
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

        #region SendMails
        public string SendMailsForApprovals(TotalTimeSheetTimeDetails sheetObj, int timesheetid, int? userid, List<string> data)
        {
            Conn = new SqlConnection(str); int Userid = 0; int sessionuserid = 0;
            DataTable dtaccMgr = new DataTable();
            var getprojid = sheetObj.timesheets.ClientProjectId;
            if (getprojid == 0)
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand1 = new SqlCommand("GET_ClientProjectId_Basedon_TimesheetId", Conn);
                objCommand1.CommandType = CommandType.StoredProcedure;
                objCommand1.Parameters.AddWithValue("@TimeSheet", timesheetid);

                SqlDataAdapter daCP = new SqlDataAdapter();

                daCP = new SqlDataAdapter(objCommand1);
                DataSet DSCI = new DataSet();
                daCP.Fill(DSCI);

                DataTable DTCI = new DataTable();
                DTCI = DSCI.Tables[0];
                getprojid = Convert.ToInt32(DTCI.Rows[0]["ClientProjtId"]);
            }

            List<manageremails> objManagerlist = new List<manageremails>();
            if ((sheetObj.timesheets.UserID != 0))
            {
                Userid = sheetObj.timesheets.UserID;
            }
            else
            {
                UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
                sessionuserid = sessId.UserId;
                Userid = sessionuserid;

            }
            var AdminEmailid = (from u in db.Users where u.Usr_UserID == Userid select u.Usr_LoginId).FirstOrDefault();
            var Adminname = (from u in db.UsersProfiles where u.UsrP_UserID == Userid select u.UsrP_FirstName).FirstOrDefault();

            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[GetManagerEmailIdsTesting]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@userid", Userid);
                objCommand.Parameters.AddWithValue("@ClientId", getprojid);

                da = new SqlDataAdapter(objCommand);

                da.Fill(ds);
                dt = ds.Tables[0];
                dtaccMgr = ds.Tables[1];
                SqlDataReader dr = objCommand.ExecuteReader();

                if (dt != null)
                {
                    lstusers = new timesheet()
                    {
                        ManagerEmail1 = dt.Rows[0]["ManagerLevel1"].ToString(),
                        ManagerEmail2 = dt.Rows[0]["ManagerLevel2"].ToString(),
                        AccManagerEmail = dt.Rows[0]["Acc_EmailID"].ToString(),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        UserEmailId = dt.Rows[0]["UserEmailid"].ToString(),
                        ManagerID1 = dt.Rows[0]["L1_ManagerID"].ToString(),
                        ManagerID2 = dt.Rows[0]["L2_ManagerID"].ToString(),
                        ManagerId = sheetObj.timesheets.ManagerId,
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        SubmittedFlag = sheetObj.timesheets.SubmittedFlag,
                        ManagerName1 = dt.Rows[0]["L1_ManagerName"].ToString(),
                        ManagerName2 = dt.Rows[0]["L2_ManagerName"].ToString(),
                        AccountImageLogo = dt.Rows[0]["Acc_CompanyLogo"].ToString(),
                        Is_DirectManager = Convert.ToInt32(dt.Rows[0]["IsL2_DirectManager"]),
                        Admin_Name = Adminname,
                        Admin_Mailid = AdminEmailid,
                        ClientProjectId = getprojid,
                    };

                }

                foreach (DataRow accmanager in ds.Tables[1].Rows)
                {

                    objManagerlist.Add(new manageremails
                    {
                        manageremail = accmanager["Usr_LoginId"].ToString(),

                    });

                }

                Conn.Close();
                Conn.Open();


                //var checkTimesheettypeMode = (from up in db.UserProjects where up.UProj_UserID == Userid select up.TimesheetMode).FirstOrDefault();


                var checkTimesheettypeMode = (from up in db.UserProjects where up.UProj_UserID == Userid && up.ClientprojID == getprojid select up.TimesheetMode).FirstOrDefault();

                if ((checkTimesheettypeMode == 2) || (checkTimesheettypeMode == 3) || (checkTimesheettypeMode == 4))
                {
                    objCommand = new SqlCommand("[ByWeeklygetTimeSheetEmailDetails]", Conn);
                }
                else
                {
                    objCommand = new SqlCommand("[getTimeSheetEmailDetails]", Conn);

                }
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@userid", Userid);

                objCommand.Parameters.AddWithValue("@Timesheetmonth", Convert.ToDateTime(sheetObj.timesheets.TimeSheetMonth).ToShortDateString());
                objCommand.Parameters.AddWithValue("@clientPrjId", getprojid);

                ds1 = new DataSet();
                da = new SqlDataAdapter(objCommand);
                da.Fill(ds1);
                dtheadings = new DataTable();
                dtData = new DataTable();
                dtheadings = ds1.Tables[0];
                dtData = ds1.Tables[1];
                string subject = string.Empty;
                //dtheadings.Columns.Add(new DataColumn("Comments"));




                // dtheadings.Columns.Add("comments", sheetObj.timesheets.Comments);
                // dtheadings.
                if ((!string.IsNullOrEmpty(sheetObj.timesheets.SubmittedType)) && (sheetObj.timesheets.SubmittedType == "Submit"))
                {

                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "1"));
                    SendMailsBySendGrid(lstusers, objManagerlist, dtheadings, body, 1, "", timeSheetID, userid, data);

                }
                dtheadings.Rows[0]["comments"] = sheetObj.timesheets.Comments;
                //if ((!string.IsNullOrEmpty(sheetObj.timesheets.SubmittedFlag)) && sheetObj.timesheets.SubmittedFlag.ToString() == "2" || sheetObj.timesheets.SubmittedFlag==null)
                if ((!string.IsNullOrEmpty(sheetObj.timesheets.SubmittedFlag)) && sheetObj.timesheets.SubmittedFlag.ToString() == "2")

                {
                    if ((sheetObj.timesheets.Transoutput == 1) || (sheetObj.timesheets.Transoutput == 2) || (sheetObj.timesheets.Transoutput == 3) || (sheetObj.timesheets.Transoutput == 4))

                    //if ((sheetObj.timesheets.Transoutput == 0)||(sheetObj.timesheets.Transoutput == 1) || (sheetObj.timesheets.Transoutput == 2) || (sheetObj.timesheets.Transoutput == 3) || (sheetObj.timesheets.Transoutput == 4))
                    {
                        string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                        StatusMsg = SendMailsBySendGrid(lstusers, objManagerlist, dtheadings, body, 2, sheetObj.timesheets.ActionType, timeSheetID, userid, data);
                        return StatusMsg;
                    }

                    else
                    {
                        string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "3"));
                        return body;
                    }



                }

                if (sheetObj.timesheets.SubmittedType.ToString() == "3")
                {
                    UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
                    sessionuserid = sessId.UserId;
                    lstusers = new timesheet()
                    {
                        ManagerEmail1 = dt.Rows[0]["ManagerLevel1"].ToString(),
                        ManagerEmail2 = dt.Rows[0]["ManagerLevel2"].ToString(),
                        AccManagerEmail = dt.Rows[0]["Acc_EmailID"].ToString(),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        UserEmailId = dt.Rows[0]["UserEmailid"].ToString(),
                        ManagerID1 = dt.Rows[0]["L1_ManagerID"].ToString(),
                        ManagerID2 = dt.Rows[0]["L2_ManagerID"].ToString(),
                        EmailAppOrRejStatus = "1",
                        ManagerId = Convert.ToString(sessionuserid),
                        ManagerName1 = dt.Rows[0]["L1_ManagerName"].ToString(),
                        ManagerName2 = dt.Rows[0]["L2_ManagerName"].ToString(),
                        AccountImageLogo = dt.Rows[0]["Acc_CompanyLogo"].ToString(),
                        Admin_Name = Adminname,
                        Admin_Mailid = AdminEmailid,
                        Is_DirectManager = Convert.ToInt32(dt.Rows[0]["IsL2_DirectManager"]),

                    };
                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                    SendMailsBySendGrid(lstusers, objManagerlist, dtheadings, body, 2, "2", timeSheetID, userid, data);
                    StatusMsg = lstusers.ManagerName1 + "-" + lstusers.ManagerName2;

                    //else
                    //{
                    //    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                    //    StatusMsg = SendMailsByMailGun(lstusers, objManagerlist, dtheadings, body, 2, "2");
                    //    return StatusMsg;
                    //}

                }

                if (sheetObj.timesheets.SubmittedType.ToString() == "4")
                {
                    UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
                    sessionuserid = sessId.UserId;
                    string roleid = sessId.RoleId.ToString();
                    lstusers = new timesheet()
                    {
                        ManagerEmail1 = dt.Rows[0]["ManagerLevel1"].ToString(),
                        ManagerEmail2 = dt.Rows[0]["ManagerLevel2"].ToString(),
                        AccManagerEmail = dt.Rows[0]["Acc_EmailID"].ToString(),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        UserEmailId = dt.Rows[0]["UserEmailid"].ToString(),
                        ManagerID1 = dt.Rows[0]["L1_ManagerID"].ToString(),
                        ManagerID2 = dt.Rows[0]["L2_ManagerID"].ToString(),
                        EmailAppOrRejStatus = "0",
                        ManagerId = Convert.ToString(roleid),
                        ManagerName1 = dt.Rows[0]["L1_ManagerName"].ToString(),
                        ManagerName2 = dt.Rows[0]["L2_ManagerName"].ToString(),
                        AccountImageLogo = dt.Rows[0]["Acc_CompanyLogo"].ToString(),
                        Admin_Name = Adminname,
                        Admin_Mailid = AdminEmailid,
                        Is_DirectManager = Convert.ToInt32(dt.Rows[0]["IsL2_DirectManager"]),

                    };
                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                    SendMailsBySendGrid(lstusers, objManagerlist, dtheadings, body, 2, "2", timeSheetID, userid, data);
                    StatusMsg = lstusers.ManagerName1 + "-" + lstusers.ManagerName2;

                }

                if (sheetObj.timesheets.SubmittedType.ToString() == "5")
                {
                    UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
                    sessionuserid = sessId.UserId;
                    string RoleName = sessId.RoleName;
                    string roleid = sessId.RoleId.ToString();
                    lstusers = new timesheet()
                    {
                        ManagerEmail1 = dt.Rows[0]["ManagerLevel1"].ToString(),
                        ManagerEmail2 = dt.Rows[0]["ManagerLevel2"].ToString(),
                        AccManagerEmail = dt.Rows[0]["Acc_EmailID"].ToString(),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        UserEmailId = dt.Rows[0]["UserEmailid"].ToString(),
                        ManagerID1 = dt.Rows[0]["L1_ManagerID"].ToString(),
                        ManagerID2 = dt.Rows[0]["L2_ManagerID"].ToString(),
                        EmailAppOrRejStatus = "0",
                        ManagerId = Convert.ToString(roleid),
                        ManagerName1 = dt.Rows[0]["L1_ManagerName"].ToString(),
                        ManagerName2 = dt.Rows[0]["L2_ManagerName"].ToString(),
                        AccountImageLogo = dt.Rows[0]["Acc_CompanyLogo"].ToString(),
                        Admin_Name = sheetObj.timesheets.Admin_Name,
                        Admin_Mailid = sheetObj.timesheets.Admin_Mailid,
                        Is_DirectManager = Convert.ToInt32(dt.Rows[0]["IsL2_DirectManager"]),

                    };
                    string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "2"));
                    SendMailsBySendGrid(lstusers, objManagerlist, dtheadings, body, 2, "2", timeSheetID, userid, data);
                    // StatusMsg = lstusers.ManagerName1 + "-" + lstusers.ManagerName2;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return StatusMsg;

        }
        #endregion

        protected static string GetBase64StringForImage(string imgPath)
        {
            //byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            //string base64String = Convert.ToBase64String(imageBytes);
            //return base64String;
            System.Drawing.Image image = System.Drawing.Image.FromFile(imgPath);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            Byte[] bytes = new Byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(bytes, 0, (int)bytes.Length);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            string imageUrl = "data:image/png;base64," + base64String;
            return imageUrl;
        }

        static string baseimageone;
        static string baseimagetwo;
        #region ConvertDataTableToHTML 
        public static string ConvertDataTableToHTML(DataTable dtHeading, DataTable TimeSheetdt, string flag)
        {
            string html = ""; string html1 = string.Empty; string ApproveBtn = string.Empty, RejectBtn = string.Empty;
            string TimesheetIds = dtHeading.Rows[0]["TimesheetId"].ToString();
            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
            string UrlEmailAddress = string.Empty;


            var imagepathone = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~" + @"\Content\images\ec_inc500.png"));
            baseimageone = //"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw / eHBhY2tldCBiZWdpbj0i77u / IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8 + IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjYyMjJEODc3RkJGQjExRUE5QjFEQjcxQjg2ODJGNjY4IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjYyMjJEODc4RkJGQjExRUE5QjFEQjcxQjg2ODJGNjY4Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6NjIyMkQ4NzVGQkZCMTFFQTlCMURCNzFCODY4MkY2NjgiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6NjIyMkQ4NzZGQkZCMTFFQTlCMURCNzFCODY4MkY2NjgiLz4gPC9yZGY6RGVzY3JpcHRpb24 + IDwvcmRmOlJERj4gPC94OnhtcG1ldGE + IDw / eHBhY2tldCBlbmQ9InIiPz773GleAAAcI0lEQVR42tSbB5RV1bnHv3vnToUBBmbogxRpUZRqBLs8FYhgJaKIXVCjEqLL2FgPYyyJZRnNM3YURUGDQQwiQQQEQaRILzL0NsDAML3f / b7fN2df7xA1mkDy3l5rr3Puuefss / dX / l / bJyTHvjXW3kH7Cdo7a2 + pPVN7lvZG2qu05wV9v / bt2tdo / 1r7Fu3V8v + wsdA7tU / TvjcUCjk9umbNmtmxRYsW7sILL3SNGzd25557rhs8eLBLTU113bp1s / +4R3u59tXa / 6R9cECso94SjuJY9bUP1f6E9seDSXe + 6qqr6jdt2lQOHTokTz75pHz55ZfSo0cPOf / 882XTpk2SkZEhTZo0kV27dsmDDz4oa9aska5du8qwYcMiO3bsaKr / 9XbOXVleXn6Vjtc + kJK9R2vS4aMwRrr20dqXan9LuXjB0KFDU5Tr9ucJJ5wgymnp3Lmz / VaCyP79 + 6WsrEyaN28uBw4ckI0bN4pKhxQVFdl / EASi6cKNUNdddx2PttF + h / bF2qdqP / P / ggSM0P6GLnZEQkJCZvfu3eWUU06RTp06SUVFhezcuVOSkpJk8 + bNkp + fLx9++KF89dVXdn3dunWyYsUK2bt3r + zevVsOHjwohYWFJiksPjk5WRYuXCiM + d5770mjRo2kuLhYBg0aFFZidVGJuU4J1Enfvz7Aj3 + qhf7J5zpqfwox14XLuHHj5G9 / +5ssX75cMjMz5aabbpKlS5fKBx98EHsgLS1NOnToIK1btzbOs6CUlBSpqamRqqoqyc3NlS1btsiePXuMINFotPZFHTvK9u3bTS0UH + S4446z / yZPnmxjqJoU6G0Pa386wI5j3q5ED3Uibvjw4S4SibiHH37Yvfbaa065bwCWnp5ux549ezrVa6fEccp1p1x2vikHv / VcJcIpDrjx48e7a6 + 91ilGeFB0p512mnv00Ufdz3 / +c3vXq6++6gYMGOD / n649 + 1gv / rFwOGwvHDNmjE1COWMTAtG5zoRvvfVWt2jRIlddXW0L8u3dd991lZWVdv7mm286xQGn4GbnNJUE9 / 7778fuVzxw +/ btcxMmTHD9 +/ e38du0aeNUitw999zjnnvuOdevXz + nOOF + 8pOf8P9W7acfi4UnA3BM4Gc / +5kbMmSIUyR3Y8eOdQpqMa4zKRVXp8AWW8SkSZOc6rWdT5s2zRZFmzJlilOddwp8dk7jOVUbOz98 + LB75513YuNACCTpjDPOsPepStkcBg4caHO4++67PROKtF96NK1AivZJDRs2HH7JJZfI / PnzTS / R35dfflnefvttueyyy + Tzzz + X3 / 3ud6IckmXLlsnatWvt4eOPP950mwbSFxQU1FJUQQ79VymxsWi6aMMHGsAJZtCwEuDLeeedJ5999pmoxJilAFAZB3wBQMEHtTqY48kYnKNBgETtb2u / +PLLLxe6mjhDZVAdUwYB / vznP4s6MkYE2qmnnirr16 + 38 / bt24tyz84bNGhgZs6DImBGr1evnl3jPoCNBihCPNrq1avNutBY9Jlnnmkgi3mcOnWqnTMH5nbNNddI7969I6qqbwR + yb9EgKezsrIu0QEN0UFnFoDpoiEN6tHZedu2bWXr1q3GYZUWqV +/ vjk3ODpwGrOIeeNIg2DYef6DGDTFB5MSxQW7zrNYBf7Hl8AMQgzGob / wwgvyxBNPmDlNTEw0pvDes846C2mI6JCvaT / ln9X7Ueia2l33 / PPPO7XHBkBc0xfEEH3ixIlu27Ztds5RbX0MzT / ++GM7X7JkicvJyXHKbffpp5 / aNZ204QH3rVq1yq7NmjXLjiryTtXIzmfMmBED0unTpzv1HmPjK9djeKJq5NTnMKvDPLEYgYVA / 5r9WAnoDvcvuOAC2bBhg4kY4o / Xds4555gN9nqLBMydO1dKSkpMB9Usmt1Gl1UMTU24jtTgHWLz4S4OEuLPb87hPhJB49527dqZNHGNsXCePKYgRTNnzjRVoF166aWigGnv++1vfysnn3yyqHUQJSxS1E5vefHHcD5J + xJQ / frrr3dvvfWWBSlQF9QFuXWxZvc92qvox5AcVP / LX / 5i3FYgc0o8u / 7JJ5 / EpIEx4CT2n3sUJM1SrFy50ilxYpKgcYNTR8fGYkyeo2EplDGx973xxhtmQpEIAi9MY6tWrdx9993nRo8e7YJgbNQPdYXv1n7N / fffL2pbDZmhNmirIm3eGPqmL7DrcBcOoZ9wHuSG60hLly5dLLgBCNFPD3ZwHmmAu + g713keXUcS + B8dB0gJnJQwBo68a / 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";
                GetBase64StringForImage(imagepathone);
            var imagepathtwo = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~" + @"\Content\images\ec_inc.png"));
            baseimagetwo = //"data:image/png;base64,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";
                GetBase64StringForImage(imagepathtwo);




            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;

            }
            else
            {
                UrlEmailAddress = port1;

            }
            ClientComponent obj = new ClientComponent();
            List<string> data1 = obj.GetImages(Convert.ToInt32(TimesheetIds));
            if (flag == "1")
            {
                ApproveBtn = "block";
                RejectBtn = "block";
            }
            else if (flag == "3")
            {
                ApproveBtn = "none";
                RejectBtn = "none";
            }
            else
            {
                ApproveBtn = "block";
                RejectBtn = "block";
            }
            var totalRows = TimeSheetdt.Rows.Count;
            int halfway = totalRows / 2;
            var firstHalfdt = TimeSheetdt.AsEnumerable().Take(halfway).CopyToDataTable();
            var secondHalfdt = TimeSheetdt.AsEnumerable().Skip(halfway).CopyToDataTable();



            //<!--[if gte mso 9]> <style type='text/css'><link href='https://fonts.googleapis.com/css?family=Roboto' rel='stylesheet'> <style type='text/css'> label[for='comment'] ~ p.MsoNormal{display: table-cell !important;width: 25% !important;}</style> <![endif]-->
            //<!-- [if !mso]> --> <link href='https://fonts.googleapis.com/css?family=Roboto' rel='stylesheet'> <style type='text/css'> label[for='comment'] ~ p.MsoNormal{display: table-cell !important;width: 25% !important;} </style><!-- <![endif] -->
            //<!-- [if !mso]> --><head><style> div[id*='secIdApp'], div[id*='secIdApp'] + div { width: 25%; display: table-cell; padding: 0 15px; vertical-align: middle; } div[id*='secIdApp'] + div + div { width: 50%; display: table-cell; padding: 0 15px; vertical-align: middle; }</style></head><!-- <![endif] -->
            html += "<!--[if gte mso 9]> <style type='text/css'><link href='https://fonts.googleapis.com/css?family=Roboto' rel='stylesheet'> <style type='text/css'> label[for='comment'] ~ p.MsoNormal{display: table-cell !important;width: 25% !important;}</style> <![endif]--><form style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>" +
             "<div style='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:0 10px;'> " +
           " <table style='width:100%;border-spacing:0px 20px;border-collapse:separate;border:0;-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
          "<tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
          "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> Employee Name: </th> " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'>" + dtHeading.Rows[0]["Employee Name"].ToString() + "</td>" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> Client Name: </th> " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'>" + dtHeading.Rows[0]["Proj_ClientName"].ToString() + " </td> " +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> Project Name: </th> " +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'>" + dtHeading.Rows[0]["ProjectName"].ToString() + " </td> " +
          "</tr><tr style='-webkit-box-sizing: border-box;-moz-box-sizing:border-box; box-sizing: border-box'> " +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'>Timesheet Cycle: </th>" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> " + dtHeading.Rows[0]["TimesheetType"].ToString() + " </td>" +
          "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> Duration : </th>" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'>" + dtHeading.Rows[0]["TimesheetMonth"].ToString() + "</td>" +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> Submitted Date: </th>" +
          "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;font-family: Roboto,sans-serif;'> " + dtHeading.Rows[0]["SubmittedDate"].ToString() + "</td>" +
           "</tr></tbody></table></div>";
            //fisrt table

            html += "<table id='tblvertical' style='width:100%;table-layout: fixed;'><tr><td style='vertical-align: top;'>";
            html += "<div style='overflow-x:auto;'><div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:relative;min-height:1px;'> " +
           "<table style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color: #fff;'>" +
           "<thead style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
           "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'> " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'>Date</th> " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'> Task </th> " +
           "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'> Hours </th>" +
           "</tr></thead><tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>";

            for (int i = 0; i < firstHalfdt.Rows.Count; i++)
            {
                html += "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>";

                for (int j = 0; j < firstHalfdt.Columns.Count; j++)
                {
                    html += "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;'>" + firstHalfdt.Rows[i][j].ToString() + "</td>";

                }
                html += "</tr>";
            }

            html += "</tbody></table></div></div></td>";
            //second datatable
            html += "<td style='vertical-align: top;'><div style='overflow-x:auto;'><div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:relative;min-height:1px;'> " +
              "<table style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color: #fff;'>" +
              "<thead style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>" +
              "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'> " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'>Date</th> " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'> Task </th> " +
              "<th style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;background:#666;color:white;font-family: Roboto,sans-serif;'> Hours </th>" +
              "</tr></thead><tbody style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box'>";

            for (int i = 0; i < secondHalfdt.Rows.Count; i++)
            {
                html += "<tr style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;background-color:#ffffff;'>";


                for (int j = 0; j < secondHalfdt.Columns.Count; j++)
                {
                    html += "<td style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;padding:8px;border:1px solid #ccc;font-size:13px;font-family: Roboto,sans-serif;'>" + secondHalfdt.Rows[i][j].ToString() + "</td>";

                }
                html += "</tr>";
            }

            html += "</tbody></table></div></div></td></tr></table>";
            // comments tab
            html += "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;'> " +
   "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
   "<label style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0010px;font-weight:700;display:none;font-family: Roboto,sans-serif;' for='comment'> Comments :</label>" +
   "<textarea style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;border-radius:5px;box-shadow:none;border-color:#d2d6de;font-family:sans-serif;-webkit-appearance:none;-moz-appearance:none;appearance:none;width:100%;' rows='3' id='comment' readonly>" + dtHeading.Rows[0]["comments"].ToString() + " </textarea></div></div>";

            //         html += "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;'> " +
            //"<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
            //"<img id='previewFlImage' src='" + "https://" + UrlEmailAddress + "/TimeSheetimages/" + data1[0] + "' style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0 0 10px;font-weight:700;max-height: 100px; max-width: 100px;' />" +
            //"</div></div>";

            //start add Approve and Reject Buttons
            //********************change href*****************//
            html += "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:" +
                "border-box;overflow-x:auto;text-align:center;padding:10px;display:inline-table;'>" +
                 "<div id='secIdApp' style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;" +
                 "text-align:right;width:25%;display:table-cell;position:relative;padding:0 15px;min-height:1px;vertical-align: middle;'>" +
                 "<a id='ApproveID' runat='server' hreflang  type name target='_blank' " +
                 "style='text-decoration:none;-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;width:100%;display:" + ApproveBtn + ";padding:0px;line-height: 35px;margin-bottom:0;font-size:14px;font-weight:400; " +
                 "text-align:center;white-space:nowrap;vertical-align:middle;-ms-touch-action:manipulation;touch-action:manipulation;cursor:pointer;-webkit-user-select:none; -moz-user-select:none;-ms-user-select:none; user-select:none;border:1px solid #b2cc68;" +
                 "border-radius:4px;color:#fff;background-color:#cae285;'> <span style='color:#6e8a40;border:1px solid #cae285;display:inline-block;color:#6e8a40;background-color:#cae285;font-family: Roboto,sans-serif;'> <font color='#6e8a40' style='text-shadow: 1px 2px 2px #bedc6a;' face='Roboto'> Approve </font> </span> </a></div> " +

                 "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align:left;" +
                 "width:25%;display:table-cell;position:relative;padding:0 15px;min-height:1px;vertical-align: middle;'>" +
                 "<a id='RejectId' runat='server'  media type title rev target='_blank' " +
                 "style='text-decoration:none;-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;display:" + RejectBtn + ";padding:0px;line-height: 35px;margin-bottom:0;font-size:14px;font-weight:400; " +
                 "text-align:center;white-space:nowrap;vertical-align:middle;-ms-touch-action:manipulation;touch-action:manipulation;cursor:pointer;-webkit-user-select:none; -moz-user-select:none;-ms-user-select:none; user-select:none;border:1px solid #ce002b;" +
                 "border-radius:4px;color:#a90326;background-color:#ea90a3;'> <span style='color:#a90326;border:1px solid #ea90a3;display:inline-block;color:#a90326;background-color:#ea90a3;font-family: Roboto,sans-serif;'> <font color='#a90326' style='text-shadow: 1px 2px 2px #fbadbc;' face='Roboto'> Reject </font> </span> </a></div>";

            //end Approve and Reject Buttons
            if (data1.Count == 1)
            {
                html += "<div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;width:50%;display:table-cell; position: relative; padding: 0;vertical-align: middle; min-height: 1px;overflow: auto;'><table style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;table-layout: fixed;" +
             "border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color:#fff;'>" +
             "<tbody style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
             "<tr style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing: border-box'>" +
             "<th style ='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;" +
             " font-size:18px; text-align:center; padding: 8px;border: 1px solid #ccc;background: #666; color: white;'> " +
             "Total Working Hours :</th>" +
             "<td style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;font-weight:600;width:75px;" +
             "font-size:18px;text-align:center;padding:8px;border:1px solid #ccc;'> " + dtHeading.Rows[0]["TotalWorkingHours"].ToString() + " </td> " +
             "</tr></tbody></table></div></div></form> <div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;'> " +
              "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
              "<img id='previewFlImage' src='" + "https://" + UrlEmailAddress + "/TimeSheetimages/" + data1[0] + "' style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0 0 10px;font-weight:700;max-height: 100px; max-width: 100%;' />" +
              "</div></div>" +
              "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;border-top: 1px solid #ddd; border-bottom-right-radius: 3px; border-bottom-left-radius: 3px;'> <div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align: right;'> <table style='display:inline-table;' cellspacing='0' cellpadding='0' border='0'> <tbody> <tr> " +
              "<td style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;font-weight: 600;vertical-align: middle;padding:0 5px;text-align:center;font-size: 10px'> <div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;text-align: left;display: inline-block;'> <p style='margin: 0 0 5px;'>An Award-Winning IT Solutions Firm</p> <p style='color: #f7a74e; margin: 0 0 5px;'>Together, the future is limitless.</p> <p style='margin: 0 0 5px;'>Let's Innovate</p> </div> </td> " +
              //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimageone + "'  alt='logo' style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimagetwo + "'   alt='logo' style='border:0;hieght:60px;cursor:pointer;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc.png'   alt style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc500.png'   placeholder  style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +

" </tr> </tbody> </table> </div> </div>" +
              "</div></div></body></html>";

            }
            else if (data1.Count == 2)
            {
                html += "<div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;width:50%;display:table-cell; position: relative; padding: 0;vertical-align: middle; min-height: 1px;overflow: auto;'><table style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;table-layout: fixed;" +
             "border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color:#fff;'>" +
             "<tbody style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
             "<tr style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing: border-box'>" +
             "<th style ='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;" +
             " font-size:18px; text-align:center; padding: 8px;border: 1px solid #ccc;background: #666; color: white;'> " +
             "Total Working Hours :</th>" +
             "<td style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;font-weight:600;width:75px;" +
             "font-size:18px;text-align:center;padding:8px;border:1px solid #ccc;'> " + dtHeading.Rows[0]["TotalWorkingHours"].ToString() + " </td> " +
             "</tr></tbody></table></div></div></form> <div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;'> " +
              "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
              "<img src='" + "https://" + UrlEmailAddress + "/TimeSheetimages/" + data1[0] + "' style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0 0 10px;font-weight:700;max-height: 100px; max-width: 100px;display:inline-block;' />" +
              "<img src='" + "https://" + UrlEmailAddress + "/TimeSheetimages/" + data1[1] + "' style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;margin:0 0 10px;font-weight:700;max-height: 100px; max-width: 100px;display:inline-block;' />" +
              "</div></div>" +
              "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;border-top: 1px solid #ddd; border-bottom-right-radius: 3px; border-bottom-left-radius: 3px;'> <div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align: right;'> <table style='display:inline-table;' cellspacing='0' cellpadding='0' border='0'> <tbody> <tr> " +
              "<td style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;font-weight: 600;vertical-align: middle;padding:0 5px;text-align:center;font-size: 10px'> <div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;text-align: left;display: inline-block;'> <p style='margin: 0 0 5px;'>An Award-Winning IT Solutions Firm</p> <p style='color: #f7a74e; margin: 0 0 5px;'>Together, the future is limitless.</p> <p style='margin: 0 0 5px;'>Let's Innovate</p> </div> </td> " +
              //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimageone + "'  alt='logo' style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimagetwo + "' alt='logo' style='border:0;cursor: pointer;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc.png'   alt style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
              "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc500.png'   placeholder  style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +

" </tr> </tbody> </table> </div> </div>" +
              "</div></div></body></html>";
            }
            else if (data1.Count == 0)
            {
                html += "<div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;width:50%;display:table-cell; position: relative; padding: 0;vertical-align: middle; min-height: 1px;overflow: auto;'><table style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;table-layout: fixed;" +
             "border-collapse:collapse;border-spacing:0;width:100%;border:1px solid #ddd;background-color:#fff;'>" +
             "<tbody style ='-webkit-box-sizing: border-box;-moz-box-sizing:border-box;box-sizing:border-box;'>" +
             "<tr style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing: border-box'>" +
             "<th style ='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;" +
             " font-size:18px; text-align:center; padding: 8px;border: 1px solid #ccc;background: #666; color: white;'> " +
             "Total Working Hours :</th>" +
             "<td style ='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;font-weight:600;width:75px;" +
             "font-size:18px;text-align:center;padding:8px;border:1px solid #ccc;'> " + dtHeading.Rows[0]["TotalWorkingHours"].ToString() + " </td> " +
             "</tr></tbody></table></div></div></form>" +
             "<div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:10px;border-top: 1px solid #ddd; border-bottom-right-radius: 3px; border-bottom-left-radius: 3px;'> <div style='-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;text-align: right;'> <table style='display:inline-table;' cellspacing='0' cellpadding='0' border='0'> <tbody> <tr> " +
             "<td style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;font-weight: 600;vertical-align: middle;padding:0 5px;text-align:center;font-size: 10px'> <div style='-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;text-align: left;display: inline-block;'> <p style='margin: 0 0 5px;'>An Award-Winning IT Solutions Firm</p> <p style='color: #f7a74e; margin: 0 0 5px;'>Together, the future is limitless.</p> <p style='margin: 0 0 5px;'>Let's Innovate</p> </div> </td> " +
             //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimageone + "'   alt style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
             //"<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='" + baseimagetwo + "'   placeholder  style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
             "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc.png'   alt style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +
             "<td style='padding:0 5px;text-align:right;vertical-align: middle;-webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'><img border='0' src='https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/ec_inc500.png'   placeholder  style='border:0;cursor: pointer;height: 60px;' onclick='window.open(/'https://www.inc.com/profile/evolutyz'/, /'_blank'/);'></td>" +

             " </tr> </tbody> </table> </div> </div>" +
             "</div></div></body></html>";
            }


            return html;

        }
        #endregion

        //Based on login Credentials data is retrived
        public ActionResult Usertimesheet()
        {

            //UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;

            UserSessionInfo objUserSession = new UserSessionInfo();

            int uid = objUserSession.UserId;

            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            TempData["USERID"] = objUserSession.UserId;
            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);

            //var projid =
            List<ProjectEntity> projects = new List<ProjectEntity>();
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = new SqlCommand("getAssignedUsersList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Userid", uid);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                projects.Add(new ProjectEntity
                {
                    Proj_ProjectID = Convert.ToInt32(sdr["UProj_ProjectID"]),
                    Proj_ProjectName= sdr["Proj_ProjectName"].ToString(),
                    CL_ProjectTitle = sdr["ClientProjTitle"].ToString(),
                    clientprojId = Convert.ToInt32(sdr["ClientprojID"]),
                    TimesheetMode_id = Convert.ToInt32(sdr["Timesheetmode"]),



                });
            }
            ViewBag.AssignedProjs = projects;


            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;

                this.Session["TaskId"] = objuser;
                return View(objuser);
            }
            else
            {
                return View();
            }

        }

        public ActionResult PreviewMonthlyTimeSheet()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();

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
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                if (image.Acc_CompanyLogo != null)
                    ViewBag.imageid = image.Acc_CompanyLogo;
                else
                    ViewBag.imageid = "evolutyzcorplogo.png";
                this.Session["TaskId"] = objuser;
            }
            else
            {
                ViewBag.User_ID = objUserSession.UserId;
            }

            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                if (item.ModuleName == "Add/Submit Timesheet")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }


            return View(objuser);
        }





        public ActionResult PreviewTimesheets()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;


            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
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
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                if (image.Acc_CompanyLogo != null)
                    ViewBag.imageid = image.Acc_CompanyLogo;
                else
                    ViewBag.imageid = "evolutyzcorplogo.png";
                this.Session["TaskId"] = objuser;
            }
            else
            {
                ViewBag.User_ID = objUserSession.UserId;
            }

            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                if (item.ModuleName == "Add/Submit Timesheet")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }


            return View(objuser);
        }

        public ActionResult EditTimesheet()
        {
            return View();
        }

        public ActionResult GetPreviewTimesheets()
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                objmanagerdetails.userAttachements = new List<UserAttachementsTimesheet>();

                Conn.Open();
                //SqlCommand cmd = new SqlCommand("[WebGetTimeSheetforApproval]", Conn);
                SqlCommand cmd = new SqlCommand("[GETALL_USERSTIMESHEETS_WEB]", Conn);



                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                ////cmd.Parameters.AddWithValue("@userid", UserID);
                //if (roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                cmd.Parameters.AddWithValue("@userid", UserID);
                //}

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((roleid == "1002") || (roleid == "1007") || (roleid == "1010") || (roleid == "1011") || (roleid == "1053") || (roleid == "1061") || (roleid == "1006"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                ClientprojectId = Convert.ToInt32(dr["ClientProjtId"] == System.DBNull.Value ? "" : dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                TimesheetMode = Convert.ToInt32(dr["TimesheetMode"] == System.DBNull.Value ? "" : dr["TimesheetMode"]),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                Month_Year = dr["MonthYearName"].ToString(),
                                TotalMonthName = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                            {
                                ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                ClientprojectId = Convert.ToInt32(druser["ClientProjtId"] == System.DBNull.Value ? "" : druser["ClientProjtId"]),
                                ClientProjectName = druser["ClientProjTitle"].ToString(),
                                TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                                Month_Year = druser["TimesheetMonth"].ToString(),
                                TimesheetDuration = druser["TimesheetDuration"] == System.DBNull.Value ? "" : druser["TimesheetDuration"].ToString(),
                                ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                                TimesheetMode = Convert.ToInt32(druser["TimesheetMode"] == System.DBNull.Value ? "" : druser["TimesheetMode"]),
                                CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"] == System.DBNull.Value ? 0 : druser["Companyworkinghours"]),
                                TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                                userName = druser["Usr_Username"].ToString(),
                                ManagerApprovalStatus = druser["FinalStatus"].ToString(),
                                SubmittedDate = druser["SubmittedDate"].ToString(),
                                ApprovedDate = (druser["L2_ManagerName"].ToString() != null && druser["L2_ManagerName"].ToString() != "" && druser["L2_ManagerName"].ToString() != "0") ? druser["L2_ApproverDate"].ToString() : druser["L1_ApproverDate"].ToString(),
                                //druser["L1_ApproverDate"].ToString(),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerName2 = druser["L2_ManagerName"].ToString(),
                                UProj_L1ManagerId = druser["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = druser["UProj_L2_ManagerId"].ToString(),
                                TimesheetType = druser["TimesheetType"].ToString(),
                                ByMonthlyDates = druser["TimesheetDuration"].ToString(),
                                TotalMonthName = druser["TimesheetDuration"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[2].Rows)
                        {

                            //objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                            objmanagerdetails.userAttachements.Add(new UserAttachementsTimesheet
                            {

                                UploadedImages = druser["uploadedimages"].ToString(),
                                TimeSheetID = Convert.ToInt16(druser["timesheetid"]),
                                AttachmentId = Convert.ToInt16(druser["attachmentid"]),

                            });

                        }


                    }


                }


                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                ClientprojectId = Convert.ToInt32(dr["ClientProjtId"] == System.DBNull.Value ? "" : dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                TimesheetMode = Convert.ToInt32(dr["TimesheetMode"] == System.DBNull.Value ? "" : dr["TimesheetMode"]),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                //dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                            });
                        }
                    }
                }
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


            var result = new { objmanagerdetails.mytimesheets, objmanagerdetails.timesheetsforapproval, roleid };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region (getUserTimesheet)Preview the Specific MonthData of  Specific userdata 
        public ActionResult ViewSubmitedTimesheet(string TimesheetMonth, int TimesheetUserid, int clientProjectid, int TimesheetID)
        {


            if (TimesheetID == 0 || TimesheetID.ToString() == "")
            {
                //string outputDate = DateTime.Parse(TimesheetMonth).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                try
                {
                    var tm = Convert.ToDateTime(TimesheetMonth);
                    TimesheetID = db.TIMESHEETs.Where(a => a.TimesheetMonth == tm && a.UserID == TimesheetUserid && a.ClientProjtId == clientProjectid).Select(z => z.TimesheetID).FirstOrDefault();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            objGetTimeSheet.UploadTimesheetimage = new List<UploadTimesheetimages>();
            // objGetTimeSheet.Status = new List<statusMessage>();

            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("WebgetUserTimesheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", TimesheetUserid);
                cmd.Parameters.AddWithValue("@Timesheetmonth", TimesheetMonth);
                cmd.Parameters.AddWithValue("@ClientprojID", clientProjectid);
                cmd.Parameters.AddWithValue("@TimesheetID", TimesheetID);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                    {
                        TimesheetID = Convert.ToInt32(dr["TimesheetID"]),
                        Taskid = Convert.ToInt32(dr["taskid"]),
                        Month_Year = dr["MonthYearName"].ToString(),
                        Taskname = dr["taskname"].ToString(),
                        NoofHoursWorked = Convert.ToDouble(dr["HoursWorked"].ToString()),
                        MonthYearName = dr["MonthYearName"].ToString(),
                        Comments = dr["Comments"].ToString(),
                        UserName = dr["Usr_Username"].ToString(),
                        TotalMonthName = dr["TotalMonthName"].ToString(),
                        ProjectName = dr["Proj_ProjectName"].ToString(),
                        ProjectClientName = dr["ClientProjTitle"].ToString(),
                        ManagerName1 = dr["L1_ManagerName"].ToString(),
                        ManagerName2 = dr["L2_ManagerName"].ToString(),
                        SubmittedDate = dr["SubmittedDate"].ToString(),
                        ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                        Submitted_Type = Convert.ToInt32(dr["submittedtype"]),
                        L1_ApproverStatus = Convert.ToInt32(dr["L1_ApproverStatus"]),
                        L2_ApproverStatus = Convert.ToInt32(dr["L2_ApproverStatus"]),




                    });


                }

                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    objGetTimeSheet.UploadTimesheetimage.Add(new UploadTimesheetimages
                    {
                        Uploadedimages = dr["uploadedimages"].ToString(),
                        Attachmentid = Convert.ToInt16(dr["attachmentid"]),

                    });


                }



            }
            catch (Exception ex)
            {

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


            return Json(new { objGetTimeSheet.timeSheetDetails, objGetTimeSheet.UploadTimesheetimage }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region UpdateTaskDetails
        public ActionResult updateTimesheetTaskDetails(TotalTimeSheetTimeDetails sheetObj)
        {

            Conn = new SqlConnection(str); int Trans_Output = 0; string result = string.Empty;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            List<string> data = TempData["errorNotification"] as List<string>;
            //  SendEmail objMail = new SendEmail();Admin@evolutyz.in
            UserSessionInfo info = new UserSessionInfo();
            int? userid = info.UserId;

            var GetTimesheetMonth = (from ts in db.TIMESHEETs where ts.TimesheetID == sheetObj.timesheets.TimesheetID select ts.TimesheetMonth).FirstOrDefault();
            sheetObj.timesheets.TimeSheetMonth = GetTimesheetMonth.ToString("yyyy-MM-dd");
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[WebEditSubmitTimesheet]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.timesheets.TimesheetID);
                objCommand.Parameters.AddWithValue("@UserID", UserID);
                if (!string.IsNullOrEmpty(sheetObj.timesheets.Comments))
                {
                    objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                }
                else
                {
                    objCommand.Parameters.AddWithValue("@Comments", null);
                }

                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.timesheets.SubmittedType);
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());

                if (Trans_Output == 104)
                {
                    foreach (var item in sheetObj.listtimesheetdetails)
                    {

                        objCommand = new SqlCommand("[WebEditSubmitTaskDetails]", Conn);
                        objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.timesheets.TimesheetID);
                        //objCommand.Parameters.AddWithValue("@ProjectID", item.projectid);
                        objCommand.Parameters.AddWithValue("@TaskId", item.taskid);
                        objCommand.Parameters.AddWithValue("@HoursWorked", item.hoursWorked);
                        objCommand.Parameters.AddWithValue("@TaskDate", Convert.ToDateTime(item.taskDate).ToShortDateString());
                        objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                        objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                        objCommand.ExecuteNonQuery();
                        Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());

                    }

                    if (Trans_Output == 1)
                    {
                        var saveimages = obj.SavetimeSheetImages(sheetObj.timesheets.TimesheetID, userid, data, UpId);
                        if (sheetObj.timesheets.SubmittedType == "Submit")
                        {
                            var timesheetid = sheetObj.timesheets.TimesheetID;
                            //SendMailsForApprovals(sheetObj, timeSheetID, userid, data);
                            SendMailsForApprovals(sheetObj, timesheetid, userid, data);



                            result = "Timesheet Submited Successfully";

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        else
                        {
                            result = "Timesheet Saved Successfully";

                            return Json(result, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                else if (Trans_Output == 105)
                {
                    result = "Invalid TimesheetId";
                }

                else
                {
                    result = "Wrong Inputs";
                }

                return Json(result, JsonRequestBehavior.AllowGet);
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
        }
        #endregion


        #region CheckTimesheetByWeeklyExists   

        public ActionResult CheckTimesheetByWeeklyExists(string timesheetstartdate, string timesheetenddate,int clientprojid)
        {
            Conn = new SqlConnection(str); int Trans_Output = 0; string result = string.Empty;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            int timesheetmode = Convert.ToInt32(sessId.TimesheetMode);
            try
            {
                SqlCommand objCommand = new SqlCommand();
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();


                objCommand = new SqlCommand("[ByWeeklyWebValidTimesheetMonthExits]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@Weekstartdate", timesheetstartdate);
                objCommand.Parameters.AddWithValue("@Weekenddate", timesheetenddate);
                objCommand.Parameters.AddWithValue("@UserID", UserID);
                objCommand.Parameters.AddWithValue("@clientprojid", clientprojid);

                
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());

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
            return Json(Trans_Output, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CheckTimesheetExits

        public ActionResult CheckTimesheetExits(string timesheetsdate, int clientProjectid)
        {
            Conn = new SqlConnection(str); int Trans_Output = 0; string result = string.Empty;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            int timesheetmode = Convert.ToInt32(sessId.TimesheetMode);
            try
            {
                SqlCommand objCommand = new SqlCommand();
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                objCommand = new SqlCommand("[WebValidTimesheetMonthExits]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@TimesheetMonth", timesheetsdate);
                objCommand.Parameters.AddWithValue("@UserID", UserID);
                objCommand.Parameters.AddWithValue("@ClientProjtId", clientProjectid);

                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());

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
            return Json(Trans_Output, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region TimeSheetManagerActions
        public ActionResult TimeSheetManagerActionWeb(TotalTimeSheetTimeDetails sheetObj)
        {
            timesheet lstobjtime = new timesheet();

            int Trans_Output = 0; string Userid = string.Empty;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;

            if ((sheetObj.timesheets.UserID != 0))
            {
                Userid = sheetObj.timesheets.UserID.ToString();
            }
            else
            {

                Userid = sessId.UserId.ToString();
            }
            try
            {
                Conn = new SqlConnection(str);
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand("[ManagerActionsfromEmail]", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.timesheets.UserID);
                objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.timesheets.TimesheetID);
                objCommand.Parameters.AddWithValue("@Projectid", sheetObj.timesheets.ProjectID);
                objCommand.Parameters.AddWithValue("@ManagerId", sessId.UserId);
                objCommand.Parameters.AddWithValue("@Comments", sheetObj.timesheets.Comments);
                objCommand.Parameters.AddWithValue("@ClientprojID", sheetObj.timesheets.ClientProjectId);


                if (sheetObj.timesheets.SubmittedType == "3")
                {
                    objCommand.Parameters.AddWithValue("@SubmittedType", "1");
                }
                else if (sheetObj.timesheets.SubmittedType == "4")
                {
                    objCommand.Parameters.AddWithValue("@SubmittedType", "0");
                }
                else if (sheetObj.timesheets.SubmittedType == "5")
                {
                    objCommand.Parameters.AddWithValue("@SubmittedType", "0");
                }

                objCommand.Parameters.AddWithValue("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                List<string> UploadedImagesList = obj.GetImages(sheetObj.timesheets.TimesheetID);
                var checkL1approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == sheetObj.timesheets.TimesheetID select ts.L1_ApproverStatus).FirstOrDefault();
                var checkL2approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == sheetObj.timesheets.TimesheetID select ts.L2_ApproverStatus).FirstOrDefault();
                if (checkL1approvestatus == 5 && checkL2approvestatus == 5 && sessId.RoleId != 1002)
                {
                    Trans_Output = 5;
                    lstobjtime = new timesheet()
                    {
                        Message = "Timesheet is already revoked by Admin",

                    };
                }

                if (Trans_Output == 0)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already rejected",
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }
                if (Trans_Output == 900)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "",
                        Message = "ManagerId is Incorrect",
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }

                if (Trans_Output == 1)
                {

                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Approved by" + " " + objm1[0],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };

                }

                if (Trans_Output == 2)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    if (sheetObj.timesheets.SubmittedType == "5")
                    {
                        Message = "Revoked by Admin";
                    }
                    else
                    {
                        Message = "Rejected by" + " " + objm1[0];
                    }


                    lstobjtime = new timesheet()
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = Message,
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };


                }
                if (Trans_Output == 3)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');

                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        // Message = "Approved by Level-2 Manager",
                        Message = "Approved by" + " " + objm1[1],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }
                if (Trans_Output == 4)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Rejected by" + " " + objm1[1],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }

                if (Trans_Output == 104)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already approved by" + " " + objm1[0],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }

                if (Trans_Output == 106)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        // Message = "Timesheet is already rejected by Level1 manager",
                        Message = "Timesheet is already rejected by" + " " + objm1[0],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }
                if (Trans_Output == 105)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');

                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        // Message = "Timesheet is already approved by Level2 manager",
                        Message = "Timesheet is already approved by" + " " + objm1[1],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };

                }
                if (Trans_Output == 107)
                {
                    string Message = SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                    string[] objm1 = Message.Split('-');
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Timesheet is already rejected by" + " " + objm1[1],
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }
                //if (Trans_Output == 109)
                //{
                //    string Message = SendMailsForApprovals(sheetObj);
                //    //string[] objm1 = Message.Split('-');
                //    lstobjtime = new timesheet()
                //    {
                //        Transoutput = Trans_Output,
                //        Position = "L2",
                //        Message = "Timesheet is revoked by admin",
                //        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                //        UserID = sheetObj.timesheets.UserID,
                //        ManagerId = sheetObj.timesheets.ManagerId,

                //    };
                //}

                if (Trans_Output == 108)
                {
                    string Message = string.Empty;
                    string[] objm1 = Message.Split('-');
                    if (sheetObj.timesheets.SubmittedType == "5")
                    {
                        Message = "Timesheet is already revoked by Admin";
                    }

                    lstobjtime = new timesheet()
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = Message,
                        EmailAppOrRejStatus = sheetObj.timesheets.EmailAppOrRejStatus,
                        UserID = sheetObj.timesheets.UserID,
                        ManagerId = sheetObj.timesheets.ManagerId,

                    };
                }


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




            return Json(lstobjtime.Message, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region CheckTimesheetExists
        public ActionResult GetHoursWorkedByCalender(string tdate, string Userid, string cmpnyAccountid, string UsrProjectId, int ClientprojID)
        {
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            timesheet objEmpInfo = new timesheet();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            Conn = new SqlConnection(str);
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                //SqlCommand objCommand = new SqlCommand("[TempSubmitTimesheet]", Conn);
                //SqlCommand objCommand = new SqlCommand("[TempTimesheetonLeavesandHolidays]", Conn);
                SqlCommand objCommand = new SqlCommand("[GetTempTimesheetLeavesandHolidays]", Conn);

                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@TimesheetMonth", tdate);
                objCommand.Parameters.AddWithValue("@Userid", Userid);
                objCommand.Parameters.AddWithValue("@Accountid", cmpnyAccountid);
                objCommand.Parameters.AddWithValue("@Projectid", UsrProjectId);
                objCommand.Parameters.AddWithValue("@ClientprojID", ClientprojID);

                SqlDataAdapter sda = new SqlDataAdapter(objCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                        {

                            TaskDate = dr["MonthYearName"].ToString(),
                            NoofHoursWorked = Convert.ToDouble(dr["HoursWorked"].ToString()),
                            colour = dr["colour"].ToString(),
                        });
                    }
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[1].Rows)
                    {
                        objEmpInfo = new timesheet()
                        {

                            UserName = dr1["Username"].ToString(),
                            ManagerName1 = dr1["L1_ManagerName"].ToString(),
                            ManagerName2 = dr1["L2_ManagerName"].ToString(),
                            TotalMonthName = dr1["TotalMonthName"].ToString(),

                        };

                    }
                }

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

            var result = new { objGetTimeSheet.timeSheetDetails, objEmpInfo };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ApplyColoursaccording to Leaves  and Holidays
        public ActionResult HoursWorkedTimesheet(string TimesheetMonth, int TimesheetUserid)
        {

            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();

            // objGetTimeSheet.Status = new List<statusMessage>();

            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("WebgetUserTimesheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", TimesheetUserid);
                cmd.Parameters.AddWithValue("@Timesheetmonth", TimesheetMonth);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);



                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                    {
                        colour = dr["colour"].ToString(),

                    });

                }



            }
            catch (Exception ex)
            {

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

            return Json(objGetTimeSheet.timeSheetDetails, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region BymonthlyTimesheets
        public ActionResult ByMonthlyTimesheets()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            TempData["USERID"] = objUserSession.UserId;
            LoginComponent loginComponent = new LoginComponent();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);
            List<ProjectEntity> projects = new List<ProjectEntity>();
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = new SqlCommand("getAssignedUsersList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Userid", objUserSession.UserId);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                projects.Add(new ProjectEntity
                {
                    Proj_ProjectID = Convert.ToInt32(sdr["UProj_ProjectID"]),
                    CL_ProjectTitle = sdr["ClientProjTitle"].ToString(),
                    clientprojId = Convert.ToInt32(sdr["ClientprojID"]),
                    TimesheetMode_id = Convert.ToInt32(sdr["Timesheetmode"]),



                });
            }
            ViewBag.AssignedProjs = projects;

            var clientprojectid = db.ClientProjects.Where(a => a.ClientProjTitle == usersprojects.ProjectClientName && a.Proj_ProjectID == usersprojects.Proj_ProjectID).Select(a => a.CL_ProjectID).FirstOrDefault();
            TempData["CL_PriId"] = clientprojectid;
            Session["Cpid"] = clientprojectid;
            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;

                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ByMonthlyTimesheets15days()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            TempData["USERID"] = objUserSession.UserId;
            LoginComponent loginComponent = new LoginComponent();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            List<ProjectEntity> projects = new List<ProjectEntity>();
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = new SqlCommand("getAssignedUsersList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Userid", objUserSession.UserId);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                projects.Add(new ProjectEntity
                {
                    Proj_ProjectID = Convert.ToInt32(sdr["UProj_ProjectID"]),
                    CL_ProjectTitle = sdr["ClientProjTitle"].ToString(),
                    clientprojId = Convert.ToInt32(sdr["ClientprojID"]),
                    TimesheetMode_id = Convert.ToInt32(sdr["Timesheetmode"]),



                });
            }
            ViewBag.AssignedProjs = projects;



            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);
            var clientprojectid = db.ClientProjects.Where(a => a.ClientProjTitle == usersprojects.ProjectClientName && a.Proj_ProjectID == usersprojects.Proj_ProjectID).Select(a => a.CL_ProjectID).FirstOrDefault();
            TempData["CL_PriId"] = clientprojectid;
            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                ViewBag.ClientProjectID = objuser.ClientProjectId;
                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }
        #endregion

        #region  WeeklyTimesheets
        public ActionResult WeeklyTimesheets()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            TempData["USERID"] = objUserSession.UserId;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();


            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);
            List<ProjectEntity> projects = new List<ProjectEntity>();
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = new SqlCommand("getAssignedUsersList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Userid", objUserSession.UserId);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                projects.Add(new ProjectEntity
                {
                    Proj_ProjectID = Convert.ToInt32(sdr["UProj_ProjectID"]),
                    CL_ProjectTitle = sdr["ClientProjTitle"].ToString(),
                    clientprojId = Convert.ToInt32(sdr["ClientprojID"]),
                    TimesheetMode_id = Convert.ToInt32(sdr["Timesheetmode"]),



                });
            }
            ViewBag.AssignedProjs = projects;
            var clientid = db.UserProjects.Where(a => a.UProj_UserID == usersprojects.User_ID && a.UProj_ProjectID == usersprojects.Proj_ProjectID).Select(a => a.ClientprojID).FirstOrDefault();
            TempData["clientid"] = clientid;
            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }
        #endregion

        public ActionResult PreviewbyMonthlyTimesheets()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
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
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                if (image.Acc_CompanyLogo != null)
                    ViewBag.imageid = image.Acc_CompanyLogo;
                else
                    ViewBag.imageid = "evolutyzcorplogo.png";

                this.Session["TaskId"] = objuser;
            }
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                if (item.ModuleName == "Add/Submit Timesheet")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }
            return View(objuser);

        }

        #region By monthly timesheets(15days)
        public ActionResult PreviewBymonthlytimesheets15days()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            //TempData["Error"] = "Event not raised";
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);

            var clientprojectid = db.ClientProjects.Where(a => a.ClientProjTitle == usersprojects.ProjectClientName && a.Proj_ProjectID == usersprojects.Proj_ProjectID).Select(a => a.CL_ProjectID).FirstOrDefault();
            TempData["CL_PriId"] = clientprojectid;
            ViewBag.CL_ProjId = clientprojectid;


            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                if (image.Acc_CompanyLogo != null)
                    ViewBag.imageid = image.Acc_CompanyLogo;
                else
                    ViewBag.imageid = "evolutyzcorplogo.png";

                this.Session["TaskId"] = objuser;
            }
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                if (item.ModuleName == "Add/Submit Timesheet")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }


            // return View("PreviewTimesheets", objuser);
            return View(objuser);
        }
        #endregion


        public ActionResult PreviewWeeklyTimesheets()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();


            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            var usersprojects = loginComponent.GetUserProjectsDetailsInfo(objUserSession);


            var clientprojectid = db.ClientProjects.Where(a => a.ClientProjTitle == usersprojects.ProjectClientName && a.Proj_ProjectID == usersprojects.Proj_ProjectID).Select(a => a.CL_ProjectID).FirstOrDefault();
            TempData["CL_PriId"] = clientprojectid;
            if (usersprojects != null)
            {
                objuser.User_ID = usersprojects.User_ID;
                objuser.AccountName = usersprojects.AccountName;
                objuser.Usr_Username = usersprojects.Usr_Username;
                objuser.projectName = usersprojects.projectName;
                objuser.ProjectClientName = usersprojects.ProjectClientName;
                objuser.tsktaskID = usersprojects.tsktaskID;
                objuser.Proj_ProjectID = usersprojects.Proj_ProjectID;
                objuser.RoleCode = usersprojects.RoleCode;
                objuser.ClientProjectId = usersprojects.ClientProjectId;
                ViewBag.accountid = usersprojects.Account_ID;
                ViewBag.tsktaskID = objuser.tsktaskID;
                ViewBag.User_ID = objuser.User_ID;
                ViewBag.Projectid = objuser.Proj_ProjectID;
                ViewBag.ProjectName = objuser.projectName;
                ViewBag.ClientProjectName = objuser.ProjectClientName;
                if (image.Acc_CompanyLogo != null)
                    ViewBag.imageid = image.Acc_CompanyLogo;
                else
                    ViewBag.imageid = "evolutyzcorplogo.png";
                this.Session["TaskId"] = objuser;
            }
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {
                if (item.ModuleName == "Add/Submit Timesheet")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }
            //return View("PreviewTimesheets", objuser);
            return View(objuser);
        }
        #region  LoadByWeeklyDates
        public ActionResult LoadByWeeklyDates()
        {
            UserProjects objuserpro = new UserProjects();
            Conn = new SqlConnection(str);
            ManagerDetails objprojects = new ManagerDetails();
            objprojects.UserProject = new List<UserProjects>();

            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetDatesofByWeeklyTesting", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    objprojects.UserProject.Add(new UserProjects
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        dateName = dr["StartDate"].ToString(),

                    });

                }

            }
            catch (Exception ex)
            {

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

            return Json(objprojects.UserProject, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult LoadByMonthlyDates()
        {
            UserProjects objuserpro = new UserProjects();
            Conn = new SqlConnection(str);
            ManagerDetails objprojects = new ManagerDetails();
            objprojects.UserProject = new List<UserProjects>();

            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetDatesofByWeekly15days", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    objprojects.UserProject.Add(new UserProjects
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        dateName = dr["StartDate"].ToString(),

                    });

                }

            }
            catch (Exception ex)
            {

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

            return Json(objprojects.UserProject, JsonRequestBehavior.AllowGet);
        }

        #region LoadWeeklyDates
        public ActionResult LoadWeeklyDates()
        {
            UserProjects objuserpro = new UserProjects();
            Conn = new SqlConnection(str);
            ManagerDetails objprojects = new ManagerDetails();
            objprojects.UserProject = new List<UserProjects>();

            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetDatesofWeeklyTesting", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    objprojects.UserProject.Add(new UserProjects
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        dateName = dr["StartDate"].ToString(),

                    });

                }

            }
            catch (Exception ex)
            {

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

            return Json(objprojects.UserProject, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region CheckTimesheetExists
        public ActionResult GetByWeeklyHoursWorked(string tdate, string Userid, string cmpnyAccountid, string UsrProjectId, int ClientprojID)
        {
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            timesheet objEmpInfo = new timesheet();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            string[] objm1 = tdate.Split('-');
            Conn = new SqlConnection(str);
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                //SqlCommand objCommand = new SqlCommand("[TempSubmitTimesheet]", Conn);
                SqlCommand objCommand = new SqlCommand("[TempAddByWeeklyTimesheet]", Conn);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@timesheetMonthstart", objm1[0]);
                objCommand.Parameters.AddWithValue("@timesheetMonthEnd", objm1[1]);
                objCommand.Parameters.AddWithValue("@Userid", Userid);
                objCommand.Parameters.AddWithValue("@Accountid", cmpnyAccountid);
                objCommand.Parameters.AddWithValue("@Projectid", UsrProjectId);
                objCommand.Parameters.AddWithValue("@ClientprojID", ClientprojID);
                SqlDataAdapter sda = new SqlDataAdapter(objCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                        {

                            TaskDate = dr["MonthYearName"].ToString(),
                            NoofHoursWorked = Convert.ToDouble(dr["HoursWorked"].ToString()),
                            colour = dr["colour"].ToString(),
                        });
                    }
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[1].Rows)
                    {
                        objEmpInfo = new timesheet()
                        {

                            UserName = dr1["Username"].ToString(),
                            ManagerName1 = dr1["L1_ManagerName"].ToString(),
                            ManagerName2 = dr1["L2_ManagerName"].ToString(),
                            TotalMonthName = dr1["TotalMonthName"].ToString(),

                        };

                    }
                }

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

            var result = new { objGetTimeSheet.timeSheetDetails, objEmpInfo };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult GetByMonthlyPreviewTimesheets(int mode)
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[ByMonthly_WebGetTimeSheetforApproval]", Conn);
                //SqlCommand cmd = new SqlCommand("[Test_ByMonthly_WebGetTimeSheetforApproval]", Conn);

                
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //if (roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                cmd.Parameters.AddWithValue("@userid", UserID);
                cmd.Parameters.AddWithValue("@mode", mode);

                //s }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((roleid == "1001") || (roleid == "1002") || (roleid == "1007") || (roleid == "1010") || (roleid == "1011") || (roleid == "1053") || (roleid == "1061") || (roleid == "1006"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                ClientProjectName=dr["ClientProjTitle"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                            {
                                ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                ClientProjectName = druser["ClientProjTitle"].ToString(),

                                TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                                Month_Year = druser["monthyearname"].ToString(),
                                ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                                CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"]),
                                TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                                userName = druser["Usr_Username"].ToString(),
                                ManagerApprovalStatus = druser["FinalStatus"].ToString(),
                                SubmittedDate = druser["SubmittedDate"].ToString(),
                                ApprovedDate = (druser["L2_ManagerName"].ToString() != null && druser["L2_ManagerName"].ToString() != "") ? druser["L2_ApproverDate"].ToString() : druser["L1_ApproverDate"].ToString(),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerName2 = druser["L2_ManagerName"].ToString(),
                                ByMonthlyDates = druser["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(druser["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = druser["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = druser["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }




                }


                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),

                                ClientprojectId = Convert.ToInt16(dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),

                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });
                        }
                    }
                }
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


            var result = new { objmanagerdetails.mytimesheets, objmanagerdetails.timesheetsforapproval, roleid };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetByMonthlyPreviewTimesheets15day()
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[ByMonthly_WebGetTimeSheetforApproval15day]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //if (roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                cmd.Parameters.AddWithValue("@userid", UserID);
                //s }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((roleid == "1001") || (roleid == "1002") || (roleid == "1007") || (roleid == "1010") || (roleid == "1011") || (roleid == "1053") || (roleid == "1061") || (roleid == "1006"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),

                                ClientprojectId = Convert.ToInt16(dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                            {
                                ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                                ClientprojectId = Convert.ToInt16(druser["ClientProjtId"]),
                                ClientProjectName = druser["ClientProjTitle"].ToString(),
                                Month_Year = druser["monthyearname"].ToString(),
                                ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                                CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"]),
                                TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                                userName = druser["Usr_Username"].ToString(),
                                ManagerApprovalStatus = druser["FinalStatus"].ToString(),
                                SubmittedDate = druser["SubmittedDate"].ToString(),
                                ApprovedDate = (druser["L2_ManagerName"].ToString() != null && druser["L2_ManagerName"].ToString() != "") ? druser["L2_ApproverDate"].ToString() : druser["L1_ApproverDate"].ToString(),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerName2 = druser["L2_ManagerName"].ToString(),
                                ByMonthlyDates = druser["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(druser["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = druser["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = druser["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }




                }


                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                ClientprojectId = Convert.ToInt16(dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });
                        }
                    }
                }
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


            var result = new { objmanagerdetails.mytimesheets, objmanagerdetails.timesheetsforapproval, roleid };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWeeklyPreviewTimesheets()
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[Weekly_WebGetTimeSheetforApproval]", Conn);



                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //if (roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                cmd.Parameters.AddWithValue("@userid", UserID);
                //cmd.Parameters.AddWithValue("@TSheetMode", tsmode);


                //s }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((roleid == "1001") || (roleid == "1002") || (roleid == "1007") || (roleid == "1010") || (roleid == "1011") || (roleid == "1053") || (roleid == "1061") || (roleid == "1006"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),

                                ClientprojectId = Convert.ToInt32(dr["ClientProjtId"] == System.DBNull.Value ? "" : dr["ClientProjtId"]),

                                TimesheetMode = Convert.ToInt32(dr["TimesheetMode"] == System.DBNull.Value ? "" : dr["TimesheetMode"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                            {
                                ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                                ClientprojectId = Convert.ToInt32(druser["ClientProjtId"] == System.DBNull.Value ? "" : druser["ClientProjtId"]),
                                ClientProjectName = druser["ClientProjTitle"].ToString(),
                                TimesheetMode = Convert.ToInt32(druser["TimesheetMode"] == System.DBNull.Value ? "" : druser["TimesheetMode"]),

                                Month_Year = druser["monthyearname"].ToString(),
                                ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                                CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"]),
                                TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                                userName = druser["Usr_Username"].ToString(),
                                ManagerApprovalStatus = druser["FinalStatus"].ToString(),
                                SubmittedDate = druser["SubmittedDate"].ToString(),
                                ApprovedDate = (druser["L2_ManagerName"].ToString() != null && druser["L2_ManagerName"].ToString() != "" && druser["L2_ManagerName"].ToString() != "0") ? druser["L2_ApproverDate"].ToString() : druser["L1_ApproverDate"].ToString(),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerName2 = druser["L2_ManagerName"].ToString(),
                                ByMonthlyDates = druser["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(druser["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = druser["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = druser["UProj_L2_ManagerId"].ToString(),

                            });

                        }
                    }




                }


                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objmanagerdetails.mytimesheets.Add(new UserTimesheets
                            {
                                ProjectId = Convert.ToInt32(dr["Proj_ProjectID"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                ClientprojectId = Convert.ToInt32(dr["ClientProjtId"] == System.DBNull.Value ? "" : dr["ClientProjtId"]),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                TimesheetMode = Convert.ToInt32(dr["TimesheetMode"] == System.DBNull.Value ? "" : dr["TimesheetMode"]),

                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_UserID = Convert.ToInt32(dr["UserID"].ToString()),
                                userName = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                                SubmittedDate = dr["SubmittedDate"].ToString(),
                                ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                                ManagerName1 = dr["L1_ManagerName"].ToString(),
                                ManagerName2 = dr["L2_ManagerName"].ToString(),
                                ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                                TimesheetMonth = Convert.ToDateTime(dr["TimesheetMonth"]).ToShortDateString(),
                                UProj_L1ManagerId = dr["UProj_L1_ManagerId"].ToString(),
                                UProj_L2ManagerId = dr["UProj_L2_ManagerId"].ToString(),
                            });
                        }
                    }
                }
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


            var result = new { objmanagerdetails.mytimesheets, objmanagerdetails.timesheetsforapproval, roleid };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region (getUserTimesheet)Preview the Specific MonthData of  Specific userdata 
        public ActionResult ViewByWeeklySubmitedTimesheet(int TimesheetUserid, string Timesheetstartdate, string TimesheetEnddate, int Accountid, int Projectid, int ClientProjectId)
        {
            //if()

            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            objGetTimeSheet.UploadTimesheetimage = new List<UploadTimesheetimages>();
            //objGetTimeSheet.Status = new List<statusMessage>();
            UserSessionInfo info = new UserSessionInfo();

            int? timesheetid = info.TimesheetMode;
            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand();
                //if (timesheetid== 4)
                //{
                //cmd = new SqlCommand("WebgetUserbyMonthlyTimesheet15days", Conn);
                //}
                //else if(timesheetid == 3)
                //{
                cmd = new SqlCommand("WebgetUserbyMonthlyTimesheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", TimesheetUserid);
                cmd.Parameters.AddWithValue("@Timesheetstart", Convert.ToDateTime(Timesheetstartdate).ToString("dd/MMM/yyyy"));
                cmd.Parameters.AddWithValue("@Timesheetend", Convert.ToDateTime(TimesheetEnddate).ToString("dd/MMM/yyyy"));
                cmd.Parameters.AddWithValue("@accountid", Accountid);
                cmd.Parameters.AddWithValue("@projectid", Projectid);
                cmd.Parameters.AddWithValue("@ClientprojID", ClientProjectId);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                    {
                        TimesheetID = Convert.ToInt32(dr["TimesheetID"]),
                        Taskid = Convert.ToInt32(dr["taskid"]),
                        Month_Year = dr["monthyearname"].ToString(),
                        Taskname = dr["taskname"].ToString(),
                        NoofHoursWorked = Convert.ToDouble(dr["HoursWorked"].ToString()),
                        MonthYearName = dr["monthyearname"].ToString(),
                        //dr["MonthYearName"].ToString(),
                        Comments = dr["Comments"].ToString(),
                        UserName = dr["Usr_Username"].ToString(),
                        Submitted_Type=Convert.ToInt32(dr["SubmittedType"]),
                        TotalMonthName = dr["TotalMonthName"].ToString(),
                        ProjectName = dr["Proj_ProjectName"].ToString(),
                        ProjectClientName = dr["ClientProjTitle"].ToString(),
                        ManagerName1 = dr["L1_ManagerName"].ToString(),
                        ManagerName2 = dr["L2_ManagerName"].ToString(),
                        SubmittedDate = dr["SubmittedDate"].ToString(),
                        //dr["SubmittedDate"].ToString(),
                        //ApprovedDate = dr["L1_ApproverDate"].ToString()
                        ApprovedDate = (dr["L2_ManagerName"].ToString() != null && dr["L2_ManagerName"].ToString() != "" && dr["L2_ManagerName"].ToString() != "0") ? dr["L2_ApproverDate"].ToString() : dr["L1_ApproverDate"].ToString(),
                        // ByMonthlyDates = dr["ByMonthlyDates"].ToString(),
                        L1_ApproverStatus = Convert.ToInt32(dr["L1_ApproverStatus"]),
                        L2_ApproverStatus = Convert.ToInt32(dr["L2_ApproverStatus"]),
                    });


                }


                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    objGetTimeSheet.UploadTimesheetimage.Add(new UploadTimesheetimages
                    {
                        Uploadedimages = dr["uploadedimages"].ToString(),
                        Attachmentid = Convert.ToInt16(dr["attachmentid"]),

                    });


                }



            }
            catch (Exception ex)
            {

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


            return Json(new { objGetTimeSheet.timeSheetDetails, objGetTimeSheet.UploadTimesheetimage }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region ApplyColoursaccording to Leaves  and Holidays
        public ActionResult LoadColourBymonthly(int TimesheetUserid, string Timesheetstartdate, string TimesheetEnddate, int Accountid, int Projectid)
        {

            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            Timesheetlist objGetTimeSheet = new Timesheetlist();
            objGetTimeSheet.timeSheetDetails = new List<TimeSheetDetails>();
            // objGetTimeSheet.Status = new List<statusMessage>();

            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("WebgetUserbyweeklyTimesheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", TimesheetUserid);
                cmd.Parameters.AddWithValue("@Timesheetstart", Timesheetstartdate);
                cmd.Parameters.AddWithValue("@Timesheetend", TimesheetEnddate);
                cmd.Parameters.AddWithValue("@accountid", Accountid);
                cmd.Parameters.AddWithValue("@projectid", Projectid);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    objGetTimeSheet.timeSheetDetails.Add(new TimeSheetDetails
                    {
                        colour = dr["colour"].ToString(),

                    });

                }

            }
            catch (Exception ex)
            {

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

            return Json(objGetTimeSheet.timeSheetDetails, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Push notifications
        public void Getdata(string Title, string Message, string userid)
        {

            var SendMessage = Message;
            var Messagetitle = Title;
            var Selectgrade = lstusers;
            Conn = new SqlConnection(str);
            //Conn.ConnectionString = Conn;
            Conn.Open();
            using (SqlCommand cmd2 = new SqlCommand("Select TokenID from [UserDevicesToken] where UserId = '" + userid + "'", Conn))
            {
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                adapter1.Fill(dt);
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var deviceid = Convert.ToString(dt.Rows[i]["TokenID"]);
                            //string App_ID = "a122d8e1-6175-4b03-a3d2-3bed67f79672";
                            //string Rest_API_KEY = "YjA4MDczMjctYjFlMy00MzBiLTgyYTctMTE1OWJhNzA5NDlh";
                            ////string SERVER_API_KEY = "AAAAuP0gfzU:APA91bFP-UKu97svEZy-b42JVuxMjRyxs8RWt_ndXgRgIatjZoKN5ehNf0yznMy2iu-y_4zE_b0IRV3P-k33aFHYis_Ory73GX7iu1b3jA8MRvh9rjtPOELA_WvXOfdpz5oisCPktoEk";
                            ////string SENDER_ID = "794520747829";
                            //string device_Id = deviceid;
                            //WebRequest tRequest = WebRequest.Create("https://onesignal.com/api/v1/notifications");
                            string SERVER_API_KEY = "AAAAkdZpjkI:APA91bH2bExO4DyObdbiGxYcyax7JvLFAYqmYFqMvpltvC5Rdb-U0XhHf_h_bLuHKcFJfaqtLL7_D5Rk_GOwwVQDPhFm6Hfjg56kSBdTn6czIJYBKsbJwkMsSZDQBKtfIo2d9bzs_5Df";

                            string SENDER_ID = "626367499842";
                            string device_Id = deviceid;
                            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                            tRequest.Method = "post";
                            tRequest.ContentType = "application/json";
                            var data = new
                            {
                                to = device_Id,
                                notification = new
                                {
                                    body = Message,
                                    title = Title,
                                    sound = "Enabled"
                                }
                            };

                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(data);
                            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                            //tRequest.Headers.Add(string.Format("Authorization: key={0}", Rest_API_KEY));
                            //tRequest.Headers.Add(string.Format("Sender: id={0}", App_ID));
                            tRequest.ContentLength = byteArray.Length;

                            using (Stream dataStream = tRequest.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);
                                using (WebResponse tResponse = tRequest.GetResponse())
                                {
                                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                    {
                                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                        {
                                            String sResponseFromServer = tReader.ReadToEnd();
                                            string str = sResponseFromServer;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }

        }
        #endregion
        #region Manager Push notifications
        public void ManagerPushNotifications(string Title, string Message, string userid, string ManagerID)
        {

            var SendMessage = Message;
            var Messagetitle = Title;
            var Selectgrade = lstusers;
            Conn = new SqlConnection(str);
            //Conn.ConnectionString = Conn;
            Conn.Open();
            using (SqlCommand cmd2 = new SqlCommand("Select TokenID from [UserDevicesToken] where UserId = '" + ManagerID + "'", Conn))
            {
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                adapter1.Fill(dt);
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var deviceid = Convert.ToString(dt.Rows[i]["TokenID"]);
                            //string App_ID = "a122d8e1-6175-4b03-a3d2-3bed67f79672";
                            //string Rest_API_KEY = "YjA4MDczMjctYjFlMy00MzBiLTgyYTctMTE1OWJhNzA5NDlh";
                            ////string SERVER_API_KEY = "AAAAuP0gfzU:APA91bFP-UKu97svEZy-b42JVuxMjRyxs8RWt_ndXgRgIatjZoKN5ehNf0yznMy2iu-y_4zE_b0IRV3P-k33aFHYis_Ory73GX7iu1b3jA8MRvh9rjtPOELA_WvXOfdpz5oisCPktoEk";
                            ////string SENDER_ID = "794520747829";
                            //string device_Id = deviceid;
                            //WebRequest tRequest = WebRequest.Create("https://onesignal.com/api/v1/notifications");
                            string SERVER_API_KEY = "AAAAkdZpjkI:APA91bH2bExO4DyObdbiGxYcyax7JvLFAYqmYFqMvpltvC5Rdb-U0XhHf_h_bLuHKcFJfaqtLL7_D5Rk_GOwwVQDPhFm6Hfjg56kSBdTn6czIJYBKsbJwkMsSZDQBKtfIo2d9bzs_5Df";
                            string SENDER_ID = "626367499842";
                            string device_Id = deviceid;
                            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                            tRequest.Method = "post";
                            tRequest.ContentType = "application/json";
                            var data = new
                            {
                                to = device_Id,
                                notification = new
                                {
                                    body = Message,
                                    title = Title,
                                    sound = "Enabled"
                                }
                            };

                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(data);
                            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                            //tRequest.Headers.Add(string.Format("Authorization: key={0}", Rest_API_KEY));
                            //tRequest.Headers.Add(string.Format("Sender: id={0}", App_ID));
                            tRequest.ContentLength = byteArray.Length;

                            using (Stream dataStream = tRequest.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);
                                using (WebResponse tResponse = tRequest.GetResponse())
                                {
                                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                    {
                                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                        {
                                            String sResponseFromServer = tReader.ReadToEnd();
                                            string str = sResponseFromServer;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }

        }
        #endregion


        #region SendMailsBySendGrid
        // public static string SendMailsByMailGun(timesheet lstusers, List<manageremails> objManagerlist, DataTable dtHeading, string body, int flag, string ActionType)
        public string SendMailsBySendGrid(timesheet lstusers, List<manageremails> objManagerlist, DataTable dtHeading, string body, int flag, string ActionType, int timesheetid, int? userid, List<string> data)
        {

            string[] ToMuliId = new string[0];
            string subject = string.Empty;
            string ManagersIDs = string.Empty;
            string href = string.Empty; string ManagerID = string.Empty;
            string comments = string.Empty;
            string LevelManagerID = string.Empty, TimesheetId = string.Empty, Timesheetmonth = string.Empty,
            UserID = string.Empty; string disbledstr = string.Empty; string Projectid = string.Empty;
            string ManagerNameL1 = string.Empty; string ManagerNameL2 = string.Empty; string clientid = string.Empty;
            Encrypt objEncrypt = new Encrypt();
            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
            bool flagforemail = false; string UrlEmailAddress = string.Empty; string UrlEmailImage = string.Empty;
            //var src1="src='"+baseimageone+"'";
            //var src2 = "src='" + baseimagetwo + "'";

            SendGridMessage msgs = new SendGridMessage();
            TimesheetController objTimesheet = new TimesheetController();

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;

            }
            else
            {
                UrlEmailAddress = port1;

            }
            // var companylogo = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~" + @"\uploadimages\images\evolutyzcorplogo.png"));

            UrlEmailImage = "<img alt='Company Logo' style='max-width:100%;max-height:100px;'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + lstusers.AccountImageLogo + "'/>";

            // UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + lstusers.AccountImageLogo + "'";


            int j = 0;
            try
            {

                var client2 = new SendGridClient("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");
                if (flag == 1)
                {

                    // ToMuliId = new string[2] { "sreelakshmi.evolutyz@gmail.com", "" };  
                    ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString(), "" };

                    for (int i = 0; i < ToMuliId.Length; i++)
                    {

                        if (i == 0 && flagforemail == false)
                        {

                            //msgs.AddTo(new EmailAddress(ToMuliId[0], lstusers.ManagerName1));

                            ManagerID = lstusers.ManagerID1.ToString();

                            string LevelManagerID1 = string.Empty;
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
                            objEncrypt = new Encrypt();
                            LevelManagerID1 = objEncrypt.Encryption(ManagerID).ToString();
                            TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
                            Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
                            UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
                            Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
                            ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
                            ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
                            clientid = objEncrypt.Encryption(lstusers.ClientProjectId.ToString());
                            comments = objEncrypt.Encryption(dtHeading.Rows[0]["comments"].ToString());
                            disbledstr = "display:block;";
                            //body = body.Replace("alt", src1);
                            // body = body.Replace("placeholder", src2);

                            if (body.Contains("ApproveID"))
                            {
                                j = 1;
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "&CID=" + clientid + "&COM=" + comments + "'";
                                    //href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
                                    if (body.Contains("hreflang"))
                                    {
                                        body = body.Replace("hreflang", href);
                                    }
                                    //body = body.Replace("href", href);

                                }
                            }
                            if (body.Contains("RejectId"))
                            {
                                j = 0;
                                href = string.Empty;
                                if (ManagerID != "0")
                                {
                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "&CID=" + clientid + "&COM=" + comments + "'";
                                    //href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
                                    body = body.Replace("media", href);
                                    //body = body.Replace("title", href);
                                }
                                //  ping to title
                            }
                        }
                        if (i == 1 && flagforemail == true)
                        {
                            ManagerID = string.Empty;
                            ManagerID = lstusers.ManagerID2.ToString();
                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
                            msgs.Subject = subject;
                            disbledstr = "display:none;";

                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }

                            // string LevelManagerID2 = string.Empty;
                            //objEncrypt = new Encrypt();
                            //LevelManagerID2 = objEncrypt.Encryption(ManagerID).ToString();
                            // TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
                            // Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
                            // UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
                            // Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
                            // ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
                            // ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
                            // if (body.Contains("ApproveID"))
                            // {
                            //     j = 1;
                            //     href = string.Empty;
                            //     body = body.Replace("name", href);
                            //     if (ManagerID != "0")
                            //     {
                            //         href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
                            //         body = body.Replace("name", href);
                            //     }
                            // }                          
                            // if (body.Contains("RejectId"))
                            // {

                            // j = 0;

                            // href = string.Empty;
                            // body = body.Replace("rev", href);
                            //     if (ManagerID != "0")
                            //     {
                            //         href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
                            //         body = body.Replace("rev", href);

                            //     }
                            // }

                        }

                        var emailcontent = "<html>" +
                                                            "<body>" +
                                                            "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                            " <tr>" +
                                                            "<td style=' padding: 8px 4px; text-align:center'>" +
                                                            UrlEmailImage +
                                                            //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                            " </td>" +
                                                            "</tr>" +
                                                            " <tr>" +
                                                            "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                            body +
                                                            " </td>" +
                                                            "</tr>" +
                                                            " <tr>" +
                                                            "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                            //"<b>" + msg + "</b> " +
                                                            " </td>" +
                                                            "</tr>" +
                                                            " <tr style='background-color: #6cb0c9;'>" +
                                                            "<td style='font-size: 9px;text-align:center; '>" +
                                                            "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                            " </td>" +
                                                            "</tr>" +
                                                            "</table>" +
                                                            "</body>" +
                                                            "</html>";



                        msgs = new SendGridMessage()
                        {
                            // From = new EmailAddress(lstusers.UserEmailId, lstusers.UserName),
                            From = new EmailAddress("noreply@evolutyz.com", lstusers.UserName),
                            Subject = subject,
                            HtmlContent = emailcontent
                        };
                        //for (int img = 0; img <= data.Count - 1; img++)
                        //{
                        //    //var path1 = Server.MapPath("~/TimeSheetimages/");
                        //    var path1 = Server.MapPath("~/TimeSheetimages/");

                        //    var fileName = data[img];
                        //    var attchment = path1 + fileName;
                        //    var bytes = System.IO.File.ReadAllBytes(attchment);
                        //    var file = Convert.ToBase64String(bytes);
                        //    msgs.AddAttachment(fileName, file);
                        //}
                        msgs.AddTo(new EmailAddress(ToMuliId[i], ToMuliId[i]));
                        var responses = client2.SendEmailAsync(msgs);
                        objTimesheet.ManagerPushNotifications("'" + lstusers.UserName + "'", subject, lstusers.UserID.ToString(),
                             lstusers.ManagerId.ToString());
                    }

                    for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                    {

                        /// msgs.AddTo(new EmailAddress(objManagerlist[AcctMgrid].manageremail, objManagerlist[AcctMgrid].managername));
                        subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is submitted to managers levels";
                        disbledstr = "display:none;";
                        if (body.Contains("ApproveID"))
                        {
                            body = body.Replace("display:block", disbledstr);
                        }

                        if (body.Contains("RejectId"))
                        {
                            body = body.Replace("display:block", disbledstr);

                        }

                        var emailcontent = "<html>" +
                                                           "<body>" +
                                                          "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 8px 4px; text-align:center'>" +
                                                           //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                           UrlEmailImage +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          body +
                                                          // "HI" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr>" +
                                                          "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                          //"<b>" + msg + "</b> " +
                                                          " </td>" +
                                                          "</tr>" +
                                                          " <tr style='background-color: #6cb0c9;'>" +
                                                          "<td style='font-size: 9px;text-align:center; '>" +
                                                          "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                          " </td>" +
                                                          "</tr>" +
                                                          "</table>" +
                                                          "</body>" +
                                                          "</html>";


                        msgs = new SendGridMessage()
                        {
                            //From = new EmailAddress(lstusers.UserEmailId, lstusers.UserName),
                            From = new EmailAddress("noreply@evolutyz.com", lstusers.UserName),
                            Subject = subject,
                            HtmlContent = emailcontent

                        };
                        for (int img = 0; img <= data.Count - 1; img++)
                        {
                            var path1 = Server.MapPath("~/TimeSheetimages/");
                            var fileName = data[img];
                            var attchment = path1 + fileName;
                            var bytes = System.IO.File.ReadAllBytes(attchment);
                            var file = Convert.ToBase64String(bytes);
                            msgs.AddAttachment(fileName, file);
                        }

                        msgs.AddTo(new EmailAddress(objManagerlist[AcctMgrid].manageremail, objManagerlist[AcctMgrid].managername));
                        var responses = client2.SendEmailAsync(msgs);

                    }

                    objTimesheet.Getdata("'" + lstusers.UserName + "'", subject, lstusers.UserID.ToString());
                }
                if (flag == 2)
                {

                    string TimesheetIds = dtHeading.Rows[0]["TimesheetId"].ToString();
                    List<string> data1 = obj.GetImages(Convert.ToInt32(TimesheetIds));
                    //Manager1 will send mails to manager2 and emp
                    if (lstusers.ManagerId.ToString() == lstusers.ManagerID1.ToString())
                    {

                        if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                        {
                            // ToMuliId = new string[2] { "sreelakshmichinnala@gmail.com", "sreelakshmi.evolutyz@gmail.com" };
                            ToMuliId = new string[2] { lstusers.ManagerEmail2.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };
                        }
                        else
                        {
                            ///ToMuliId = new string[2] { "", "sreelakshmi.evolutyz@gmail.com" };
                            ToMuliId = new string[2] { "", lstusers.UserEmailId.ToString().Trim() };
                        }


                        for (int i = 0; i < ToMuliId.Length; i++)
                        {
                            ManagerID = lstusers.ManagerID1.ToString();


                            if (i == 0 && flagforemail == false)
                            {
                                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                                {
                                    //  msgs.AddTo(new EmailAddress(ToMuliId[0], lstusers.ManagerName2));
                                    string LevelManagerID2 = string.Empty;
                                    objEncrypt = new Encrypt();
                                    if (lstusers.Is_DirectManager == 0)
                                    {
                                        subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by  (" + lstusers.ManagerName1 + ")";
                                    }
                                    else
                                    {



                                        if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                                        {

                                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by " + lstusers.ManagerName1 + " " + "and waiting for your approval";
                                        }
                                        else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                        {
                                            subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
                                            //subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by " + lstusers.ManagerName2 + "";
                                        }



                                    }

                                    LevelManagerID2 = objEncrypt.Encryption(lstusers.ManagerID2.ToString().Trim()).ToString();
                                    TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
                                    Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
                                    UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
                                    Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
                                    ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
                                    ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
                                    clientid = objEncrypt.Encryption(lstusers.ClientProjectId.ToString());
                                    disbledstr = "display:block;";

                                    if (body.Contains("ApproveID"))
                                    {
                                        body = body.Replace("display:none", disbledstr);
                                    }

                                    if (body.Contains("RejectId"))
                                    {
                                        body = body.Replace("display:block", disbledstr);
                                    }


                                    if (body.Contains("ApproveID"))
                                    {
                                        j = 1;
                                        if (ManagerID != "0")
                                        {
                                            href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "&CID=" + clientid + "'";
                                            //href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";

                                            //body = body.Replace("name", href);
                                            body = body.Replace("href", href);
                                        }
                                    }
                                    if (body.Contains("RejectId"))
                                    {

                                        j = 0;
                                        //href = string.Empty;
                                        //body = body.Replace("title", href);
                                        if (ManagerID != "0")
                                        {
                                            href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "&CID=" + clientid + "'";
                                            //href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";

                                            //
                                            // body = body.Replace("rev", href);
                                            body = body.Replace("title", href);

                                        }
                                    }

                                }

                                if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                {
                                    subject = string.Empty;

                                    ///msgs.AddTo("", "");
                                }

                            }
                            if (i == 1 && flagforemail == false)
                            {

                                disbledstr = "display:none;";

                                if (body.Contains("ApproveID"))
                                {
                                    body = body.Replace("display:block", "display:none;");
                                }

                                if (body.Contains("RejectId"))
                                {
                                    body = body.Replace("display:block", "display:none;");

                                }

                                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                                {
                                    //ToMuliId = new string[2] { "", lstusers.UserEmailId.ToString().Trim() };

                                    subject = "Timesheet of " + lstusers.UserName + " for Month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by " + lstusers.ManagerName1 + "";


                                }
                                else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                {
                                    //ToMuliId = new string[2] { "", lstusers.UserEmailId.ToString().Trim() };
                                    subject = "Timesheet of " + lstusers.UserName + " for Month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by " + lstusers.ManagerName1 + "";


                                }



                            }
                            var emailcontent2 = "<html>" +
                                                        "<body>" +
                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                      " <tr>" +
                                                      "<td style=' padding: 8px 4px; text-align:center'>" +
                                                       UrlEmailImage +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr>" +
                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                      body +
                                                      // "HI" +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr>" +
                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                      //"<b>" + msg + "</b> " +
                                                      " </td>" +
                                                      "</tr>" +
                                                      " <tr style='background-color: #6cb0c9;'>" +
                                                      "<td style='font-size: 9px;text-align:center; '>" +
                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                      " </td>" +
                                                      "</tr>" +
                                                      "</table>" +
                                                      "</body>" +
                                                      "</html>";



                            if (i == 0)
                            {
                                if (lstusers.Is_DirectManager == 1)
                                {
                                    msgs = new SendGridMessage()
                                    {
                                        From = new EmailAddress("noreply@evolutyz.com", lstusers.UserName),
                                        // From = new EmailAddress(lstusers.UserEmailId, lstusers.UserName),
                                        Subject = subject,
                                        HtmlContent = emailcontent2

                                    };
                                    //for (int img = 0; img <= data1.Count - 1; img++)
                                    //{
                                    //    var path1 = Server.MapPath("~/TimeSheetimages/");
                                    //    var fileName = data[img];
                                    //    var attchment = path1 + fileName;
                                    //    var bytes = System.IO.File.ReadAllBytes(attchment);
                                    //    var file = Convert.ToBase64String(bytes);
                                    //    msgs.AddAttachment(fileName, file);
                                    //}

                                }
                                else
                                {

                                    msgs = new SendGridMessage()
                                    {
                                        From = new EmailAddress("noreply@evolutyz.com", lstusers.ManagerName1),
                                        // From = new EmailAddress(lstusers.ManagerEmail1, lstusers.ManagerName1),
                                        Subject = subject,
                                        HtmlContent = emailcontent2

                                    };
                                    //for (int img = 0; img <= data1.Count - 1; img++)
                                    //{
                                    //    var path1 = Server.MapPath("~/TimeSheetimages/");
                                    //    var fileName = data[img];
                                    //    var attchment = path1 + fileName;
                                    //    var bytes = System.IO.File.ReadAllBytes(attchment);
                                    //    var file = Convert.ToBase64String(bytes);
                                    //    msgs.AddAttachment(fileName, file);
                                    //}

                                }

                                msgs.AddTo(new EmailAddress(ToMuliId[0], lstusers.ManagerName2));
                            }
                            else if (i == 1)
                            {
                                msgs = new SendGridMessage()
                                {
                                    // From = new EmailAddress("sreelakshmi.evolutyz@gmail.com", lstusers.ManagerName1),
                                    From = new EmailAddress("noreply@evolutyz.com", lstusers.ManagerName1),
                                    Subject = subject,
                                    HtmlContent = emailcontent2

                                };
                                //for (int img = 0; img <= data1.Count - 1; img++)
                                //{
                                //    var path1 = Server.MapPath("~/TimeSheetimages/");
                                //    var fileName = data[img];
                                //    var attchment = path1 + fileName;
                                //    var bytes = System.IO.File.ReadAllBytes(attchment);
                                //    var file = Convert.ToBase64String(bytes);
                                //    msgs.AddAttachment(fileName, file);
                                //}
                                msgs.AddTo(new EmailAddress(ToMuliId[1], lstusers.UserName));
                            }

                            var responses = client2.SendEmailAsync(msgs);
                        }

                        for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                        {

                            //disbledstr = "disabled='true'";

                            disbledstr = "display:none;";

                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }
                            // objTimesheet.Getdata("'" + lstusers.UserName + "'", subject, objManagerlist[AcctMgrid].manageremail);
                            var emailcontent = "<html>" +
                                                              "<body>" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 8px 4px; text-align:center'>" +
                                                               //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                               UrlEmailImage +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              // "HI" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              //"<b>" + msg + "</b> " +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            msgs = new SendGridMessage()
                            {

                                //From = new EmailAddress(lstusers.ManagerEmail1, lstusers.ManagerName1),
                                From = new EmailAddress("noreply@evolutyz.com", lstusers.ManagerName1),
                                Subject = subject,
                                HtmlContent = emailcontent

                            };
                            //for (int img = 0; img <= data1.Count - 1; img++)
                            //{
                            //    var path1 = Server.MapPath("~/TimeSheetimages/");
                            //    var fileName = data[img];
                            //    var attchment = path1 + fileName;
                            //    var bytes = System.IO.File.ReadAllBytes(attchment);
                            //    var file = Convert.ToBase64String(bytes);
                            //    msgs.AddAttachment(fileName, file);
                            //}

                            msgs.AddTo(new EmailAddress(objManagerlist[AcctMgrid].manageremail, objManagerlist[AcctMgrid].managername));
                            var responses = client2.SendEmailAsync(msgs);


                        }

                    }
                    //Manager2 will send mails to manager1 and emp
                    if (lstusers.ManagerId.ToString() == lstusers.ManagerID2.ToString())
                    {
                        //ToMuliId = new string[2] { "sreelakshmichinnala@gmail.com", "sreelakshmi.evolutyz@gmail.com" };
                        ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };

                        for (int i = 0; i < ToMuliId.Length; i++)
                        {

                            if (i == 0 && flagforemail == false)
                            {
                                ManagerID = lstusers.ManagerID1.ToString();

                                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                                {

                                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by " + lstusers.ManagerName2 + "";
                                }
                                else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                {

                                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by " + lstusers.ManagerName2 + "";
                                }

                            }

                            if (i == 1 && flagforemail == false)
                            {
                                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
                                {

                                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by " + lstusers.ManagerName2 + "";
                                }
                                else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
                                {

                                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by " + lstusers.ManagerName2 + "";
                                }

                            }



                            disbledstr = "display:none;";
                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }

                            var emailcontent = "<html>" +
                                                               "<body>" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 8px 4px; text-align:center'>" +
                                                              //"<img src=" + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                              UrlEmailImage +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";




                            msgs = new SendGridMessage()
                            {
                                ///From = new EmailAddress(lstusers.ManagerEmail2, lstusers.ManagerName2),
                                From = new EmailAddress("noreply@evolutyz.com", lstusers.ManagerName2),
                                Subject = subject,
                                HtmlContent = emailcontent

                            };
                            //for (int img = 0; img <= data1.Count - 1; img++)
                            //{
                            //    var path1 = "http://" + UrlEmailAddress + "/TimeSheetimages/";
                            //    var fileName = data[img];
                            //    var attchment = path1 + fileName;
                            //    var bytes = System.IO.File.ReadAllBytes(attchment);
                            //    var file = Convert.ToBase64String(bytes);
                            //    msgs.AddAttachment(fileName, file);
                            //}
                            if (i == 0)
                            {
                                msgs.AddTo(new EmailAddress(ToMuliId[0], lstusers.ManagerName1));
                            }
                            else
                            {
                                msgs.AddTo(new EmailAddress(ToMuliId[1], lstusers.UserName));
                            }

                            var responses = client2.SendEmailAsync(msgs);
                        }


                        for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                        {

                            //disbledstr = "disabled='true'";

                            disbledstr = "display:none;";

                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }
                            // objTimesheet.Getdata("'" + lstusers.UserName + "'", subject, objManagerlist[AcctMgrid].manageremail);
                            var emailcontent = "<html>" +
                                                               "<body>" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 8px 4px; text-align:center '>" +
                                                               //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                               UrlEmailImage +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              // "HI" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              //"<b>" + msg + "</b> " +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            msgs = new SendGridMessage()
                            {
                                // From = new EmailAddress(lstusers.ManagerEmail2, lstusers.ManagerName2),
                                From = new EmailAddress("noreply@evolutyz.com", lstusers.ManagerName2),
                                Subject = subject,
                                HtmlContent = emailcontent

                            };
                            //for (int img = 0; img <= data1.Count - 1; img++)
                            //{
                            //    var path1 = "http://" + UrlEmailAddress + "/TimeSheetimages/";
                            //    var fileName = data[img];
                            //    var attchment = path1 + fileName;
                            //    var bytes = System.IO.File.ReadAllBytes(attchment);
                            //    var file = Convert.ToBase64String(bytes);
                            //    msgs.AddAttachment(fileName, file);
                            //}
                            msgs.AddTo(new EmailAddress(objManagerlist[AcctMgrid].manageremail, objManagerlist[AcctMgrid].managername));
                            var responses = client2.SendEmailAsync(msgs);


                        }
                    }

                    if (lstusers.ManagerId.ToString() == "1002")
                    {
                        UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
                        var sessionuserid = sessId.UserId;
                        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                        var AdminEmailid = (from u in db.Users where u.Usr_UserID == sessionuserid select u.Usr_LoginId).FirstOrDefault();
                        var Adminname = (from u in db.UsersProfiles where u.UsrP_UserID == sessionuserid select u.UsrP_FirstName).FirstOrDefault();

                        ToMuliId = new string[3] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.ManagerEmail2.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };
                        subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  revoked by admin";


                        for (int i = 0; i < ToMuliId.Length; i++)
                        {

                            disbledstr = "display:none;";
                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            var emailcontent = "<html>" +
                                                               "<body>" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 8px 4px; text-align:center '>" +
                                                               UrlEmailImage +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";

                            msgs = new SendGridMessage()
                            {

                                // From = new EmailAddress(AdminEmailid, Adminname),
                                From = new EmailAddress("noreply@evolutyz.com", Adminname),
                                Subject = subject,
                                HtmlContent = emailcontent

                            };
                            //for (int img = 0; img <= data1.Count - 1; img++)
                            //{
                            //    var path1 = Server.MapPath("~/TimeSheetimages/");
                            //    var fileName = data[img];
                            //    var attchment = path1 + fileName;
                            //    var bytes = System.IO.File.ReadAllBytes(attchment);
                            //    var file = Convert.ToBase64String(bytes);
                            //    msgs.AddAttachment(fileName, file);
                            //}
                            msgs.AddTo(new EmailAddress(ToMuliId[i], ToMuliId[i]));
                            var responses = client2.SendEmailAsync(msgs);

                        }

                        for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
                        {

                            //disbledstr = "disabled='true'";

                            disbledstr = "display:none;";

                            if (body.Contains("ApproveID"))
                            {
                                body = body.Replace("display:block", disbledstr);
                            }

                            if (body.Contains("RejectId"))
                            {
                                body = body.Replace("display:block", disbledstr);

                            }
                            // objTimesheet.Getdata("'" + lstusers.UserName + "'", subject, objManagerlist[AcctMgrid].manageremail);
                            var emailcontent = "<html>" +
                                                                "<body>" +
                                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 8px 4px; text-align:center'>" +
                                                               //"<img src= " + UrlEmailAddress + "'/Content/images/CompanyLogos/502.png'  style='display: block; max-width: 100%; height: 40px;'>" +
                                                               UrlEmailImage +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              body +
                                                              // "HI" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr>" +
                                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                              //"<b>" + msg + "</b> " +
                                                              " </td>" +
                                                              "</tr>" +
                                                              " <tr style='background-color: #6cb0c9;'>" +
                                                              "<td style='font-size: 9px;text-align:center; '>" +
                                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                              " </td>" +
                                                              "</tr>" +
                                                              "</table>" +
                                                              "</body>" +
                                                              "</html>";


                            msgs = new SendGridMessage()
                            {
                                //From = new EmailAddress(lstusers.Admin_Mailid, lstusers.Admin_Name),
                                From = new EmailAddress("noreply@evolutyz.com", lstusers.Admin_Name),
                                Subject = subject,
                                HtmlContent = emailcontent

                            };
                            //for (int img = 0; img <= data1.Count - 1; img++)
                            //{
                            //    var path1 = Server.MapPath("~/TimeSheetimages/");
                            //    var fileName = data[img];
                            //    var attchment = path1 + fileName;
                            //    var bytes = System.IO.File.ReadAllBytes(attchment);
                            //    var file = Convert.ToBase64String(bytes);
                            //    msgs.AddAttachment(fileName, file);
                            //}
                            msgs.AddTo(new EmailAddress(objManagerlist[AcctMgrid].manageremail, objManagerlist[AcctMgrid].managername));
                            var responses = client2.SendEmailAsync(msgs);
                        }

                    }


                    return body;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "Success";
        }
        #endregion


        private CloudStorageAccount GetCloudBlobStorage()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("EvolutyzBlobTimeSheetImages"));
            return storageAccount;
        }

        // string[] namesimg ;
        List<string> badEmail = new List<string>();

        [HttpPost]
        public void SaveToTemp(saveimage formData)
        {

            CloudStorageAccount storageAccount = GetCloudBlobStorage();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("timesheetimages");

            string path = Server.MapPath("~/TimeSheetimages/");


            UpId = Convert.ToInt32(Request.Form["ID"]);
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase fil = files[i];
                //namesimg[]=files[i].FileName;
                string extension = Path.GetExtension(fil.FileName);
                var str = RandomString() + extension;
                var file = str.ToString();
                badEmail.Add(file);

                fil.SaveAs(path + file);

                CloudBlockBlob blob = container.GetBlockBlobReference(path);

                using (var fileStream = System.IO.File.OpenRead(path + file))
                {
                    blob.UploadFromStream(fileStream);
                }

                //if (System.IO.File.Exists(path + file))
                //{
                //    System.IO.File.Delete(path + file);
                //}
            }
            TempData["errorNotification"] = badEmail;
        }
        private static Random random = new Random();
        public string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        #region AdminPreviewTimesheets
        public ActionResult AdminPreviewTimesheets()
        {
            return View();
        }
        #endregion
        #region AdminPreviewaRevokeTimesheets
        public ActionResult AdminPreviewaRevokeTimesheets(string TimesheetProjectid, string Year, string Month)
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            int accountid = sessId.AccountId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            var AdminEmailid = (from u in db.Users where u.Usr_UserID == UserID select u.Usr_LoginId).FirstOrDefault();
            var Adminname = (from u in db.UsersProfiles where u.UsrP_UserID == UserID select u.UsrP_FirstName).FirstOrDefault();

            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[Admin_WebGetTimeSheetforApproval]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@companyAccountID", accountid);
                cmd.Parameters.AddWithValue("@TimesheetProjectid", TimesheetProjectid);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Month", Month);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow druser in ds.Tables[0].Rows)
                    {

                        objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                        {
                            ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                            ProjectName = druser["Proj_ProjectName"].ToString(),
                            TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                            Month_Year = druser["monthyearname"].ToString(),
                            ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                            CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"] == System.DBNull.Value ? 0 : druser["Companyworkinghours"]),
                            TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                            Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                            userName = druser["Usr_Username"].ToString(),
                            ManagerName1 = druser["L1_ManagerName"].ToString(),
                            ManagerName2 = druser["L2_ManagerName"].ToString(),
                            TimesheetMonth = druser["TimesheetMonth"].ToString(),
                            AdminEmailid = AdminEmailid,
                            AdminName = Adminname,
                            TimesheetDuration = druser["TimesheetDuration"] == System.DBNull.Value ? "" : druser["TimesheetDuration"].ToString(),
                        });

                    }
                }


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


            var result = new { objmanagerdetails.timesheetsforapproval, roleid };


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        ////#region SendMailsByMailGun
        ////public static string SendMailsByMailGun(timesheet lstusers, List<manageremails> objManagerlist, DataTable dtHeading, string body, int flag, string ActionType)
        ////{


        ////    string[] ToMuliId = new string[0];
        ////    string subject = string.Empty;
        ////    string ManagersIDs = string.Empty; bool flagforemail = false;
        ////    RestClient client = new RestClient();
        ////    string href = string.Empty; string ManagerID = string.Empty;
        ////    string LevelManagerID = string.Empty, TimesheetId = string.Empty, Timesheetmonth = string.Empty,
        ////    UserID = string.Empty; string disbledstr = string.Empty; string Projectid = string.Empty;
        ////    string ManagerNameL1 = string.Empty; string ManagerNameL2 = string.Empty;

        ////    HttpStatusCode statusCode; int numericStatusCode; RestRequest request = new RestRequest();
        ////    string host = System.Web.HttpContext.Current.Request.Url.Host;
        ////    string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
        ////    string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

        ////    string UrlEmailAddress = string.Empty;
        ////    string UrlEmailImage = string.Empty;

        ////    if (host == "localhost")
        ////    {
        ////        UrlEmailAddress = host + ":" + port;

        ////    }
        ////    else
        ////    {
        ////        UrlEmailAddress = port1;

        ////    }

        ////    //UrlEmailImage = "<img alt='Company Logo' style='height:100px;max-width:100%;margin:auto;display:block;'  src='" + "https://" + UrlEmailAddress + "/uploadimages/Images/" + lstusers.AccountImageLogo + "'";
        ////    UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/Images/thumb/" + lstusers.AccountImageLogo + "'";
        ////    int j = 0;
        ////    try
        ////    {
        ////        client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        ////        client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
        ////        //request.AddHeader
        ////        Encrypt objEncrypt;
        ////        if (flag == 1)
        ////        {
        ////            // ToMuliId = new string[2] { "sreelakshmichinnala@gmail.com", "sreelakshmi.evolutyz@gmail.com" };
        ////            ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString(), "" };


        ////            for (int i = 0; i < ToMuliId.Length; i++)
        ////            {
        ////                request = new RestRequest();
        ////                request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                request.Resource = "{domain}/messages";
        ////                request.AddParameter("from", "Evolutyz IT Services<mailgun@evolutyzstaging.com>");
        ////                if (i == 0 && flagforemail == false)
        ////                {
        ////                    request.AddParameter("to", ToMuliId[0]);
        ////                    ManagerID = lstusers.ManagerID1.ToString();
        ////                    string LevelManagerID1 = string.Empty;
        ////                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
        ////                    objEncrypt = new Encrypt();
        ////                    LevelManagerID1 = objEncrypt.Encryption(ManagerID).ToString();
        ////                    TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
        ////                    Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
        ////                    UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
        ////                    Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
        ////                    ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
        ////                    ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        j = 1;
        ////                        if (ManagerID != "0")
        ////                        {
        ////                            href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
        ////                            body = body.Replace("href", href);
        ////                        }
        ////                    }
        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        j = 0;
        ////                        href = string.Empty;
        ////                        if (ManagerID != "0")
        ////                        {
        ////                            href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID1 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL1 + "'";
        ////                            body = body.Replace("title", href);
        ////                        }
        ////                        //  ping to title
        ////                    }
        ////                }
        ////                if (i == 1 && flagforemail == true)
        ////                {
        ////                    request.AddParameter("to", ToMuliId[1]);
        ////                    ManagerID = string.Empty;
        ////                    ManagerID = lstusers.ManagerID2.ToString();
        ////                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  submitted  and waiting for your approval";
        ////                    disbledstr = "display:none;";

        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);

        ////                    }


        ////                    // string LevelManagerID2 = string.Empty;
        ////                    //objEncrypt = new Encrypt();
        ////                    //LevelManagerID2 = objEncrypt.Encryption(ManagerID).ToString();
        ////                    // TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
        ////                    // Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
        ////                    // UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
        ////                    // Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
        ////                    // ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
        ////                    // ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());
        ////                    // if (body.Contains("ApproveID"))
        ////                    // {
        ////                    //     j = 1;
        ////                    //     href = string.Empty;
        ////                    //     body = body.Replace("name", href);
        ////                    //     if (ManagerID != "0")
        ////                    //     {
        ////                    //         href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
        ////                    //         body = body.Replace("name", href);
        ////                    //     }
        ////                    // }                          
        ////                    // if (body.Contains("RejectId"))
        ////                    // {

        ////                    // j = 0;

        ////                    // href = string.Empty;
        ////                    // body = body.Replace("rev", href);
        ////                    //     if (ManagerID != "0")
        ////                    //     {
        ////                    //         href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
        ////                    //         body = body.Replace("rev", href);

        ////                    //     }
        ////                    // }




        ////                }


        ////                var emailcontent = "<html>" +
        ////                                                  "<body />" +
        ////                                                  "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 8px 4px; text-align:center;'>" +
        ////                                                  UrlEmailImage +
        ////                                                  //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                  body +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                  //"<b>" + msg + "</b> " +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr style='background-color: #6cb0c9;'>" +
        ////                                                  "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                  "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  "</table>" +
        ////                                                  "</body>" +
        ////                                                  "</html>";


        ////                request.AddParameter("subject", subject);
        ////                request.AddParameter("html", emailcontent);
        ////                request.Method = Method.POST;
        ////                var a = client.Execute(request);
        ////                statusCode = a.StatusCode;
        ////                numericStatusCode = (int)statusCode;

        ////                if (numericStatusCode == 200)
        ////                {
        ////                    flagforemail = true;

        ////                }
        ////                else
        ////                {
        ////                    flagforemail = true;
        ////                }


        ////            }


        ////            for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
        ////            {

        ////                request = new RestRequest();
        ////                request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                request.Resource = "{domain}/messages";
        ////                request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
        ////                subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is submitted to managers levels";
        ////                //disbledstr = "disabled='true'";

        ////                disbledstr = "display:none;";

        ////                if (body.Contains("ApproveID"))
        ////                {
        ////                    body = body.Replace("display:block", disbledstr);
        ////                }

        ////                if (body.Contains("RejectId"))
        ////                {
        ////                    body = body.Replace("display:block", disbledstr);

        ////                }

        ////                var emailcontent = "<html>" +
        ////                                                  "<body />" +
        ////                                                  "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 8px 4px; text-align:center;'>" +
        ////                                                  UrlEmailImage +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                  body +
        ////                                                  // "HI" +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr>" +
        ////                                                  "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                  //"<b>" + msg + "</b> " +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  " <tr style='background-color: #6cb0c9;'>" +
        ////                                                  "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                  "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                  " </td>" +
        ////                                                  "</tr>" +
        ////                                                  "</table>" +
        ////                                                  "</body>" +
        ////                                                  "</html>";


        ////                request.AddParameter("subject", subject);
        ////                request.AddParameter("html", emailcontent);
        ////                request.Method = Method.POST;
        ////                var a1 = client.Execute(request);

        ////                statusCode = a1.StatusCode;
        ////                numericStatusCode = (int)statusCode;

        ////                if (numericStatusCode == 200)
        ////                {
        ////                    flagforemail = true;


        ////                }
        ////                else
        ////                {
        ////                    flagforemail = true;
        ////                }
        ////            }


        ////        }


        ////        if (flag == 2)
        ////        {
        ////            //Manager1 will send mails to manager2 and emp
        ////            if (lstusers.ManagerId.ToString() == lstusers.ManagerID1.ToString())
        ////            {

        ////                //ToMuliId = new string[2] { "sreelakshmi.evolutyz@gmail.com", "sreelakshmichinnala@gmail.com" };
        ////                ToMuliId = new string[2] { lstusers.ManagerEmail2.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };

        ////                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
        ////                {
        ////                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by level_1 manager (" + lstusers.ManagerName1 + ")";

        ////                }
        ////                else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
        ////                {
        ////                    subject = "Timesheet of " + lstusers.UserName + " for Month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_1 manager " + lstusers.ManagerName1 + "";
        ////                    disbledstr = "display:none;";

        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);

        ////                    }

        ////                }

        ////                for (int i = 0; i < ToMuliId.Length; i++)
        ////                {
        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    ManagerID = lstusers.ManagerID1.ToString();


        ////                    if (i == 0 && flagforemail == false)
        ////                    {
        ////                        if (lstusers.EmailAppOrRejStatus.ToString() == "1")
        ////                        {
        ////                            request.AddParameter("to", ToMuliId[0]);
        ////                            string LevelManagerID2 = string.Empty;
        ////                            objEncrypt = new Encrypt();
        ////                            LevelManagerID2 = objEncrypt.Encryption(lstusers.ManagerID2.ToString().Trim()).ToString();
        ////                            TimesheetId = objEncrypt.Encryption(dtHeading.Rows[0]["TimesheetId"].ToString());
        ////                            Timesheetmonth = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["TimesheetMonthforMail"]));
        ////                            UserID = objEncrypt.Encryption(dtHeading.Rows[0]["Usr_UserID"].ToString());
        ////                            Projectid = objEncrypt.Encryption(Convert.ToString(dtHeading.Rows[0]["ProjectID"].ToString()));
        ////                            ManagerNameL1 = objEncrypt.Encryption(lstusers.ManagerName1.ToString());
        ////                            ManagerNameL2 = objEncrypt.Encryption(lstusers.ManagerName2.ToString());

        ////                            disbledstr = "display:block;";

        ////                            if (body.Contains("ApproveID"))
        ////                            {
        ////                                body = body.Replace("display:block", disbledstr);
        ////                            }

        ////                            if (body.Contains("RejectId"))
        ////                            {
        ////                                body = body.Replace("display:block", disbledstr);

        ////                            }

        ////                            if (body.Contains("ApproveID"))
        ////                            {
        ////                                j = 1;
        ////                                // href = string.Empty;
        ////                                // body = body.Replace("href", href);
        ////                                if (ManagerID != "0")
        ////                                {
        ////                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
        ////                                    body = body.Replace("name", href);
        ////                                    //body = body.Replace("href", href);
        ////                                }
        ////                            }
        ////                            if (body.Contains("RejectId"))
        ////                            {

        ////                                j = 0;
        ////                                //href = string.Empty;
        ////                                //body = body.Replace("title", href);
        ////                                if (ManagerID != "0")
        ////                                {
        ////                                    href = "href='" + "http://" + UrlEmailAddress + "/TimeSheetActions.aspx?MID=" + LevelManagerID2 + "&TID=" + TimesheetId + "&AT=" + objEncrypt.Encryption(Convert.ToString(j)) + "&TD=" + Timesheetmonth + "&Uid=" + UserID + "&F=" + objEncrypt.Encryption("2") + "&Pr=" + Projectid + "&MNOL=" + ManagerNameL2 + "'";
        ////                                    // body = body.Replace("title", href);
        ////                                    body = body.Replace("rev", href);

        ////                                }
        ////                            }
        ////                        }

        ////                        if (lstusers.EmailAppOrRejStatus.ToString() == "0")
        ////                        {
        ////                            request.AddParameter("to", "");

        ////                        }
        ////                    }
        ////                    if (i == 1 && flagforemail == true)
        ////                    {
        ////                        request.AddParameter("to", ToMuliId[1]);
        ////                        disbledstr = "display:none;";

        ////                        if (body.Contains("ApproveID"))
        ////                        {
        ////                            body = body.Replace("display:block", disbledstr);
        ////                        }

        ////                        if (body.Contains("RejectId"))
        ////                        {
        ////                            body = body.Replace("display:block", disbledstr);

        ////                        }

        ////                    }
        ////                    var emailcontent2 = "<html>" +
        ////                                              "<body />" +
        ////                                              "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                              " <tr>" +
        ////                                              "<td style=' padding: 8px 4px; text-align:center;'>" +
        ////                                              UrlEmailImage +
        ////                                              " </td>" +
        ////                                              "</tr>" +
        ////                                              " <tr>" +
        ////                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                              body +
        ////                                              // "HI" +
        ////                                              " </td>" +
        ////                                              "</tr>" +
        ////                                              " <tr>" +
        ////                                              "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                              //"<b>" + msg + "</b> " +
        ////                                              " </td>" +
        ////                                              "</tr>" +
        ////                                              " <tr style='background-color: #6cb0c9;'>" +
        ////                                              "<td style='font-size: 9px;text-align:center; '>" +
        ////                                              "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                              " </td>" +
        ////                                              "</tr>" +
        ////                                              "</table>" +
        ////                                              "</body>" +
        ////                                              "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent2);
        ////                    request.Method = Method.POST;
        ////                    var a6 = client.Execute(request);
        ////                    statusCode = a6.StatusCode;
        ////                    numericStatusCode = (int)statusCode;

        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                }


        ////                for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
        ////                {

        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
        ////                    //disbledstr = "disabled='true'";

        ////                    disbledstr = "display:none;";

        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);

        ////                    }

        ////                    var emailcontent = "<html>" +
        ////                                                      "<body />" +
        ////                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 8px 4px; text-align:center;'>" +
        ////                                                      UrlEmailImage +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      body +
        ////                                                      // "HI" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      //"<b>" + msg + "</b> " +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr style='background-color: #6cb0c9;'>" +
        ////                                                      "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      "</table>" +
        ////                                                      "</body>" +
        ////                                                      "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent);
        ////                    request.Method = Method.POST;
        ////                    var a1 = client.Execute(request);

        ////                    statusCode = a1.StatusCode;
        ////                    numericStatusCode = (int)statusCode;

        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;


        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                }

        ////            }
        ////            //Manager2 will send mails to manager1 and emp
        ////            if (lstusers.ManagerId.ToString() == lstusers.ManagerID2.ToString())
        ////            {
        ////                // ToMuliId = new string[2] { "sreelakshmichinnala@gmail.com", "" };
        ////                ToMuliId = new string[2] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };
        ////                if (lstusers.EmailAppOrRejStatus.ToString() == "1")
        ////                {
        ////                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  approved by level_2 manager " + lstusers.ManagerName2 + "";
        ////                }
        ////                else if (lstusers.EmailAppOrRejStatus.ToString() == "0")
        ////                {
        ////                    subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  rejected by level_2 manager " + lstusers.ManagerName2 + "";
        ////                }
        ////                for (int i = 0; i < ToMuliId.Length; i++)
        ////                {
        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    ManagerID = lstusers.ManagerID1.ToString();
        ////                    disbledstr = "display:none;";

        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);

        ////                    }

        ////                    if (i == 0 && flagforemail == false)
        ////                    {
        ////                        request.AddParameter("to", ToMuliId[0]);


        ////                    }
        ////                    if (i == 1 && flagforemail == true)
        ////                    {
        ////                        request.AddParameter("to", ToMuliId[1]);

        ////                    }

        ////                    var emailcontent = "<html>" +
        ////                                                      "<body />" +
        ////                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 8px 4px; text-align:center;'>" +
        ////                                                      UrlEmailImage +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      body +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr style='background-color: #6cb0c9;'>" +
        ////                                                      "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      "</table>" +
        ////                                                      "</body>" +
        ////                                                      "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent);
        ////                    request.Method = Method.POST;
        ////                    var a5 = client.Execute(request);
        ////                    statusCode = a5.StatusCode;
        ////                    numericStatusCode = (int)statusCode;
        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;

        ////                    }


        ////                }


        ////                for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
        ////                {

        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
        ////                    disbledstr = "display:none;";

        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);

        ////                    }
        ////                    var emailcontent = "<html>" +
        ////                                                      "<body />" +
        ////                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 8px 4px; text-align:center'>" +
        ////                                                      UrlEmailImage +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      body +
        ////                                                      // "HI" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      //"<b>" + msg + "</b> " +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr style='background-color: #6cb0c9;'>" +
        ////                                                      "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      "</table>" +
        ////                                                      "</body>" +
        ////                                                      "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent);
        ////                    request.Method = Method.POST;
        ////                    var a1 = client.Execute(request);

        ////                    statusCode = a1.StatusCode;
        ////                    numericStatusCode = (int)statusCode;

        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;


        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                }

        ////            }

        ////            //Admin will revoke and send mails to respected manager's and emp's
        ////            if (lstusers.ManagerId.ToString() == "1002")
        ////            {
        ////                ToMuliId = new string[3] { lstusers.ManagerEmail1.ToString().Trim(), lstusers.ManagerEmail1.ToString().Trim(), lstusers.UserEmailId.ToString().Trim() };
        ////                subject = "Timesheet of " + lstusers.UserName + " for month " + dtHeading.Rows[0]["TimesheetMonth"].ToString() + " is  revoked by admin";

        ////                for (int i = 0; i < ToMuliId.Length; i++)
        ////                {
        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    ManagerID = lstusers.ManagerID1.ToString();
        ////                    disbledstr = "display:none;";
        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }
        ////                    if (i == 0 && flagforemail == false)
        ////                    {
        ////                        request.AddParameter("to", "sreelakshmi.evolutyz@gmail.com");

        ////                    }
        ////                    if (i == 1 && flagforemail == false)
        ////                    {
        ////                        request.AddParameter("to", "");
        ////                    }
        ////                    if (i == 2 && flagforemail == true)
        ////                    {
        ////                        request.AddParameter("to", "");
        ////                    }

        ////                    var emailcontent = "<html>" +
        ////                                                      "<body />" +
        ////                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 8px 4px; text-align:center'>" +
        ////                                                       UrlEmailImage +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      body +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr style='background-color: #6cb0c9;'>" +
        ////                                                      "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      "</table>" +
        ////                                                      "</body>" +
        ////                                                      "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent);
        ////                    request.Method = Method.POST;
        ////                    var a5 = client.Execute(request);
        ////                    statusCode = a5.StatusCode;
        ////                    numericStatusCode = (int)statusCode;
        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;

        ////                    }


        ////                }


        ////                for (int AcctMgrid = 0; AcctMgrid < objManagerlist.Count; AcctMgrid++)//Account Managerids
        ////                {

        ////                    request = new RestRequest();
        ////                    request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
        ////                    request.Resource = "{domain}/messages";
        ////                    request.AddParameter("from", "Evolutyz ITServices <mailgun@evolutyzstaging.com>");
        ////                    request.AddParameter("to", objManagerlist[AcctMgrid].manageremail);
        ////                    disbledstr = "display:none;";
        ////                    if (body.Contains("ApproveID"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    if (body.Contains("RejectId"))
        ////                    {
        ////                        body = body.Replace("display:block", disbledstr);
        ////                    }

        ////                    var emailcontent = "<html>" +
        ////                                                      "<body />" +
        ////                                                      "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 8px 4px; text-align:center'>" +
        ////                                                     UrlEmailImage +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      body +
        ////                                                      // "HI" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr>" +
        ////                                                      "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
        ////                                                      //"<b>" + msg + "</b> " +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      " <tr style='background-color: #6cb0c9;'>" +
        ////                                                      "<td style='font-size: 9px;text-align:center; '>" +
        ////                                                      "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
        ////                                                      " </td>" +
        ////                                                      "</tr>" +
        ////                                                      "</table>" +
        ////                                                      "</body>" +
        ////                                                      "</html>";


        ////                    request.AddParameter("subject", subject);
        ////                    request.AddParameter("html", emailcontent);
        ////                    request.Method = Method.POST;
        ////                    var a1 = client.Execute(request);

        ////                    statusCode = a1.StatusCode;
        ////                    numericStatusCode = (int)statusCode;

        ////                    if (numericStatusCode == 200)
        ////                    {
        ////                        flagforemail = true;


        ////                    }
        ////                    else
        ////                    {
        ////                        flagforemail = true;
        ////                    }
        ////                }

        ////            }


        ////            return body;

        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }

        ////    return "Success";
        ////}
        ////#endregion



        #region ShareMails
        public string ShareTimesheets(ShareTimeSheets formData)
        {

            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
            string UrlEmailAddress = string.Empty; string UrlEmailImage = string.Empty;
            Conn = new SqlConnection(str); int Userid = 0; int sessionuserid = 0;
            DataTable dtaccMgr = new DataTable();


            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();


                SqlCommand objCommand = new SqlCommand();

                var Timesheetuserid = (from ts in db.TIMESHEETs where ts.TimesheetID == formData.TimeSheetId select ts.UserID).FirstOrDefault();
                var UserEmailid = (from u in db.Users where u.Usr_UserID == Timesheetuserid select u.Usr_LoginId).FirstOrDefault();
                var TimesheetMonth = (from ts in db.TIMESHEETs where ts.TimesheetID == formData.TimeSheetId select ts.TimesheetMonth).FirstOrDefault();
                var checkTimesheettypeMode = (from up in db.UserProjects where up.UProj_UserID == Timesheetuserid select up.TimesheetMode).FirstOrDefault();
                var ClientProjId = (from ts in db.TIMESHEETs where ts.TimesheetID == formData.TimeSheetId select ts.ClientProjtId).FirstOrDefault();

                if ((checkTimesheettypeMode == 2) || (checkTimesheettypeMode == 3))
                {
                    objCommand = new SqlCommand("[ByWeeklygetTimeSheetEmailDetails]", Conn);
                }
                else
                {
                    objCommand = new SqlCommand("[getTimeSheetEmailDetails]", Conn);

                }
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@userid", Timesheetuserid);

                objCommand.Parameters.AddWithValue("@Timesheetmonth", TimesheetMonth.ToString("yyyy-MM-dd"));
                objCommand.Parameters.AddWithValue("@clientPrjId", ClientProjId);
                ds1 = new DataSet();
                da = new SqlDataAdapter(objCommand);
                da.Fill(ds1);
                dtheadings = new DataTable();
                dtData = new DataTable();
                dtheadings = ds1.Tables[0];
                dtData = ds1.Tables[1];
                string subject = string.Empty;

                string body = objemail.SendEmail(lstusers, ConvertDataTableToHTML(dtheadings, dtData, "1"));
                ShareTimesheetsusingSendGrid(formData, body, Timesheetuserid, UserEmailid);

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return StatusMsg;

        }
        #endregion



        #region sharetimesheets

        public string ShareTimesheetsusingSendGrid(ShareTimeSheets formData, string body, int userid, string UserEmailid)
        {
            SendGridMessage msgs = new SendGridMessage();
            var client = new SendGridClient("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");

            try
            {
                string host = System.Web.HttpContext.Current.Request.Url.Host;
                string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
                string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
                string UrlEmailAddress = string.Empty; string UrlEmailImage = string.Empty;

                if (host == "localhost")
                {
                    UrlEmailAddress = host + ":" + port;

                }
                else
                {
                    UrlEmailAddress = port1;

                }

                var getaccntid = (from u in db.Users where u.Usr_UserID == userid select u.Usr_AccountID).FirstOrDefault();
                var getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
                UrlEmailImage = "<img alt='Company Logo' style='max-width:100%;max-height:100px;'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'/>";

                if (body.Contains("ApproveID"))
                {
                    body = body.Replace("display:block", "display:none;");
                }

                if (body.Contains("RejectId"))
                {
                    body = body.Replace("display:block", "display:none;");

                }


                var emailcontent = "<html>" +
                                                           "<body>" +
                                                           "<table style='width:100%;border:1px solid #d6d6d6;border-collapse: collapse;'>" +
                                                           " <tr>" +
                                                           "<td style=' padding: 8px 4px; text-align:center'>" +
                                                           UrlEmailImage +
                                                           //"<img alt='Company Logo' src='http://evolutyz.in/img/evolutyz-logo.png'/>" +
                                                           " </td>" +
                                                           "</tr>" +
                                                           " <tr>" +
                                                           "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                           body +
                                                           " </td>" +
                                                           "</tr>" +
                                                           " <tr>" +
                                                           "<td style=' padding: 10px 5px;    background-color: #ffffff; '>" +
                                                           //"<b>" + msg + "</b> " +
                                                           " </td>" +
                                                           "</tr>" +
                                                           " <tr style='background-color: #6cb0c9;'>" +
                                                           "<td style='font-size: 9px;text-align:center; '>" +
                                                           "<a style= 'color: #ffffff;' href='www.evolutyz.com' target='_blank'>www.evolutyz.com</a>" +
                                                           " </td>" +
                                                           "</tr>" +
                                                           "</table>" +
                                                           "</body>" +
                                                           "</html>";


                msgs = new SendGridMessage()
                {
                    // From = new EmailAddress(lstusers.UserEmailId, lstusers.UserName),
                    // From = new EmailAddress(UserEmailid),
                    From = new EmailAddress("noreply@evolutyz.com"),
                    Subject = formData.Subject,
                    HtmlContent = emailcontent,
                };
                for (int i = 0; i <= formData.mail.Count - 1; i++)
                {
                    if (formData.mail[i] != "")
                    {
                        msgs.AddTo(new EmailAddress(formData.mail[i]));
                    }

                }

                //    msgs.AddTo(new EmailAddress(EmailIdTwo));

                var responses = client.SendEmailAsync(msgs);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }


        #endregion


        public ActionResult AccountTimeSheet()
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            if (image.Acc_CompanyLogo != null)
                ViewBag.imageid = image.Acc_CompanyLogo;
            else
                ViewBag.imageid = "evolutyzcorplogo.png";
            return View();
        }

        public JsonResult GetAccTimeSheet()
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetails objmanagerdetails = new ManagerDetails();
            Conn = new SqlConnection(str);
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
            var image = db.Accounts.Where(x => x.Acc_AccountID == objUserSession.AccountId).ToList().FirstOrDefault();
            var AdminEmailid = (from u in db.Users where u.Usr_UserID == UserID select u.Usr_LoginId).FirstOrDefault();

            //var clientprjid = (from u in db.UserProjects where u.UProj_UserID == UserID  select u.ClientprojID).FirstOrDefault();
            //TempData["clientprjid"] = clientprjid;
            var Adminname = (from u in db.UsersProfiles where u.UsrP_UserID == UserID select u.UsrP_FirstName).FirstOrDefault();
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheets>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[WebGetAccountHolderTimeSheets]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userid", UserID);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow druser in ds.Tables[0].Rows)
                    {

                        objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovals
                        {
                            ProjectId = Convert.ToInt32(druser["Proj_ProjectID"]),
                            ProjectName = druser["Proj_ProjectName"].ToString(),
                            TimesheetID = Convert.ToInt16(druser["Timesheetid"]),
                            //Month_Year = druser["TimesheetMonth"].ToString(),
                            Month_Year = druser["MonthYearName"].ToString(),

                            ClientprojectId = Convert.ToInt32(druser["ClientProjtId"] == System.DBNull.Value ? "" : druser["ClientProjtId"]),
                            ClientProjectName = druser["ClientProjTitle"].ToString(),

                            //Convert.ToInt16(Techaerresource.Rows[i]["Ttrid"] == System.DBNull.Value ? 0 : Techaerresource.Rows[i]["Ttrid"]);
                            TimesheetDuration = druser["TimesheetDuration"] == System.DBNull.Value ? "" : druser["TimesheetDuration"].ToString(),
                            ResourceWorkingHours = Convert.ToInt16(druser["workedhours"]),
                            CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"] == System.DBNull.Value ? 0 : druser["Companyworkinghours"]),
                            TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                            Usr_UserID = Convert.ToInt32(druser["UserID"].ToString()),
                            userName = druser["Usr_Username"].ToString(),
                            ManagerApprovalStatus = druser["FinalStatus"].ToString(),
                            SubmittedDate = druser["SubmittedDate"].ToString(),
                            ApprovedDate = druser["L1_ApproverDate"].ToString(),
                            ManagerName1 = druser["L1_ManagerName"].ToString(),
                            ManagerName2 = druser["L2_ManagerName"].ToString(),
                            UProj_L1ManagerId = druser["UProj_L1_ManagerId"].ToString(),
                            UProj_L2ManagerId = druser["UProj_L2_ManagerId"].ToString(),
                            TimesheetType = druser["TimesheetType"].ToString(),
                            ByMonthlyDates = druser["TimesheetDuration"].ToString(),
                            TotalMonthName = druser["TimesheetDuration"].ToString(),
                            TimesheetMonth = druser["TimesheetMonth"].ToString(),
                            AdminEmailid = AdminEmailid,
                            AdminName = Adminname,
                        });

                    }
                }

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
                    objuser.RoleCode = usersprojects.RoleCode;
                    ViewBag.accountid = usersprojects.Account_ID;
                    ViewBag.tsktaskID = objuser.tsktaskID;
                    ViewBag.User_ID = objuser.User_ID;
                    ViewBag.Projectid = objuser.Proj_ProjectID;
                    ViewBag.ProjectName = objuser.projectName;
                    ViewBag.ClientProjectName = objuser.ProjectClientName;
                    ViewBag.RoleCode = objuser.RoleCode;
                    this.Session["TaskId"] = objuser;
                }
                TempData["AccountId"] = Session["Acountname"];
                TempData["ProjectId"] = objuser.Proj_ProjectID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(objmanagerdetails.timesheetsforapproval, JsonRequestBehavior.AllowGet);
        }
    }
}

