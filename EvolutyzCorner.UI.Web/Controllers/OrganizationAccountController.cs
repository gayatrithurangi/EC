using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Evolutyz.Entities;
using Evolutyz.Business;

using System.IO;
using System.Web.Configuration;
using System.Drawing;


//
namespace EvolutyzCorner.UI.Web.Controllers
{

    [Authorize]
    [EvolutyzCorner.UI.Web.MvcApplication.SessionExpire]
    [EvolutyzCorner.UI.Web.MvcApplication.NoDirectAccess]
    public class OrganizationAccountController : Controller
    {
        public ActionResult Index()
        {

            List<LookupStatusDetail> objStatusList = new List<LookupStatusDetail>();
            objStatusList.Add(new LookupStatusDetail { StatusID = 1, Status = "Active" });
            objStatusList.Add(new LookupStatusDetail { StatusID = 0, Status = "InActive" });

            var objStList = from cl in objStatusList
                            orderby cl.StatusID
                            select new
                            {
                                value = cl.StatusID,
                                text = cl.Status
                            };
            ViewBag.Status = objStList;
            UserSessionInfo info = new UserSessionInfo();
            ViewBag.Roleid = info.RoleName;

            HomeController hm = new HomeController();
            var obj = hm.GetAdminMenu();
            foreach (var item in obj)
            {

                if (item.ModuleName == "Add Account")
                {
                    var mk = item.ModuleAccessType;


                    ViewBag.a = mk;

                }

            }


            return View();
        }




