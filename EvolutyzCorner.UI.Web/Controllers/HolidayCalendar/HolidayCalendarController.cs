using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;



namespace EvolutyzCorner.UI.Web.Controllers.HolidayCalendar
{

    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class HolidayCalendarController : Controller
    {
        HolidayCalendarComponent _holidayComp = new HolidayCalendarComponent();
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HolidayCalendar()
        {
            var LeaveSchemeComponent = new LeaveSchemeComponent();
            var FinancialYears = LeaveSchemeComponent.Getallfinancialyears().Select(a => new SelectListItem()
            {
                Value = a.FinancialYearId.ToString(),
                Text = a.StartDate.ToString(),
            }).OrderByDescending(x=>x.Value);
            ViewBag.FinancialYears = FinancialYears;

          
            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();


            var mk = "read";
            foreach (var item in obj)
            {
                if (item.ModuleName== "Add Holiday Calendar")
                {
                     mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }
            }
            return View();
        }

      
        [HttpPost]
        public string CreateHoliday(string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isDeleted)
        {
            string strResponse = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            int accountid = objinfo.AccountId;
            if (ModelState.IsValid)
            {
                strResponse = _holidayComp.AddHoliday(accountid, HolidayName, HolidayDate, FinancialYearId, isOptionalHoliday, isDeleted);
            }
            return strResponse;
        }

        public string UpdateCalenderControl(string HolidayCalendarID, string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isDeleted, string HolidayCalendarProjectId)
        {
            string resp = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    resp = _holidayComp.UpdateHoliday(HolidayCalendarID, HolidayName, HolidayDate, FinancialYearId, isOptionalHoliday, isDeleted, HolidayCalendarProjectId);
                }
                return resp;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string DeleteHoliday(string id)
        {
            return _holidayComp.DeleteHoliday(id);
        }

        public JsonResult GetHolidaysCollection()
        {
             EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            List<HolidayCalendarEntity> holidays = null;
            UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int accountID = _objSessioninfo.AccountId;
            int? projectid = _objSessioninfo.projectid;
            int userid = _objSessioninfo.UserId;


            try
            {
                holidays = _holidayComp.GetHolidayCalendar(accountID, projectid);
            }

            catch (Exception ex)
            {
                return null;
            }
            return Json(holidays, JsonRequestBehavior.AllowGet);
        }

        //Select Query
        public JsonResult SelectCalenderControl()
        {
            List<HolidayCalendarEntity> holidays = _holidayComp.SelectHolidayDetail();
            return Json(holidays, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCalenderDetailByID(int ID)
        {
            HolidayCalendarEntity holiday = _holidayComp.GetHolidayByID(ID);
            return Json(holiday, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string CreateHolidayforclient(List<HolidayCalendarEntity> holidays)
        {
            string strResponse = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            
            if (ModelState.IsValid)
            {
                strResponse = _holidayComp.AddHolidayforclient(holidays);
            }
            return strResponse;
        }

        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
           strResponse = _holidayComp.ChangeStatus(id, status);

            return strResponse;
        }

       
    }
}