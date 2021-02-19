﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evolutyz.Entities;
using Evolutyz.Data;


namespace Evolutyz.Business
{
    public class HolidayCalendarComponent
    {
        HolidayCalendarDAC holidayDAC = new HolidayCalendarDAC();

        public string AddHoliday(int accountid, string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isActive)
        {
            return holidayDAC.AddHoliday(accountid, HolidayName, HolidayDate, FinancialYearId, isOptionalHoliday, isActive);
        }
        public string AddHolidayforclient(List<HolidayCalendarEntity> holidays)
        {
            return holidayDAC.AddHolidayforclient(holidays);
        }

        public string UpdateHoliday(string HolidayCalendarID, string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isActive, string HolidayCalendarProjectId)
        {
            return holidayDAC.UpdateHoliday(HolidayCalendarID, HolidayName, HolidayDate, FinancialYearId, isOptionalHoliday, isActive, HolidayCalendarProjectId);
        }

        public string DeleteHoliday(string ID)
        {
            return holidayDAC.DeleteHoliday(ID);
        }

        public List<HolidayCalendarEntity> GetHolidayCalendar(int accountID,int? Pid)
        {
            return holidayDAC.GetHolidayCalendar(accountID, Pid);
        }

        public HolidayCalendarEntity GetHolidayByID(int ID)
        {
            return holidayDAC.GetHolidayByID(ID);
        }

        public List<HolidayCalendarEntity> SelectHolidayDetail()
        {
            return holidayDAC.SelectHolidayDetail();
        }

        public string ChangeStatus(string id, string status)
        {
            return holidayDAC.ChangeStatus(id, status);
        }
    }
}