        public ActionResult showAllRecords()
        {
            List<OrganizationAccountEntity> AccDetails = null;
            OrganizationAccountEntity currentAccount = new OrganizationAccountEntity();
            try
            {
                if (ViewBag.currentRecord != null)
                {
                    currentAccount = ViewBag.currentRecord;
                    ViewBag.currentRecord = null;
                }
                else
                {

                    var objDtl = new OrganizationAccountComponent();
                    AccDetails = objDtl.GetOrganizationAccounts();
                    currentAccount = AccDetails.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return View(currentAccount);
        }

        public ActionResult BrowseAccountRecords(int id, string navigation)
        {
            OrganizationAccountEntity currentRecord = null;
            try
            {
                var objDtl = new OrganizationAccountComponent();
                currentRecord = objDtl.BrowseAccountRecords(id, navigation);
                ViewBag.currentRecord = currentRecord;
            }
            catch (Exception ex)
            {
                return null;
            }

            return View("showAllRecords");
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


        public string CreateOrganizationAccount(OrganizationAccountEntity AccDtl)
        {
            string strResponse = string.Empty;
            string fileExtension = string.Empty;
            var orgcomponent = new OrganizationAccountComponent();
            string imgcrop = AccDtl.imgCropped;
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;
            AccDtl.Acc_CreatedBy = _userID;
            string imagename = string.Empty;
            if (AccDtl.Acc_CompanyLogo == "undefined")
            {
                AccDtl.Acc_CompanyLogo = "User.png";
            }
            else if (AccDtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(AccDtl.imgCropped.Split(',')[1]);
                string sourceFile = "";
                string filePath = String.Empty;
                string base64 = AccDtl.imgCropped;
                filePath = "/uploadimages/Images/thumb/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                sourceFile = filePath.Replace("/uploadimages/Images/thumb/", "").ToString();
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                string physicalPath = Server.MapPath("/uploadimages/Images/thumb/" + sourceFile);
                var path = Path.Combine(Server.MapPath("~" + @"\upload\topic\"), sourceFile);
                fileExtension = Path.GetExtension(path).ToLower();
                imagename = sourceFile;
                AccDtl.Acc_CompanyLogo = imagename;
            }
            else if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/images/thumb/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
                //UploadImage.Crop(100, 100, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/images/thumb/") + file.FileName));
            }
            if (imagename == "")
            {
                AccDtl.Acc_CompanyLogo = "User.png";
            }
            else
            {
                AccDtl.Acc_CompanyLogo = imagename;
            }

            #region MyRegion
            //string imagename = string.Empty;
            //string base64 = AccDtl.imgCropped;
            // if (Request.Files.Count > 0)
            //{
            //    var file = Request.Files[0];
            //    var fileName = "/uploadimages/images/" + file.FileName;
            //    imagename = file.FileName;
            //    var imagepath = Server.MapPath(fileName);
            //    file.SaveAs(imagepath);
            //    UploadImage.Crop(100, 100, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/images/thumb/") + file.FileName));
            //  //  UploadImage.Crop( 240,180, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/Images/thumb/") + file.FileName));
            //}
            //if (imagename == "")
            //{
            //    AccDtl.Acc_CompanyLogo = "User.png";
            //}
            //else
            //{
            //    AccDtl.Acc_CompanyLogo = imagename;
            //} 
            #endregion

            int r = orgcomponent.AddOrganizationAccount(AccDtl);
            if (r == 1)
            {
                strResponse = "Account created successfully";
            }
            else if (r == 0)
            {
                strResponse = "Account already exists";
            }
            else if (r == -1)
            {
                strResponse = "Please Fill Mandatory Fields";
            }
            else if (r == 2)
            {
                strResponse = "AccountName already exists";
            }
            else if (r == 3)
            {
                strResponse = "AccountCode already exists";
            }
            //else if (r == 4)
            //{
            //    strResponse = "AccountCode already exists";
            //}
            return strResponse;

        }

        public string UpdateOrganizationAccount(OrganizationAccountEntity accdtl)
        {
            string fileExtension = string.Empty;
            var orgcomponent = new OrganizationAccountComponent();
            UserSessionInfo objSessioninfo = Session["UserSessionInfo"] as UserSessionInfo;
            int _userID = objSessioninfo.UserId;

            accdtl.Acc_ModifiedBy = _userID;
            string imagename = string.Empty;

            #region MyRegion
            //if (accdtl.imgCropped != "undefined")
            //{
            //    byte[] bytes = Convert.FromBase64String(accdtl.imgCropped.Split(',')[1]);
            //    string sourceFile = "";
            //    string filePath = String.Empty;
            //    string base64 = accdtl.imgCropped;
            //    filePath = "/uploadimages/Images/thumb/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            //    sourceFile = filePath.Replace("/uploadimages/Images/thumb/", "").ToString();
            //    using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
            //    {
            //        stream.Write(bytes, 0, bytes.Length);
            //        stream.Flush();
            //    }
            //    string physicalPath = Server.MapPath("/uploadimages/Images/thumb/" + sourceFile);

            //    imagename = sourceFile;
            //    accdtl.Acc_CompanyLogo = imagename;
            //}
            //else if (Request.Files.Count > 0)
            //{


            //    var file = Request.Files[0];
            //    //if (file != null)
            //    //{
            //        var fileName = "/uploadimages/Images/thumb/" + file.FileName;
            //        imagename = file.FileName;
            //        var imagepath = Server.MapPath(fileName);
            //        file.SaveAs(imagepath);
            //    //}
            //    //else
            //    //{
            //    //    imagename = accdtl.Acc_CompanyLogo;
            //    //}

            //    //  UploadImage.Crop(240,180, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/Images/thumb/") + file.FileName));
            //    //UploadImage.Crop(100, 100, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/images/thumb/") + file.FileName));
            //}

            //else
            //{
            //    accdtl.Acc_CompanyLogo = "User.png";

            //} 
            #endregion

            if (accdtl.Acc_CompanyLogo == "undefined")
            {
                accdtl.Acc_CompanyLogo = "User.png";
            }
            else if (accdtl.imgCropped != "undefined")
            {
                byte[] bytes = Convert.FromBase64String(accdtl.imgCropped.Split(',')[1]);
                string sourceFile = "";
                string filePath = String.Empty;
                string base64 = accdtl.imgCropped;
                filePath = "/uploadimages/Images/thumb/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                sourceFile = filePath.Replace("/uploadimages/Images/thumb/", "").ToString();
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                string physicalPath = Server.MapPath("/uploadimages/Images/thumb/" + sourceFile);
                var path = Path.Combine(Server.MapPath("~" + @"\upload\topic\"), sourceFile);
                fileExtension = Path.GetExtension(path).ToLower();
                imagename = sourceFile;
                accdtl.Acc_CompanyLogo = imagename;
            }
            else if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = "/uploadimages/images/thumb/" + file.FileName;
                imagename = file.FileName;
                var imagepath = Server.MapPath(fileName);
                file.SaveAs(imagepath);
                //UploadImage.Crop(100, 100, file.InputStream, Path.Combine(Server.MapPath("~/uploadimages/images/thumb/") + file.FileName));
            }
           
            else
            {
                imagename = accdtl.Acc_CompanyLogo;
            }

            accdtl.Acc_CompanyLogo = imagename;
            string response = orgcomponent.UpdateOrganizationAccount(accdtl);
            return response;

        }




        public string DeleteOrganizationAccount(int accID)
        {
            string strResponse = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var Org = new OrganizationAccountComponent();
                    int r = Org.DeleteOrganizationAccount(accID);
                    if (r > 0)
                    {
                        strResponse = "Account deleted successfully";
                    }
                    else if (r == 0)
                    {
                        strResponse = "Account does not exists";
                    }
                    else if (r < 0)
                    {
                        strResponse = "Error occured in DeleteOrganizationAccount";
                    }
                }
            }
            catch (Exception ex)
            {
                return strResponse;
            }
            return strResponse;
        }

        public JsonResult GetOrganizationAccountCollection()
        {
            List<OrganizationAccountEntity> AccDetails = null;
            try
            {
                var objDtl = new OrganizationAccountComponent();
                AccDetails = objDtl.GetOrganizationAccounts();
                ViewBag.OrgAccDetails = AccDetails[0].Acc_Version;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(AccDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganizationAccountByID(int catID)
        {
            OrganizationAccountEntity AccDetails = null;
            try
            {
                var objDtl = new OrganizationAccountComponent();
                AccDetails = objDtl.GetOrganizationAccountByID(catID);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(AccDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganizationHistoryAccountByID(int catID)
        {
            List<HistoryOrganizationAccountEntity> HisAccDetails = null;
            try
            {
                var objDtl = new OrganizationAccountComponent();
                HisAccDetails = objDtl.GetOrganizationHistoryAccountsByID(catID);
                //ViewBag.OrgAccDetails = HisAccDetails[0].History_Acc_Version;
                ViewBag.OrgAccDetails = HisAccDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(HisAccDetails, JsonRequestBehavior.AllowGet);
        }


        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            var objDtl = new OrganizationAccountComponent();
            strResponse = objDtl.ChangeStatus(id, status);

            return strResponse;
        }


    }
}