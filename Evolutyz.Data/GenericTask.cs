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
    
    public partial class GenericTask
    {
        public int tsk_TaskID { get; set; }
        public string tsk_TaskName { get; set; }
        public string tsk_TaskDescription { get; set; }
        public bool tsk_ActiveStatus { get; set; }
        public short tsk_Version { get; set; }
        public System.DateTime tsk_CreatedDate { get; set; }
        public int tsk_CreatedBy { get; set; }
        public Nullable<System.DateTime> tsk_ModifiedDate { get; set; }
        public Nullable<int> tsk_ModifiedBy { get; set; }
        public bool tsk_isDeleted { get; set; }
        public Nullable<int> Acc_AccountID { get; set; }
    }
}
