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
    using System.Collections.Generic;
    
    public partial class UserLeaf
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
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> CL_Projectid { get; set; }
    }
}
