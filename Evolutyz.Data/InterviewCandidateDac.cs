using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;


namespace Evolutyz.Data
{
    public class InterviewCandidateDac : DataAccessComponent
    {
        //public List<InterviewCandidateEntity> GetInterviewCandidateDetails()
        //{
        //    List<InterviewCandidateEntity> result = new List<InterviewCandidateEntity>();

        //    using (var db = new EvolutyzCornerDataEntities())
        //    {
        //        try
        //        {

        //            result = (from q in db.InterviewCandidates
        //                      select new InterviewCandidateEntity
        //                      {
        //                          ICID = q.ICID,
        //                          AssignmentDate = q.AssessmentDate,
        //                          Assessment_For_Positionid = q.Assessment_For_Positionid,
        //                          InterviewForPositionname = q.Assessment_For_Position.InterviewForPositionname,
        //                          Createdby = q.CreatedBy,
        //                          CreatedDate = q.CreatedDate,
        //                          Email = q.Emailid,
        //                          FirstName = q.FirstName,
        //                          LastName = q.LastName,
        //                          MobileNumber = q.MobileNo,
        //                          Modifiedby = q.ModifiedBy,
        //                          ModifiedDate = q.ModifiedDate,
        //                          Password = q.Password,
        //                          RecrutementUserid = q.RecrutementUserid,
        //                          status = q.status


        //                      }).ToList();


        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    return result;
        //}

        //public InterviewCandidateEntity AddInterviewCandidateDetailById(int ICID)
        //{
        //    InterviewCandidateEntity result = new InterviewCandidateEntity();

        //    using (var db = new EvolutyzCornerDataEntities())
        //    {
        //        try
        //        {
        //            result = (from q in db.InterviewCandidates
        //                      select new InterviewCandidateEntity
        //                      {
        //                          ICID = q.ICID,
        //                          AssignmentDate = q.AssessmentDate,
        //                          Assessment_For_Positionid = q.Assessment_For_Positionid,
        //                          InterviewForPositionname = q.Assessment_For_Position.InterviewForPositionname,
        //                          Createdby = q.CreatedBy,
        //                          CreatedDate = q.CreatedDate,
        //                          Email = q.Emailid,
        //                          FirstName = q.FirstName,
        //                          LastName = q.LastName,
        //                          MobileNumber = q.MobileNo,
        //                          Modifiedby = q.ModifiedBy,
        //                          ModifiedDate = q.ModifiedDate,
        //                          Password = q.Password,
        //                          RecrutementUserid = q.RecrutementUserid,
        //                          status = q.status


        //                      }).FirstOrDefault();

        //            result.IsSuccessful = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccessful = false;
        //            result.Message = "Error Occured in GetInterviewCandidateDetailById(ID)";
        //            result.Detail = ex.Message.ToString();

        //        }
        //    }
        //    return result;
        //}

        //public InterviewCandidateEntity UpdateInterviewCandidateDetailById(int ICID)
        //{
        //    InterviewCandidateEntity result = new InterviewCandidateEntity();

        //    using (var db = new EvolutyzCornerDataEntities())
        //    {
        //        try
        //        {
        //            result = (from q in db.InterviewCandidates
        //                      select new InterviewCandidateEntity
        //                      {
        //                          ICID = q.ICID,
        //                          AssignmentDate = q.AssessmentDate,
        //                          Assessment_For_Positionid = q.Assessment_For_Positionid,
        //                          InterviewForPositionname = q.Assessment_For_Position.InterviewForPositionname,
        //                          Createdby = q.CreatedBy,
        //                          CreatedDate = q.CreatedDate,
        //                          Email = q.Emailid,
        //                          FirstName = q.FirstName,
        //                          LastName = q.LastName,
        //                          MobileNumber = q.MobileNo,
        //                          Modifiedby = q.ModifiedBy,
        //                          ModifiedDate = q.ModifiedDate,
        //                          Password = q.Password,
        //                          RecrutementUserid = q.RecrutementUserid,
        //                          status = q.status


        //                      }).FirstOrDefault();

