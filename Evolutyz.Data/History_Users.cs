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
    
    public partial class History_Users
    {
        public int History_User_ID { get; set; }
        public int History_Usr_UserID { get; set; }
        public int History_Usr_AccountID { get; set; }
        public int History_Usr_UserTypeID { get; set; }
        public int History_Usr_RoleID { get; set; }
        public Nullable<int> History_Usr_Manager { get; set; }
        public Nullable<int> History_Usr_Manager2 { get; set; }
        public string History_Usr_Username { get; set; }
        public string History_Usr_LoginId { get; set; }
        public string History_Usr_Password { get; set; }
        public bool History_Usr_ActiveStatus { get; set; }
        public Nullable<int> History_Usr_TaskID { get; set; }
        public short History_Usr_Version { get; set; }
        public System.DateTime History_Usr_CreatedDate { get; set; }
        public Nullable<int> History_Usr_CreatedBy { get; set; }
        public Nullable<System.DateTime> History_Usr_ModifiedDate { get; set; }
        public Nullable<int> History_Usr_ModifiedBy { get; set; }
        public bool History_Usr_isDeleted { get; set; }
    }
}
