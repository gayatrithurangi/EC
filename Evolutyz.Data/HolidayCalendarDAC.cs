using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Evolutyz.Data
{
    public class HolidayCalendarDAC : DataAccessComponent
    {
        #region To add Holiday Calendar in Database
        public string AddHoliday(int accountid, string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isDeleted)
        {
            string strResponse = string.Empty;
            int financialyearid = Convert.ToInt16(FinancialYearId);
            bool isoptionalid = Convert.ToBoolean(isOptionalHoliday);
            bool isactive = Convert.ToBoolean(isDeleted);
            UserSessionInfo objinfo = new UserSessionInfo();
            int userid = objinfo.UserId;
            DateTime holidaydate = Convert.ToDateTime(HolidayDate);
            int year = holidaydate.Year;
            using (var db = new DbContext(CONNECTION_NAME))
            {
                HolidayCalendar holidayDetails = db.Set<HolidayCalendar>().Where(s => (s.HolidayName == HolidayName && s.AccountID == accountid && s.ProjectID == null && s.Year == financialyearid && s.isDeleted == false)).FirstOrDefault<HolidayCalendar>();
                HolidayCalendar holidayDetail = db.Set<HolidayCalendar>().Where(s => (s.HolidayDate == holidaydate && s.AccountID == accountid && s.ProjectID == null && s.Year == financialyearid && s.isDeleted == false)).FirstOrDefault<HolidayCalendar>();
                FinancialYear startYear = db.Set<FinancialYear>().Where(s => s.FinancialYearId == financialyearid).FirstOrDefault<FinancialYear>();
                if (holidayDetails != null)
                {
                    return strResponse = "HolidayName Already Exist In This Account";
                }

                if (holidayDetail != null)
                {
                    return strResponse = "HolidayDate Already Exist In This Account";
                }
                if (year != startYear.StartDate)
                {
                    return strResponse = "Please Select Correct Financial Year";
                }
                try
                {
                    HolidayCalendar holidayData = new HolidayCalendar();
                    holidayData.AccountID = accountid;
                    holidayData.Year = financialyearid;
                    holidayData.HolidayName = HolidayName;
                    holidayData.HolidayDate = Convert.ToDateTime(HolidayDate);
                    // holidayData.Year = Convert.ToInt16(holiday.Year);
                    holidayData.isOptionalHoliday = isoptionalid;
                    holidayData.isDeleted = isactive;
                    holidayData.CreatedBy = userid;
                    holidayData.CreatedDate = System.DateTime.Now;

                    db.Set<HolidayCalendar>().Add(holidayData);
                    db.SaveChanges();
                    strResponse = "Holiday Successfully Created";
                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
            }
            return strResponse;
        }
        #endregion

        #region To update existing Holiday in Database
        public string UpdateHoliday(string HolidayCalendarID, string HolidayName, string HolidayDate, string FinancialYearId, string isOptionalHoliday, string isActive, string HolidayCalendarProjectId)
        {
            HolidayCalendar holidayDetails = null;

            string strResponse = string.Empty;
            int hcid = Convert.ToInt32(HolidayCalendarID);
            int financialyearid = Convert.ToInt16(FinancialYearId);
            bool isoptionalid = Convert.ToBoolean(isOptionalHoliday);
            bool isactive = Convert.ToBoolean(isActive);
            UserSessionInfo objinfo = new UserSessionInfo();
            int userid = objinfo.UserId;
            int accountid = objinfo.AccountId;
            DateTime holidaydate = Convert.ToDateTime(HolidayDate);
            int Pid = Convert.ToInt32(HolidayCalendarProjectId);
            int year = holidaydate.Year;
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    holidayDetails = db.Set<HolidayCalendar>().Where(s => s.HolidayCalendarID == hcid).FirstOrDefault<HolidayCalendar>();
                    HolidayCalendar holidayDet = db.Set<HolidayCalendar>().Where(s => (s.HolidayName == HolidayName && s.HolidayCalendarID != hcid && s.AccountID == accountid && s.Year == financialyearid && s.isDeleted == false)).FirstOrDefault<HolidayCalendar>();
                    HolidayCalendar holidayDetail = db.Set<HolidayCalendar>().Where(s => (s.HolidayDate == holidaydate && s.HolidayCalendarID != hcid && s.AccountID == accountid && s.Year == financialyearid && s.isDeleted == false)).FirstOrDefault<HolidayCalendar>();
                    FinancialYear startYear = db.Set<FinancialYear>().Where(s => s.FinancialYearId == financialyearid).FirstOrDefault<FinancialYear>();

                    if (holidayDet != null)
                    {
                        return strResponse = "HolidayName Already Exist";
                    }

                    if (holidayDetail != null)
                    {
                        return strResponse = "HolidayDate Already Exist";
                    }
                    if (year != startYear.StartDate)
                    {
                        return strResponse = "Please Select Correct financialyear";
                    }
                    if (holidayDetails == null)
                    {
                        return null;
                    }
                    //holidayDetails.AccountID = holiday.AccountID;
                    holidayDetails.HolidayName = HolidayName;
                    holidayDetails.HolidayDate = Convert.ToDateTime(HolidayDate);
                    holidayDetails.Year = financialyearid;
                    holidayDetails.isOptionalHoliday = isoptionalid;
                    holidayDetails.isDeleted = isactive;
                    holidayDetails.ModifiedBy = userid;
                    holidayDetails.ModifiedDate = System.DateTime.Now;
                    holidayDetails.CL_ProjectID = Pid;

                    db.Entry(holidayDetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    strResponse = "Holiday successfully updated";
                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
                return strResponse;
            }
        }
        #endregion

        #region To delete existing Holiday from Database
        public string DeleteHoliday(string ID)
        {
            string strResponse = string.Empty;
            HolidayCalendar holidayData = null;
            int did = Convert.ToInt32(ID);
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    holidayData = db.Set<HolidayCalendar>().Where(s => s.HolidayCalendarID == did).FirstOrDefault<HolidayCalendar>();
                    if (holidayData == null)
                    {
                        return null;
                    }
                    holidayData.isDeleted = true;
                    // holidayData.isActive = false;
                    db.Entry(holidayData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    strResponse = "Holiday successfully deleted";
                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
            }
            return strResponse;
        }
        #endregion

        #region To get all details of HolidayCalendar from Database
        public List<HolidayCalendarEntity> GetHolidayCalendar(int accountID, int? projectid)
        {
            //UserSessionInfo info = new UserSessionInfo();
            //int? projectid = info.Projectid;
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    if (projectid == 0)
                    {
                        var query = (from h in db.HolidayCalendars
                                     join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                     join f in db.FinancialYears on h.Year equals f.FinancialYearId
                                     where h.AccountID == accountID && h.ProjectID == null && f.StartDate != 2018
                                     //f.StartDate == 2020
                                     select new HolidayCalendarEntity
                                     {
                                         HolidayCalendarID = h.HolidayCalendarID,
                                         HolidayName = h.HolidayName,
                                         HolidayDate = h.HolidayDate,
                                         Year = h.Year,
                                         AccountID = h.AccountID,
                                         AccountName = a.Acc_AccountName,
                                         isOptionalHoliday = h.isOptionalHoliday,
                                         //isActive = h.isActive,
                                         StartDate = f.StartDate,
                                         EndDate = f.EndDate,
                                         // financialyear = f.StartDate /*+ "-" + f.EndDate*/,
                                         CreatedBy = h.CreatedBy,
                                         CreatedDate = h.CreatedDate,
                                         ModifiedBy = h.ModifiedBy,
                                         ModifiedDate = h.ModifiedDate,
                                         isDeleted = h.isDeleted,
                                     }).OrderByDescending(x => x.HolidayDate).ToList();
                        foreach (var item in query)
                        {
                             item.HolidayWeek = item.HolidayDate.DayOfWeek.ToString();
                             item.HolDate = item.HolidayDate.ToString("MMMM dd, yyyy");
                        }
                        return query;

                    }
                    else
                    {
                        var query = (from h in db.HolidayCalendars
                                     join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                     join f in db.FinancialYears on h.Year equals f.FinancialYearId
                                     where h.AccountID == accountID && h.ProjectID == projectid && f.StartDate != 2018
                                     //f.StartDate == 2020
                                     && (h.CL_ProjectID == null || h.CL_ProjectID == 0) 
                                     select new HolidayCalendarEntity
                                     {
                                         HolidayCalendarID = h.HolidayCalendarID,
                                         HolidayName = h.HolidayName,
                                         HolidayDate = h.HolidayDate,
                                         Year = h.Year,
                                         AccountID = h.AccountID,
                                         AccountName = a.Acc_AccountName,
                                         isOptionalHoliday = h.isOptionalHoliday,
                                         //isActive = h.isActive,
                                         StartDate = f.StartDate,
                                         EndDate = f.EndDate,
                                         // financialyear = f.StartDate + "-" + f.EndDate,
                                         CreatedBy = h.CreatedBy,
                                         CreatedDate = h.CreatedDate,
                                         ModifiedBy = h.ModifiedBy,
                                         ModifiedDate = h.ModifiedDate,
                                         isDeleted = h.isDeleted,
                                     }).OrderByDescending(x => x.HolidayDate).ToList();
                        if (query.Count() == 0)
                        {
                            query = (from h in db.HolidayCalendars
                                     join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                     join f in db.FinancialYears on h.Year equals f.FinancialYearId
                                     where h.AccountID == accountID && h.ProjectID == null
                                     && (h.CL_ProjectID == null || h.CL_ProjectID == 0)
                                     select new HolidayCalendarEntity
                                     {
                                         HolidayCalendarID = h.HolidayCalendarID,
                                         HolidayName = h.HolidayName,
                                         HolidayDate = h.HolidayDate,
                                         Year = h.Year,
                                         AccountID = h.AccountID,
                                         AccountName = a.Acc_AccountName,
                                         isOptionalHoliday = h.isOptionalHoliday,
                                         // isActive = h.isActive,
                                         StartDate = f.StartDate,
                                         EndDate = f.EndDate,
                                         // financialyear = f.StartDate + "-" + f.EndDate,
                                         CreatedBy = h.CreatedBy,
                                         CreatedDate = h.CreatedDate,
                                         ModifiedBy = h.ModifiedBy,
                                         ModifiedDate = h.ModifiedDate,
                                         isDeleted = h.isDeleted,
                                     }).OrderByDescending(x => x.HolidayDate).ToList();
                        }
                        foreach (var item in query)
                        {
                            item.HolidayWeek = item.HolidayDate.DayOfWeek.ToString();
                            item.HolDate = item.HolidayDate.ToString("MMMM dd, yyyy");
                        }
                        return query;
                    }
                }



                //}
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        #endregion

        #region To get particular Holiday details from Database
        public HolidayCalendarEntity GetHolidayByID(int ID)
        {
            HolidayCalendarEntity response = new HolidayCalendarEntity();
            
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    response = (from h in db.HolidayCalendars
                                join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                //join cp in db.ClientProjects on h.CL_ProjectID equals cp.CL_ProjectID
                                where h.HolidayCalendarID == ID
                                select new HolidayCalendarEntity
                                {
                                    HolidayCalendarID = h.HolidayCalendarID,
                                    HolidayName = h.HolidayName,
                                    HolidayDate = h.HolidayDate,
                                    HolidayCalendarProjectId = h.CL_ProjectID,
                                    //ProjectName = cp.ClientProjTitle,
                                    //Year = h.Year,
                                    FinancialYearId = h.Year,
                                    //HolidayWeek = h.HolidayDate.DayOfWeek.ToString(),
                                    //AccountID = h.AccountID,
                                    // AccountName = a.Acc_AccountName,
                                    isOptionalHoliday = h.isOptionalHoliday,
                                    //isActive = h.isActive,
                                    CreatedBy = h.CreatedBy,
                                    CreatedDate = h.CreatedDate,
                                    ModifiedBy = h.ModifiedBy,
                                    ModifiedDate = h.ModifiedDate,
                                    isDeleted = h.isDeleted,
                                }).FirstOrDefault();
                    response.IsSuccessful = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.IsSuccessful = false;
                    response.Message = "Error Occured in GetTaskDetailDetailByID(ID)";
                    response.Detail = ex.Message.ToString();
                    return response;
                }
            }
        }
        #endregion

        #region To get TaskDetail details for select list
        public List<HolidayCalendarEntity> SelectHolidayDetail()
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var query = (from h in db.HolidayCalendars
                                 join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                 select new HolidayCalendarEntity
                                 {
                                     HolidayCalendarID = h.HolidayCalendarID,
                                     HolidayName = h.HolidayName,
                                     HolidayDate = h.HolidayDate,
                                     Year = h.Year,
                                     HolidayWeek = h.HolidayDate.DayOfWeek.ToString(),
                                     AccountID = h.AccountID,
                                     AccountName = a.Acc_AccountName,
                                     isActive = h.isActive,
                                     CreatedBy = h.CreatedBy,
                                     CreatedDate = h.CreatedDate,
                                     ModifiedBy = h.ModifiedBy,
                                     //ModifiedDate = h.ModifiedDate,
                                     isDeleted = h.isDeleted,
                                 }).OrderBy(x => x.AccountID).ThenBy(y => y.Year).ThenBy(z => z.HolidayDate).ToList();
                    return query;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        #endregion

        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            HolidayCalendar holidayData = null;
            bool Status = Convert.ToBoolean(status);
            int did = Convert.ToInt32(id);
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    holidayData = db.Set<HolidayCalendar>().Where(s => s.HolidayCalendarID == did).FirstOrDefault<HolidayCalendar>();
                    if (holidayData == null)
                    {
                        return null;
                    }
                    holidayData.isDeleted = Status;
                    // holidayData.isActive = false;
                    db.Entry(holidayData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (status == "true")
                    {
                        strResponse = "Status Changed to InActive";
                    }
                    else
                    {
                        strResponse = "Status Changed to Active";
                    }

                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
            }
            return strResponse;
        }

        public string AddHolidayforclient(List<HolidayCalendarEntity> holidays)
        {
            string strResponse = string.Empty;
            for (int i = 0; i <= holidays.Count - 1; i++)
            {
                int financialyearid = Convert.ToInt16(holidays[i].FinancialYearId);
                string holidayname = holidays[i].HolidayName;
                bool isoptionalid = Convert.ToBoolean(holidays[i].isOptionalHoliday);
                bool isactive = Convert.ToBoolean(holidays[i].isActive);
                UserSessionInfo objinfo = new UserSessionInfo();
                int userid = objinfo.UserId;
                int accountid = objinfo.AccountId;
                int peojectid = Convert.ToInt32(holidays[i].ProjectID);
                int? ClientCalPid = Convert.ToInt32(holidays[i].HolidayCalendarProjectId);
                DateTime holidaydate = Convert.ToDateTime(holidays[i].HolidayDate);
                int year = holidaydate.Year;
                try
                {
                    using (var db = new DbContext(CONNECTION_NAME))
                    {
                        HolidayCalendar holidayDetails = db.Set<HolidayCalendar>().Where(s => (s.HolidayName == holidayname && s.AccountID == accountid && s.ProjectID == peojectid && s.Year == financialyearid)).FirstOrDefault<HolidayCalendar>();
                        HolidayCalendar holidayDetail = db.Set<HolidayCalendar>().Where(s => (s.HolidayDate == holidaydate && s.AccountID == accountid && s.ProjectID == peojectid && s.Year == financialyearid)).FirstOrDefault<HolidayCalendar>();
                        FinancialYear startYear = db.Set<FinancialYear>().Where(s => s.FinancialYearId == financialyearid).FirstOrDefault<FinancialYear>();
                      

                        //if (holidayDetails != null && ClientCalPid == null)
                        //{
                        //    return strResponse = "HolidayName Already Exist In This Project";
                        //}

                        //if (holidayDetail != null && ClientCalPid != null)
                        //{
                        //    return strResponse = "HolidayDate Already Exist In This Project";
                        //}
                        if (year != startYear.StartDate)
                        {
                            return strResponse = "Please Select Correct Financialyear";
                        }

                        HolidayCalendar holidayData = new HolidayCalendar();
                        holidayData.AccountID = accountid;
                        holidayData.Year = financialyearid;
                        holidayData.HolidayName = holidays[i].HolidayName;
                        holidayData.HolidayDate = Convert.ToDateTime(holidays[i].HolidayDate);
                        // holidayData.Year = Convert.ToInt16(holiday.Year);
                        holidayData.isOptionalHoliday = isoptionalid;
                        //holidayData.isActive = isactive;
                        holidayData.CreatedBy = userid;
                        holidayData.CreatedDate = System.DateTime.Now;
                        holidayData.ProjectID = peojectid;
                        holidayData.CL_ProjectID = ClientCalPid;
                        holidayData.isDeleted = isactive;
                        db.Set<HolidayCalendar>().Add(holidayData);
                        db.SaveChanges();
                        strResponse = "Holiday Successfully Created";
                    }
                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }

            }
            return strResponse;
        }



        public List<LeaveTypeEntity> GetHolidayDates(int accountid)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from h in db.HolidayCalendars
                                 join a in db.Accounts on h.AccountID equals a.Acc_AccountID
                                 where h.AccountID == accountid
                                 select new LeaveTypeEntity
                                 {
                                     HolidayCalendarID = h.HolidayCalendarID,
                                     HolidayName = h.HolidayName,
                                     HolidayDate = h.HolidayDate,
                                     //Year = h.Year,
                                     //AccountID = h.AccountID,
                                     //AccountName = a.Acc_AccountName,
                                     //isOptionalHoliday = h.isOptionalHoliday,
                                     //isActive = h.isActive,                                   
                                     //CreatedBy = h.CreatedBy,
                                     //CreatedDate = h.CreatedDate,
                                     //ModifiedBy = h.ModifiedBy,
                                     //ModifiedDate = h.ModifiedDate,
                                     //isDeleted = h.isDeleted,

                                 }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }



        }
    }
}