using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using evolCorner.Models;
using Evolutyz.Data;
using RestSharp.Authenticators;
using Evolutyz.Business;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;

namespace EvolutyzCorner.UI.Web.Controllers
{
    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    //[EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class DashBoardController : Controller
    {

        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult DashBoard1()

        {
            UserSessionInfo info = new UserSessionInfo();
            string rolename = info.RoleName;
            int accid = info.AccountId;
            int Mid = info.UserId;

            ViewBag.userid = Convert.ToInt32(Session["UserId"]);
            LeaveTypeComponent LeaveTypeComponent = new LeaveTypeComponent();
            var projectslist = LeaveTypeComponent.GetAllProjects().Select(a => new SelectListItem()
            {
                Value = a.Proj_ProjectID.ToString(),
                Text = a.Proj_ProjectName,
            });

            var newsboardcomp = new NewBoardComponent();
            var news = newsboardcomp.GetNewsCollection();

            ViewBag.news = news;

            ViewBag.projectslist = projectslist;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            int accountcount = (from a in db.Accounts
                                where a.Acc_AccountID != 502 && a.Acc_isDeleted == false
                                select new DashboardMails
                                {
                                    accountid = a.Acc_AccountID
                                }).Count();

            int userscount = (from a in db.Users
                              where a.Usr_UserID != 501 && a.Usr_isDeleted == false && a.Usr_AccountID == accid
                              select new DashboardMails
                              {
                                  users = a.Usr_UserID
                              }).Count();
            int allusers = (from a in db.Users
                            where a.Usr_UserID != 501 && a.Usr_isDeleted == false
                            select new DashboardMails
                            {
                                users = a.Usr_UserID
                            }).Count();
            int projects = (from a in db.Projects
                            where a.Proj_isDeleted == false && a.Proj_AccountID == accid
                            select new DashboardMails
                            {
                                Proj_ProjectID = a.Proj_ProjectID
                            }).Count();
            int skillslist = (from a in db.Skills
                              where a.Acc_AccountID == accid && a.Sk_isDeleted == false
                              select new DashboardMails
                              {
                                  SkillId = a.SkillId
                              }).Count();
            int timesheetscnt = (from t in db.TIMESHEETs
                                 join u in db.Users on t.UserID equals u.Usr_UserID
                                 join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                 where a.Acc_AccountID == accid
                                 select new DashboardMails
                                 {
                                     TimesheetID = t.TimesheetID
                                 }).Count();
            //var getempsformangrs = (from up in db.UserProjects
            //                        where up.UProj_L1_ManagerId == Mid || up.UProj_L2_ManagerId == Mid
            //                        select new DashboardMails
            //                        {
            //                            UProj_UserID = up.UProj_UserID
            //                        }).ToList();
            //var getempsformangrs = ()

            var gettimesheetid = db.UserProjects.Where(a => a.UProj_UserID == info.UserId).Select(a => a.TimesheetMode).FirstOrDefault();
            //foreach(var item in gettimesheetid)
            //{

            //}
            TempData["gettimesheetid"] = gettimesheetid;
            ViewBag.timesheetscnt = timesheetscnt;
            ViewBag.skillslist = skillslist;
            ViewBag.projects = projects;
            ViewBag.users = userscount;
            ViewBag.allusers = allusers;
            ViewBag.acountcount = accountcount;
            ViewBag.username = rolename;

            int? userid = info.UserId;
            var userstatus = (from u in db.Users
                              where u.Usr_UserID == userid
                              select new UserEntity
                              {
                                  Usr_ActiveStatus = u.Usr_ActiveStatus
                              }).ToList();
            bool? status = userstatus[0].Usr_ActiveStatus;
            if (status == false)
            {
                return RedirectToAction("ChangePassword");
            }
            else
            {
                return View();
            }

        }



        public JsonResult getprofiles(string Prefix)
        {

            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var Usermail = (from c in db.UsersProfiles
                            where c.UsrP_EmailID.Contains(Prefix.Trim())
                            select new { c.UsrP_EmailID, c.UsrP_EmployeeID }).ToList();

            return Json(Usermail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult DashBoard2()
        {
            return View();
        }

        // GET: DashBoard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashBoard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashBoard/Create
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

        // GET: DashBoard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashBoard/Edit/5
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

        // GET: DashBoard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashBoard/Delete/5
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

        public ActionResult GetDashboardTimeSheet()
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection Conn = new SqlConnection();
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string roleid = sessId.RoleId.ToString();
            ManagerDetailsEntity objmanagerdetails = new ManagerDetailsEntity();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.mytimesheets = new List<UserTimesheetsEntity>();
                objmanagerdetails.timesheetsforapproval = new List<ManagerTimesheetsforApprovalsEntity>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("GetDashBoardTimeSheet", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //if (RoleName == "Admin")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                cmd.Parameters.AddWithValue("@userid", UserID);
                /// }

                cmd.Parameters.AddWithValue("@StartPosition", 1);
                cmd.Parameters.AddWithValue("@EndPosition", 5);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((roleid == "1002") || (roleid == "1007") || (roleid == "1010") || (roleid == "1011") || (roleid == "1053"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.mytimesheets.Add(new UserTimesheetsEntity
                            {

                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_Username = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.timesheetsforapproval.Add(new ManagerTimesheetsforApprovalsEntity
                            {

                                TimesheetID = Convert.ToInt16(druser["Timesheetid"] == DBNull.Value ? 0 : druser["Timesheetid"]),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                ClientProjectName = druser["ClientProjTitle"].ToString(),
                                Month_Year = druser["MonthYearName"].ToString(),
                                ResourceWorkingHours = Convert.ToInt16(druser["WorkedHours"] == DBNull.Value ? 0 : druser["WorkedHours"]),
                                CompanyBillingHours = Convert.ToInt16(druser["Companyworkinghours"] == DBNull.Value ? 0 : druser["Companyworkinghours"]),
                                TimesheetApprovalStatus = druser["ResultSubmitStatus"].ToString(),
                                Usr_Username = druser["Usr_Username"].ToString(),
                                ManagerApprovalStatus = druser["FinalStatus"].ToString(),

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

                            objmanagerdetails.mytimesheets.Add(new UserTimesheetsEntity
                            {
                                TimesheetID = Convert.ToInt16(dr["Timesheetid"]),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                ClientProjectName = dr["ClientProjTitle"].ToString(),
                                Month_Year = dr["MonthYearName"].ToString(),
                                CompanyBillingHours = dr["Companyworkinghours"].ToString(),
                                ResourceWorkingHours = dr["WorkedHours"].ToString(),
                                TimesheetApprovalStatus = dr["ResultSubmitStatus"].ToString(),
                                Usr_Username = dr["Usr_Username"].ToString(),
                                ManagerApprovalStatus = dr["FinalStatus"].ToString(),
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


        public void GetModules()
        {
            (from a in db.Accounts
             join u in db.Users on a.Acc_AccountID equals u.Usr_AccountID
             join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
             join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
             join rm in db.RoleModules on r.Rol_RoleID equals rm.RMod_RoleID
             join mm in db.Master_Sub_Module on rm.Sub_ModuleID equals mm.Sub_ModuleID
             join mat in db.ModuleAccessTypes on rm.ModuleAccessTypeID equals mat.ModuleAccessTypeID
             where u.Usr_UserID == Convert.ToInt32(Session["userid"])
             select new

             {
                 UserID = u.Usr_UserID,
                 AccounId = a.Acc_AccountID,
                 RoleName = gr.Title,
                 ModuleAccessType = mat.ModuleAccessType1,
                 ModuleName = mm.Sub_ModuleName
             }).ToList();


        }


        public PartialViewResult GetAdminMenu()
        {

            var model = (from a in db.Accounts
                         join u in db.Users on a.Acc_AccountID equals u.Usr_AccountID
                         join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                         join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
                         join rm in db.RoleModules on r.Rol_RoleID equals rm.RMod_RoleID
                         join mm in db.Master_Sub_Module on rm.Sub_ModuleID equals mm.Sub_ModuleID
                         join mat in db.ModuleAccessTypes on rm.ModuleAccessTypeID equals mat.ModuleAccessTypeID
                         where u.Usr_UserID == Convert.ToInt32(Session["userid"])
                         select new

                         {
                             UserID = u.Usr_UserID,
                             AccounId = a.Acc_AccountID,
                             RoleName = gr.Title,
                             ModuleAccessType = mat.ModuleAccessType1,
                             ModuleName = mm.Sub_ModuleName
                         }).ToList();

            //  var model = new AdminMenuViewModel();

            return PartialView(model);
        }
        
    


    }
}
