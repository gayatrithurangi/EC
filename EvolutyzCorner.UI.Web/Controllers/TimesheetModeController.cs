using evolCorner.Models;
using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvolutyzCorner.UI.Web.Controllers
{
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class TimesheetModeController : Controller
    {
        // GET: TimesheetMode
        LoginComponent loginComponent = new LoginComponent();
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult BiWeeklyTimeSheet(int userid, string timesheetstartdate, string timesheetenddate,string Byweeklyid,int projid, int mode)
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;
            LoginComponent loginComponent = new LoginComponent();
           
            TempData["USERID"] = objUserSession.UserId;
            var Clientprojectname = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.ClientProjTitle).FirstOrDefault();
            var projectid = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.Proj_ProjectID).FirstOrDefault();
            ViewBag.StartDate = timesheetstartdate;
            ViewBag.EndDate   = timesheetenddate;
            ViewBag.Byweeklyid = Byweeklyid;

            ViewBag.Cprojid = projid;
            ViewBag.Clientprojectname = Clientprojectname;
            ViewBag.projectid = projectid;
            ViewBag.Mode = mode;
            TempData["Cprojid"] = projid;

            var AccountClientName = (from p in db.Projects
                                     join cp in db.ClientProjects on p.Proj_ProjectID equals cp.Proj_ProjectID
                                     where cp.CL_ProjectID == projid
                                     select new
                                     {
                                         p.Proj_ProjectName
                                     }).Distinct().FirstOrDefault();
            ViewBag.AccClientName = AccountClientName.Proj_ProjectName;


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

                this.Session["TaskId"] = objuser;
                return View(objuser);
            }
            else
            {
                return View();
            }

        }

        public ActionResult BiMonthlyUserTimeSheet(int userid, string timesheetstartdate, string timesheetenddate, string Bymonthlyid, int projid,int mode)
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;

            ViewBag.StartDate = timesheetstartdate;
            ViewBag.EndDate = timesheetenddate;
            ViewBag.Bymonthlyid = Bymonthlyid;


            ViewBag.ClientProjectid = projid;
            var ClientProjectName = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.ClientProjTitle).FirstOrDefault();
            

            var clientProj_Projectid  = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.Proj_ProjectID).FirstOrDefault();
            ViewBag.clientprojname = ClientProjectName;
            ViewBag.clientProj_Projectid = clientProj_Projectid;
            //ViewBag.CL_Projectid = CL_Projectid;
            ViewBag.Mode = mode;
            LoginComponent loginComponent = new LoginComponent();
        
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
                //ViewBag.CL_Projectid = CL_Projectid;

                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }

        public ActionResult WeeklyUserTimeSheetData(int userid, string timesheetstartdate, string timesheetenddate, string Bymonthlyid, int projid,int mode)
        {
            UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;

            ViewBag.StartDate = timesheetstartdate;
            ViewBag.EndDate = timesheetenddate;
            ViewBag.Bymonthlyid = Bymonthlyid;


            ViewBag.ClientProjectid = projid;
            var ClientProjectName = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.ClientProjTitle).FirstOrDefault();
            var AccountClientName = (from p in db.Projects
                                     join cp in db.ClientProjects on p.Proj_ProjectID equals cp.Proj_ProjectID
                                     where cp.CL_ProjectID == projid
                                     select new
                                     {
                                          p.Proj_ProjectName
                                     }).Distinct().FirstOrDefault();
            ViewBag.AccClientName = AccountClientName.Proj_ProjectName;

            var clientProj_Projectid = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.Proj_ProjectID).FirstOrDefault();
            ViewBag.clientprojname = ClientProjectName;
            ViewBag.clientProj_Projectid = clientProj_Projectid;
            ViewBag.Mode = mode;
            LoginComponent loginComponent = new LoginComponent();

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
                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }
        public ActionResult MonthlyUserDataTimeSheet(int userid, string timesheetyear, string timesheetmonth, string Bymonthlyid, int projid)
        {
            
               UserSessionInfo objUserSession = Session["UserSessionInfo"] as UserSessionInfo;
            string _userID = objUserSession.LoginId;
            string _paswd = objUserSession.Password;
            UserProjectdetailsEntity objuser = new UserProjectdetailsEntity();
            int UserName = objuser.User_ID;
            string Password = objuser.Usr_Password;

            ViewBag.year = timesheetyear;
            ViewBag.month = timesheetmonth;
            ViewBag.Bymonthlyid = Bymonthlyid;


            ViewBag.ClientProjectid = projid;
            var ClientProjectName = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.ClientProjTitle).FirstOrDefault();
            var myprojId = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.Proj_ProjectID).FirstOrDefault();

            ViewBag.prjid = myprojId;
           var clientProj_Projectid = db.ClientProjects.Where(a => a.CL_ProjectID == projid).Select(a => a.Proj_ProjectID).FirstOrDefault();
            ViewBag.clientprojname = ClientProjectName;
            ViewBag.clientProj_Projectid = clientProj_Projectid;
            LoginComponent loginComponent = new LoginComponent();

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
                this.Session["TaskId"] = objuser;

                return View(objuser);
            }
            else
            {
                return View();
            }
        }




    }
                    
}