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
    
    public partial class ProjectSpecificTask
    {
        public int Proj_SpecificTaskId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Proj_SpecificTaskName { get; set; }
        public string RTMId { get; set; }
        public Nullable<System.DateTime> Actual_StartDate { get; set; }
        public Nullable<System.DateTime> Actual_EndDate { get; set; }
        public Nullable<System.DateTime> Plan_StartDate { get; set; }
        public Nullable<System.DateTime> Plan_EndDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<bool> StatusId { get; set; }
        public Nullable<int> tsk_TaskID { get; set; }
    }
}
