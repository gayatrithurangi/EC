using Evolutyz.Data;
using Evolutyz.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EvolutyzCorner.UI.Web.Controllers
{
    public class InterviewQuestionsController : Controller
    {
        // GET: InterviewQuestions
        public ActionResult Index()
        {
            return View();
        }

        private CloudStorageAccount GetCloudBlobStorage()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("evolutyzcornerwebblob_AzureStorageConnectionString"));
            return storageAccount;
        }

        public ActionResult uploadPartial()
        {
            CloudStorageAccount storageAccount = GetCloudBlobStorage();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("images");
            var list = container.ListBlobs();
            List<imagesviewmodel> images = new List<imagesviewmodel>();
            var latestBlob = (container.ListBlobs().OfType<CloudBlockBlob>().OrderByDescending(m => m.Properties.LastModified).Take(20));

            foreach (var blobItem in latestBlob)
            {
                images.Add(
                    new imagesviewmodel
                    {
                        Url = Convert.ToString(blobItem.StorageUri.PrimaryUri),
                    });

            }
            return View(images);
        }

        public string uploadnow(HttpPostedFileWrapper upload)
        {
            string strResult = string.Empty;
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string imagename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(upload.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/uploadimages/images"), imagename);
                upload.SaveAs(path);
                CloudStorageAccount storageAccount = GetCloudBlobStorage();
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("images");
                var projectLocation = Server.MapPath("~/uploadimages/images/");
                string localPath = projectLocation;
                string sourceFile = imagename;
                CloudBlockBlob blob = container.GetBlockBlobReference(sourceFile);
                strResult = blob.StorageUri.PrimaryUri.ToString();
                TempData["imagename"] = strResult;
                using (var fileStream = System.IO.File.OpenRead(path))
                {
                    blob.UploadFromStream(fileStream);
                }
                var list = container.ListBlobs();
                var latestBlob = (container.ListBlobs().OfType<CloudBlockBlob>().OrderByDescending(m => m.Properties.LastModified));

            }
            return strResult;
        }
        public ActionResult AddQuestions()
        {
            var questiontypelist = TechnologyStachType().Select(a => new SelectListItem()
            {
                Value = a.TID.ToString(),
                Text = a.Title,
            });
            ViewBag.TID = questiontypelist;
            return View();
        }
        public ActionResult Edit(int QuestionID)
        {
            var questiontypelist = TechnologyStachType().Select(a => new SelectListItem()
            {
                Value = a.TID.ToString(),
                Text = a.Title,
            });
            ViewBag.TID = questiontypelist;

            TempData["QuestionID"] = QuestionID;
            QuestionsEntity question;
            try
            {
                question = EditQ(QuestionID);
              
                return View("AddQuestions", question);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult EditQuestion(QuestionsEntity ques)
        {
            string strResponse = string.Empty;
            try
            {
                //  ques.QuestionID = Convert.ToInt32(TempData["QuestionID"]);
                // int UniversalTopicid = Convert.ToInt32(TempData["UniversalTopicid"]);
                if (ModelState.IsValid)
                {
                   
                    strResponse = EditQues(ques);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("FetchQuestions");
        }

        public string EditQues(QuestionsEntity questions)
        {
            UserSessionInfo sessionInfo = new UserSessionInfo();
            int uid = sessionInfo.UserId;
            object input = null;
            QuestionBank qs = null;
            string strResponse = string.Empty;
            using (EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities())
            {
                qs = db.Set<QuestionBank>().Where(s => s.QBID == questions.Qid).FirstOrDefault<QuestionBank>();
                // change required details in disconnected mode (out of DBContext scope)
                if (qs != null)
                {
                    qs.QBID = questions.Qid;
                    qs.TechnologyStackId = questions.TechnologyStackId;
                    qs.Question = questions.Question1;
                    qs.Option1 = questions.Option1;
                    qs.Option2 = questions.Option2;
                    qs.Option3 = questions.Option3;
                    qs.Option4 = questions.Option4;
                    qs.Answer = questions.Answer;
                    qs.CreatedDate = System.DateTime.Now;
                    qs.ModifiedBy = uid;
                    qs.ModifiedDate = System.DateTime.Now;
                    // db.Entry(qs).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    strResponse = "Record successfully updated";
                }
                return strResponse;
            }
        }
        public QuestionsEntity EditQ(int QuestionID)
        {
            QuestionsEntity question;

            using (EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    question = (from q in db.QuestionBanks
                               
                                select new QuestionsEntity
                                {
                                   
                                    Question1 = q.Question,
                                    Option1 = q.Option1,
                                    Option2 = q.Option2,
                                    Option3 = q.Option3,
                                    Option4 = q.Option4,
                                    Answer = q.Answer,
                                    TechnologyStackId=q.TechnologyStackId,
                                    Qid=q.QBID
                                }).FirstOrDefault();
                    //(@"\\", "\\")
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return question;
        }
        public List<TechnologyStackEntity> TechnologyStachType()
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from c in db.TechnologyStacks
                                     //    //join s in db.Status on c.StatusID equals s.StatusID
                                     //where c.StatusID == (short)StatusEnum.Active
                                 select new TechnologyStackEntity
                                 {
                                     TID = c.TID,
                                     Title = c.Title
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult DispalyQuestions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Questions(QuestionsEntity questions)
        {

            string strResponse = string.Empty;
            try
            {
                strResponse = SaveQuestions(questions);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return View("Question","Admin");
            return RedirectToAction("FetchQuestions");

        }

        public ActionResult DisplayQuestions()
        {


            return View();
        }

        public JsonResult Questionsfortopics()
        {
            var Questions = DisplayTechnologyQuestions();
            Regex rx = new Regex("<[^>]*>");
            var result = new List<QuestionsEntity>();
            foreach (var item in Questions)
            {
                var v_entity = new QuestionsEntity();

                if (item.Question1 != null)
                    v_entity.Question1 = rx.Replace(item.Question1.Replace("\\r\\n", ""), "");
                if (item.Option1 != null)
                    v_entity.Option1 = rx.Replace(item.Option1.Replace("\\r\\n", ""), "");
                if (item.Option2 != null)
                    v_entity.Option2 = rx.Replace(item.Option2.Replace("\\r\\n", ""), "");
                if (item.Option3 != null)
                    v_entity.Option3 = rx.Replace(item.Option3.Replace("\\r\\n", ""), "");
                if (item.Option4 != null)
                    v_entity.Option4 = rx.Replace(item.Option4.Replace("\\r\\n", ""), "");
                if (item.Solution != null)
                    v_entity.Solution = rx.Replace(item.Solution.Replace("\\r\\n", ""), "");
                v_entity.Answer = item.Answer;
                v_entity.Title = item.Title;
                v_entity.QBID = item.Qid;
                result.Add(v_entity);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<QuestionsEntity> DisplayTechnologyQuestions()
        {
            List<QuestionsEntity> query = new List<QuestionsEntity>();
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    query = (from u in db.QuestionBanks
                             select new QuestionsEntity
                             {
                                 Question1 = u.Question,
                                 Option1 = u.Option1,
                                 Option2 = u.Option2,
                                 Option3 = u.Option3,
                                 Option4 = u.Option4,
                                 Title = (from em in db.TechnologyStacks.Where(x => x.TID == u.TechnologyStackId) select em.Title).FirstOrDefault(),
                                 Qid=u.QBID
                             }).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return query;
            }
        }

        public string SaveQuestions(QuestionsEntity questions)
        {
            UserSessionInfo objinfo = new UserSessionInfo();

            string strResponse = string.Empty;
            UserSessionInfo sessionInfo = new UserSessionInfo();
            //   int uid = sessionInfo.userid;
            using (EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    db.Set<QuestionBank>().Add(new QuestionBank
                    {

                        TechnologyStackId = questions.TechnologyStackId,
                        Question = questions.Question1,
                        Option1 = questions.Option1,
                        Option2 = questions.Option2,
                        Option3 = questions.Option3,
                        Option4 = questions.Option4,
                        Answer = questions.Answer,
                        CreatedDate = DateTime.Now,
                        CreatedBy = objinfo.UserId
                    });
                    db.SaveChanges();
                    strResponse = "Record Sucessfully Created ";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return strResponse;
        }
    }
}