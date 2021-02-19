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
    
    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            this.ClientProjects = new HashSet<ClientProject>();
        }
    
        public int Proj_ProjectID { get; set; }
        public int Proj_AccountID { get; set; }
        public string Proj_ClientName { get; set; }
        public string Proj_ProjectCode { get; set; }
        public string Proj_ProjectName { get; set; }
        public string Proj_ProjectDescription { get; set; }
        public System.DateTime Plan_StartDate { get; set; }
        public Nullable<System.DateTime> Plan_EndDate { get; set; }
        public bool Proj_ActiveStatus { get; set; }
        public short Proj_Version { get; set; }
        public System.DateTime Proj_CreatedDate { get; set; }
        public int Proj_CreatedBy { get; set; }
        public Nullable<System.DateTime> Proj_ModifiedDate { get; set; }
        public Nullable<int> Proj_ModifiedBy { get; set; }
        public bool Proj_isDeleted { get; set; }
        public Nullable<System.DateTime> Actual_StartDate { get; set; }
        public Nullable<System.DateTime> Actual_EndDate { get; set; }
        public Nullable<bool> Is_Timesheet_ProjectSpecific { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<int> StateID { get; set; }
        public string WebUrl { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientProject> ClientProjects { get; set; }
    }
}
