using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace EvolutyzCorner.UI.Web.Controllers
{

    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class ClientController : Controller
    {
        int projectid;

        public ActionResult Index(int proid)

        {

            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {

                if (item.ModuleName == "Add User(Manager/Employee)")
                {
                    var mk = item.ModuleAccessType;
                    ViewBag.b = mk;
                }
                if (item.ModuleName == "Add Client Holidays")
                {
                    var mk = item.ModuleAccessType;
                    ViewBag.c = mk;
                }

                if (item.ModuleName == "Add Project/Client")
                {
                    var mk = item.ModuleAccessType;

                    ViewBag.a = mk;

                }
                if (item.ModuleName == "Project Task(Sub tasks)")
                {
                    var mk = item.ModuleAccessType;

                    ViewBag.d = mk;
                }
                if (item.ModuleName == "Associate Employee")
                {
                    var mk = item.ModuleAccessType;

                    ViewBag.e = mk;
                }
                if (item.ModuleName == "AddHoliday Calendar")
                {
                    var mk = item.ModuleAccessType;

                    ViewBag.f = mk;
                }
            }
            if (proid != null)
            {


                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int AccountId = _objSessioninfo.AccountId;
                int UserId = _objSessioninfo.UserId;
                ViewBag.AccountID = AccountId;
                ViewBag.UserId = UserId;
                bool? usaccount = _objSessioninfo.UsAccount;
                ViewBag.usaccount = usaccount;
                //dynamic id = TempData["mydata"];
                //ViewBag.id = id;
                TempData["proid"] = proid;
                ViewBag.id = proid;
                dynamic procode = TempData["projcode"];
                ViewBag.procode = procode;

                ClientComponent clientcomponent = new ClientComponent();
                var Gender = clientcomponent.GetGenders().Select(a => new SelectListItem()
                {
                    Value = a.Usr_GenderId.ToString(),
                    Text = a.Gender,

                });
                ViewBag.Gender = Gender;

                var accspecifictasks = clientcomponent.Getaccountspecifictasks().Select(a => new SelectListItem()
                {
                    Value = a.Acc_SpecificTaskId.ToString(),
                    Text = a.Acc_SpecificTaskName,

                });

                ViewBag.accspecifictasks = accspecifictasks;
                ViewBag.usertitle = clientcomponent.GetTitle().Select(a => new SelectListItem()
                {
                    Value = a.Usr_Titleid.ToString(),
                    Text = a.TitlePrefix,

                });
                ViewBag.countrycode = clientcomponent.GetCountryCodes().Select(a => new SelectListItem()
                {
                    Value = a.CountryId.ToString(),
                    Text = a.PhoneCode,

                });
                var countries = clientcomponent.GetCountryNames().Select(a => new SelectListItem()
                {
                    Value = a.CountryId.ToString(),
                    Text = a.CountryName,

                });
                ViewBag.countries = countries;
                var Timesheetmodes = clientcomponent.GetTimeSheetModes(AccountId).Select(a => new SelectListItem()
                {
                    Value = a.TimesheetMode_id.ToString(),
                    Text = a.TimeModeName,

                });
                ViewBag.Timesheetmodes = Timesheetmodes;
                var LeaveSchemeComponent = new LeaveSchemeComponent();
                var FinancialYears = LeaveSchemeComponent.Getallfinancialyears().Select(a => new SelectListItem()
                {
                    Value = a.FinancialYearId.ToString(),
                    Text = a.StartDate.ToString(),
                }).OrderByDescending(x => x.Value);
                ViewBag.FinancialYears = FinancialYears;
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();

                var client = (from em in db.Projects select em).Select(a => new SelectListItem()
                {

                    Value = a.Proj_ProjectID.ToString(),
                    Text = a.Proj_ProjectName,


                });

                ViewBag.client = client;
                var ProjectNames1 = clientcomponent.GetAllProjects(proid).Select(a => new SelectListItem()
                {
                    Value = a.clientprojId.ToString(),
                    Text = a.Proj_ProjectName,



                });
                ViewBag.ProjectNames1 = ProjectNames1;

                List<ProjectEntity> lst = new List<ProjectEntity>();
                var ProjectNames = clientcomponent.GetAllProjects(proid).Select(a => new { a.Proj_ProjectID, a.clientprojId, a.Proj_ProjectName }).ToList();

                ViewBag.ProjectNames = ProjectNames;

                return View();
            }
            else
            {
                return View();
            }
        }

        public JsonResult Getallfinancialyears()
        {
            List<FinancialYearEntity> finacialyear = null;

            var objDtl = new LeaveSchemeComponent();
            finacialyear = objDtl.Getallfinancialyears();

            return Json(finacialyear, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getclientforproject(int projid)
        {

            List<ClientEntity> ProjectDetails = null;

            var objDtl = new ClientComponent();
            ProjectDetails = objDtl.getclientprojectsdropdown(projid);

            return Json(ProjectDetails, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EditIndex(int proid)
        {
            int indexprojectid = proid;
            TempData["mydata"] = indexprojectid;
            return RedirectToAction("Index");

        }

        public string CreateClientProject(int? client, string clientprojecttitle, string clientprojdescription)
        {
            string strresponse = string.Empty;
            try
            {
                EvolutyzCornerDataEntities DB = new EvolutyzCornerDataEntities();

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int? AccountId = _objSessioninfo.AccountId;
                //Role orgCheck = db.Set<Role>().Where(s => (s.Rol_RoleCode == user.Rol_RoleCode && s.Rol_AccountID == user.Rol_AccountID)).FirstOrDefault<Role>();

                ClientProject checktitle = DB.Set<ClientProject>().Where(s => (s.ClientProjTitle == clientprojecttitle && s.Proj_ProjectID == client)).FirstOrDefault<ClientProject>();
                if (checktitle != null)
                {
                    return strresponse = "Project Name Already Existed";
                }
                DB.Set<ClientProject>().Add(new ClientProject
                {

                    Proj_ProjectID = client,
                    ClientProjDesc = clientprojdescription,
                    ClientProjTitle = clientprojecttitle,
                    Accountid = AccountId,

                });

                DB.SaveChanges();
                strresponse = "Successfully Project Added";
            }

            catch (Exception ex)
            {
                throw ex;

            }

            return strresponse;
        }
        public string updatecp(int client, string clientprojecttitle, string clientprojdescription, int accid, int projid)
        {
            string strresponse = string.Empty;
            try
            {
                EvolutyzCornerDataEntities DB = new EvolutyzCornerDataEntities();

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int? AccountId = _objSessioninfo.AccountId;


                //    DB.Set<ClientProject>().Add(new ClientProject
                //  {
                ClientProject checktitle = DB.Set<ClientProject>().Where(s => (s.ClientProjTitle == clientprojecttitle && s.Proj_ProjectID == projid && s.CL_ProjectID != client)).FirstOrDefault<ClientProject>();
                if (checktitle != null)
                {
                    return strresponse = "Project Name Already Existed";
                }
                ClientProject obj = new ClientProject();

                //  Proj_ProjectID = client,
                obj.Accountid = accid;
                obj.Proj_ProjectID = projid;

                obj.CL_ProjectID = client;
                obj.ClientProjDesc = clientprojdescription;
                obj.ClientProjTitle = clientprojecttitle;
                //Accountid = AccountId,

                //    });
                DB.Entry(obj).State = System.Data.Entity.EntityState.Modified;

                DB.SaveChanges();
                return strresponse = "Project successfully updated";
            }

            catch (Exception ex)
            {



            }

            return strresponse;
        }
        public JsonResult UpdateClientProject(int id)
        {
            ProjectAllocationEntities ProjectUserDetails = null;
            string strResponse = string.Empty;
            short UsTCurrentVersion = 0;
            try
            {

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = _objSessioninfo.UserId;

                var Org = new ClientComponent();


                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();

                ProjectUserDetails = Org.GetClientProjbyid(id);


            }
            catch (Exception ex)
            {

            }
            return Json(ProjectUserDetails, JsonRequestBehavior.AllowGet);
        }
        // [HttpGet]
        public JsonResult getclientprojects(int projid)
        {

            List<ClientEntity> ProjectDetails = null;

            var objDtl = new ClientComponent();
            ProjectDetails = objDtl.getclientprojects(projid);

            return Json(ProjectDetails, JsonRequestBehavior.AllowGet);
        }


        public string SequenceCode()
        {
            int getProjectCode;
            string mix = string.Empty;
            try
            {
                SqlConnection con = null;
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                con = new SqlConnection(constr);

                con.Open();
                SqlCommand cmd = new SqlCommand("select NEXT VALUE FOR ProjectCode", con);
                getProjectCode = Convert.ToInt16(cmd.ExecuteScalar());
                con.Close();
                string prefix = "PROJ";
                int i = getProjectCode;
                string s = i.ToString().PadLeft(5, '0');
                mix = prefix + " " + s;
                TempData["projcode"] = mix;
                return mix;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetManagerByRole(int projid, int userid)
        {
            List<UserEntity> l2Manger = null;
            UserSessionInfo objinfo = new UserSessionInfo();
            int AccountId = objinfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.GetManagerByRole(projid, userid, AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindManagerforProj(int CL_ProjectId, int userid)
        {
            List<UserEntity> l2Manger = null;
            UserSessionInfo objinfo = new UserSessionInfo();
            int AccountId = objinfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.BindManagerByProject(CL_ProjectId, userid, AccountId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetManagerOnChange(int projid, int ManagerID, int Client_ProjId)
        {
            List<UserEntity> l2Manger = null;
            UserSessionInfo objinfo = new UserSessionInfo();
            int AccountId = objinfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.GetManagerOnChange(projid, ManagerID, Client_ProjId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManager1onProjectChange(int CL_ProjectId)
        {
            List<UserEntity> l2Manger = null;
            UserSessionInfo objinfo = new UserSessionInfo();
            int AccountId = objinfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.GetManager1onProjectChange(CL_ProjectId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetL2Manager(int projid/*, int cl_projectid*/)
        {
            List<UserEntity> l2Manger = null;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.GetManagerNames2(projid/*, cl_projectid*/);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetL2ManagerforNewEmp(int projid)
        {
            List<UserEntity> l2Manger = null;
            try
            {
                var objDtl = new ClientComponent();
                l2Manger = objDtl.GetL2ManagerNamesforNewEmp(projid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l2Manger, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetL1Manager(int projid)
        {
            List<UserEntity> l1Manger = null;
            try
            {
                var objDtl = new ClientComponent();
                l1Manger = objDtl.GetManagerNames(projid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(l1Manger, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountries()
        {
            List<ProjectEntity> Countries = null;
            try
            {
                var objDtl = new ClientComponent();
                Countries = objDtl.GetCountryNames();

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(Countries, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStates(int CountryId)
        {
            List<ProjectEntity> States = null;
            try
            {
                var objDtl = new ClientComponent();
                States = objDtl.GetStateNames(CountryId);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(States, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoles()
        {
            List<OrganizationAccountEntity> UserRoles = null;
            UserSessionInfo sessionInfo = new UserSessionInfo();
            int accountid = sessionInfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetUserRolenames(accountid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllRoles()
        {
            List<OrganizationAccountEntity> UserRoles = null;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetAllRoles();

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllNewRoles()
        {
            List<OrganizationAccountEntity> UserRoles = null;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetAllNewRoles();

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRoleNamesbyemp()
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accountid = objinfo.AccountId;
            List<OrganizationAccountEntity> UserRoles = null;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetRoleNamesbyemp(accountid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAlltasknames(int projid, int Roleid)
        {
            List<ProjectAllocationEntities> TaskNames = null;
            try
            {
                var objDtl = new ClientComponent();
                TaskNames = objDtl.GetAlltasknames(projid, Roleid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(TaskNames, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmptasknames(int projid, int Roleid)
        {
            List<ProjectAllocationEntities> TaskNames = null;
            try
            {
                var objDtl = new ClientComponent();
                TaskNames = objDtl.GetAllEmptasknames(projid, Roleid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(TaskNames, JsonRequestBehavior.AllowGet);
        }
        public JsonResult gettasknames(int projid)
        {
            List<ProjectAllocationEntities> TaskNames = null;
            try
            {
                var objDtl = new ClientComponent();
                TaskNames = objDtl.gettasknames(projid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(TaskNames, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetallNewManagersList(int accid, int clientprjid)
        {
            List<UserEntity> users = null;
            try
            {
                var objDtl = new UserComponent();
                users = objDtl.GetallNewManagersList(accid, clientprjid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateClient(ProjectEntity ProjectDtl)
        {
            //int strResponse ;
            int ProjectId;
            try
            {
                var ProjectComponent = new ClientComponent();
                string code = SequenceCode();

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;

                int AccountId = _objSessioninfo.AccountId;
                int _userID = _objSessioninfo.UserId;
                ProjectDtl.Proj_ProjectCode = code;
                ProjectDtl.Proj_CreatedBy = _userID;
                ProjectDtl.Proj_AccountID = AccountId;

                var Org = new ClientComponent();
                var r = Org.AddProject(ProjectDtl);

            }

            catch (Exception ex)
            {
                return Json(ProjectDtl, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ProjectId = ProjectDtl.Proj_ProjectID }, JsonRequestBehavior.AllowGet);
        }

        public string UpdateClient(ProjectEntity project)
        {
            string strResponse = string.Empty;
            short UsTCurrentVersion = 0;
            try
            {

                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = _objSessioninfo.UserId;
                project.Proj_ModifiedBy = _userID;
                //while udating increment version by1
                project.Proj_Version = ++UsTCurrentVersion;
                // project.Proj_ActiveStatus = UsTCurrentStatus;
                var Org = new ClientComponent();
                int r = Org.UpdateclientDetails(project);
                if (r > 0)
                {
                    strResponse = "Client Updated Successfully";
                }
                else if (r == 0)
                {
                    strResponse = "Client Does Not Exists";
                }
                else if (r < 0)
                {
                    strResponse = "Please Fill All Mandatory Fields";
                }

            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                //lower  
                byte2String += targetData[i].ToString("x2");
                //upper  
                //byte2String += targetData[i].ToString("X2");  
            }
            return byte2String;
        }

        public string CreateManager([Bind(Exclude = "Ufp_UsersForProjectsID")] ProjectAllocationEntity ProjectDtl)
        {
            string strResponse = string.Empty;
            try
            {
                //var ProjectComponent = new ProjectAssignComponent();

                //if (ModelState.IsValid)
                //{
                UserSessionInfo _objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
                int _userID = _objSessioninfo.UserId;
                ProjectDtl.UProj_CreatedBy = _userID;

                var Org = new ClientComponent();
                int r = Org.AddManager(ProjectDtl);
                if (r > 0)
                {
                    strResponse = "User created successfully";
                }
                else if (r == 0)
                {
                    strResponse = "User already exists";
                }
                else if (r < 0)
                {
                    strResponse = "Error occured in CreateProjectAllocation";
                }
                // }
            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }

        public JsonResult GetClientByID(int catID)
        {
            ProjectEntity ProjectDetails = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectDetails = objDtl.GetClientDetailByID(catID);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string ManagerSave(ClientEntity formdata)
        {
            string fileExtension = string.Empty;
            var clicomponent = new ClientComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            formdata.UProj_CreatedBy = _userID;
            string imagename = string.Empty;
            int acid = objSessioninfo.AccountId;


            #region Old ImageUpload
            //if (Request.Files.Count > 0)
            //{
            //    var file = Request.Files[0];
            //    var fileName = "/uploadimages/Images/" + file.FileName;
            //    imagename = file.FileName;
            //    var imagepath = Server.MapPath(fileName);
            //    file.SaveAs(imagepath);
            //}
            //if (imagename == "")
            //{
            //    formdata.Usrp_ProfilePicture = "User.png";
            //}
            //else
            //{
            //    formdata.Usrp_ProfilePicture = imagename;
            //} 
            #endregion
            if (formdata.Usrp_ProfilePicture == "undefined")
            {
                formdata.Usrp_ProfilePicture = "User.png";
            }
            else if (formdata.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(formdata.imgCropped.Split(',')[1]);
                string sourceFile = "";
                string filePath = String.Empty;
                string base64 = formdata.imgCropped;
                filePath = "/uploadimages/Images/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                sourceFile = filePath.Replace("/uploadimages/Images/", "").ToString();
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                string physicalPath = Server.MapPath("/uploadimages/Images/" + sourceFile);
                var path = Path.Combine(Server.MapPath("~" + @"\upload\topic\"), sourceFile);
                fileExtension = Path.GetExtension(path).ToLower();
                imagename = sourceFile;
                formdata.Usrp_ProfilePicture = imagename;
            }
            else if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/Images/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
                formdata.Usrp_ProfilePicture = imagename;
            }




            //
            string Password = formdata.Usr_Password;
            formdata.Usr_Password = GetMD5(formdata.Usr_Password);
            string response = clicomponent.Savemanager(formdata);

            if (response == "Successfully Added")
            {
                if (formdata.IsDirectManager == true)
                {
                    EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                    string username = formdata.Email;
                    string password = Password;
                    string fname = formdata.UsrP_FirstName;
                    int roleid = Convert.ToInt32(formdata.RoleName);
                    string usrname = formdata.Usr_Username;
                    var profileimage = (from a in db.Accounts where a.Acc_AccountID == acid select a.Acc_CompanyLogo).FirstOrDefault();
                    SendEmailToresetpasswordForManager(username, password, fname, roleid, usrname, profileimage);
                }


            }

            return response;

            //return Json("", JsonRequestBehavior.AllowGet);
        }
        public static IRestResponse SendEmailToresetpassword(string username, string password, string fname, int roleid, string usrname, string profileimage)
        {

            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
            string MSstream = System.Configuration.ConfigurationManager.AppSettings["MSstreamURL"];


            string UrlEmailAddress = string.Empty;
            string MSstreamURL = string.Empty;

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
               
            }
            else
            {
                UrlEmailAddress = port1;

            }
            var emailcontent = "";
            emailcontent = "<html>" +
          "<head><meta charset='UTF-8'>" +
          "<title>Reset your password</title>" +
          "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
          "<meta content='width=device-width' name='viewport'>" +
          "<style type='text/css'> @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 600; font-style: normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 500; font-style: normal; src: local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 400; font-style: normal; src: local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); } </style> <style media='screen and (max-width: 680px)'> @media screen and (max-width: 680px) { .page-center { padding-left: 0 !important; padding-right: 0 !important; } .footer-center { padding-left: 20px !important; padding-right: 20px !important; } }</style>" +
          "</head>" +
          "<body style='background-color: #f4f4f5;'> " +
          "<table cellpadding='0' cellspacing='0' style='width: 100%; height: 100%; background-color: #f4f4f5; text-align: center;'>" +
          "<tbody><tr><td style='text-align: center;'> " +
          "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color: #fff; width: 100%; min-height: 100vh; padding: 15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position: 100% top; background-repeat: no-repeat; background-size: 100%; max-width: 680px; height: 100%;'> " +
          "<tbody> <tr> <td> <table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom: 88px; width: 100%; padding-left: 120px; padding-right: 120px;'> <tbody> <tr> <td style='padding-top: 24px;'>  <img src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + profileimage + "' style='width: 100px;'> " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td colspan='2'>" +
          "<span style='padding-top:60px;color: rgba(29, 25, 25, 0.7098039215686275);font-family:sans-serif;font-size: 19px;font-style:normal;font-weight:600;letter-spacing:-0.25px;line-height:35px;text-decoration:none;'>Hi " + fname + " ,</span>" +
          "<p style='-ms-text-size-adjust: 100%;-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: 100%;color: rgba(0, 0, 0, 0.75);font-family: sans-serif;font-size: 16px;font-smoothing: always;font-style: normal;font-weight: 500;letter-spacing: -0.25px;line-height: 20px;mso-line-height-rule: exactly;text-decoration: none;margin: 20px 0 0;text-indent: 40px;'>Welcome to Evolutyz family, please submit your timesheet in the Evolutyz Corner Portal and find your credentials</p>  " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style='padding-top:48px; padding-bottom:48px;'><table cellpadding='0' cellspacing='0' style='width: 100%'>" +
          "<tbody><tr><td style='width: 100%; height: 1px; max-height: 1px; background-color: #d9dbe0; opacity: 0.81'>" +
          "</td> " +
          "</tr> " +
          "</tbody> " +
          "</table> " +
          "</td>" +
          "</tr>" +         
          "<tr>" +
          "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none;vertical-align:top;width: 100%;'>" +
          "<table style='width: 100%;table-layout:fixed;'>" +
          "<tr>" +
          "<td>User Name :</td> " +
          "<td>" + usrname + "</td>" +
          " </tr>" +
          "<tr>" +
          "<td>Email Id :</td> " +
          "<td>" + username + "</td>" +
          " </tr>" +
          "<tr>" +
          "<td>Password :</td>" +
          "<td>" + password + "</td>" +
          "</tr>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style = 'padding-top: 24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to redirect to Evolutyz Corner. " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td>" +
          "<a data-click-track-id = '37' href = 'https://" + UrlEmailAddress + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Click here to Login</a>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td>" +
          "<a data-click-track-id = '37' href = 'https://" + MSstream + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Help Video Click here</a>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "<table align='center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width: 100%; max-width: 680px; height: 100%;'><tbody><tr><td><table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left: 120px; padding-right: 120px;'>" +
          " <tbody>" +
          " <tr>" +
          " <td colspan = '2' style='padding-top: 50px;padding-bottom: 15px;width: 100%;color: #f8c26c;font-size: 40px;'> Evolutyz Corner</td>" +
          "</tr>" +
          "<tr>" +
          "<td colspan = '2' style= 'padding-top: 24px; padding-bottom: 48px;' >" +
          "<table cellpadding= '0' cellspacing= '0' style= 'width: 100%'>" +
          "<tbody>" +
          "<tr>" +
          "<td style= 'width: 100%; height: 1px; max-height: 1px; background-color: #EAECF2; opacity: 0.19' >" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style= '-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size: 15px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: 0; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:noreplyk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight: 500; color: #ffffff' target='_blank'>Help Center</a>. " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style='height:72px;'>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</body>" +
          "</html>";

            var client = new SendGridClient("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");

            var msgs = new SendGridMessage()
            {
                From = new EmailAddress("noreply@evolutyz.com"),
                Subject = "Credentials for EvolutyzCorner Portal",
                //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                HtmlContent = emailcontent

            };
            msgs.AddTo(new EmailAddress(username));

            var responses = client.SendEmailAsync(msgs);
            return null;
        }

        public static IRestResponse SendEmailToresetpasswordForManager(string username, string password, string fname, int roleid, string usrname, string profileimage)
        {

            string host = System.Web.HttpContext.Current.Request.Url.Host;
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];
            string MSstream = System.Configuration.ConfigurationManager.AppSettings["MSstreamURL"];

            string MSstreamURL = string.Empty;
            string UrlEmailAddress = string.Empty;

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }
            var emailcontent = "";
            emailcontent = "<html>" +
          "<head><meta charset='UTF-8'>" +
          "<title>Reset your password</title>" +
          "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>" +
          "<meta content='width=device-width' name='viewport'>" +
          "<style type='text/css'> @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 600; font-style: normal; src: local(&#x27;Postmates Std Bold&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-bold.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 500; font-style: normal; src: local(&#x27;Postmates Std Medium&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-medium.woff) format(&#x27;woff&#x27;); } @font-face { font-family: &#x27;Postmates Std&#x27;; font-weight: 400; font-style: normal; src: local(&#x27;Postmates Std Regular&#x27;), url(https://s3-us-west-1.amazonaws.com/buyer-static.postmates.com/assets/email/postmates-std-regular.woff) format(&#x27;woff&#x27;); } </style> <style media='screen and (max-width: 680px)'> @media screen and (max-width: 680px) { .page-center { padding-left: 0 !important; padding-right: 0 !important; } .footer-center { padding-left: 20px !important; padding-right: 20px !important; } }</style>" +
          "</head>" +
          "<body style='background-color: #f4f4f5;'> " +
          "<table cellpadding='0' cellspacing='0' style='width: 100%; height: 100%; background-color: #f4f4f5; text-align: center;'>" +
          "<tbody><tr><td style='text-align: center;'> " +
          "<table align='center' cellpadding='0' cellspacing='0' id='body' style='background-color: #fff; width: 100%; min-height: 100vh; padding: 15px; background-image: url(https://csg33dda4407ebcx4721xba0.blob.core.windows.net/companylogos/hero-alt-long-trans.png); background-position: 100% top; background-repeat: no-repeat; background-size: 100%; max-width: 680px; height: 100%;'> " +
          "<tbody> <tr> <td> <table align='center' cellpadding='0' cellspacing='0' class='page-center' style='text-align: left; padding-bottom: 88px; width: 100%; padding-left: 120px; padding-right: 120px;'> <tbody> <tr> <td style='padding-top: 24px;'>  <img src='" + "https://" + UrlEmailAddress + "/uploadimages/images/thumb/" + profileimage + "' style='width: 100px;'> " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td colspan='2'>" +
          "<span style='padding-top:60px;color: rgba(29, 25, 25, 0.7098039215686275);font-family:sans-serif;font-size: 19px;font-style:normal;font-weight:600;letter-spacing:-0.25px;line-height:35px;text-decoration:none;'>Hi " + fname + " ,</span>" +
          "<p style='-ms-text-size-adjust: 100%;-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: 100%;color: rgba(0, 0, 0, 0.75);font-family: sans-serif;font-size: 16px;font-smoothing: always;font-style: normal;font-weight: 500;letter-spacing: -0.25px;line-height: 20px;mso-line-height-rule: exactly;text-decoration: none;margin: 20px 0 0;text-indent: 40px;'>Welcome to Evolutyz family, please submit your timesheet in the Evolutyz Corner Portal and find your credentials</p>  " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style='padding-top:48px; padding-bottom:48px;'><table cellpadding='0' cellspacing='0' style='width: 100%'>" +
          "<tbody><tr><td style='width: 100%; height: 1px; max-height: 1px; background-color: #d9dbe0; opacity: 0.81'>" +
          "</td> " +
          "</tr> " +
          "</tbody> " +
          "</table> " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style='-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family:  sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none;vertical-align:top;width: 100%;'>" +
          "<table style='width: 100%;table-layout:fixed;'>" +
          "<tr>" +
          "<td>User Name :</td> " +
          "<td>" + usrname + "</td>" +
          " </tr>" +
          "<tr>" +
          "<td>Email Id :</td> " +
          "<td>" + username + "</td>" +
          " </tr>" +
          "<tr>" +
          "<td>Password :</td>" +
          "<td>" + password + "</td>" +
          "</tr>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style = 'padding-top: 24px; -ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095a2; font-family: sans-serif; font-size: 16px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: -0.18px; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > Please tap the button below to redirect to Evolutyz Corner. " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td>" +
          "<a data-click-track-id = '37' href = 'https://" + UrlEmailAddress + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Click here to Login</a>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td>" +
          "<a data-click-track-id = '37' href = 'https://" + MSstream + "'style='margin-top:36px;-ms-text-size-adjust:100%;-ms-text-size-adjust:100%;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:100%;color:#ffffff;font-family:sans-serif;font-size:12px;font-smoothing:always;font-style:normal;font-weight:600;letter-spacing:0.7px;line-height:48px;mso-line-height-rule:exactly;text-decoration:none;vertical-align:top;width:220px;background-color:#795548;border-radius:28px;display:block;text-align:center;text-transform:uppercase'target='_blank'>Help Video Click here</a>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "<table align='center' cellpadding = '0' cellspacing = '0' id = 'footer' style = 'background-color: #3c3c3c; width: 100%; max-width: 680px; height: 100%;'><tbody><tr><td><table align = 'center' cellpadding = '0' cellspacing = '0' class='footer-center' style='text-align: left; width: 100%; padding-left: 120px; padding-right: 120px;'>" +
          " <tbody>" +
          " <tr>" +
          " <td colspan = '2' style='padding-top: 50px;padding-bottom: 15px;width: 100%;color: #f8c26c;font-size: 40px;'> Evolutyz Corner</td>" +
          "</tr>" +
          "<tr>" +
          "<td colspan = '2' style= 'padding-top: 24px; padding-bottom: 48px;' >" +
          "<table cellpadding= '0' cellspacing= '0' style= 'width: 100%'>" +
          "<tbody>" +
          "<tr>" +
          "<td style= 'width: 100%; height: 1px; max-height: 1px; background-color: #EAECF2; opacity: 0.19' >" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style= '-ms-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; color: #9095A2; font-family: sans-serif; font-size: 15px; font-smoothing: always; font-style: normal; font-weight: 400; letter-spacing: 0; line-height: 24px; mso-line-height-rule: exactly; text-decoration: none; vertical-align: top; width: 100%;' > If you have any questions or concerns, we are here to help. Contact us via our <a data-click-track-id='1053' href='mailto:noreplyk@evolutyz.in' title='helpdesk@evolutyz.in' style='font-weight: 500; color: #ffffff' target='_blank'>Help Center</a>. " +
          "</td>" +
          "</tr>" +
          "<tr>" +
          "<td style='height:72px;'>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</td>" +
          "</tr>" +
          "</tbody>" +
          "</table>" +
          "</body>" +
          "</html>";

            var client = new SendGridClient("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");

            var msgs = new SendGridMessage()
            {
                From = new EmailAddress("noreply@evolutyz.com"),
                Subject = "Credentials for EvolutyzCorner Portal",
                //TemplateId = "d-0741723a4269461e99a89e57a58dc0d3",
                HtmlContent = emailcontent

            };
            msgs.AddTo(new EmailAddress(username));

            var responses = client.SendEmailAsync(msgs);
            return null;
        }


        public class UploadImage
        {
            public static void Crop(int Width, int Height, Stream streamImg, string saveFilePath)
            {
                Bitmap sourceImage = new Bitmap(streamImg);
                using (Bitmap objBitmap = new Bitmap(Width, Height))
                {
                    objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                    using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                    {
                        // Set the graphic format for better result cropping   
                        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        objGraphics.DrawImage(sourceImage, 0, 0, Width, Height);

                        // Save the file path, note we use png format to support png file   
                        objBitmap.Save(saveFilePath);
                    }
                }
            }
        }

        public string SaveEmployee(ClientEntity ProjectDtl)
        {
            string fileExtension = string.Empty;
            var clicomponent = new ClientComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            int acid = objSessioninfo.AccountId;
            ProjectDtl.UProj_CreatedBy = _userID;
            string imagename = string.Empty;


            #region Old Upload
            //if (Request.Files.Count > 0)
            //{
            //    var file = Request.Files[0];
            //    var fileName = "/uploadimages/Images/" + file.FileName;
            //    imagename = file.FileName;
            //    var imagepath = Server.MapPath(fileName);
            //    file.SaveAs(imagepath);
            //    UploadImage.Crop(320, 240, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/Images/thumb/") + file.FileName));

            //}
            //if (imagename == "")
            //{
            //    ProjectDtl.Usrp_ProfilePicture = "User.png";
            //}
            //else
            //{
            //    ProjectDtl.Usrp_ProfilePicture = imagename;
            //}
            // ProjectDtl.Usrp_ProfilePicture = imagename; 
            #endregion


            if (ProjectDtl.Usrp_ProfilePicture == "undefined")
            {
                ProjectDtl.Usrp_ProfilePicture = "User.png";
            }
            else if (ProjectDtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(ProjectDtl.imgCropped.Split(',')[1]);
                string sourceFile = "";
                string filePath = String.Empty;
                string base64 = ProjectDtl.imgCropped;
                filePath = "/uploadimages/Images/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                sourceFile = filePath.Replace("/uploadimages/Images/", "").ToString();
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                string physicalPath = Server.MapPath("/uploadimages/Images/" + sourceFile);
                var path = Path.Combine(Server.MapPath("~" + @"\upload\topic\"), sourceFile);
                fileExtension = Path.GetExtension(path).ToLower();
                imagename = sourceFile;
                ProjectDtl.Usrp_ProfilePicture = imagename;
            }
            else if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/Images/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
                ProjectDtl.Usrp_ProfilePicture = imagename;
            }
            string Password = ProjectDtl.Usr_Password;
            ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);

            string response = clicomponent.SaveEmployee(ProjectDtl);
            if (response == "Successfully Added")
            {
                EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
                string username = ProjectDtl.Email;
                string password = Password;
                string fname = ProjectDtl.UsrP_FirstName;
                int roleid = Convert.ToInt32(ProjectDtl.RoleName);
                string usrname = ProjectDtl.Usr_Username;
                var profileimage = (from a in db.Accounts where a.Acc_AccountID == acid select a.Acc_CompanyLogo).FirstOrDefault();
                SendEmailToresetpassword(username, password, fname, roleid, usrname, profileimage);

            }
            return response;

        }

        public JsonResult GetProjectAllocationCollection(int id)
        {
            List<ProjectAllocationEntities> ProjectDetails = null;
            try
            {
                ClientComponent objDtl = new ClientComponent();
                ProjectDetails = objDtl.GetProjectUserDetails(id);
                TempData["projectdata"] = ProjectDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GEtprojectdetails(int id)
        {
            List<ProjectAllocationEntities> ProjectDetails = null;
            try
            {
                ClientComponent objDtl = new ClientComponent();
                ProjectDetails = objDtl.GetProjectUserDetails(id);
                TempData["projectdata"] = ProjectDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserDetailById(int id, string proid, int CL_ProjID)
        {
            ProjectAllocationEntities ProjectUserDetails = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectUserDetails = objDtl.GetUserDetailById(id, proid, CL_ProjID);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectUserDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetassUserDetailById(int id)
        {
            ProjectAllocationEntities ProjectUserDetails = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectUserDetails = objDtl.GetassUserDetailById(id);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectUserDetails, JsonRequestBehavior.AllowGet);
        }

        public string updateuserprojectbyid(ClientEntity ProjectDtl)
        {
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            var clicomponent = new ClientComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            ProjectDtl.UProj_CreatedBy = _userID;
            string imagename = string.Empty;

            if (ProjectDtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(ProjectDtl.imgCropped.Split(',')[1]);
                try
                {
                    string sourceFile = "";
                    string filePath = String.Empty;
                    string base64 = ProjectDtl.imgCropped;
                    filePath = "/uploadimages/Images/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                    sourceFile = filePath.Replace("/Upload/grade/", "").ToString();
                    using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }

                    imagename = sourceFile;
                    ProjectDtl.Usrp_ProfilePicture = imagename;
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            else
            {
                if (Request.Files.Count > 0)
                {
                    var mediaFile = Request.Files[0];



                    if (mediaFile != null)
                    {

                        var fileName = "/uploadimages/Images/" + mediaFile.FileName;
                        imagename = mediaFile.FileName;
                        var imagepath = Server.MapPath(fileName);
                        mediaFile.SaveAs(imagepath);
                    }



                }
                else
                {
                    imagename = ProjectDtl.Usrp_ProfilePicture;


                }
            }



            ProjectDtl.Usrp_ProfilePicture = imagename;
            var em = (from p in db.Users where p.Usr_UserID == ProjectDtl.Usr_UserID select p.Usr_Password).FirstOrDefault();
            if (ProjectDtl.Usr_Password == null)
            {
                ProjectDtl.Usr_Password = em;
            }
            else if (ProjectDtl.Usr_Password != em)
            {

                ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);

            }
            else
            {
                ProjectDtl.Usr_Password = em;
            }
            //if (ProjectDtl.Usr_Password == null)
            //{
            //    ProjectDtl.Usr_Password = "";
            //}
            //else
            //{
            //    ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);
            //}

            string response = clicomponent.updateuserdetails(ProjectDtl);

            return response;
        }

        public string DeleteUser(int userid)
        {
            string strResponse = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var Org = new ClientComponent();
                    int r = Org.DeleteUser(userid);

                    if (r > 0)
                    {
                        strResponse = "User deleted successfully";
                    }
                    else if (r == 0)
                    {
                        strResponse = "User does not exists";
                    }
                    else if (r < 0)
                    {
                        strResponse = "Error occured in DeleteProject";
                    }
                }
            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }

        public JsonResult GetHolidays(int projectid)
        {
            List<ProjectAllocationEntities> holidaylist = new List<ProjectAllocationEntities>();
            var clientcomponent = new ClientComponent();

            holidaylist = clientcomponent.GetHolidays(projectid);
            return Json(holidaylist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getprotasks(int projectid)
        {
            List<ProjectAllocationEntities> tasklist = new List<ProjectAllocationEntities>();
            var clientcomponent = new ClientComponent();

            tasklist = clientcomponent.Getprotasks(projectid);
            return Json(tasklist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string Addprotasks(string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            string strResponse = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            int accountid = objinfo.AccountId;
            var clientcomponent = new ClientComponent();

            if (ModelState.IsValid)
            {
                strResponse = clientcomponent.Addprotasks(Acc_SpecificTaskName, ProjectID, Proj_SpecificTaskName, RTMId, Actual_StartDate, Actual_EndDate, Plan_StartDate, Plan_EndDate, StatusId);
            }
            return strResponse;
        }

        [HttpPost]
        public string updatetasks(int id, string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            string strResponse = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            int accountid = objinfo.AccountId;
            var clientcomponent = new ClientComponent();

            if (ModelState.IsValid)
            {
                strResponse = clientcomponent.updatetasks(id, Acc_SpecificTaskName, ProjectID, Proj_SpecificTaskName, RTMId, Actual_StartDate, Actual_EndDate, Plan_StartDate, Plan_EndDate, StatusId);
            }
            return strResponse;
        }

        public JsonResult getprojecttaskbyid(int id)
        {
            List<ProjectAllocationEntities> tasklist = new List<ProjectAllocationEntities>();
            var clientcomponent = new ClientComponent();

            tasklist = clientcomponent.getprojecttaskbyid(id);
            return Json(tasklist, JsonRequestBehavior.AllowGet);
        }

        public string DeleteProjecttask(string id)
        {
            string response = string.Empty;
            ClientComponent compobj = new ClientComponent();
            response = compobj.DeleteProjecttask(id);
            return response;
        }

        public JsonResult Associatemanagers(int projectid)
        {
            UserSessionInfo info = new UserSessionInfo();
            int accountid = info.AccountId;
            ClientComponent component = new ClientComponent();
            List<ProjectAllocationEntities> managerlist = new List<ProjectAllocationEntities>();
            managerlist = component.Associatemanagers(projectid, accountid);
            return Json(managerlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssociateEmployees(int projectid)
        {
            UserSessionInfo info = new UserSessionInfo();
            int accountid = info.AccountId;
            ClientComponent component = new ClientComponent();
            List<ProjectAllocationEntities> managerlist = new List<ProjectAllocationEntities>();
            managerlist = component.AssociateEmployees(projectid, accountid);
            return Json(managerlist, JsonRequestBehavior.AllowGet);
        }

        public string AssociateEmployee(ClientEntity ProjectDtl)
        {
            var clicomponent = new ClientComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            ProjectDtl.UProj_CreatedBy = _userID;
            string imagename = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/Images/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
            }
            ProjectDtl.Usrp_ProfilePicture = imagename; if (ProjectDtl.Usr_Password == null)
            {
                ProjectDtl.Usr_Password = "";
            }
            else
            {
                ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);
            }
            ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);

            string response = clicomponent.AssociateEmployee(ProjectDtl);
            return response;

        }

        public string AssociateManager(ClientEntity ProjectDtl)
        {
            var clicomponent = new ClientComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            ProjectDtl.UProj_CreatedBy = _userID;
            string imagename = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/Images/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
            }
            ProjectDtl.Usrp_ProfilePicture = imagename; if (ProjectDtl.Usr_Password == null)
            {
                ProjectDtl.Usr_Password = "";
            }
            else
            {
                ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);
            }
            ProjectDtl.Usr_Password = GetMD5(ProjectDtl.Usr_Password);

            string response = clicomponent.AssociateManager(ProjectDtl);
            return response;

        }

        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            var objDtl = new ClientComponent();
            strResponse = objDtl.ChangeStatus(id, status);

            return strResponse;
        }


        #region kalyani

        [HttpPost]
        public string AddSelectedManager(ClientEntity formdata)
        {
            var clicomponent = new ClientComponent();
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            formdata.UProj_CreatedBy = _userID;
            string imagename = string.Empty;
            // var getTimeSheetId = db.UserProjects.Where(a => a.UProj_UserID == formdata.Usr_UserID).Select(t => t.TimesheetMode).FirstOrDefault();

            //formdata.TimesheetMode_id = (int)getTimeSheetId;
            string response = clicomponent.AddSelectedManager(formdata);



            return response;



            //return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManagerDetailByMId(int selectedManager)
        {
            ProjectAllocationEntities ProjectManagerDetails = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectManagerDetails = objDtl.GetManagerDetailByMId(selectedManager);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectManagerDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getManagersBySelectProject(int ProjectId, int CL_ProjId, int AccountId)
        {

            List<UserEntity> ProjectDtls = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectDtls = objDtl.getManagersBySelectProject(ProjectId, CL_ProjId, AccountId);



            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectDtls, JsonRequestBehavior.AllowGet);

        }
        public ActionResult getEmployeesBySelectProject(int ProjectId, int AccountId, int CL_ProjId)
        {

            List<UserEntity> ProjectDtls = null;
            try
            {
                var objDtl = new ClientComponent();
                ProjectDtls = objDtl.getEmployeesBySelectProject(ProjectId, AccountId, CL_ProjId);



            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(ProjectDtls, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public string AddSelectedEmployee(ClientEntity formdata)
        {
            var clicomponent = new ClientComponent();
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            formdata.UProj_CreatedBy = _userID;
            string imagename = string.Empty;
            // var getTimeSheetId = db.UserProjects.Where(a => a.UProj_UserID == formdata.Usr_UserID).Select(t => t.TimesheetMode).FirstOrDefault();

            //formdata.TimesheetMode_id = (int)getTimeSheetId;
            string response = clicomponent.AddSelectedEmployee(formdata);



            return response;



            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult bindManagersForNewEmp(int accid, int prjid, int c_prjid)
        {
            List<UserEntity> users = null;
            //List<ManagerEntity> users = null;

            try
            {
                var objDtl = new UserComponent();
                users = objDtl.bindManagersForNewEmp(accid, prjid, c_prjid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoleNamesbyemployee()
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accountid = objinfo.AccountId;
            List<OrganizationAccountEntity> UserRoles = null;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetRoleNamesbyemployee(accountid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEmpRoles()
        {
            List<OrganizationAccountEntity> UserRoles = null;
            UserSessionInfo sessionInfo = new UserSessionInfo();
            int accountid = sessionInfo.AccountId;
            try
            {
                var objDtl = new ClientComponent();
                UserRoles = objDtl.GetEmpRolenames(accountid);

            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(UserRoles, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public string DeleteProjectnotAssigned(int CL_ProjectID,int ProjectId)
        {
            string strResponse = string.Empty;
            var objDtl = new ClientComponent();
            strResponse = objDtl.DeleteProjectsData(CL_ProjectID,ProjectId);
            return strResponse;
        }
        
    }
}