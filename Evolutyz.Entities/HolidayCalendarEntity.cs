using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutyz.Entities
{
    public class HolidayCalendarEntity : ResponseHeader
    {
        public int HolidayCalendarID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string HolidayName { get; set; }
        public int Year { get; set; }
        public System.DateTime HolidayDate { get; set; }
        public string HolidayWeek { get; set; }
        public bool isOptionalHoliday { get; set; }
        public bool isActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public int? FinancialYearId { get; set; }
        public Nullable<int> StartDate { get; set; }
        public Nullable<int> EndDate { get; set; }
        public string financialyear { get; set; }
        public string HolDate { get; set; }
        public string ProjectName { get; set; }
        public int? HolidayCalendarProjectId { get; set; }
        public int optionalholidays { get; set; }
        public int useroptionalholidays { get; set; }
        //public int? ClientProjectId { get; set; }
        public UserLeave userLeaf { get; set; }
    }

    public class timesheetEntity
    {
        public string UserName { get; set; }
        public int TimesheetID { get; set; }
        public int UserID { get; set; }

        public String TimeSheetMonth { get; set; }
        public string Comments { get; set; }
        public int ProjectID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string TaskId { get; set; }
        public int FlagEmailStatus { get; set; }
        public int Transoutput { get; set; }
        public string ManagerId { get; set; }
        public string ActionType { get; set; }
        public string SubmittedType { get; set; }
        public string SubmittedFlag { get; set; }
        public string ManagerID1 { get; set; }
        public string ManagerEmail1 { get; set; }
        public string ManagerID2 { get; set; }
        public string ManagerEmail2 { get; set; }
        public string AccManagerEmail { get; set; }
        public string AccManagerID { get; set; }
        public string UserEmailId { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public string L1ApproverStatus { get; set; }
        public string L2ApproverStatus { get; set; }
        public string statusmsg { get; set; }
        public Nullable<System.DateTime> L1_ApproverDate { get; set; }
        public Nullable<System.DateTime> L2_ApproverDate { get; set; }
        public Nullable<System.DateTime> L1_RejectedDate { get; set; }
        public Nullable<System.DateTime> L2_RejectedDate { get; set; }
        public string EmailAppOrRejStatus { get; set; }
        public string Position { get; set; }
        public string Message { get; set; }
        public string SubmittedState { get; set; }
        public string Proj_ProjectName { get; set; }
        public string ManagerLevel1 { get; set; }
        public string ManagerLevel2 { get; set; }


    }


    public  class UserLeave
    {
        public int Usrl_LeaveId { get; set; }
        public Nullable<int> Usrl_UserId { get; set; }
        public Nullable<System.DateTime> LeaveStartDate { get; set; }
        public Nullable<System.DateTime> LeaveEndDate { get; set; }
        public Nullable<int> Tot_No_Days { get; set; }
        public string Comments { get; set; }
        public Nullable<int> L_StatusId { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> RejectedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<int> RejectedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> OnHoldDate { get; set; }
        public Nullable<int> OnHoldBy { get; set; }
        public Nullable<bool> isoptionalholiday { get; set; }
    }

    public class ProjectDetails
    {
        public string CL_ProjId { get; set; }
    }

    public class OptionalHolidaysData
    {

      
        public GetOptionalHolidaysCount HolidayList { get; set; }

    }

    public class GetOptionalHolidaysCount
    {
        public int IsOptionalHolidayCount { get; set; }
        public int TotalOptionalHolidays { get; set; }
    }

}
