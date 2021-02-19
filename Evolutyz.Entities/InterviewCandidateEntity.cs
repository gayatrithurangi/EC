using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Evolutyz.Entities
{
    public class InterviewCandidateEntity : ResponseHeader
    {
        [Key]
        public int ICID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public int? Assessment_For_Positionid { get; set; }
        public int? RecrutementUserid { get; set; }
        //public int? RecutedUserid { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool? status { get; set; }
        public int APID { get; set; }
        public string InterviewForPositionname { get; set; }
        public string Result { get; set; }
        public string CandidateName { get; set; }
        public string Recruiter { get; set; }
        public System.DateTime ADate { get; set; }
        public int SessionUsr { get; set; }

    }

    public class Assessment_For_PositionEntity : ResponseHeader
    {

        public int APID { get; set; }
        public string InterviewForPositionname { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }



    }
    


}
