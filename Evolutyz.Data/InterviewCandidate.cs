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
    
    public partial class InterviewCandidate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InterviewCandidate()
        {
            this.CandidateInterviewResults = new HashSet<CandidateInterviewResult>();
        }
    
        public int ICID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Emailid { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public Nullable<int> Assessment_For_Positionid { get; set; }
        public Nullable<int> RecrutementUserid { get; set; }
        public Nullable<System.DateTime> AssessmentDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> status { get; set; }
        public string AssessmentTime { get; set; }
    
        public virtual Assessment_For_Position Assessment_For_Position { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateInterviewResult> CandidateInterviewResults { get; set; }
    }
}