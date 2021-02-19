using Evolutyz.Data;
using Evolutyz.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using System.IO;
using System.Web.Helpers;

namespace EvolutyzCorner.UI.Web.Controllers
{
    public class InterviewassessmentController : Controller
    {
        EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

     
        public void InitializeController(RequestContext context)
        {
            base.Initialize(context);
        }
        // GET: Interviewassessment
        public ActionResult Index()
        {
            return View();
        }
        public string Login(string Username, string Password)
        {
            try
            {
                var userid = Convert.ToInt32(Username);
                var pwd = GetMD5(Password);
                var result = db.InterviewCandidates.Where(x => x.ICID == userid && x.Password == pwd && x.status == true).Count();
                var result1 = db.InterviewCandidates.Where(x => x.ICID == userid && x.Password == pwd).Count();

                if (result > 0 )
                {
                    var resultdata = db.InterviewCandidates.Where(x => x.ICID == userid && x.Password == pwd && x.status == true).ToList().FirstOrDefault();
                    (from p in db.InterviewCandidates
                     where p.ICID == resultdata.ICID 
                     select p).ToList().ForEach(x =>
                     {
                         x.status = false;
                         db.SaveChanges();
                     });

                    Session["ExamUid"] = resultdata.ICID;
                    var data = Session["ExamUid"].ToString();
                    TempData["ExamUid"] = resultdata.ICID; ;
                    return "1";
                }
                else if(result1 > 0)
                {
                    return "3";
                }
                else
                {
                    return "2";
                }
               
            }
            catch(Exception)
            {
                throw;
            }
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");               
            }
            return byte2String;
        }

        public ActionResult getQuestion()
        {
            return View();
        }

        public ActionResult Assessment()
        {
            return View();
        }
        public List<QuestionsEntity> getAllQuestion(int xyz)
        {

            try
            {
                List<QuestionsEntity> Q = new List<QuestionsEntity>();
                List<QuestionsEntity> Q1 = new List<QuestionsEntity>();
                List<QuestionsEntity> Q2 = new List<QuestionsEntity>();
                List<QuestionsEntity> Q3 = new List<QuestionsEntity>();
                DataSet dataset = new DataSet();
                using (SqlConnection Con = new SqlConnection(str))
                {
                    using (SqlCommand cmd = new SqlCommand("GetallQuestions", Con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", xyz);

                        using (SqlDataAdapter ADP = new SqlDataAdapter(cmd))
                        {
                            ADP.Fill(dataset);
                        }
                    }
                }
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                if (dataset.Tables.Count > 0)
                {
                    dt = dataset.Tables["Table"];
                    if (dt.Rows.Count >= 1)
                    {

                        foreach (DataRow dr in dt.Rows)
                        {

                            Q.Add(new QuestionsEntity
                            {
                                
                                QBID = Convert.ToInt32(dr["QBID"]),
                                TechnologyStackId = Convert.ToInt32(dr["TechnologyStackId"]),
                                Question1 = dr["Question"].ToString(),
                                Option1 = dr["Option1"].ToString(),
                                Option2 = dr["Option2"].ToString(),
                                Option3 = dr["Option3"].ToString(),
                                Option4 = dr["Option4"].ToString(),
                            });

                        }
                    }
                    dt1 = dataset.Tables["Table2"];
                    if (dt1.Rows.Count >= 1)
                    {
                        foreach (DataRow dr1 in dt1.Rows)
                        {
                           Q1  = Q.Where(x => x.TechnologyStackId == Convert.ToInt32(dr1["TechnologyStackId"])).Take(Convert.ToInt32(dr1["No_of_Questions"])).ToList();
                            Q2.AddRange(Q1);
                        }
                    }
                }
                int i = 1;
                int j = 1;
                foreach(var data in Q2)
                {
                    Q3.Add(new QuestionsEntity
                    {
                        Number = "Q" + i++,
                        index = j++,
                        QBID = data.QBID,
                        TechnologyStackId = data.TechnologyStackId,
                        Question1 = data.Question1,
                        Option1 = data.Option1,
                        Option2 = data.Option2,
                        Option3 = data.Option3,
                        Option4 = data.Option4,
                    }); ;
                }

                return Q3;
            
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        public examuserdetails getAllUsers(int xyz)
        {

            try
            {
                examuserdetails Q = new examuserdetails();
                DataSet dataset = new DataSet();
                using (SqlConnection Con = new SqlConnection(str))
                {
                    using (SqlCommand cmd = new SqlCommand("GetallQuestions", Con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", xyz);

                        using (SqlDataAdapter ADP = new SqlDataAdapter(cmd))
                        {
                            ADP.Fill(dataset);
                        }
                    }
                }
                DataTable dt1 = new DataTable();
                if (dataset.Tables.Count > 0)
                {
               
                    dt1 = dataset.Tables["Table1"];
                    if (dt1.Rows.Count >= 1)
                    {

                        foreach (DataRow dr1 in dt1.Rows)
                        {

                            Q.QuestionCount = Convert.ToInt32(dr1["QuestionCount"]);
                            Q.Assessment_TimePeriod_in_sec = (Convert.ToInt32(dr1["Assessment_TimePeriod_in_sec"]) / 60);
                            Q.Name = dr1["Name"].ToString();
                            Q.InterviewForPositionname = dr1["InterviewForPositionname"].ToString();
                            Q.Actualtimeinsec = Convert.ToInt32(dr1["Assessment_TimePeriod_in_sec"]);
                        }
                    }
                }
                return Q;
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public string SaveResult(List<Obj> values)
        {
            try
            
            {
                var data = Session["ExamUid"].ToString();

                foreach (var value in values)
                {
                    string actualquestionno = (value.Actualquesno);
                    var QuestionId = Convert.ToInt32(actualquestionno);
                    var answer = db.QuestionBanks.Where(a => a.QBID == QuestionId).FirstOrDefault().Answer;
                    int? result = null;
                    if(value.Ans == null || value.Ans == "" || value.Ans == "0")
                    {
                        result = 2; 
                    }

                    else if (Convert.ToInt32(answer) == Convert.ToInt32(value.Ans))
                    {
                        result = 1;
                        
                    }
                    else if(Convert.ToInt32(answer) != Convert.ToInt32(value.Ans))
                    {
                        result = 0;
                    }
                    
                    db.CandidateInterviewResults.Add(new CandidateInterviewResult{
                       Candidateid = Convert.ToInt32(Session["ExamUid"]),
                       Questionid = Convert.ToInt32(value.Actualquesno),
                       Status = result,
                       SelectedOption = Convert.ToInt32(value.Ans),
                       CreatedDate = DateTime.Now,
                       CreatedBy = Convert.ToInt32(Session["ExamUid"]),
                       Descriptionforoptionchoosen = value.Description,
                    });
                    db.SaveChanges();
                }



                return "1";
            }
            catch(Exception)
            {
                throw;
            }

        }


        public ActionResult ThankYou()
        {
            try
            {
                return View();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}