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
    
    public partial class Interview_Position_TechnologyStack
    {
        public int IPID { get; set; }
        public Nullable<int> TechnologyStack_Id { get; set; }
        public Nullable<int> Assessment_For_Position_Positionid { get; set; }
        public Nullable<int> No_of_Questions { get; set; }
        public Nullable<int> Assessment_TimePeriod_in_sec { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        public virtual Assessment_For_Position Assessment_For_Position { get; set; }
        public virtual TechnologyStack TechnologyStack { get; set; }
    }
}
