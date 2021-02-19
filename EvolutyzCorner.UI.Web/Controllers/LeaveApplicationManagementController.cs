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
    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class LeaveApplicationManagementController : Controller
    {
        SqlConnection Conn = new SqlConnection();
        public static string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static string host = System.Web.HttpContext.Current.Request.Url.Host;
        public static string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
        public static string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
        public static string UrlEmailAddress = string.Empty;
        public static string UrlEmailImage = string.Empty;


        public ActionResult Index()
        {
            GetHolidayDates();
            UserSessionInfo objSessioninfo = new UserSessionInfo();
            var ussacnt = objSessioninfo.UsAccount;
            //if (ussacnt == true)
            //{
                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int AccountId = _objSessioninfo.AccountId;
                int UserId = _objSessioninfo.UserId;
                ViewBag.AccountID = AccountId;
                ViewBag.UserId = UserId;


                LeaveTypeComponent LeaveTypes = new LeaveTypeComponent();
                //var UserProjects = LeaveTypes.GetAllProjectsofUser(UserId).Select(a => new SelectListItem()
                //{
                //    Value = a.CL_ProjId.ToString(),
                //    Text = a.ProjectTitle,

                //});
                //ViewBag.UserProjects = UserProjects;



                List<LeaveTypeEntity> lst = new List<LeaveTypeEntity>();
                var UserProjects = LeaveTypes.GetAllProjectsofUser(UserId).Select(a => new { a.CL_ProjId, a.ProjectTitle }).ToList();

                ViewBag.UserProjects = UserProjects;

                return RedirectToAction("USLeaveCalendar");

           // }
            //else
            //{
            //    // Calendar();
            //    return RedirectToAction("Calendar");
            //}
        }
        public ActionResult PreviewLeaves()
        {

            return View();
        }

        public ActionResult PreviewWorkFromHome()
        {
            return View();
        }





        [HttpPost]
        public ActionResult GetUSaccmail()
        {
            AdminComponent admcomp = new AdminComponent();
            UserSessionInfo info = new UserSessionInfo();
            int userid = info.UserId;
            var query = admcomp.GetUSaccmail(userid);
            return Json(query, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LeaveApplication()
        {
            LeaveTypeComponent LeaveTypes = new LeaveTypeComponent();
            var Employeementtypes = LeaveTypes.GetAllLeaveTypes().Select(a => new SelectListItem()
            {
                Value = a.LTyp_LeaveTypeID.ToString(),
                Text = a.LTyp_LeaveType,

            });
            ViewBag.Employeementtypes = Employeementtypes;
            return View();
        }
        public JsonResult GetEmpIds()
        {
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var query = (from ei in db.UsersProfiles
                         select new LeaveTypeEntity
                         {
                             UsrP_UserProfileID = ei.UsrP_UserProfileID,
                             UsrP_EmployeeID = ei.UsrP_EmployeeID
                         }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetLeaveTypes(string id)
        {
            AdminComponent admcmp = new AdminComponent();
            var query = admcmp.GetLeaveTypes(id);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeaves(string id)
        {
            AdminComponent admcmp = new AdminComponent();
            var query = admcmp.GetLeaves(id);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmpNames(string id)
        {
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            int ID = Convert.ToInt32(id);
            var query = (from en in db.UsersProfiles
                         where en.UsrP_UserProfileID == ID
                         select new LeaveTypeEntity
                         {
                             UsrP_FirstName = en.UsrP_FirstName,
                             UsrP_LastName = en.UsrP_LastName
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        DateTime newdate;
        public ActionResult Calendar()
        {
            UserSessionInfo objSessioninfo = new UserSessionInfo();
            bool? accid = objSessioninfo.UsAccount;

            ViewBag.isusacc = accid;
            var ussacnt = objSessioninfo.UsAccount;
            if (objSessioninfo.UserId != 0)
            {
                var uid = objSessioninfo.UserId;

                var Usacc = objSessioninfo.UsAccount.HasValue ? objSessioninfo.UsAccount : true;
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                var getuserid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmployeeID).FirstOrDefault();
                var getempname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_FirstName).FirstOrDefault();
                var getlastname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_LastName).FirstOrDefault();
                var getusermailid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
                var getusrwfhId = (from wfh in db.UserworkfromHomes where wfh.Usrl_UserId == uid select wfh.UserwfhID).FirstOrDefault();
                var getuserfullname = getempname + "  " + getlastname;

                TempData["User_id"] = uid;
                TempData["UserId"] = getuserid;
                TempData["Name"] = getuserfullname;
                TempData["FromAddress"] = getusermailid;
                TempData["UsrWrkFrmId"] = getusrwfhId;
            }
            var Daylst = new List<WeekDays>();
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now.Date;
            int totalDays = DateTime.DaysInMonth(currentYear, currentMonth);

            LeaveTypeEntity objcal = new LeaveTypeEntity();
            WeekDays days = new WeekDays();

            AdminComponent admcmp = new AdminComponent();
            var getempdetails = admcmp.GetApprovedLeaveCount();
            for (int index = 1; index <= totalDays; index++)
            {
                var dayOfWeek = new DateTime(currentYear, currentMonth, index).DayOfWeek;
                newdate = Convert.ToDateTime(currentYear + "-" + currentMonth + "-" + index);
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        objcal.Su = Convert.ToString(index);
                        days.Su = Convert.ToString(index);
                        days.SuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Monday:
                        objcal.Mo = Convert.ToString(index);
                        days.Mo = Convert.ToString(index);
                        days.MoLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Tuesday:
                        objcal.Tu = Convert.ToString(index);
                        days.Tu = Convert.ToString(index);
                        days.TuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Wednesday:
                        objcal.We = Convert.ToString(index);
                        days.We = Convert.ToString(index);
                        days.WeLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Thursday:
                        days.Th = Convert.ToString(index);
                        objcal.Th = Convert.ToString(index);
                        days.ThLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Friday:
                        objcal.Fr = Convert.ToString(index);
                        days.Fr = Convert.ToString(index);
                        days.FrLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;
                    case DayOfWeek.Saturday:
                        days.Sa = Convert.ToString(index);
                        objcal.Sa = Convert.ToString(index);
                        days.SaLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        Daylst.Add(days);
                        objcal = new LeaveTypeEntity();
                        days = new WeekDays();
                        break;
                }
            }

            return View(Daylst);


        }



        public ActionResult USLeaveCalendar()
        {
            UserSessionInfo objSessioninfo = new UserSessionInfo();
            bool? accid = objSessioninfo.UsAccount;
            var uid = objSessioninfo.UserId;
            // ViewBag.isusacc = accid;
            if (objSessioninfo.UserId != 0)
            {
               

                var Usacc = objSessioninfo.UsAccount.HasValue ? objSessioninfo.UsAccount : true;
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                var getuserid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmployeeID).FirstOrDefault();
                var getempname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_FirstName).FirstOrDefault();
                var getlastname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_LastName).FirstOrDefault();
                var getusermailid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
                var getusrwfhId = (from wfh in db.UserworkfromHomes where wfh.Usrl_UserId == uid select wfh.UserwfhID).FirstOrDefault();
                var getuserfullname = getempname + "  " + getlastname;
                TempData["User_id"] = uid;
                TempData["UserId"] = getuserid;
                TempData["Name"] = getuserfullname;
                TempData["FromAddress"] = getusermailid;
                TempData["UsrWrkFrmId"] = getusrwfhId;
            }
            var Daylst = new List<WeekDays>();

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now.Date;
            int curntDate = currentDate.Day;

            int totalDays = DateTime.DaysInMonth(currentYear, currentMonth);

            LeaveTypeEntity objcal = new LeaveTypeEntity();
            WeekDays days = new WeekDays();

            AdminComponent admcmp = new AdminComponent();
            // var gethldydetails = admcmp.GetHolidayDates(accountid);
            var getempdetails = admcmp.GetApprovedLeaveCount();
            for (int index = 1; index <= totalDays; index++)
            {

                var dayOfWeek = new DateTime(currentYear, currentMonth, index).DayOfWeek;
                newdate = Convert.ToDateTime(currentYear + "-" + currentMonth + "-" + index);
                if (currentDate == newdate)
                {
                    WeekDays obj = new WeekDays();
                    // obj.disablePreviousDates = 1;
                    days.disablePreviousDates = 1;
                    //break;
                }
                else
                {
                    days.disablePreviousDates = 0;
                    //WeekDays obj = new WeekDays();
                    //obj.disablePreviousDates = 0;
                }

                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        objcal.Su = Convert.ToString(index);
                        days.Su = Convert.ToString(index);
                        days.SuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);
                        break;

                    case DayOfWeek.Monday:
                        objcal.Mo = Convert.ToString(index);
                        days.Mo = Convert.ToString(index);
                        days.MoLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);

                        break;
                    case DayOfWeek.Tuesday:
                        objcal.Tu = Convert.ToString(index);
                        days.Tu = Convert.ToString(index);
                        days.TuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);

                        break;
                    case DayOfWeek.Wednesday:
                        objcal.We = Convert.ToString(index);
                        days.We = Convert.ToString(index);
                        days.WeLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);

                        break;
                    case DayOfWeek.Thursday:
                        days.Th = Convert.ToString(index);
                        objcal.Th = Convert.ToString(index);
                        days.ThLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);

                        break;
                    case DayOfWeek.Friday:
                        objcal.Fr = Convert.ToString(index);
                        days.Fr = Convert.ToString(index);
                        days.FrLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        FinalizeDaysList(Daylst, totalDays, days, index);


                        break;
                    case DayOfWeek.Saturday:
                        days.Sa = Convert.ToString(index);
                        objcal.Sa = Convert.ToString(index);
                        days.SaLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        Daylst.Add(days);
                        objcal = new LeaveTypeEntity();
                        days = new WeekDays();



                        break;
                }
                //if (currentDate > newdate)
                //{
                //    days.disablePreviousDates = 1;
                //    break;
                //}
                LeaveTypeComponent LeaveTypes = new LeaveTypeComponent();
              
                var UserProjects = LeaveTypes.GetAllProjectsofUser(uid).Select(a => new { a.CL_ProjId, a.ProjectTitle }).ToList();

                ViewBag.UserProjects = UserProjects;
            }

            return View(Daylst);
        }

        private static void FinalizeDaysList(List<WeekDays> Daylst, int totalDays, WeekDays objcal, int index)
        {
            var id = index;
            int currentmonth = DateTime.Now.Month;
            int currentyear = DateTime.Now.Year;

            DateTime currentDate = DateTime.Now.Date;
            int curntDate = currentDate.Day;

            DateTime newdate = Convert.ToDateTime(currentyear + "-" + currentmonth + "-" + id);

            AdminComponent admcmp = new AdminComponent();

            var getempdetails = admcmp.GetApprovedLeaveCount();
            var data = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
            if (data.Count > 0)
            {

            }
            if (index == totalDays)
            {
                Daylst.Add(objcal);

            }


        }


        private static void ChangedFinalizeDaysList(List<WeekDays> Daylst, int totalDays, WeekDays objcal, int index, int Selmonth, int SelYear)
        {
            var id = index;

            int currentyear = SelYear;
            int selectedMonth = Selmonth;

            DateTime newdate = Convert.ToDateTime(currentyear + "-" + selectedMonth + "-" + id);

            AdminComponent admcmp = new AdminComponent();
            var getempdetails = admcmp.GetApprovedLeaveCount();
            var data = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
            if (data.Count > 0)
            {

            }

            if (index == totalDays)
            {
                Daylst.Add(objcal);

            }
        }


        [HttpPost]
        public JsonResult SelectedMonth(int Selmonth, int Selyear)
        {

            var Daylst = new List<WeekDays>();
            int totalDays = DateTime.DaysInMonth(Selyear, Selmonth);

            LeaveTypeEntity objcal = new LeaveTypeEntity();
            WeekDays days = new WeekDays();

            AdminComponent admcmp = new AdminComponent();
            var getempdetails = admcmp.GetApprovedLeaveCount();
            for (int index = 1; index <= totalDays; index++)
            {
                var dayOfWeek = new DateTime(Selyear, Selmonth, index).DayOfWeek;
                DateTime newdate = Convert.ToDateTime(Selyear + "-" + Selmonth + "-" + index);
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        objcal.Su = Convert.ToString(index);
                        days.Su = Convert.ToString(index);
                        days.SuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Monday:
                        objcal.Mo = Convert.ToString(index);
                        days.Mo = Convert.ToString(index);
                        days.MoLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Tuesday:
                        objcal.Tu = Convert.ToString(index);
                        days.Tu = Convert.ToString(index);
                        days.TuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Wednesday:
                        objcal.We = Convert.ToString(index);
                        days.We = Convert.ToString(index);
                        days.WeLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Thursday:
                        days.Th = Convert.ToString(index);
                        objcal.Th = Convert.ToString(index);
                        days.ThLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Friday:
                        objcal.Fr = Convert.ToString(index);
                        days.Fr = Convert.ToString(index);
                        days.FrLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Saturday:
                        days.Sa = Convert.ToString(index);
                        objcal.Sa = Convert.ToString(index);
                        days.SaLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        Daylst.Add(days);
                        objcal = new LeaveTypeEntity();
                        days = new WeekDays();
                        break;
                }
            }

            return Json(Daylst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult selectedYear(int Selmonth, int Selyear)
        {
            var Daylst = new List<WeekDays>();
            int totalDays = DateTime.DaysInMonth(Selyear, Selmonth);

            LeaveTypeEntity objcal = new LeaveTypeEntity();
            WeekDays days = new WeekDays();

            AdminComponent admcmp = new AdminComponent();
            var getempdetails = admcmp.GetApprovedLeaveCount();
            for (int index = 1; index <= totalDays; index++)
            {
                var dayOfWeek = new DateTime(Selyear, Selmonth, index).DayOfWeek;
                DateTime newdate = Convert.ToDateTime(Selyear + "-" + Selmonth + "-" + index);
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        objcal.Su = Convert.ToString(index);
                        days.Su = Convert.ToString(index);
                        days.SuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Monday:
                        objcal.Mo = Convert.ToString(index);
                        days.Mo = Convert.ToString(index);
                        days.MoLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Tuesday:
                        objcal.Tu = Convert.ToString(index);
                        days.Tu = Convert.ToString(index);
                        days.TuLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Wednesday:
                        objcal.We = Convert.ToString(index);
                        days.We = Convert.ToString(index);
                        days.WeLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Thursday:
                        days.Th = Convert.ToString(index);
                        objcal.Th = Convert.ToString(index);
                        days.ThLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Friday:
                        objcal.Fr = Convert.ToString(index);
                        days.Fr = Convert.ToString(index);
                        days.FrLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        ChangedFinalizeDaysList(Daylst, totalDays, days, index, Selmonth, Selyear);
                        break;
                    case DayOfWeek.Saturday:
                        days.Sa = Convert.ToString(index);
                        objcal.Sa = Convert.ToString(index);
                        days.SaLeaveList = (from newdateslst in getempdetails where newdateslst.LeaveStartDate == newdate select newdateslst).ToList();
                        Daylst.Add(days);
                        objcal = new LeaveTypeEntity();
                        days = new WeekDays();
                        break;
                }
            }

            return Json(Daylst, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public string CheckLeave(string leaves)
        {
            string response = string.Empty;
            UserSessionInfo sessionInfo = new UserSessionInfo();
            int userid = sessionInfo.UserId;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var query = (from ls in db.LeaveSchemes
                         join u in db.UserTypes on ls.LSchm_UserTypeID equals u.UsT_UserTypeID
                         join ut in db.Users on u.UsT_UserTypeID equals ut.Usr_UserTypeID
                         join a in db.Accounts on ls.LSchm_AccountID equals a.Acc_AccountID
                         join lt in db.LeaveTypes on ls.LSchm_LeaveTypeID equals lt.LTyp_LeaveTypeID

                         where ls.LSchm_UserTypeID == u.UsT_UserTypeID
                         group ls by
                         new
                         {
                             ls.LSchm_UserTypeID,
                             u.UsT_UserTypeID,
                             ls.LSchm_LeaveSchemeID,

                         } into gs

                         select new LeaveSchemeEntity
                         {
                             LSchm_LeaveSchemeID = gs.Key.LSchm_LeaveSchemeID,
                             Noofdays = gs.Sum(p => p.LSchm_LeaveCount),

                         }).FirstOrDefault();

            return response;
        }
        static string accmail = string.Empty;
        DateTime newleavestartdate;
        int userid = 0;
        [HttpPost]
        public string saveleavecount(List<AppliedLeaves> leaveupdate, string LeaveStartDate, string LeaveEndDate, string Usrl_UserId, string Acc_EmailID, string comments)
        {
            DateTime sdate = Convert.ToDateTime(LeaveStartDate);
            DateTime ldate = Convert.ToDateTime(LeaveEndDate);
            int CalDates = Convert.ToInt32(1 + (ldate - sdate).TotalDays);
            TimeSpan span = ldate - sdate;
            int businessdays = span.Days + 1;
            int weekcount = businessdays / 7;
            if (businessdays > weekcount * 7)
            {
                int FirstDayOfWeek = sdate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)sdate.DayOfWeek;
                int LastDayOfWeek = ldate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ldate.DayOfWeek;
                if (LastDayOfWeek < FirstDayOfWeek)
                    LastDayOfWeek += 7;
                if (FirstDayOfWeek <= 6)
                {
                    if (LastDayOfWeek >= 7)
                        businessdays -= 2;
                    else if (LastDayOfWeek >= 6)
                        businessdays -= 1;
                }
                else if (FirstDayOfWeek <= 7 && LastDayOfWeek >= 7)
                    businessdays -= 1;
                businessdays -= weekcount + weekcount;
            }
            newleavestartdate = Convert.ToDateTime(LeaveStartDate);
            AdminComponent admcomp = new AdminComponent();
            accmail = Acc_EmailID;
            string response = string.Empty;
            int sid = admcomp.saveleavecount(leaveupdate, LeaveStartDate, LeaveEndDate, Usrl_UserId, comments);
            int leaveid = Convert.ToInt32(sid);
            //MailLeaveApplication(LeaveStartDate, LeaveEndDate, businessdays, CalDates, leaveid, comments);
            response = "Successfully Leave Applied";
            return response;
        }


        [HttpPost]
        public string saveleavecountForUS(string LeaveStartDate, string LeaveEndDate, string Usrl_UserId, string Acc_EmailID, string UserwfhID, string comments,bool isOptionalHoliday,int CL_ProjectId)
        {
            DateTime sdate = Convert.ToDateTime(LeaveStartDate);

            DateTime ldate = Convert.ToDateTime(LeaveEndDate);
            int CalDates = Convert.ToInt32(1 + (ldate - sdate).TotalDays);
            TimeSpan span = ldate - sdate;
            int businessdays = span.Days + 1;
            int weekcount = businessdays / 7;
            if (businessdays > weekcount * 7)
            {
                int FirstDayOfWeek = sdate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)sdate.DayOfWeek;
                int LastDayOfWeek = ldate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ldate.DayOfWeek;
                if (LastDayOfWeek < FirstDayOfWeek)
                    LastDayOfWeek += 7;
                if (FirstDayOfWeek <= 6)
                {
                    if (LastDayOfWeek >= 7)
                        businessdays -= 2;
                    else if (LastDayOfWeek >= 6)
                        businessdays -= 1;
                }
                else if (FirstDayOfWeek <= 7 && LastDayOfWeek >= 7)
                    businessdays -= 1;
                businessdays -= weekcount + weekcount;
            }

            

            newleavestartdate = Convert.ToDateTime(LeaveStartDate);
            AdminComponent admcomp = new AdminComponent();
            accmail = Acc_EmailID;
            string response = string.Empty;
            int sid = admcomp.saveleavecountForUS(LeaveStartDate, LeaveEndDate, Usrl_UserId, businessdays, UserwfhID, comments, isOptionalHoliday, CL_ProjectId);
            int leaveid = Convert.ToInt32(sid);
            MailLeaveApplication(LeaveStartDate, LeaveEndDate, businessdays, CalDates, leaveid, comments, CL_ProjectId, isOptionalHoliday);
            response = "Successfully Leave Applied";
            return response;
        }



        [HttpPost]
        public string savewrkfrmhomeForUS(string LeaveStartDate, string LeaveEndDate, string Usrl_UserId, string Acc_EmailID, string UserWfhId, string comments,int Projid)
        {
            DateTime sdate = Convert.ToDateTime(LeaveStartDate);
            DateTime ldate = Convert.ToDateTime(LeaveEndDate);
            int CalDates = Convert.ToInt32(1 + (ldate - sdate).TotalDays);
            TimeSpan span = ldate - sdate;
            int businessdays = span.Days + 1;
            int weekcount = businessdays / 7;
            if (businessdays > weekcount * 7)
            {
                int FirstDayOfWeek = sdate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)sdate.DayOfWeek;
                int LastDayOfWeek = ldate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ldate.DayOfWeek;
                if (LastDayOfWeek < FirstDayOfWeek)
                    LastDayOfWeek += 7;
                if (FirstDayOfWeek <= 6)
                {
                    if (LastDayOfWeek >= 7)
                        businessdays -= 2;
                    else if (LastDayOfWeek >= 6)
                        businessdays -= 1;
                }
                else if (FirstDayOfWeek <= 7 && LastDayOfWeek >= 7)
                    businessdays -= 1;
                businessdays -= weekcount + weekcount;
            }
            //newleavestartdate = Convert.ToDateTime(LeaveStartDate);
            AdminComponent admcomp = new AdminComponent();
            accmail = Acc_EmailID;
            string response = string.Empty;
            int res = admcomp.savewrkfrmhomeForUS(LeaveStartDate, LeaveEndDate, Usrl_UserId, businessdays, UserWfhId, comments, Projid);
            ApplyWorkFromHome(LeaveStartDate, LeaveEndDate, businessdays, CalDates, res, comments, Projid);
            response = "Successfully Applied";
            return response;
        }

        [HttpPost]
        public string savewrkfrmhome(string fromdate, string todate, string Usrl_UserId, string Acc_EmailID, string comments)
        {
            DateTime sdate = Convert.ToDateTime(fromdate);
            DateTime ldate = Convert.ToDateTime(todate);
            int CalDates = Convert.ToInt32(1 + (ldate - sdate).TotalDays);
            TimeSpan span = ldate - sdate;
            int businessdays = span.Days + 1;
            int weekcount = businessdays / 7;
            if (businessdays > weekcount * 7)
            {
                int FirstDayOfWeek = sdate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)sdate.DayOfWeek;
                int LastDayOfWeek = ldate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ldate.DayOfWeek;
                if (LastDayOfWeek < FirstDayOfWeek)
                    LastDayOfWeek += 7;
                if (FirstDayOfWeek <= 6)
                {
                    if (LastDayOfWeek >= 7)
                        businessdays -= 2;
                    else if (LastDayOfWeek >= 6)
                        businessdays -= 1;
                }
                else if (FirstDayOfWeek <= 7 && LastDayOfWeek >= 7)
                    businessdays -= 1;
                businessdays -= weekcount + weekcount;
            }
            newleavestartdate = Convert.ToDateTime(fromdate);
            AdminComponent admcomp = new AdminComponent();
            accmail = Acc_EmailID;
            int res = admcomp.savewrkfrmhome(fromdate, todate, Usrl_UserId);
           // ApplyWorkFromHome(fromdate, todate, businessdays, CalDates, res, comments);
            string response = "Successfully  Applied";
            return response;
        }





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



        public static string ApplyWorkFromHome(string fromdate, string todate, int businessdays, int CalDates, int userwfhId, string comments,int CL_ProjectId)
        {
          

            DateTime frmdtByUser = DateTime.Parse(fromdate).Date;
            DateTime todtByUser = DateTime.Parse(todate).Date;
            var FromDt = frmdtByUser.ToLongDateString();
            var ToDt = todtByUser.ToLongDateString();

            UserSessionInfo objSessioninfo = new UserSessionInfo();
            string UrlEmailAddress = string.Empty;
            string UrlEmailImage = string.Empty;
            var getaccntlogo = "";
            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }



            Encrypt objenc = new Encrypt();

            //RestClient client = new RestClient();
            //RestRequest request = new RestRequest();
            //     mail = objenc.Encryption(accmail);
            newuwfhid = userwfhId.ToString();
            //  mail = objenc.Encryption(accmail);
            var userwfhid = objenc.Encryption(userwfhId.ToString());
            uid = objSessioninfo.UserId;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var getaccntid = (from u in db.Users where u.Usr_UserID == uid select u.Usr_AccountID).FirstOrDefault();
            var getuserid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmployeeID).FirstOrDefault();
            var getempname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_FirstName).FirstOrDefault();
            var getlastname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_LastName).FirstOrDefault();
            var useremail = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
            getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
            var ProjectTitle = (from u in db.ClientProjects where u.CL_ProjectID == CL_ProjectId select u.ClientProjTitle).FirstOrDefault();
            //  UrlEmailImage = "<img alt='Company Logo' style='height:100px;margin:auto;display:block;max-width:100%;' src='" + "https://" + UrlEmailAddress + "/uploadimages/Images/" + getaccntlogo + "'";
            UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'";

            var getuserfullname = getempname + "  " + getlastname;
            UserSessionInfo info = new UserSessionInfo();
            AdminComponent admcmp = new AdminComponent();
            string userid = (info.UserId).ToString();
            var Uid = objenc.Encryption(userid);
            var res = admcmp.GetAllManagersEmails(userid, CL_ProjectId);
            var manager1 = res.Select(a => a.ManagerLevel1).FirstOrDefault();
            var manager2 = res.Select(a => a.ManagerLevel2).FirstOrDefault();
            string[] manager1emailids = manager1.Split('/');
            string[] manageremailids = manager2.Split('/');
            var manager1email = manager1emailids[0];
            var manager1id = manager1emailids[1];
            var manager1name = manager1emailids[2];
            var manager2email = manageremailids[0];
            var manager2id = manageremailids[1];
            var manager2name = manageremailids[2];
            var objectToSerialize = new managerjson();
            objectToSerialize.items = new List<manageremails>
                          {
                             new manageremails { manageremail = manager1email, managerid = manager1id, managername = manager1name },
                          };
            var emailcontent = "";
            string newemail = string.Empty;
            string newid = string.Empty;
            string newname = string.Empty;
            string newlevel = string.Empty;
            for (var i = 0; i <= objectToSerialize.items.Count - 1; i++)
            {
                email = objectToSerialize.items[i].manageremail;
                id = objectToSerialize.items[i].managerid;
                name = objectToSerialize.items[i].managername;
                newemail = objenc.Encryption(email);
                newid = objenc.Encryption(id);
                newname = objenc.Encryption(name);
                emailcontent = "<html>" +
               "<body bgcolor='#f6f6f6' style='-webkit-font-smoothing: antialiased; background:#f6f6f6; margin:0 auto; padding:0;'>" +
               "<div style='background:#f4f4f4; border: 0px solid #f4f4f4; margin:0; padding:0'>" +
               "<center>" +
               "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt; width:600px'>" +
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
               "<table bgcolor='#ffffff' border='0' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; table-layout:fixed; mso-table-lspace:0pt; mso-table-rspace:0pt;border: 1px solid #d1d2d1; width: 600px;'>" +
               "<tbody>" +
               "<tr>" +
               "<td align='center'>" +
               "<table bgcolor='#ffffff' cellpadding='0' cellspacing='0' class='mobile' style='background: #ffffff; border-collapse:collapse !important; width:80%; mso-table-lspace:0pt; mso-table-rspace:0pt;'>" +
               " <tbody>" +
               " <tr>" +
               "<td align=center bgcolor='#ffffff' style='background: #ffffff; padding-bottom:0px'; padding-top:0' valign='top'>" +
               "<table align='center' border='0' cellpadding = '0' cellspacing = '0' style = 'border-collapse: collapse !important; max-width:600px; mso-table-lspace:0pt; mso-table-rspace:0pt; width:80%'>" +
               " <tbody>" +
               "<tr>" +
               "<td align='left' bgcolor='#ffffff' style='background: #ffffff; font-family:Lato, Helevetica, Arial, sans-serif; font-size:18px; line-height:28px; padding:10px 0px 20px' valign='top'>" +
               "<table align='center' border='0' cellpadding='0' cellspacing='0' style='border-collapse:collapse !important; max-width:600px; width:100%'>" +
               "<tbody>" +
               "<td align='left' bgcolor='#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 0px; width: 40%' valign='middle'>" +
               "<p align='left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong>" +
               " Employee Name" +
               "</strong>" +
               "<br>" +
               " </p>" +
               "</td>" +
               "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 0px; width: 40%' valign='middle'>" +
               "<p align = 'right' style='color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               getuserfullname +
               " </p>" +
               " </td>" +
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
               " From Date " +
               " </strong>" +
               "<br> " +
               "</p>" +
               " </td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong>" +
               FromDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " To Date " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ToDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Project " +
               "</strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ProjectTitle +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Calendar Days " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               CalDates +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               "No. Of Days Applied " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
                businessdays +
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
               comments +
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
               "<a href = 'https://" + UrlEmailAddress + "/WrkFrmHomeComments/AddWFHComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 1 + "&userwfhId=" + userwfhid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + fromdate + "&endDate=" + todate + "'javascript:; style='-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approve</a>" +
               "<a href = 'https://" + UrlEmailAddress + "/WrkFrmHomeComments/AddWFHComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 2 + "&userwfhId=" + userwfhid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + fromdate + "&endDate=" + todate + "'javascript:; style='-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #cae285; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#f997b0), to(#f56778)); background: -moz-linear-gradient(#f997b0, #f56778); background: linear-gradient(#f997b0, #f56778); border: solid 1px #ee8090; border-bottom: solid 3px #cb5462; box-shadow: inset 0 0 0 1px #fbc1d0; color: #913944; text-shadow: 0 1px 0 #f9a0ad; text-decoration: none'>Reject</a>" +
               //"<a href = 'https://" + UrlEmailAddress + "/WrkFrmHomeComments/AddWFHComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 3 + "&userwfhId=" + userwfhid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + fromdate + "&endDate=" + todate + "'javascript:; style='-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 0px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #feda71; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#feda71), to(#febe4d)); background: -moz-linear-gradient(#feda71, #febe4d); background: linear-gradient(#feda71, #febe4d); border: solid 1px #eab551; border-bottom: solid 3px #b98a37; box-shadow: inset 0 0 0 1px #fee9aa; color: #996633; text-shadow: 0 1px 0 #fedd9b; text-decoration: none'>Hold</a>" +
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
                //client = new RestClient();
                //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                //client.Authenticator = new HttpBasicAuthenticator("api", "key-25b8e8c23e4a09ef67b7957d77eb7413");
                //request = new RestRequest();
                //request.AddParameter("domain", "evolutyzstaging.com", ParameterType.UrlSegment);
                //request.Resource = "{domain}/messages";
                //request.AddParameter("from", "Evolutyz ITServices <postmaster@evolutyzstaging.com>");
                //request.AddParameter("to", manager1email);
                ////request.AddParameter("to", "sridhar.kambala@evolutyz.in"); request.AddParameter("to", "prasanna.kalyanam@evolutyz.in"); request.AddParameter("to", "kalyanibnw@gmail.com");
                ////request.AddParameter("to", "tulasi.evolutyz@gmail.com"); request.AddParameter("to", "sreelakshmi.evolutyz@gmail.com");
                //request.AddParameter("subject", "Work From Home Application from " + getuserfullname);
                //request.AddParameter("html", emailcontent);
                ////request.AddFile("attachment", Path.Combine("files", "test.jpg"));
                //request.Method = Method.POST;
                //client.Execute(request);
                var client = new SendGridClient("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");

                var msgs = new SendGridMessage()
                {
                    //From = new EmailAddress(useremail),
                    From = new EmailAddress(FromMailAddress, getuserfullname),
                    Subject = "Work From Home Application from " + getuserfullname,
                    //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                    HtmlContent = emailcontent

                };
                msgs.AddTo(new EmailAddress(manager1email));

                var responses = client.SendEmailAsync(msgs);

            }
            return null;

        }





        public static string MailLeaveApplication(string LeaveStartDate, string LeaveEndDate, int businessdays, int CalDates, int leaveid, string comments,int CL_ProjectId,bool isOptionalHoliday)
        {
            DateTime LeavfrmdtByUser = DateTime.Parse(LeaveStartDate).Date;
            DateTime LeavtodtByUser = DateTime.Parse(LeaveEndDate).Date;
            var FromDt = LeavfrmdtByUser.ToLongDateString();
            var ToDt = LeavtodtByUser.ToLongDateString();

            UserSessionInfo objSessioninfo = new UserSessionInfo();
            //string UrlEmailAddress = string.Empty;
            //string UrlEmailImage = string.Empty;
            SendGridClient client = new SendGridClient("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");
            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }

            newleavid = leaveid.ToString();
            Encrypt objenc = new Encrypt();
            //RestClient client = new RestClient();
            //RestRequest request = new RestRequest();
            // mail = objenc.Encryption(accmail);
            var leavid = objenc.Encryption(leaveid.ToString());
            uid = objSessioninfo.UserId;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var getaccntid = (from u in db.Users where u.Usr_UserID == uid select u.Usr_AccountID).FirstOrDefault();
            var getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
            // UrlEmailImage = "<img alt='Company Logo' style='height:100px;margin:auto;max-width:100%;display:block;' src='" + "https://" + UrlEmailAddress + "/uploadimages/Images/" + getaccntlogo + "'";
            UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'";
            var getuserid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmployeeID).FirstOrDefault();
            var getempname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_FirstName).FirstOrDefault();
            var getlastname = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_LastName).FirstOrDefault();
            var useremail = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
            var ProjectTitle = (from u in db.ClientProjects where u.CL_ProjectID == CL_ProjectId select u.ClientProjTitle).FirstOrDefault();
            var getuserfullname = getempname + "  " + getlastname;
            UserSessionInfo info = new UserSessionInfo();
            AdminComponent admcmp = new AdminComponent();
            string userid = (info.UserId).ToString();
            var Uid = objenc.Encryption(userid);
            var res = admcmp.GetAllManagersEmails(userid, CL_ProjectId);
            var manager1 = res.Select(a => a.ManagerLevel1).FirstOrDefault();
            var manager2 = res.Select(a => a.ManagerLevel2).FirstOrDefault();
            string[] manager1emailids = manager1.Split('/');
            string[] manageremailids = manager2.Split('/');
            var manager1email = manager1emailids[0];
            var manager1id = manager1emailids[1];
            var manager1name = manager1emailids[2];
            var manager2email = manageremailids[0];
            var manager2id = manageremailids[1];
            var manager2name = manageremailids[2];
            var objectToSerialize = new managerjson();
            objectToSerialize.items = new List<manageremails>
                          {
                             new manageremails { manageremail = manager1email, managerid = manager1id, managername = manager1name },
                          };
            var emailcontent = "";
            //string responses = string.Empty;
            string newemail = string.Empty;
            string newid = string.Empty;
            string newname = string.Empty;
            string newlevel = string.Empty;
            for (var i = 0; i <= objectToSerialize.items.Count - 1; i++)
            {
                email = objectToSerialize.items[i].manageremail;
                id = objectToSerialize.items[i].managerid;
                name = objectToSerialize.items[i].managername;
                newemail = objenc.Encryption(email);
                newid = objenc.Encryption(id);
                newname = objenc.Encryption(name);
                if(isOptionalHoliday == false)
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
               " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
               "<td align='left' bgcolor='#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 0px; width: 40%' valign='top'>" +
               "<p align='left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong>" +
               " Employee Name" +
               "</strong>" +
               "<br>" +
               " </p>" +
               "</td>" +
               "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px;max-width: 80%; padding: 0px; width: 40%' valign='top'>" +
               "<p align = 'right' style='color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               getuserfullname +
               " </p>" +
               " </td>" +
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
               " From Date " +
               " </strong>" +
               "<br> " +
               "</p>" +
               " </td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong>" +
               FromDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " To Date " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ToDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Project " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ProjectTitle +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Calendar Days " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               CalDates +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               "No. Of Days Applied " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
                businessdays +
               "</strong>" +
               "</p>" +
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
               "<h5>" +
               comments +
               "</h5>" +
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
               //   "<a href = '" + "http://" + UrlEmailAddress + "/LeaveStatus?accemail="+mail+"&userid="+Uid+"&status="+1+"&leaveid="+leavid+"&managermail="+newemail+"&managerid="+newid+"&managername="+newname+"&startDate="+LeaveStartDate+"&endDate="+LeaveEndDate+"'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approved</a>" +
               //"<a href = 'http://" + UrlEmailAddress + "/LeaveApplicationManagement/LeaveStatus?accemail=" + mail + "&userid=" + Uid + "&status=" + 1 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approve</a>" +
               "<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 1 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approve</a>" +
               "<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 2 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style=' -moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #cae285; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#f997b0), to(#f56778)); background: -moz-linear-gradient(#f997b0, #f56778); background: linear-gradient(#f997b0, #f56778); border: solid 1px #ee8090; border-bottom: solid 3px #cb5462; box-shadow: inset 0 0 0 1px #fbc1d0; color: #913944; text-shadow: 0 1px 0 #f9a0ad; text-decoration: none'>Reject</a>" +
               //"<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 3 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 0px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #feda71; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#feda71), to(#febe4d)); background: -moz-linear-gradient(#feda71, #febe4d); background: linear-gradient(#feda71, #febe4d); border: solid 1px #eab551; border-bottom: solid 3px #b98a37; box-shadow: inset 0 0 0 1px #fee9aa; color: #996633; text-shadow: 0 1px 0 #fedd9b; text-decoration: none'>Hold</a>" +
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


                    var msgs = new SendGridMessage()
                    {
                        // From = new EmailAddress(useremail),
                        From = new EmailAddress(FromMailAddress, getuserfullname),
                        Subject = "Leave Application from " + getuserfullname,
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(manager1email));

                    var responses = client.SendEmailAsync(msgs);

                }
               

                if(isOptionalHoliday != false)
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
               " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Optional Leave Application</h2>" +
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
               "<td align='left' bgcolor='#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 0px; width: 40%' valign='top'>" +
               "<p align='left' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong>" +
               " Employee Name" +
               "</strong>" +
               "<br>" +
               " </p>" +
               "</td>" +
               "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px;max-width: 80%; padding: 0px; width: 40%' valign='top'>" +
               "<p align = 'right' style='color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               getuserfullname +
               " </p>" +
               " </td>" +
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
               " From Date " +
               " </strong>" +
               "<br> " +
               "</p>" +
               " </td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong>" +
               FromDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " To Date " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ToDt +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Project " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               ProjectTitle +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               "</td>" +
               "</tr>" +
               "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               " Calendar Days " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
               CalDates +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
                "<tr>" +
               "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               "<strong> " +
               "No. Of Days Applied " +
               " </strong>" +
               "<br>" +
               "</p>" +
               "</td>" +
               "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               "<strong> " +
                businessdays +
               "</strong>" +
               "</p>" +
               "</td>" +
               "</tr>" +
               "</tbody>" +
               "</table>" +
               //"<table align = center bgcolor = #ffffff border =0 cellpadding =0 cellspacing = 0 style = 'background: #ffffff; border-collapse: collapse !important; border-top-color: #D1D2D1; border-top-style: solid; border-top-width: 1px; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%'>" +
               //"<tbody>" +
               //" <tr>" +
               //"<td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; line-height: 28px; padding: 10x 0px 20px' valign='middle'>" +
               //"<table align = center border = 0 cellpadding = 0 cellspacing = 0 style = 'border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 80%'>" +
               //"<tbody>" +
               //"<tr>" +
               //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
               //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
               //"<strong> " +
               //"Comments " +
               //"</strong>" +
               //"</p>" +
               //"</td>" +
               //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
               //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
               //"<h5>" +
               //comments +
               //"</h5>" +
               //"</p>" +
               //"</td>" +
               //"</tr>" +
               //"</tbody>" +
               //"</table>" +
               //"</td>" +
               //"</tr>" +
               //"</tbody>" +
               //"</table>" +
               //" </td>" +
               //"</tr>" +
               //"</tbody>" +
               //" </table>" +
               "<table bgcolor = #ffffff cellpadding =0 cellspacing =0 class=mobile style='background: #ffffff; border-collapse: collapse !important; max-width: 600px; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 96%'>" +
               "<tbody>" +
               "<tr>" +
               "<td align = center style='padding-bottom: 20px' valign=top>" +
               "<table align = center border=0 cellpadding=0 cellspacing=0 style='border-collapse: collapse !important; mso-table-lspace: 0pt; mso-table-rspace: 0pt'>" +
               "<tbody>" +
               "<tr>" +
               "<td align = center style='padding-bottom: 0px; padding-top: 30px' valign=top>" +
               //   "<a href = '" + "http://" + UrlEmailAddress + "/LeaveStatus?accemail="+mail+"&userid="+Uid+"&status="+1+"&leaveid="+leavid+"&managermail="+newemail+"&managerid="+newid+"&managername="+newname+"&startDate="+LeaveStartDate+"&endDate="+LeaveEndDate+"'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approved</a>" +
               //"<a href = 'http://" + UrlEmailAddress + "/LeaveApplicationManagement/LeaveStatus?accemail=" + mail + "&userid=" + Uid + "&status=" + 1 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approve</a>" +
               "<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 1 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius:3px;-webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #f997b0; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#cae285), to(#a3cd5a)); background: -moz-linear-gradient(#cae285, #a3cd5a); background: linear-gradient(#cae285, #a3cd5a); border: solid 1px #aad063; border-bottom: solid 3px #799545; box-shadow: inset 0 0 0 1px #e0eeb6; color: #5d7731; text-shadow: 0 1px 0 #d0e5a4; text-decoration: none'>Approve</a>" +
               "<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 2 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style=' -moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 15px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #cae285; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#f997b0), to(#f56778)); background: -moz-linear-gradient(#f997b0, #f56778); background: linear-gradient(#f997b0, #f56778); border: solid 1px #ee8090; border-bottom: solid 3px #cb5462; box-shadow: inset 0 0 0 1px #fbc1d0; color: #913944; text-shadow: 0 1px 0 #f9a0ad; text-decoration: none'>Reject</a>" +
               //"<a href = 'https://" + UrlEmailAddress + "/LeaveComments/AddLeaveComments?accemail=" + mail + "&userid=" + Uid + "&status=" + 3 + "&leaveid=" + leavid + "&managermail=" + newemail + "&managerid=" + newid + "&managername=" + newname + "&startDate=" + LeaveStartDate + "&endDate=" + LeaveEndDate + "'javascript:; style='-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; display: inline-block; font: bold 12px Arial, Helvetica, Clean, sans-serif; margin: 0 0px 10px 0; padding: 10px 20px; position: relative; text-align: center;background: #feda71; background: -webkit-gradient(linear, 0 0, 0 bottom, from(#feda71), to(#febe4d)); background: -moz-linear-gradient(#feda71, #febe4d); background: linear-gradient(#feda71, #febe4d); border: solid 1px #eab551; border-bottom: solid 3px #b98a37; box-shadow: inset 0 0 0 1px #fee9aa; color: #996633; text-shadow: 0 1px 0 #fedd9b; text-decoration: none'>Hold</a>" +
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


                    var msgss = new SendGridMessage()
                    {
                        // From = new EmailAddress(useremail),
                        From = new EmailAddress(FromMailAddress, getuserfullname),
                        Subject = "Optional Leave Application from " + getuserfullname,
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgss.AddTo(new EmailAddress(manager1email));

                    var responsess = client.SendEmailAsync(msgss);



                }


               


            }
            return "";
        }

        [HttpPost]
        public string WebLeaveApproval(string Userid, string LeaveStartDate, string LeaveEndDate, int leaveid, string accntmail, string Leavestatus, int ManagerId, string ManagerName, string ManagerMail, string UserMail)
        {
            newleavid = leaveid.ToString();
            id = ManagerId.ToString();
            string Lstatus = Leavestatus;
            string UrlEmailImage = string.Empty;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            // var getusermailid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
            var getaccntid = (from u in db.Users where u.Usr_UserID == uid select u.Usr_AccountID).FirstOrDefault();
            var getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
            UrlEmailImage = "<img alt='Company Logo' style ='max-height: 100px; max-width: 100%'  src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'/>";
            var emailcontent = "";
            if (Leavestatus == "4")
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
                " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                "Leave Approved By" +
                " </strong>" +
                "<br>" +
                "</p>" +
                "</td>" +
                "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                 ManagerName +
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
                " From Date " +
                " </strong>" +
                "<br> " +
                "</p>" +
                " </td>" +
                "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                "<strong>" +
                LeaveStartDate +
                "</strong>" +
                "</p>" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td align = left bgcolor = #ffffff style = background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                "<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                "<strong> " +
                " To Date " +
                " </strong>" +
                "<br>" +
                "</p>" +
                "</td>" +
                "<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                "<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                "<strong> " +
                LeaveEndDate +
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
                //"<tr>" +
                //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                //"<strong> " +
                //"Comments " +
                //"</strong>" +
                //"</p>" +
                //"</td>" +
                //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                //"<strong>" +
                //"----" +
                //"</strong>" +
                //"</p>" +
                //"</td>" +
                //"</tr>" +
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
                //request.AddParameter("to", UserMail);
                //request.AddParameter("to", accntmail);
                //request.AddParameter("subject", "Leave Approved");
                //request.AddParameter("html", emailcontent);
                //request.Method = Method.POST;
                //client.Execute(request);

                var client = new SendGridClient("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");

                var msgs = new SendGridMessage()
                {
                    // From = new EmailAddress(ManagerMail),
                    From = new EmailAddress(FromMailAddress),
                    Subject = "Leave Approved",
                    //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                    HtmlContent = emailcontent

                };
                msgs.AddTo(new EmailAddress(UserMail));


                var responses = client.SendEmailAsync(msgs);

            }
            else
            {
                if (Leavestatus == "5")
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
                    " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                    "Leave Approved By" +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                    "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                     ManagerName +
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
                    LeaveStartDate +
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
                    LeaveEndDate +
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
                    //"<tr>" +
                    //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    //"<strong> " +
                    //"Comments " +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    //"<strong>" +
                    //"----" +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"</tr>" +
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
                    //request.AddParameter("to", UserMail);
                    //request.AddParameter("to", accntmail);
                    //request.AddParameter("subject", "Leave Rejected");
                    //request.AddParameter("html", emailcontent);
                    //request.Method = Method.POST;
                    //client.Execute(request);
                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        // From = new EmailAddress(ManagerMail),
                        From = new EmailAddress(FromMailAddress),
                        Subject = "Leave Rejected",
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(UserMail));


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
                    " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                    "Leave Approved By" +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                    "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                     ManagerName +
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
                    LeaveStartDate +
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
                    LeaveEndDate +
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
                    //"<tr>" +
                    //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    //"<strong> " +
                    //"Comments " +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    //"<strong>" +
                    //"----" +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"</tr>" +
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
                    //request.AddParameter("to", UserMail);
                    //request.AddParameter("to", accntmail);
                    //request.AddParameter("subject", "Leave Approved");
                    //request.AddParameter("html", emailcontent);
                    //request.Method = Method.POST;
                    //client.Execute(request);

                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        // From = new EmailAddress(ManagerMail),
                        From = new EmailAddress(FromMailAddress),
                        Subject = "Leave OnHold",
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(UserMail));


                    var responses = client.SendEmailAsync(msgs);
                }
            }
            AdminComponent admc = new AdminComponent();
            var res = admc.saveWebApprovalStatus(newleavid, Leavestatus, id);
            // return null;
            string response = string.Empty;

            if (Lstatus == "4")
            {
                return response = "Leave Approved Successfully";
            }
            else if (Lstatus == "5")
            {
                return response = "Leave Rejected";
            }
            else
            {
                return response = "Leave On Hold";
            }


        }
        [HttpPost]
        public string WebWrkFrmHmeApproval(string Userid, string LeaveStartDate, string LeaveEndDate, int UWFHID, string accntmail, string Leavestatus, int ManagerId, string ManagerName, string ManagerMail, string UserMail)
        {
            newuwfhid = UWFHID.ToString();
            id = ManagerId.ToString();
            string UrlEmailImage = string.Empty;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            // var getusermailid = (from u in db.UsersProfiles where u.UsrP_UserID == uid select u.UsrP_EmailID).FirstOrDefault();
            var getaccntid = (from u in db.Users where u.Usr_UserID == uid select u.Usr_AccountID).FirstOrDefault();
            var getaccntlogo = (from a in db.Accounts where a.Acc_AccountID == getaccntid select a.Acc_CompanyLogo).FirstOrDefault();
            UrlEmailImage = "<img alt='Company Logo'   src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + getaccntlogo + "'";
            string Lstatus = Leavestatus;
            var emailcontent = "";
            if (Leavestatus == "4")
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
                //"<img alt='Company Logo' src='https://evolutyz.in/img/evolutyz-logo.png'/>" +
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
                " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                "Leave Approved By" +
                " </strong>" +
                "<br>" +
                "</p>" +
                "</td>" +
                "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                 ManagerName +
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
                LeaveStartDate +
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
                LeaveEndDate +
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
                //"<tr>" +
                //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                //"<strong> " +
                //"Comments " +
                //"</strong>" +
                //"</p>" +
                //"</td>" +
                //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                //"<strong>" +
                //"----" +
                //"</strong>" +
                //"</p>" +
                //"</td>" +
                //"</tr>" +
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
                //request.AddParameter("to", UserMail);
                //request.AddParameter("to", accntmail);
                //request.AddParameter("subject", "Leave Approved");
                //request.AddParameter("html", emailcontent);
                //request.Method = Method.POST;
                //client.Execute(request);
                var client = new SendGridClient("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");

                var msgs = new SendGridMessage()
                {
                    //From = new EmailAddress(ManagerMail),
                    From = new EmailAddress(FromMailAddress),
                    Subject = "Leave Approved",
                    //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                    HtmlContent = emailcontent

                };
                msgs.AddTo(new EmailAddress(UserMail));


                var responses = client.SendEmailAsync(msgs);
            }
            else
            {
                if (Leavestatus == "5")
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
                    //"<img alt='Company Logo' src='https://evolutyz.in/img/evolutyz-logo.png'/>" +
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
                    " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                    "Leave Approved By" +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                    "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                     ManagerName +
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
                    LeaveStartDate +
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
                    LeaveEndDate +
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
                    //"<tr>" +
                    //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    //"<strong> " +
                    //"Comments " +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    //"<strong>" +
                    //"----" +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"</tr>" +
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
                    //request.AddParameter("to", UserMail);
                    //request.AddParameter("to", accntmail);
                    //request.AddParameter("subject", "Leave Approved");
                    //request.AddParameter("html", emailcontent);
                    //request.Method = Method.POST;
                    //client.Execute(request);

                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        // From = new EmailAddress(ManagerMail),
                        From = new EmailAddress(FromMailAddress),
                        Subject = "Leave Rejected",
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(UserMail));


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
                    //"<img alt='Company Logo' src='https://evolutyz.in/img/evolutyz-logo.png'/>" +
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
                    " <h2 align='center' style='color: #707070; font-weight: 400; margin:0; text-align: center'>Leave Application</h2>" +
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
                    "Leave Approved By" +
                    " </strong>" +
                    "<br>" +
                    "</p>" +
                    "</td>" +
                    "<td align = 'right' bgcolor = '#ffffff' style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10px 0px 0px' valign='top'>" +
                    "<p align = 'right' style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: 'right'>" +
                     ManagerName +
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
                    LeaveStartDate +
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
                    LeaveEndDate +
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
                    //"<tr>" +
                    //" <td align = center bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; max-width: 80%; padding: 10px 0px 0px; width: 40%' valign='middle'>" +
                    //"<p align = left style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: left'>" +
                    //"<strong> " +
                    //"Comments " +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"<td align = right bgcolor = #ffffff style = 'background: #ffffff; font-family: Lato, Helevetica, Arial, sans-serif; font-size: 18px; padding: 10x 0px 0px' valign='middle'>" +
                    //"<p align = right style = 'color: #707070; font-size: 14px; font-weight: 400; line-height: 22px; padding-bottom: 0px; text-align: right'>" +
                    //"<strong>" +
                    //"----" +
                    //"</strong>" +
                    //"</p>" +
                    //"</td>" +
                    //"</tr>" +
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
                    //request.AddParameter("to", UserMail);
                    //request.AddParameter("to", accntmail);
                    //request.AddParameter("subject", "Leave Approved");
                    //request.AddParameter("html", emailcontent);
                    //request.Method = Method.POST;
                    //client.Execute(request);

                    var client = new SendGridClient("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");

                    var msgs = new SendGridMessage()
                    {
                        //  From = new EmailAddress(ManagerMail),
                        From = new EmailAddress(FromMailAddress),
                        Subject = "Leave OnHold",
                        //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                        HtmlContent = emailcontent

                    };
                    msgs.AddTo(new EmailAddress(UserMail));


                    var responses = client.SendEmailAsync(msgs);
                }
            }
            AdminComponent admc = new AdminComponent();
            var res = admc.saveWebWFHApprovalStatus(newuwfhid, Leavestatus, id);
            //return null;
            string response = string.Empty;

            if (Lstatus == "4")
            {
                return response = "WorkFromHome Approved Successfully";
            }
            else if (Lstatus == "5")
            {
                return response = "WorkFromHome Rejected";
            }
            else
            {
                return response = "Leave On Hold";
            }
        }



        [HttpGet]
        public JsonResult GetLeaveApprovalsDetails()
        {
            AdminComponent admcmp = new AdminComponent();
            var getempdetails = admcmp.GetApprovedLeaveCount();
            return Json(getempdetails, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetLeavePreview()
        {

            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string Roleid = sessId.RoleId.ToString();
            LeavePreviewDetails objmanagerdetails = new LeavePreviewDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.myleaves = new List<UserApprovedLeaves>();
                objmanagerdetails.leavesforapproval = new List<ManagerLeavesforApprovals>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[GetUserLeavesforApproval]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", UserID);
                //if (Roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@userid", UserID);
                //}

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((Roleid == "1001") || (Roleid == "1002") || (Roleid == "1007"))
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            objmanagerdetails.myleaves.Add(new UserApprovedLeaves
                            {
                                Usrl_LeaveId = Convert.ToInt32(dr["Usrl_LeaveId"]),
                                Usrl_UserId = Convert.ToInt32(dr["Usrl_UserId"]),
                                accntmail = dr["Acc_EmailID"].ToString(),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                userName = dr["Usr_Username"].ToString(),
                                LeaveStartDate = dr["LeaveStartDate"].ToString(),
                                LeaveEndDate = dr["LeaveEndDate"].ToString(),
                                No_Of_Days = Convert.ToInt32(dr["No_Of_Days"]),
                                Leavestatus = dr["ResultSubmitStatus"].ToString(),
                                empmailid = dr["UsrP_EmailID"].ToString()

                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {

                            objmanagerdetails.leavesforapproval.Add(new ManagerLeavesforApprovals
                            {
                                Usrl_LeaveId = Convert.ToInt32(druser["Usrl_LeaveId"]),
                                Usrl_UserId = Convert.ToInt32(druser["Usrl_UserId"]),
                                accntmail = druser["Acc_EmailID"].ToString(),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                userName = druser["Usr_Username"].ToString(),
                                LeaveStartDate = druser["LeaveStartDate"].ToString(),
                                LeaveEndDate = druser["LeaveEndDate"].ToString(),
                                No_Of_Days = Convert.ToInt32(druser["No_Of_Days"]),
                                Leavestatus = druser["ResultSubmitStatus"].ToString(),
                                ManagerID1 = Convert.ToInt32(druser["L1_ManagerId"]),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerEmail1 = druser["L1_ManagerMail"].ToString(),
                                UserEmail = druser["UsrP_EmailID"].ToString()
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

                            objmanagerdetails.myleaves.Add(new UserApprovedLeaves
                            {
                                Usrl_LeaveId = Convert.ToInt32(dr["Usrl_LeaveId"]),
                                Usrl_UserId = Convert.ToInt32(dr["Usrl_UserId"]),
                                accntmail = dr["Acc_EmailID"].ToString(),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                userName = dr["Usr_Username"].ToString(),
                                LeaveStartDate = dr["LeaveStartDate"].ToString(),
                                LeaveEndDate = dr["LeaveEndDate"].ToString(),
                                No_Of_Days = Convert.ToInt32(dr["No_Of_Days"]),
                                Leavestatus = dr["ResultSubmitStatus"].ToString(),
                                empmailid = dr["UsrP_EmailID"].ToString()
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
            var result = new { objmanagerdetails.myleaves, objmanagerdetails.leavesforapproval, Roleid };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWrkfrmHomePreview()
        {

            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            string RoleName = sessId.RoleName;
            string Roleid = sessId.RoleId.ToString();
            WFHPreviewDetails objmanagerdetails = new WFHPreviewDetails();
            Conn = new SqlConnection(str);
            try
            {
                objmanagerdetails.UserWrkfrmhome = new List<UserWFHDetails>();
                objmanagerdetails.wrkfrmhmeforapproval = new List<ManagerWFHforApproval>();

                Conn.Open();
                SqlCommand cmd = new SqlCommand("[GetUserWorkFrmHomeApproval]", Conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", UserID);
                //if (Roleid == "1002")
                //{
                //    cmd.Parameters.AddWithValue("@userid", "1002");
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@userid", UserID);
                //}
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                if ((Roleid == "1001") || (Roleid == "1002") || (Roleid == "1007"))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objmanagerdetails.UserWrkfrmhome.Add(new UserWFHDetails
                            {
                                Usrl_UserId = Convert.ToInt32(dr["Usrl_UserId"]),
                                UserwfhID = Convert.ToInt32(dr["UserwfhID"]),
                                userName = dr["Usr_Username"].ToString(),
                                usrEmailID = dr["UsrP_EmailID"].ToString(),
                                LeaveStartDate = dr["LeaveStartDate"].ToString(),
                                LeaveEndDate = dr["LeaveEndDate"].ToString(),
                                Leavestatus = dr["ResultSubmitStatus"].ToString(),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                accntmail = dr["Acc_EmailID"].ToString(),
                                Tot_No_Days = Convert.ToInt32(dr["Tot_No_Days"]),
                                wfhempmailid = dr["UsrP_EmailID"].ToString()
                            });

                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow druser in ds.Tables[1].Rows)
                        {
                            objmanagerdetails.wrkfrmhmeforapproval.Add(new ManagerWFHforApproval
                            {
                                Usrl_UserId = Convert.ToInt32(druser["Usrl_UserId"]),
                                UserwfhID = Convert.ToInt32(druser["UserwfhID"]),
                                userName = druser["Usr_Username"].ToString(),
                                UserEmail = druser["UsrP_EmailID"].ToString(),
                                LeaveStartDate = druser["LeaveStartDate"].ToString(),
                                LeaveEndDate = druser["LeaveEndDate"].ToString(),
                                Leavestatus = druser["ResultSubmitStatus"].ToString(),
                                ProjectName = druser["Proj_ProjectName"].ToString(),
                                accntmail = druser["Acc_EmailID"].ToString(),
                                Tot_No_Days = Convert.ToInt32(druser["Tot_No_Days"]),
                                ManagerID1 = Convert.ToInt32(druser["L1_ManagerId"]),
                                ManagerName1 = druser["L1_ManagerName"].ToString(),
                                ManagerEmail1 = druser["L1_ManagerMail"].ToString(),
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
                            objmanagerdetails.UserWrkfrmhome.Add(new UserWFHDetails
                            {
                                Usrl_UserId = Convert.ToInt32(dr["Usrl_UserId"]),
                                UserwfhID = Convert.ToInt32(dr["UserwfhID"]),
                                userName = dr["Usr_Username"].ToString(),
                                usrEmailID = dr["UsrP_EmailID"].ToString(),
                                LeaveStartDate = dr["LeaveStartDate"].ToString(),
                                LeaveEndDate = dr["LeaveEndDate"].ToString(),
                                Leavestatus = dr["ResultSubmitStatus"].ToString(),
                                ProjectName = dr["Proj_ProjectName"].ToString(),
                                accntmail = dr["Acc_EmailID"].ToString(),
                                Tot_No_Days = Convert.ToInt32(dr["Tot_No_Days"]),
                                wfhempmailid = dr["UsrP_EmailID"].ToString(),
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
            var result = new { objmanagerdetails.wrkfrmhmeforapproval, objmanagerdetails.UserWrkfrmhome, Roleid };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        int accountid;


        public JsonResult GetHolidayDates()
        {
            AdminComponent adcmp = new AdminComponent();
            UserSessionInfo info = new UserSessionInfo();
            accountid = info.AccountId;
            var gethldydetails = adcmp.GetHolidayDates(accountid);
            return Json(gethldydetails, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetUSApprovedLeaves(int userId, string monthYear)
        //{
        //    AdminComponent admcmp = new AdminComponent();
        //    var query = admcmp.GetUSApprovedLeaves(userId, monthYear);
        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public JsonResult GetUSApprovedLeaves(int Userid, string monthYear,int Projid)
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            //Leavelist objlist = new Leavelist();
            List<USLeaveDates> usLeavesdate = new List<USLeaveDates>();
            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("Get_USUserLeavesforApproval", Conn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Userid);
                cmd.Parameters.AddWithValue("@month_year", monthYear);
                cmd.Parameters.AddWithValue("@projid", Projid);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    usLeavesdate.Add(new USLeaveDates
                    {
                        leavedates = Convert.ToDateTime(dr["LeaveDates"]),
                        dates = Convert.ToInt32(dr["dates"]),

                    });

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
            return Json(usLeavesdate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUSApprovedWFH(int Userid, string monthYear,int Projid)
        {
            UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
            int UserID = sessId.UserId;
            List<USWFHDates> usWFHdates = new List<USWFHDates>();
            Conn = new SqlConnection(str);
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("Get_USUserWFHforApproval", Conn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Userid);
                cmd.Parameters.AddWithValue("@month_year", monthYear);
                cmd.Parameters.AddWithValue("@projid", Projid);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    usWFHdates.Add(new USWFHDates
                    {
                        WFHdates = Convert.ToDateTime(dr["LeaveDates"]),
                        dates = Convert.ToInt32(dr["dates"]),

                    });

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
            return Json(usWFHdates, JsonRequestBehavior.AllowGet);
        }

        string projectid;

        //[HttpGet]
        //public JsonResult GetLeaveApproveDetailsofUS()
        //{
        //    AdminComponent admcmp = new AdminComponent();
        //    UserSessionInfo info = new UserSessionInfo();
        //    projectid = info.Projectid.ToString();
        //    var getallempdetails = admcmp.GetAllApprovedLeavesOfUS(projectid);
        //    return Json(getallempdetails, JsonRequestBehavior.AllowGet);
        //}
        int AccountId;
        public JsonResult GetOptionalHolidays(int Cl_ProjId)
        {
            List<HolidayCalendarEntity> UserOptlHolidays = null;
            UserSessionInfo info = new UserSessionInfo();
            AccountId = info.AccountId;
            int userdid = info.UserId;
            int? ClientprojID = info.ClientprojID;

           
            try
            {
                var objDtl = new LeaveTypeComponent();
                UserOptlHolidays = objDtl.GetOptionalHolidays(AccountId, userdid, Cl_ProjId);
                //TempData["AccountId"] = AccountId;
            }
             
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserOptlHolidays, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUserOptionalHolidays()
        {
            List<UserLeave> UserOptlHolidays = null;
            UserSessionInfo info = new UserSessionInfo();
            AccountId = info.AccountId;
            int userid = info.UserId;
            try
            {
                var objDtl = new LeaveTypeComponent();
                UserOptlHolidays = objDtl.GetUserOptionalHolidays(userid);
                //TempData["AccountId"] = AccountId;
            }

            catch (Exception ex)
            {
                return null;
            }
            return Json(UserOptlHolidays, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMandatoryHolidays(int Cl_ProjId)
        {
            List<HolidayCalendarEntity> UserOptlHolidays = null;
            UserSessionInfo info = new UserSessionInfo();
            AccountId = info.AccountId;
            int userdid = info.UserId;
            
            try
            {
                var objDtl = new LeaveTypeComponent();
                UserOptlHolidays = objDtl.GetMandatoryHolidays(AccountId, userdid, Cl_ProjId);
                //TempData["AccountId"] = AccountId;
            }

            catch (Exception ex)
            {
                return null;
            }
            return Json(UserOptlHolidays, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLeaveApproveDetailsofUS(int Cl_ProjId)
        {
            List<UserLeave> UserLeaves = null;           
            UserSessionInfo info = new UserSessionInfo();
            int userdid = info.UserId;
            int AccountId = info.AccountId;
            try
            {
                var objDtl = new LeaveTypeComponent();
                UserLeaves = objDtl.GetLeaveApproveDetailsofUS(userdid, Cl_ProjId);
                //TempData["AccountId"] = AccountId;
            }

            catch (Exception ex)
            {
                return null;
            }
            return Json(UserLeaves, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWfhApproveDetailsofUS(int Cl_ProjId)
        {
            List<UserLeave> UserLeaves = null;
            UserSessionInfo info = new UserSessionInfo();
            int userdid = info.UserId;
            int AccountId = info.AccountId;
            try
            {
                var objDtl = new LeaveTypeComponent();
                UserLeaves = objDtl.GetWfhApproveDetailsofUS(userdid, Cl_ProjId);
                //TempData["AccountId"] = AccountId;
            }

            catch (Exception ex)
            {
                return null;
            }
            return Json(UserLeaves, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult GetNoOfoptholidaystoOpt(int CL_ProjectId)
        //{
        //    //List<UserLeave> UserOptLeaves = null;
        //    //UserSessionInfo info = new UserSessionInfo();
        //    //int userdid = info.UserId;
        //    //int AccountId = info.AccountId;
        //    //try
        //    //{
        //    //    var objDtl = new LeaveTypeComponent();
        //    //    UserOptLeaves = objDtl.GetNoOfoptholidaystoOpt(userdid, Cl_ProjId);
        //    //    //TempData["AccountId"] = AccountId;
        //    //}

        //    //catch (Exception ex)
        //    //{
        //    //    return null;
        //    //}
        //    //return Json(UserOptLeaves, JsonRequestBehavior.AllowGet);
        //    UserSessionInfo sessId = Session["UserSessionInfo"] as UserSessionInfo;
        //    int UserID = sessId.UserId;
        //    List<USWFHDates> usWFHdates = new List<USWFHDates>();
        //    Conn = new SqlConnection(str);
        //    try
        //    {
        //        Conn.Open();
        //        SqlCommand cmd = new SqlCommand("GetTotalOptionalHolidays", Conn);
        //        cmd.CommandTimeout = 0;
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //       // cmd.Parameters.AddWithValue("@id", Userid);
        //       // cmd.Parameters.AddWithValue("@month_year", monthYear);
        //        cmd.Parameters.AddWithValue("@CL_ProjectId", CL_ProjectId);
        //        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        dataAdapter.Fill(ds);
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            usWFHdates.Add(new USWFHDates
        //            {
        //                WFHdates = Convert.ToDateTime(dr["LeaveDates"]),
        //                dates = Convert.ToInt32(dr["dates"]),

        //            });

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (Conn != null)
        //        {
        //            if (Conn.State == ConnectionState.Open)
        //            {
        //                Conn.Close();
        //                Conn.Dispose();
        //            }
        //        }
        //    }
        //    return Json(usWFHdates, JsonRequestBehavior.AllowGet);
        //}

    }

}