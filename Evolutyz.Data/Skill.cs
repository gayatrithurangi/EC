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
    
    public partial class Skill
    {
        public int SkillId { get; set; }
        public string SkillTitle { get; set; }
        public string ShortDescription { get; set; }
        public Nullable<System.DateTime> Sk_CreatedDate { get; set; }
        public Nullable<int> Sk_CreatedBy { get; set; }
        public Nullable<System.DateTime> Sk_ModifiedDate { get; set; }
        public Nullable<int> Sk_ModifiedBy { get; set; }
        public Nullable<bool> Sk_isDeleted { get; set; }
        public Nullable<int> Acc_AccountID { get; set; }
        public Nullable<int> StatusID { get; set; }
    }
}
