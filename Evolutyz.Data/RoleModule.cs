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
    
    public partial class RoleModule
    {
        public int RMod_RoleModuleID { get; set; }
        public int RMod_AccountID { get; set; }
        public int RMod_RoleID { get; set; }
        public int Sub_ModuleID { get; set; }
        public Nullable<int> ModuleAccessTypeID { get; set; }
        public bool RMod_ActiveStatus { get; set; }
        public short RMod_Version { get; set; }
        public System.DateTime RMod_CreatedDate { get; set; }
        public int RMod_CreatedBy { get; set; }
        public Nullable<System.DateTime> RMod_ModifiedDate { get; set; }
        public Nullable<int> RMod_ModifiedBy { get; set; }
        public bool RMod_isDeleted { get; set; }
    }
}