        //            result.IsSuccessful = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.IsSuccessful = false;
        //            result.Message = "Error Occured in GetInterviewCandidateDetailById(ID)";
        //            result.Detail = ex.Message.ToString();

        //        }
        //    }
        //    return result;
        //}

        public int DeleteInterviewCandidateDetailById(int ICID)
        {
            int retVal = 0;
            InterviewCandidate _interviewDtl = null;

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    _interviewDtl = db.Set<InterviewCandidate>().Where(s => s.ICID == ICID).FirstOrDefault<InterviewCandidate>();
                    if (_interviewDtl == null)
                    {
                        return retVal;
                    }
                    _interviewDtl.status = false;
                    db.Entry(_interviewDtl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
            }
            return retVal;
        }




        #region kalyani

        public List<InterviewCandidateEntity> GetAllInterviewPositionNames()
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from AP in db.Assessment_For_Position
                                    
                                 select new InterviewCandidateEntity
                                 {
                                     APID = AP.APID,
                                     InterviewForPositionname = AP.InterviewForPositionname,

                                 }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion  


        public List<InterviewCandidateEntity> GetInterviewCandidatesList()
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var query = (from q in db.InterviewCandidates
                                 join d in db.Assessment_For_Position on q.Assessment_For_Positionid equals d.APID
                                 select new InterviewCandidateEntity
                                 {
                                     ICID = q.ICID,
                                     FirstName = q.FirstName,
                                     LastName = q.LastName,
                                     Email = q.Emailid,
                                     MobileNumber = q.MobileNo,
                                     Password = q.Password,
                                     Assessment_For_Positionid = d.APID,
                                     InterviewForPositionname = d.InterviewForPositionname,
                                     RecrutementUserid = q.RecrutementUserid,
                                     AssignmentDate = q.AssessmentDate.ToString(),
                                     AssignmentTime =  q.AssessmentTime,
                                     status = q.status

                                 }).ToList();

                    List<InterviewCandidateEntity> i = new List<InterviewCandidateEntity>();
                    UserSessionInfo info = new UserSessionInfo();
                    var sessionUsrId = info.UserId;
                    foreach (var data in query)
                    {
                        var x = Convert.ToDateTime(data.AssignmentDate).ToString("dd/MM/yyyy") + " " + data.AssignmentTime.ToString();
                        var Z = db.CandidateInterviewResults.Where(a => a.Candidateid == data.ICID && a.Status == 1).Count();
                        var Y = db.CandidateInterviewResults.Where(b => b.Candidateid == data.ICID).Count();
                        var FullName = data.FirstName + " " + data.LastName;
                        var getRecname = (from u in db.Users where u.Usr_UserID == data.RecrutementUserid select u.Usr_Username).FirstOrDefault();
                        var SessionId = (from u in db.Users where u.Usr_UserID == sessionUsrId select u.Usr_UserID).FirstOrDefault();

                        // var R = db.CandidateInterviewResults.Where(a => a.Candidateid == data.RecrutementUserid).Count();

                        i.Add(new InterviewCandidateEntity
                        {


                            ICID = data.ICID,
                            FirstName = data.FirstName,
                            LastName = data.LastName,
                            Email = data.Email,
                            MobileNumber = data.MobileNumber,
                            Password = data.Password,
                            Assessment_For_Positionid = data.Assessment_For_Positionid,
                            InterviewForPositionname = data.InterviewForPositionname,
                            RecrutementUserid =data.RecrutementUserid,
                            AssignmentDate = x,
                           Result = (Z + "/" + Y),
                           CandidateName = FullName,
                           Recruiter = getRecname,
                           SessionUsr = SessionId,
                           status = data.status,
                        });

                        
                    }

