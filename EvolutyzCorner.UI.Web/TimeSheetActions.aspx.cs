using evolCorner.Models;
using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using EvolutyzCorner.UI.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace EvolutyzCorner.UI.Web.Models
{
   
    public partial class TimeSheetActions : System.Web.UI.Page
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection Conn = new SqlConnection();
        Decript objDecript = new Decript();
       static string Managerid = string.Empty,  Timesheetid = string.Empty,
            ActionType = string.Empty, submittedflag = string.Empty, TimesheetMonth = string.Empty, Userid = string.Empty;
        static string Comments = string.Empty;
        TimesheetController objtimesheet = new TimesheetController();
        ClientComponent obj = new ClientComponent();
        timesheet lstobjtime = new timesheet(); static string Emailbody = string.Empty, EmailResponseBody = string.Empty;
        static string ManagerLNames = string.Empty; static string MailProjectid = string.Empty;
        static string clientid = string.Empty;

        TotalTimeSheetTimeDetails sheetObj = new TotalTimeSheetTimeDetails();

        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Managerid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["MID"]));
                Timesheetid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["TID"]));
                ActionType = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["AT"]));
                TimesheetMonth = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["TD"]));
                Userid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["Uid"]));
                submittedflag = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["F"]));
                MailProjectid = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["Pr"]));
                ManagerLNames = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["MNOL"]));
                clientid= objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["CID"]));
                if(Request.QueryString["COM"] != null)
                Comments = objDecript.Decryption(HttpUtility.UrlDecode(Request.QueryString["COM"])); 
            }
           
            int Tid = Convert.ToInt32(Timesheetid);
            //suceessEmail("");
            var checkL1approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == Tid select ts.L1_ApproverStatus).FirstOrDefault();
            var checkL2approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == Tid select ts.L2_ApproverStatus).FirstOrDefault();
            if (ActionType != "1")
            {
                if (checkL1approvestatus == 0 && checkL2approvestatus == 0 || checkL1approvestatus == 1 || checkL2approvestatus == 1)
                {
                    Apply_Managercomments.Visible = true;
                    emailstatus.Visible = false;

                }
            }
            else
            {
                suceessEmail("");
            }
            

            //savecomments(strVal);
            //suceessEmail();

        }

        protected void Managercomments_Click(object sender, EventArgs e)
        {

            Conn = new SqlConnection(str);

            string strVal;

            strVal = txt_comments.Value;
            string text = strVal.Trim().ToString();
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();
                SqlCommand objCommand = new SqlCommand();
                int i = 0;

               
                    objCommand = new SqlCommand("[AddTimesheet_Comments]", Conn);
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.Parameters.AddWithValue("@TimesheetID", Convert.ToInt32(Timesheetid));
                    objCommand.Parameters.AddWithValue("@UserID",Convert.ToInt32(Userid));
                    objCommand.Parameters.AddWithValue("@comments", text);
                    objCommand.Parameters.AddWithValue("@Action", Convert.ToInt32(ActionType));
                    objCommand.Parameters.AddWithValue("@managerid_L1", Convert.ToInt32(Managerid));                    
                        i=    objCommand.ExecuteNonQuery();

             

                if (i>0)
                {
                    suceessEmail(strVal);
                }



                Conn.Close();

            }

            catch (Exception ex)
            {

                ex.Message.ToString();


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


        }



        public void suceessEmail(string strVal)
        {
            
            try
            {
                //string text = string.Empty;
                //if (strVal == null)
                //{
                //    strVal = txt_comments.Value;
                //    text = strVal.Trim().ToString();
                //}
                if (submittedflag == "2")
                {
                    sheetObj.timesheets = new timesheet()
                    {
                        TimesheetID = Convert.ToInt32(Timesheetid),
                        TimeSheetMonth = TimesheetMonth,
                        ManagerId = Managerid,
                        UserID = Convert.ToInt32(Userid),
                        SubmittedFlag = submittedflag,
                        EmailAppOrRejStatus = ActionType,
                        ProjectID = Convert.ToInt32(MailProjectid),
                        ManagerName1 = ManagerLNames.ToString(),
                        ClientProjectId= Convert.ToInt32(clientid),
                        Comments = Comments

                    };




                    lstobjtime = TimeSheetManagerAction(sheetObj.timesheets);

                    List<string> UploadedImagesList = obj.GetImages(sheetObj.timesheets.TimesheetID);
                    Apply_Managercomments.Visible = false;
                    emailstatus.Visible = true;
                    divEmailid.Visible = true;
                    if ((lstobjtime.Transoutput == 1) || (lstobjtime.Transoutput == 2) || (lstobjtime.Transoutput == 3) || (lstobjtime.Transoutput == 4))
                    {
                        sheetObj.timesheets.Transoutput = lstobjtime.Transoutput;
                        Emailbody = objtimesheet.SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                        if (Emailbody.Contains("display:block"))
                        {
                            Emailbody = Emailbody.Replace("display:block", "display:none");
                        }

                        lblEmailstatus.Text = lstobjtime.Message;
                        if ((lstobjtime.Transoutput == 1) || (lstobjtime.Transoutput == 3))
                        {
                            lblEmailstatus.Attributes.Add("style", "color: #00bd00");//
                        }
                        else if ((lstobjtime.Transoutput == 2) || (lstobjtime.Transoutput == 4))
                        {
                            lblEmailstatus.Attributes.Add("style", "color: #f44336");//
                        }
                    }
                    else
                    {
                        sheetObj.timesheets.Transoutput = lstobjtime.Transoutput;
                        Emailbody = objtimesheet.SendMailsForApprovals(sheetObj, sheetObj.timesheets.TimesheetID, sheetObj.timesheets.UserID, UploadedImagesList);
                        lblEmailstatus.Text = lstobjtime.Message;
                        
                       
                        lblEmailstatus.Attributes.Add("style", "color: #f44336");// 
                    }

                    divEmailid.InnerHtml = Emailbody;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region TimeSheetManagerActions
        public timesheet TimeSheetManagerAction(timesheet sheetObj)
        {
            timesheet objtimesheet = new timesheet();
            int Trans_Output = 0;
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            if ((sheetObj.UserID != 0))
            {
                Userid = sheetObj.UserID.ToString();
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
                objCommand.Parameters.AddWithValue("@UserID", sheetObj.UserID);
                objCommand.Parameters.AddWithValue("@TimesheetID", sheetObj.TimesheetID);
                objCommand.Parameters.AddWithValue("@Projectid", sheetObj.ProjectID);
                if (sheetObj.SubmittedType == "3")
                {
                    objCommand.Parameters.AddWithValue("@ManagerId", sessId.UserId);
                }
                else
                {
                    objCommand.Parameters.AddWithValue("@ManagerId", sheetObj.ManagerId);
                }
                objCommand.Parameters.AddWithValue("@ClientprojID", sheetObj.ClientProjectId);
                 objCommand.Parameters.AddWithValue("@Comments", sheetObj.Comments);
                objCommand.Parameters.AddWithValue("@SubmittedType", sheetObj.EmailAppOrRejStatus);
                //objCommand.Parameters.AddWithValue("@Trans_Output", SqlDbType.Int);

                
                objCommand.Parameters.Add("@Trans_Output", SqlDbType.Int);
                objCommand.Parameters["@Trans_Output"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();

                Trans_Output = int.Parse(objCommand.Parameters["@Trans_Output"].Value.ToString());
                var checkL1approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == sheetObj.TimesheetID select ts.L1_ApproverStatus).FirstOrDefault();
                var checkL2approvestatus = (from ts in db.TIMESHEETs where ts.TimesheetID == sheetObj.TimesheetID select ts.L2_ApproverStatus).FirstOrDefault();
                if (checkL1approvestatus == 5 && checkL2approvestatus == 5)
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
                        ManagerName1 = ManagerLNames.ToString(),
                        Message = "Timesheet is already rejected by" + " " + ManagerLNames,
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };



                }
                if (Trans_Output == 900)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "",
                        Message = "Managerid is incorrect",
                        SubmittedState = "Once",
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };


                }

                if (Trans_Output == 1)
                {


                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Approved by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments=sheetObj.Comments,
                    };

                }

                if (Trans_Output == 2)
                {

                    lstobjtime = new timesheet()
                    {

                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Rejected by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };

                }
                if (Trans_Output == 3)
                {

                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Approved by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };
                }
                if (Trans_Output == 4)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Rejected by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };
                }

                if (Trans_Output == 104)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already approved by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };
                }

                if (Trans_Output == 106)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L1",
                        Message = "Timesheet is already rejected by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };
                }
                if (Trans_Output == 105)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Timesheet is already approved by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
                    };

                }
                if (Trans_Output == 107)
                {
                    lstobjtime = new timesheet()
                    {
                        Transoutput = Trans_Output,
                        Position = "L2",
                        Message = "Timesheet is already rejected by" + " " + ManagerLNames,
                        EmailAppOrRejStatus = sheetObj.EmailAppOrRejStatus,
                        UserID = sheetObj.UserID,
                        ManagerId = sheetObj.ManagerId,
                        Comments = sheetObj.Comments,
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

            return lstobjtime;
        }
        #endregion



        public void MailsByMailGun(timesheet lstobjtime, string Emailbody)
        {
            try
            {

                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "1" && lstobjtime.ManagerID1 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Approved By Level-1 Manager')", true);
                    divEmailid.InnerHtml = Emailbody;
                }
                else if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "0" && lstobjtime.ManagerID1 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Rejected By Level-1 Manager')", true);
                }

                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "1" && lstobjtime.ManagerID2 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Approved By Level-2 Manager')", true);
                }
                if (lstobjtime.SubmittedFlag == "2" && lstobjtime.EmailAppOrRejStatus == "0" && lstobjtime.ManagerID2 == lstobjtime.ManagerId)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Rejected By Level-2 Manager')", true);
                }

                divEmailid.InnerHtml = Emailbody;



            }


            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
