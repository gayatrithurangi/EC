//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Evolutyz.Data
{
    using System;
    
    public partial class WebGetAccountHolderTimeSheets_Result
    {
        public Nullable<long> Sno { get; set; }
        public int UserID { get; set; }
        public int TimesheetID { get; set; }
        public string MonthYearName { get; set; }
        public Nullable<decimal> WorkedHours { get; set; }
        public string TimesheetType { get; set; }
        public string MonthYearName1 { get; set; }
        public Nullable<System.DateTime> ByWeeklyStartDate { get; set; }
        public Nullable<System.DateTime> ByWeeklyEndDate { get; set; }
        public string TimesheetDuration { get; set; }
        public Nullable<int> Companyworkinghours { get; set; }
        public string Proj_ProjectName { get; set; }
        public int Proj_ProjectID { get; set; }
        public string ClientProjTitle { get; set; }
        public Nullable<int> ClientProjtId { get; set; }
        public string ResultSubmitStatus { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public string Usr_Username { get; set; }
        public string L1_ManagerName { get; set; }
        public string L2_ManagerName { get; set; }
        public Nullable<int> UProj_L1_ManagerId { get; set; }
        public Nullable<int> UProj_L2_ManagerId { get; set; }
        public string SubmittedDate { get; set; }
        public Nullable<System.DateTime> L1_ApproverDate { get; set; }
        public string TimesheetMonth { get; set; }
        public string ByMonthlyDates { get; set; }
        public string FinalStatus { get; set; }
    }
}