                    return i;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public InterviewCandidateEntity GetCandidateByICID(int ICID)
        {
            InterviewCandidateEntity response = new InterviewCandidateEntity();
           
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    response = (from q in db.InterviewCandidates
                                join d in db.Assessment_For_Position on q.Assessment_For_Positionid equals d.APID
                                where q.ICID == ICID
                                select new InterviewCandidateEntity
                                {

                                    ICID = q.ICID,
                                    FirstName = q.FirstName,
                                    LastName = q.LastName,
                                    Email = q.Emailid,
                                    MobileNumber = q.MobileNo,
                                    Password = q.Password,
                                    Assessment_For_Positionid = d.APID,
                                    InterviewForPositionname = d.InterviewForPositionname,
                                    RecrutementUserid = q.RecrutementUserid,
                                    AssignmentDate = q.AssessmentDate.ToString(),
                                    AssignmentTime = q.AssessmentTime,
                                    APID = d.APID
                                    


                                }).FirstOrDefault();


                    response.IsSuccessful = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.IsSuccessful = false;
                    response.Message = "Error Occured in GetCandidateByICID(ICID)";
                    response.Detail = ex.Message.ToString();
                    return response;
                }
            }
        }


        public string UpdateInterviewCandidates(string id, string FirstName, string LastName, string Email,string MobileNumber, string InterviewForPositionname, string AssignmentDate, string AssignmentTime)
        {
            
            InterviewCandidate Candidatedetails = null;
            UserSessionInfo info = new UserSessionInfo();
            int ICID = Convert.ToInt32(id);
            //string date = AssignmentDate.ToString();
            DateTime date = Convert.ToDateTime(AssignmentDate);
            int userid = info.UserId;
            int acid = info.AccountId;
            string strResponse = string.Empty;
           // bool Status = Convert.ToBoolean(status);
            //if (status == "True")
            //{
            //    statusid = 1;
            //}
            //else
            //{
            //    statusid = 0;
            //}
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    Candidatedetails = db.Set<InterviewCandidate>().Where(s => s.ICID == ICID).FirstOrDefault<InterviewCandidate>();
                    //Department FirstName = db.Set<Department>().Where(s => (s.DepartmentName == DepartmentName && s.isDeleted == false)).FirstOrDefault<Department>();
                  //  TicketsAuthority chckusrid = db.Set<TicketsAuthority>().Where(s => (s.UserId == UserId && s.Status == false)).FirstOrDefault<TicketsAuthority>();

                    if (Candidatedetails == null)
                    {
                        return null;
                    }
                    //if (chckusrid != null)
                    //{
                    //    return "usrid Already Exists";
                    //}
                    //holidayDetails.AccountID = holiday.AccountID;
                    Candidatedetails.FirstName = FirstName;
                    Candidatedetails.LastName = LastName;
                    Candidatedetails.Emailid = Email;
                    Candidatedetails.MobileNo = MobileNumber ;
                    Candidatedetails.Assessment_For_Positionid = Convert.ToInt32(InterviewForPositionname);
                    Candidatedetails.AssessmentTime = AssignmentTime;
                    Candidatedetails.AssessmentDate = date;
                    Candidatedetails.ModifiedBy = userid;
                    Candidatedetails.ModifiedDate = System.DateTime.Now;

                    db.Entry(Candidatedetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    strResponse = "Candidatedetails successfully updated";

                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
                return strResponse;
            }

        }

        public int AddIntCandidate(InterviewCandidateEntity IntCandidateDetails)
        {
            int retVal = 0;
            //User User = new User();
            //User UserName = new User();
            //User UserName = new User();
            int IPID = Convert.ToInt32(IntCandidateDetails.InterviewForPositionname);
            DateTime date = DateTime.ParseExact(IntCandidateDetails.AssignmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            UserSessionInfo info = new UserSessionInfo();
            int Userid = info.UserId;
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    
                        db.Set<InterviewCandidate>().Add(new InterviewCandidate

                        {

                            FirstName = IntCandidateDetails.FirstName,
                            LastName = IntCandidateDetails.LastName,
                            Emailid = IntCandidateDetails.Email,
                            MobileNo = IntCandidateDetails.MobileNumber,
                            Password = IntCandidateDetails.Password,
                            Assessment_For_Positionid = IPID,
                            RecrutementUserid = Userid,
                            AssessmentDate = date,
                            AssessmentTime = IntCandidateDetails.AssignmentTime,
                            CreatedDate = System.DateTime.Now,
                            CreatedBy = Userid,

                        });
                        db.SaveChanges();
                       
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
            }
            return retVal;
        }

    }
}
