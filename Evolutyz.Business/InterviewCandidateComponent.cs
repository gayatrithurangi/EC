using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evolutyz.Entities;
using Evolutyz.Data;

namespace Evolutyz.Business
{
    public class InterviewCandidateComponent
    {
      



        public List<InterviewCandidateEntity> PostInterviewCandidateDetails()
        {
            throw new NotImplementedException();
        }
  


        #region kalyani   To  Get AllInterviewPositionNames

        public List<InterviewCandidateEntity> GetAllInterviewPositionNames()
        {
            var ent = new InterviewCandidateDac();
            return ent.GetAllInterviewPositionNames();
        }

        #endregion   

        public List<InterviewCandidateEntity> GetInterviewCandidatesList()
        {
            var list = new InterviewCandidateDac();
            return list.GetInterviewCandidatesList();
        }

        public InterviewCandidateEntity GetCandidateByICID(int ICID)
        {
            var candidateDac = new InterviewCandidateDac();
            return candidateDac.GetCandidateByICID(ICID);
        }


        public string UpdateInterviewCandidates(string id, string FirstName, string LastName, string Email,string MobileNumber, string InterviewForPositionname, string AssignmentDate, string AssignmentTime)
        {
            var candidateDac = new InterviewCandidateDac();
            return candidateDac.UpdateInterviewCandidates(id, FirstName, LastName, Email, MobileNumber, InterviewForPositionname, AssignmentDate, AssignmentTime);
        }

        public int AddIntCandidate(InterviewCandidateEntity IntCandidateDetails)
        {
            var candidateDac = new InterviewCandidateDac();
            return candidateDac.AddIntCandidate(IntCandidateDetails);
        }

    }
}

