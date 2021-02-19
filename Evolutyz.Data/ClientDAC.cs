using Evolutyz.Entities;
using evolCorner.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Evolutyz.Data
{
    public class ClientDAC : DataAccessComponent
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        #region GetcountryNamesForDropdown

        public List<ProjectEntity> GetCountryNames()
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var countryNames = (from c in db.Countries
                                        where c.StatusId == true
                                        select new ProjectEntity
                                        {
                                            CountryId = c.CountryId,
                                            CountryName = c.CountryName

                                        }).ToList();

                    //countryNames.Add(new ProjectEntity
                    //{
                    //    CountryId = 0,
                    //    CountryName = "Select Countries"
                    //});
                    return countryNames;
                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion


        public List<ProjectEntity> GetTimeSheetModes(int AccountId)
        {

            using (var db = new EvolutyzCornerDataEntities())
            {
                bool OrgCheck = db.Set<Account>().Where(s => s.Acc_AccountID == AccountId).FirstOrDefault<Account>().is_UsAccount;
                if (OrgCheck == true)
                {
                    try
                    {
                        var timesheetmodes = (from c in db.Master_TimesheetMode
                                                  //where c.TimesheetMode_id != 3
                                              select new ProjectEntity
                                              {
                                                  TimesheetMode_id = c.TimesheetMode_id,
                                                  TimeModeName = c.TimeModeName

                                              }).ToList();


                        return timesheetmodes;
                    }
                    catch (Exception ex)
                    {
                        return null;

                    }
                }
                else
                {
                    try
                    {
                        var timesheetmodes = (from c in db.Master_TimesheetMode

                                              select new ProjectEntity
                                              {
                                                  TimesheetMode_id = c.TimesheetMode_id,
                                                  TimeModeName = c.TimeModeName

                                              }).ToList();


                        return timesheetmodes;
                    }
                    catch (Exception ex)
                    {
                        return null;

                    }
                }


            }

        }


        #region GetStateNamesForDropdown

        public List<ProjectEntity> GetStateNames(int CountryId)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var StateNames = (from s in db.States
                                      where s.Countryid == CountryId && s.StatusId == true
                                      select new ProjectEntity
                                      {
                                          StateId = s.StateId,
                                          StateName = s.StateName

                                      }).ToList();

                    //StateNames.Add(new ProjectEntity
                    //{
                    //     StateId = 0,
                    //    StateName= "Select TaskName"
                    //});
                    return StateNames;
                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

        #region To add Project Detail in Database

        public ProjectEntity AddProject(ProjectEntity _project)
        {
            int Proj_ProjectID;
            int retVal = 0;
            ClientDAC dac = new ClientDAC();
            Project Project = new Project();
            ProjectEntity response = new ProjectEntity();
            response.Proj_ProjectID = 0;

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    Project = db.Set<Project>().Where(s => (s.Proj_ProjectName == _project.Proj_ProjectName && s.Proj_AccountID == _project.Proj_AccountID)).FirstOrDefault<Project>();
                    if (Project != null)
                    {
                        return response;
                    }

                    db.Set<Project>().Add(new Project
                    {
                        Proj_AccountID = _project.Proj_AccountID,
                        Proj_ProjectCode = _project.Proj_ProjectCode,
                        Proj_ProjectName = _project.Proj_ProjectName,
                        CountryID = _project.CountryId,
                        StateID = _project.StateId,
                        WebUrl = _project.WebUrl,
                        Proj_ProjectDescription = _project.Proj_ProjectDescription,
                        Plan_StartDate = System.DateTime.Now,
                        Plan_EndDate = System.DateTime.Now,
                        //Proj_ActiveStatus = _project.Proj_ActiveStatus,
                        Proj_Version = _project.Proj_Version,
                        Is_Timesheet_ProjectSpecific = _project.Is_Timesheet_ProjectSpecific,
                        Proj_CreatedBy = _project.Proj_CreatedBy,
                        Proj_CreatedDate = System.DateTime.Now,
                        Proj_isDeleted = _project.Proj_isDeleted
                    });

                    retVal = db.SaveChanges();

                    Proj_ProjectID = db.Set<Project>().OrderByDescending(p => p.Proj_ProjectID).Select(p => p.Proj_ProjectID).FirstOrDefault();
                    _project.Proj_ProjectID = Proj_ProjectID;
                    response.Message = "Success";
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    response.Proj_ProjectID = -1;
                }
            }
            return _project;
        }
        #endregion

        public ProjectAllocationEntities GetClientProjbyid(int ID)
        {
            // int projectid = Convert.ToInt32(proid);
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    //var response = (from u in db.Users
                    //                join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                    //                join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                    //                join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                    //                join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                    //                join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID

                    var response = (from u in db.ClientProjects
                                    where u.CL_ProjectID == ID
                                    select new ProjectAllocationEntities
                                    {
                                        // AccountID = u.Accountid;
                                        Accid = u.Accountid,
                                        ProjectID = u.Proj_ProjectID,

                                        Cl_projid = u.CL_ProjectID,
                                        clientprojecttitle = u.ClientProjTitle,
                                        clientprojectdescription = u.ClientProjDesc

                                    }).FirstOrDefault();
                    return response;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public string updateuserdetails(ClientEntity orgDtl)
        {
            User userdtl = null;
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            var accountid = objinfo.AccountId;
            int rolename = Convert.ToInt32(orgDtl.RoleName);
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    userdtl = db.Set<User>().Where(s => s.Usr_UserID == orgDtl.Usr_UserID).FirstOrDefault<User>();

                    string password = db.Set<User>().Where(r => (r.Usr_UserID == orgDtl.Usr_UserID)).FirstOrDefault<User>().Usr_Password;
                    var UserName = db.Set<User>().Where(s => (s.Usr_Username == orgDtl.Usr_Username.Trim() && s.Usr_UserID != orgDtl.Usr_UserID && s.Usr_isDeleted == false)).FirstOrDefault<User>();
                    var LoginId = db.Set<User>().Where(s => (s.Usr_LoginId == orgDtl.Email.Trim() && s.Usr_UserID != orgDtl.Usr_UserID && s.Usr_isDeleted == false)).FirstOrDefault<User>();
                    // Role roleid = new Role();
                    //int roleid = db.Set<Role>().Where(r => (r.Rol_AccountID == accountid && r.Rol_RoleName == rolename)).FirstOrDefault<Role>().Rol_RoleID;
                    if (UserName != null)
                    {
                        return response = "UserName already Exists";
                    }
                    else if (LoginId != null)
                    {
                        return response = "Loginid already Exists";
                    }
                    if (userdtl == null)
                    {
                        response = "User Details not updated";
                        return null;
                    }
                    if (orgDtl.Usr_Password == "")
                    {
                        password = password;
                    }
                    else
                    {
                        password = orgDtl.Usr_Password;
                    }
                    if (orgDtl.Usrp_ProfilePicture == "undefined")
                    {
                        //int roleid = db.Set<Role>().Where(r => (r.Rol_AccountID == accountid && r.Rol_RoleName == rolename)).FirstOrDefault<Role>().Rol_RoleID;

                        User u = new User();

                        userdtl.Usr_AccountID = accountid;
                        userdtl.Usr_UserTypeID = orgDtl.Usr_UserTypeID;
                        userdtl.Usr_RoleID = rolename;
                        userdtl.Usr_Username = orgDtl.Usr_Username;
                        userdtl.Usr_LoginId = orgDtl.Email.Trim();
                        userdtl.Usr_Password = password;
                        //userdtl.Usr_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userdtl.Usr_TaskID = orgDtl.Usr_TaskID;
                        userdtl.Usr_CreatedBy = orgDtl.UProj_CreatedBy;
                        userdtl.Usr_CreatedDate = DateTime.Now;
                        userdtl.Usr_Version = 1;
                        userdtl.Usr_isDeleted = orgDtl.Usr_isDeleted;
                        db.Entry(userdtl).State = System.Data.Entity.EntityState.Modified;
                        //db.Set<User>().Add(u);
                        db.SaveChanges();

                        int userid = u.Usr_UserID;

                        UsersProfile userprofiledtl = new UsersProfile();
                        userprofiledtl = db.Set<UsersProfile>().Where(s => s.UsrP_UserID == orgDtl.Usr_UserID).FirstOrDefault<UsersProfile>();
                        //userprofiledtl.UsrP_UserID = orgDtl.Usr_UserID;
                        userprofiledtl.UsrP_FirstName = orgDtl.UsrP_FirstName;
                        userprofiledtl.UsrP_LastName = orgDtl.UsrP_LastName;
                        userprofiledtl.UsrP_EmailID = orgDtl.Email.Trim();
                        //userprofiledtl.Usrp_ProfilePicture = orgDtl.Usrp_ProfilePicture;
                        userprofiledtl.Usrp_DOJ = orgDtl.Usrp_DOJ;
                        userprofiledtl.UsrP_EmployeeID = orgDtl.UsrP_EmployeeID;
                        userprofiledtl.Usrp_MobileNumber = orgDtl.Usrp_MobileNumber;
                        userprofiledtl.Usrp_CountryCode = orgDtl.Usrp_CountryCode;
                        userprofiledtl.Usr_Titleid = orgDtl.Usr_Titleid;
                        userprofiledtl.Usr_GenderId = orgDtl.Usr_GenderId;
                        userprofiledtl.UsrP_CreatedBy = orgDtl.UProj_CreatedBy;
                        userprofiledtl.UsrP_CreatedDate = DateTime.Now;
                        //userprofiledtl.UsrP_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userprofiledtl.UsrP_Version = 1;
                        db.Entry(userprofiledtl).State = System.Data.Entity.EntityState.Modified;
                        // db.Set<UsersProfile>().Add(uf);
                        db.SaveChanges();

                        UserProject userprojectdtl = new UserProject();
                        userprojectdtl = db.Set<UserProject>().Where(s => (s.UProj_UserID == orgDtl.Usr_UserID && s.UProj_ProjectID == orgDtl.Proj_ProjectID)).FirstOrDefault<UserProject>();
                        //userprojectdtl.UProj_UserID = userid;
                        userprojectdtl.UProj_ProjectID = orgDtl.Proj_ProjectID;
                        userprojectdtl.ClientprojID = orgDtl.CL_ProjectID;
                        userprojectdtl.UProj_L1_ManagerId = orgDtl.ManagerName;
                        userprojectdtl.UProj_L2_ManagerId = orgDtl.Managername2;
                        if (orgDtl.Leadformanager != null)
                        {
                            userprojectdtl.UProj_L1_ManagerId = orgDtl.Leadformanager;
                        }
                        else
                        {
                            userprojectdtl.UProj_L1_ManagerId = orgDtl.ManagerName;
                        }

                        userprojectdtl.UProj_StartDate = System.DateTime.Now; ;
                        userprojectdtl.UProj_EndDate = System.DateTime.Now; ;
                        userprojectdtl.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                        //userprojectdtl.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userprojectdtl.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                        userprojectdtl.UProj_CreatedDate = DateTime.Now;
                        userprojectdtl.UProj_Version = 1;
                        userprojectdtl.UProj_isDeleted = orgDtl.Usr_isDeleted;
                        userprojectdtl.TimesheetMode = orgDtl.TimesheetMode_id;
                        userprojectdtl.IsDirectManager = orgDtl.IsDirectManager;
                        userprojectdtl.isclientcalendar = orgDtl.isclientholidays;
                        //userprojectdtl.Is_L1_Manager = true;
                        //db.Set<UserProject>().Add(up);
                        db.Entry(userprojectdtl).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        response = "Successfully updated";
                    }
                    else
                    {
                        //int roleid = db.Set<Role>().Where(r => (r.Rol_AccountID == accountid && r.Rol_RoleName == rolename)).FirstOrDefault<Role>().Rol_RoleID;

                        User u = new User();
                        userdtl.Usr_AccountID = accountid;
                        userdtl.Usr_UserTypeID = orgDtl.Usr_UserTypeID;
                        userdtl.Usr_RoleID = rolename;
                        userdtl.Usr_Username = orgDtl.Email.Trim();
                        userdtl.Usr_LoginId = orgDtl.Email.Trim();
                        userdtl.Usr_Password = password;
                        userdtl.Usr_Username = orgDtl.Usr_Username;
                        //userdtl.Usr_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userdtl.Usr_TaskID = orgDtl.Usr_TaskID;
                        userdtl.Usr_CreatedBy = orgDtl.UProj_CreatedBy;
                        userdtl.Usr_CreatedDate = DateTime.Now;
                        userdtl.Usr_Version = 1;
                        userdtl.Usr_isDeleted = orgDtl.Usr_isDeleted;
                        db.Entry(userdtl).State = System.Data.Entity.EntityState.Modified;
                        //db.Set<User>().Add(u);
                        db.SaveChanges();

                        int userid = u.Usr_UserID;

                        UsersProfile userprofiledtl = new UsersProfile();
                        userprofiledtl = db.Set<UsersProfile>().Where(s => s.UsrP_UserID == orgDtl.Usr_UserID).FirstOrDefault<UsersProfile>();
                        //userprofiledtl.UsrP_UserID = userid;
                        userprofiledtl.UsrP_FirstName = orgDtl.UsrP_FirstName;
                        userprofiledtl.UsrP_LastName = orgDtl.UsrP_LastName;
                        userprofiledtl.UsrP_EmailID = orgDtl.Email.Trim();
                        userprofiledtl.Usrp_ProfilePicture = orgDtl.Usrp_ProfilePicture;
                        userprofiledtl.Usrp_DOJ = DateTime.Now;
                        userprofiledtl.UsrP_EmployeeID = orgDtl.UsrP_EmployeeID;
                        userprofiledtl.Usrp_MobileNumber = orgDtl.Usrp_MobileNumber;
                        userprofiledtl.Usrp_CountryCode = orgDtl.Usrp_CountryCode;
                        userprofiledtl.Usr_Titleid = orgDtl.Usr_Titleid;
                        userprofiledtl.Usr_GenderId = orgDtl.Usr_GenderId;
                        userprofiledtl.UsrP_CreatedBy = orgDtl.UProj_CreatedBy;
                        userprofiledtl.UsrP_CreatedDate = DateTime.Now;
                        //userprofiledtl.UsrP_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userprofiledtl.UsrP_Version = 1;
                        db.Entry(userprofiledtl).State = System.Data.Entity.EntityState.Modified;
                        // db.Set<UsersProfile>().Add(uf);
                        db.SaveChanges();

                        UserProject userprojectdtl = new UserProject();
                        userprojectdtl = db.Set<UserProject>().Where(s => s.UProj_UserID == orgDtl.Usr_UserID).FirstOrDefault<UserProject>();
                        //userprojectdtl.UProj_UserID = userid;
                        userprojectdtl.UProj_ProjectID = orgDtl.Proj_ProjectID;
                        userprojectdtl.ClientprojID = orgDtl.CL_ProjectID;
                        userprojectdtl.UProj_L1_ManagerId = orgDtl.ManagerName;
                        userprojectdtl.UProj_L2_ManagerId = orgDtl.Managername2;
                        if (orgDtl.Leadformanager != null)
                        {
                            userprojectdtl.UProj_L1_ManagerId = orgDtl.Leadformanager;
                        }
                        else
                        {
                            userprojectdtl.UProj_L1_ManagerId = orgDtl.ManagerName;
                        }

                        ///userprojectdtl.UProj_L1_ManagerId = orgDtl.Leadformanager;
                        userprojectdtl.UProj_StartDate = System.DateTime.Now;
                        userprojectdtl.UProj_EndDate = System.DateTime.Now;
                        userprojectdtl.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                        //userprojectdtl.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        userprojectdtl.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                        userprojectdtl.UProj_CreatedDate = DateTime.Now;
                        userprojectdtl.UProj_Version = 1;
                        userprojectdtl.UProj_isDeleted = orgDtl.Usr_isDeleted;
                        userprojectdtl.TimesheetMode = orgDtl.TimesheetMode_id;
                        userprojectdtl.IsDirectManager = orgDtl.IsDirectManager;
                        userprojectdtl.isclientcalendar = orgDtl.isclientholidays;
                        //userprojectdtl.Is_L1_Manager = true;
                        db.Entry(userprojectdtl).State = System.Data.Entity.EntityState.Modified;
                        //db.Set<UserProject>().Add(up);
                        db.SaveChanges();
                        objinfo.Usrp_ProfilePicture = orgDtl.Usrp_ProfilePicture;
                        response = "Successfully updated";
                    }

                    // Role roleid = new Role();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }


        #region To update existing client Detail in Database
        public int UpdateclientDetails(ProjectEntity project)
        {
            Project projDTl = null;
            // History_UserType _userTypeHistory = new History_UserType();

            int retVal = 0;

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    projDTl = db.Set<Project>().Where(s => s.Proj_ProjectID == project.Proj_ProjectID).FirstOrDefault<Project>();

                    if (project == null)
                    {
                        return retVal;
                    }


                    // projDTl.Proj_AccountID = project.Proj_AccountID;
                    // projDTl.Proj_ProjectCode = project.Proj_ProjectCode;
                    projDTl.Proj_ProjectName = project.Proj_ProjectName;
                    projDTl.CountryID = project.CountryId;
                    projDTl.StateID = project.StateId;
                    projDTl.WebUrl = project.WebUrl;
                    projDTl.Proj_ProjectDescription = project.Proj_ProjectDescription;
                    projDTl.Plan_StartDate = System.DateTime.Now;
                    projDTl.Plan_EndDate = System.DateTime.Now;
                    //projDTl.Proj_ActiveStatus = project.Proj_ActiveStatus;
                    projDTl.Proj_Version = project.Proj_Version;
                    projDTl.Proj_CreatedBy = project.Proj_CreatedBy;
                    projDTl.Proj_CreatedDate = System.DateTime.Now;
                    projDTl.Is_Timesheet_ProjectSpecific = project.Is_Timesheet_ProjectSpecific;
                    projDTl.Proj_ModifiedDate = System.DateTime.Now;
                    projDTl.Proj_ModifiedBy = project.Proj_ModifiedBy;
                    projDTl.Proj_isDeleted = project.Proj_isDeleted;

                    db.Entry(projDTl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
                return retVal;
            }
        }
        #endregion
        #region To add ProjectAllocation Detail in Database
        public int AddManager(ProjectAllocationEntity _ProjectAllocation)
        {
            int retVal = 0;
            UserProject ProjectAllocation = new UserProject();

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    ProjectAllocation = db.Set<UserProject>().Where(s => s.UProj_UserProjectID == ProjectAllocation.UProj_UserProjectID).FirstOrDefault<UserProject>();

                    if (ProjectAllocation != null)
                    {
                        return retVal;
                    }

                    #region Saving ProjectAllocation info Table

                    db.Set<UserProject>().Add(new UserProject
                    {

                        UProj_ProjectID = _ProjectAllocation.UProj_ProjectID,
                        UProj_UserID = _ProjectAllocation.UProj_UserID,
                        UProj_ParticipationPercentage = Convert.ToByte(_ProjectAllocation.UProj_ParticipationPercentage),
                        UProj_StartDate = Convert.ToDateTime(_ProjectAllocation.UProj_StartDate),
                        UProj_EndDate = _ProjectAllocation.UProj_EndDate,
                        UProj_ActiveStatus = _ProjectAllocation.UProj_ActiveStatus,
                        UProj_Version = _ProjectAllocation.UProj_Version,
                        UProj_CreatedDate = System.DateTime.Now,
                        UProj_CreatedBy = _ProjectAllocation.UProj_CreatedBy,
                        UProj_ModifiedDate = System.DateTime.Now,
                        UProj_ModifiedBy = _ProjectAllocation.UProj_ModifiedBy,
                        UProj_isDeleted = _ProjectAllocation.UProj_isDeleted,
                        Is_L1_Manager = _ProjectAllocation.Is_L1_Manager,
                        //Is_L2_Manager = _ProjectAllocation.Is_L2_Manager,
                        UProj_L1_ManagerId = _ProjectAllocation.UProj_L1_ManagerId,
                        UProj_L2_ManagerId = _ProjectAllocation.UProj_L2_ManagerId

                        #endregion



                    });

                    retVal = db.SaveChanges();
                    //retVal = 1;

                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
                return retVal;
            }

        }
        #endregion
        public string passcodesequence()
        {

            // conn.ConnectionString = "Data Source=DESKTOP-7AA2POF;Initial Catalog=EvolutyzCorner_Developer;User ID=sa;Password=A12#abhi";

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();

            // SqlCommand cmd = new SqlCommand("select max(AqId) FROM [AcademicQuestionPaper]", conn);
            SqlCommand cmd = new SqlCommand("select NEXT VALUE FOR [dbo].[CreatePasscode]", conn);
            string k = cmd.ExecuteScalar().ToString();
            //int k=cmd.ExecuteNonQuery();
            // k = k + 1;
            return k;

        }
        #region To get particular Project details from Database
        public ProjectEntity GetClientDetailByID(int ID)
        {
            ProjectEntity response = new ProjectEntity();

            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    response = (from q in db.Projects
                                join a in db.Accounts on q.Proj_AccountID equals a.Acc_AccountID
                                where q.Proj_ProjectID == ID
                                select new ProjectEntity
                                {
                                    Proj_ProjectID = q.Proj_ProjectID,
                                    Proj_AccountID = q.Proj_AccountID,
                                    AccountName = a.Acc_AccountName,
                                    CountryId = q.CountryID,
                                    StateId = q.StateID,
                                    WebUrl = q.WebUrl,
                                    Proj_ProjectCode = q.Proj_ProjectCode,
                                    Proj_ProjectName = q.Proj_ProjectName,
                                    Proj_ProjectDescription = q.Proj_ProjectDescription,
                                    Proj_StartDate = q.Plan_StartDate,
                                    Proj_EndDate = q.Plan_EndDate,
                                    Proj_ActiveStatus = q.Proj_ActiveStatus,
                                    Proj_Version = q.Proj_Version,
                                    Proj_CreatedBy = q.Proj_CreatedBy,
                                    Proj_CreatedDate = q.Proj_CreatedDate,
                                    Proj_ModifiedBy = q.Proj_ModifiedBy,
                                    Proj_ModifiedDate = q.Proj_ModifiedDate,
                                    Proj_isDeleted = q.Proj_isDeleted,
                                    Is_Timesheet_ProjectSpecific = q.Is_Timesheet_ProjectSpecific
                                }).FirstOrDefault();

                    response.IsSuccessful = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.IsSuccessful = false;
                    response.Message = "Error Occured in GetProjectDetailByID(ID)";
                    response.Detail = ex.Message.ToString();
                    return response;
                }
            }
        }
        #endregion

        public List<OrganizationAccountEntity> GetRoleNames(int AccountId)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.GenericRoles
                                 join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                                 where (UT.GenericRoleID == 1007 || UT.GenericRoleID == 1006 || UT.GenericRoleID == 1061 || UT.GenericRoleID == 1005) && r.Rol_AccountID == AccountId
                                 select new OrganizationAccountEntity
                                 {
                                     GenericRoleID = r.Rol_RoleID,
                                     Title = r.Rol_RoleDescription
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<OrganizationAccountEntity> GetAllRoles()
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accid = objinfo.AccountId;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.Roles
                                 join gr in db.GenericRoles on UT.Rol_RoleName equals gr.GenericRoleID
                                 where UT.Rol_AccountID == accid && UT.Rol_isDeleted == false

                                 select new OrganizationAccountEntity
                                 {
                                     Title = UT.Rol_RoleDescription,
                                     GenericRoleID = UT.Rol_RoleID
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<OrganizationAccountEntity> GetAllNewRoles()
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accid = objinfo.AccountId;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.Roles
                                 join gr in db.GenericRoles on UT.Rol_RoleName equals gr.GenericRoleID
                                 where UT.Rol_AccountID == accid && UT.Rol_isDeleted == false

                                 select new OrganizationAccountEntity
                                 {
                                     Title = UT.Rol_RoleDescription,
                                     GenericRoleID = UT.Rol_RoleID
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ClientEntity> getclientprojects(int projid)
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accid = objinfo.AccountId;
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    var OBJ = (from q in db.ClientProjects
                               join a in db.Accounts on q.Accountid equals a.Acc_AccountID
                               where q.Accountid == accid && q.Proj_ProjectID == projid
                               select new ClientEntity
                               {
                                   CL_ProjectID = q.CL_ProjectID,
                                   ProjectID = q.Proj_ProjectID,
                                   Accountid = q.Accountid,
                                   ClientProjTitle = q.ClientProjTitle,
                                   ClientProjDesc = q.ClientProjDesc.Substring(0, 25),
                                   hasUsers = ((from u in db.Users
                                                join up in db.UserProjects on u.Usr_UserID equals up.UProj_UserID
                                                where up.UProj_ProjectID == projid && up.ClientprojID == q.CL_ProjectID
                                                select new
                                                {
                                                    Userid = u.Usr_UserID
                                                }).Distinct().FirstOrDefault() != null) ? true : false
                               }).ToList();

                    return OBJ;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<ClientEntity> getclientprojectsdropdown(int projid)
        {
            UserSessionInfo objinfo = new UserSessionInfo();
            int accid = objinfo.AccountId;
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    var OBJ = (from q in db.ClientProjects
                               join a in db.Accounts on q.Accountid equals a.Acc_AccountID
                               where q.Accountid == accid && q.Proj_ProjectID == projid
                               select new ClientEntity
                               {
                                   CL_ProjectID = q.CL_ProjectID,
                                   ProjectID = q.Proj_ProjectID,
                                   Accountid = q.Accountid,
                                   ClientProjTitle = q.ClientProjTitle,
                                   ClientProjDesc = q.ClientProjDesc
                               }).ToList();

                    return OBJ;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<ProjectAllocationEntities> GetAlltasknames(int projectid, int Roleid)
        {
            UserSessionInfo info = new UserSessionInfo();

            int accid = info.AccountId;
            //  int roleid = info.RoleId;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {



                    var roleid = db.Roles.Where(d => d.Rol_RoleID == Roleid).FirstOrDefault().Rol_RoleName;

                    var query = (

                        from AT in db.ClientProjectsTasks
                        join ut in db.GenericTasks on AT.acc_specifictaskid equals ut.tsk_TaskID
                        where AT.Accountid == accid && AT.rol_roleid == roleid


                        select new ProjectAllocationEntities
                        {
                            Proj_SpecificTaskId = AT.CL_ProjectsTasksID,
                            Proj_SpecificTaskName = ut.tsk_TaskName
                        }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ProjectAllocationEntities> GetAllEmptasknames(int projectid, int Roleid)
        {
            UserSessionInfo info = new UserSessionInfo();

            int accid = info.AccountId;
            //  int roleid = info.RoleId;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {



                    var roleid = db.Roles.Where(d => d.Rol_RoleID == Roleid).FirstOrDefault().Rol_RoleName;

                    var query = (

                        from AT in db.ClientProjectsTasks
                        join ut in db.GenericTasks on AT.acc_specifictaskid equals ut.tsk_TaskID
                        where AT.Accountid == accid && AT.rol_roleid == roleid


                        select new ProjectAllocationEntities
                        {
                            Proj_SpecificTaskId = AT.CL_ProjectsTasksID,
                            Proj_SpecificTaskName = ut.tsk_TaskName
                        }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ProjectAllocationEntities> gettasknames(int projectid)
        {
            UserSessionInfo info = new UserSessionInfo();

            int accid = info.AccountId;
            //  int roleid = info.RoleId;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {



                    //var roleid = db.Roles.Where(d => d.Rol_RoleID == Roleid).FirstOrDefault().Rol_RoleName;

                    var query = (
                        from UT in db.ClientProjectsTasks
                        join AT in db.AccountSpecificTasks on UT.acc_specifictaskid equals AT.Acc_SpecificTaskId
                        where UT.Accountid == accid // && UT.rol_roleid == roleid 


                        select new ProjectAllocationEntities
                        {
                            Proj_SpecificTaskId = AT.Acc_SpecificTaskId,
                            Proj_SpecificTaskName = AT.Acc_SpecificTaskName
                        }).Distinct().ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<OrganizationAccountEntity> GetRoleNamesbyemp(int AccountId)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.GenericRoles
                                 join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                                 where UT.GenericRoleID != 1007 && r.Rol_AccountID == AccountId && UT.GenericRoleID != 1002
                                 select new OrganizationAccountEntity
                                 {
                                     GenericRoleID = r.Rol_RoleID,
                                     Title = r.Rol_RoleDescription
                                 }).ToList();

                    //var query = (from UT in db.GenericRoles
                    //             join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                    //             where UT.GenericRoleID != 1007 && r.Rol_AccountID == AccountId
                    //             select new OrganizationAccountEntity
                    //             {
                    //                 GenericRoleID = UT.GenericRoleID,
                    //                 Title = UT.Title
                    //             }).ToList();
                    //return query;
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region GetManagerNames

        public List<UserEntity> GetManagerNames(int projid)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var ManagerNames = (from UPF in db.UsersProfiles
                                        join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                        where UP.Is_L1_Manager == true && UP.UProj_ProjectID == projid && UPF.UsrP_isDeleted == false
                                        select new UserEntity
                                        {
                                            Usr_UserID = UPF.UsrP_UserID,
                                            UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,


                                        }).ToList();
                    ManagerNames.Add(new UserEntity
                    {

                        UsrP_FirstName = "Select Manager"
                    });
                    ManagerNames.OrderBy(p => p.Usr_UserID).ToList();
                    //taskNames.Add(new UserEntity
                    //{
                    //    Usr_TaskID=0,
                    //    Taskname = "Select TaskName"
                    //});
                    return ManagerNames;
                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

        #region GetManagerBYRolesNames

        public List<UserEntity> ManagerByRole(int projid, int userid, int AccountId)
        {



            using (var db = new EvolutyzCornerDataEntities())
            {
                List<UserEntity> ManagerNames = new List<UserEntity>();
                try
                {
                    dynamic Managers;
                    int getRolename = (from u in db.Users
                                       join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                                       join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
                                       where u.Usr_UserID == userid && u.Usr_AccountID == AccountId && u.Usr_isDeleted == false
                                       select gr.GenericRoleID).FirstOrDefault();

                    if (getRolename == 1007)
                    {
                        Managers = (from UPF in db.UsersProfiles
                                    join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                    where UP.Is_L1_Manager == true
                                    && UP.UProj_ProjectID == projid
                                    && UPF.UsrP_UserID != userid
                                    && UPF.UsrP_isDeleted == false
                                    select new UserEntity
                                    {
                                        Usr_UserID = UPF.UsrP_UserID,
                                        UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                    }).ToList();
                    }
                    else
                    {
                        Managers = (from UPF in db.UsersProfiles
                                    join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                    where UP.Is_L1_Manager == true && UP.UProj_ProjectID == projid && UPF.UsrP_isDeleted == false
                                    select new UserEntity
                                    {
                                        Usr_UserID = UPF.UsrP_UserID,
                                        UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                    }).ToList();
                    }
                    //Managers.Add(new UserEntity
                    //{

                    //    UsrP_FirstName = "Select Manager"
                    //});
                    ManagerNames = Managers;

                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();



                    //ManagerNames.Add(new UserEntity
                    //{
                    //    Usr_UserID = 0,
                    //    UsrP_FirstName = "Select Manager"
                    //});

                    //taskNames.Add(new UserEntity
                    //{
                    //    Usr_TaskID=0,
                    //    Taskname = "Select TaskName"
                    //});
                    return ManagerNames;
                    //taskNames.Add(new UserEntity
                    //{
                    //    Usr_TaskID=0,
                    //    Taskname = "Select TaskName"
                    //});

                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

        #region GetManageronprojectChange

        public List<UserEntity> GetManager1onProjectChange(int CL_ProjectId)
        {

            using (var db = new EvolutyzCornerDataEntities())
            {
                List<UserEntity> ManagerNames = new List<UserEntity>();
                try
                {
                    ManagerNames = (from UPF in db.UsersProfiles
                                    join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                    where UP.Is_L1_Manager == true && UP.ClientprojID == CL_ProjectId && UPF.UsrP_isDeleted == false
                                    select new UserEntity
                                    {
                                        Usr_UserID = UPF.UsrP_UserID,
                                        UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                    }).ToList();
                    ManagerNames.Add(new UserEntity
                    {

                        UsrP_FirstName = "Select Manager"
                    });

                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();


                    return ManagerNames;


                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

        #region GetManageroncChange

        public List<UserEntity> GetManagerOnChange(int projid, int ManagerID, int Client_ProjId)
        {

            using (var db = new EvolutyzCornerDataEntities())
            {
                List<UserEntity> ManagerNames = new List<UserEntity>();
                try
                {
                    ManagerNames = (from UPF in db.UsersProfiles
                                    join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                    where UP.Is_L1_Manager == true && UP.UProj_ProjectID == projid && UPF.UsrP_UserID != ManagerID
                                           && UP.ClientprojID == Client_ProjId && UPF.UsrP_isDeleted == false
                                    select new UserEntity
                                    {
                                        Usr_UserID = UPF.UsrP_UserID,
                                        UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                    }).ToList();
                    ManagerNames.Add(new UserEntity
                    {

                        UsrP_FirstName = "Select Manager"
                    });

                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();


                    return ManagerNames;


                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

        #region GetManagerNames

        public List<UserEntity> GetManagerNames2(int projid/*,int CL_Projid*/)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    var ManagerNames = (from UPF in db.UsersProfiles
                                        join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                        where UP.Is_L1_Manager == true && UP.UProj_ProjectID == projid /*&& UP.ClientprojID == CL_Projid */
                                        && UPF.UsrP_isDeleted == false
                                        select new UserEntity
                                        {
                                            Usr_UserID = UPF.UsrP_UserID,
                                            UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                        }).ToList();
                    //ManagerNames.Add(new UserEntity
                    //{
                    //    //Usr_UserID = ,
                    //    UsrP_FirstName = "Select Manager"
                    //});
                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();
                    //ManagerNames.Add(new UserEntity
                    //{
                    //    Usr_UserID = 0,
                    //    UsrP_FirstName = "Select Manager"GetL2ManagerNamesforNewEmp
                    //});


                    return ManagerNames;


                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }
        #endregion

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

        public string Savemanager(ClientEntity orgDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            var accountid = objinfo.AccountId;
            int rolename;
            bool Manager2;
            int Manager3;
            int? manager2 = orgDtl.Managername2;
            if (manager2 == 0 || manager2 == null)
            {
                Manager2 = false;
            }
            else
            {
                Manager2 = true;
            }
            int? leadManager = orgDtl.Leadformanager;
            if (leadManager == null)
            {
                orgDtl.Leadformanager = 0;
            }
            rolename = Convert.ToInt32(orgDtl.RoleName);


            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    var UserName = db.Set<User>().Where(s => s.Usr_Username == orgDtl.Usr_Username.Trim()).FirstOrDefault<User>();
                    var LoginId = db.Set<User>().Where(s => s.Usr_LoginId == orgDtl.Email.Trim()).FirstOrDefault<User>();
                    // Role roleid = new Role();
                    //int roleid = db.Set<Role>().Where(r => (r.Rol_AccountID == accountid && r.Rol_RoleName == rolename)).FirstOrDefault<Role>().Rol_RoleID;
                    if (UserName != null)
                    {
                        return response = "UserName already Exists";
                    }
                    else if (LoginId != null)
                    {
                        return response = "Loginid already Exists";
                    }
                    User u = new User();
                    u.Usr_AccountID = accountid;
                    u.Usr_UserTypeID = orgDtl.Usr_UserTypeID;
                    u.Usr_RoleID = rolename;
                    u.Usr_Username = orgDtl.Usr_Username.Trim();
                    u.Usr_LoginId = orgDtl.Email.Trim();
                    u.Usr_Password = (orgDtl.Usr_Password);
                    // u.Usr_ActiveStatus = false;
                    u.Usr_TaskID = orgDtl.Usr_TaskID;
                    u.Usr_CreatedBy = orgDtl.UProj_CreatedBy;
                    u.Usr_CreatedDate = DateTime.Now;
                    u.Usr_Version = 1;
                    u.Usr_isDeleted = orgDtl.Usr_isDeleted;

                    db.Set<User>().Add(u);
                    db.SaveChanges();

                    int userid = u.Usr_UserID;
                    string passcode = passcodesequence();
                    UsersProfile uf = new UsersProfile();
                    uf.UsrP_UserID = userid;
                    uf.UsrP_FirstName = orgDtl.UsrP_FirstName;
                    uf.UsrP_LastName = orgDtl.UsrP_LastName;
                    uf.UsrP_EmailID = orgDtl.Email.Trim();
                    uf.Usrp_ProfilePicture = orgDtl.Usrp_ProfilePicture;
                    uf.Usrp_DOJ = orgDtl.Usrp_DOJ;
                    uf.UsrP_EmployeeID = orgDtl.UsrP_EmployeeID;
                    uf.Usrp_MobileNumber = orgDtl.Usrp_MobileNumber;
                    uf.Usrp_CountryCode = orgDtl.Usrp_CountryCode;
                    uf.Usr_Titleid = orgDtl.Usr_Titleid;
                    uf.Usr_GenderId = orgDtl.Usr_GenderId;
                    uf.UsrP_CreatedBy = orgDtl.UProj_CreatedBy;
                    uf.UsrP_CreatedDate = DateTime.Now;
                    // uf.UsrP_ActiveStatus = orgDtl.UProj_ActiveStatus;
                    uf.UsrP_Version = 1;
                    uf.passcode = passcode;
                    db.Set<UsersProfile>().Add(uf);
                    db.SaveChanges();
                    UserProject up = new UserProject();
                    up.UProj_UserID = userid;
                    up.UProj_ProjectID = orgDtl.Proj_ProjectID;
                    up.ClientprojID = orgDtl.CL_ProjectID;
                    up.UProj_L1_ManagerId = orgDtl.ManagerName;
                    up.UProj_L2_ManagerId = orgDtl.Managername2;
                    up.UProj_L1_ManagerId = orgDtl.Leadformanager;
                    up.UProj_StartDate = System.DateTime.Now;
                    up.UProj_EndDate = System.DateTime.Now;
                    up.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                    // up.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                    up.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                    up.UProj_CreatedDate = DateTime.Now;
                    up.UProj_Version = 1;
                    up.UProj_isDeleted = orgDtl.Usr_isDeleted;
                    up.Is_L1_Manager = true;
                    up.Is_L2_Manager = Manager2;
                    up.TimesheetMode = orgDtl.TimesheetMode_id;
                    up.IsDirectManager = orgDtl.IsDirectManager;
                    up.isclientcalendar = orgDtl.isclientholidays;
                    db.Set<UserProject>().Add(up);
                    db.SaveChanges();

                    response = "Successfully Added";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public string SaveEmployee(ClientEntity orgDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            var accountid = objinfo.AccountId;
            int rolename = Convert.ToInt32(orgDtl.RoleName);

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    var UserName = db.Set<User>().Where(s => (s.Usr_Username == orgDtl.Usr_Username.Trim() && s.Usr_isDeleted == false)).FirstOrDefault<User>();
                    var LoginId = db.Set<User>().Where(s => (s.Usr_LoginId == orgDtl.Email.Trim() && s.Usr_isDeleted == false)).FirstOrDefault<User>();
                    if (UserName != null)
                    {
                        return response = "UserName already Exists";
                    }
                    else if (LoginId != null)
                    {
                        return response = "Loginid already Exists";
                    }
                    User u = new User();
                    u.Usr_AccountID = accountid;
                    u.Usr_UserTypeID = orgDtl.Usr_UserTypeID;
                    u.Usr_RoleID = rolename;
                    u.Usr_Username = orgDtl.Usr_Username;
                    u.Usr_LoginId = orgDtl.Email.Trim();
                    u.Usr_Password = (orgDtl.Usr_Password);
                    u.Usr_TaskID = orgDtl.Usr_TaskID;
                    u.Usr_CreatedBy = orgDtl.UProj_CreatedBy;
                    u.Usr_CreatedDate = DateTime.Now;
                    u.Usr_Version = 1;
                    u.Usr_isDeleted = orgDtl.Usr_isDeleted;

                    db.Set<User>().Add(u);
                    db.SaveChanges();

                    int userid = u.Usr_UserID;
                    string passcode = passcodesequence();
                    UsersProfile uf = new UsersProfile();
                    uf.UsrP_UserID = userid;
                    uf.UsrP_FirstName = orgDtl.UsrP_FirstName;
                    uf.UsrP_LastName = orgDtl.UsrP_LastName;
                    uf.UsrP_EmailID = orgDtl.Email.Trim();
                    uf.Usrp_ProfilePicture = orgDtl.Usrp_ProfilePicture;
                    uf.Usrp_DOJ = orgDtl.Usrp_DOJ;
                    uf.UsrP_EmployeeID = orgDtl.UsrP_EmployeeID;
                    uf.Usrp_MobileNumber = orgDtl.Usrp_MobileNumber;
                    uf.Usrp_CountryCode = orgDtl.Usrp_CountryCode;
                    uf.Usr_Titleid = orgDtl.Usr_Titleid;
                    uf.Usr_GenderId = orgDtl.Usr_GenderId;
                    uf.UsrP_CreatedBy = orgDtl.UProj_CreatedBy;
                    uf.UsrP_CreatedDate = DateTime.Now;
                    // uf.UsrP_ActiveStatus = orgDtl.UProj_ActiveStatus;
                    uf.UsrP_Version = 1;
                    uf.passcode = passcode;
                    db.Set<UsersProfile>().Add(uf);
                    db.SaveChanges();
                    UserProject up = new UserProject();
                    up.UProj_UserID = userid;
                    up.UProj_ProjectID = orgDtl.Proj_ProjectID;
                    up.ClientprojID = orgDtl.CL_ProjectID;
                    up.UProj_L1_ManagerId = orgDtl.ManagerName;
                    up.UProj_L2_ManagerId = orgDtl.Managername2;
                    up.UProj_StartDate = System.DateTime.Now;
                    up.UProj_EndDate = System.DateTime.Now;
                    up.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                    //up.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                    up.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                    up.UProj_CreatedDate = DateTime.Now;
                    up.UProj_Version = 1;
                    up.UProj_isDeleted = orgDtl.Usr_isDeleted;
                    up.Is_L1_Manager = false;
                    up.Is_L2_Manager = false;
                    up.TimesheetMode = orgDtl.TimesheetMode_id;
                    up.isclientcalendar = orgDtl.isclientholidays;
                    db.Set<UserProject>().Add(up);
                    db.SaveChanges();

                    response = "Successfully Added";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public string AssociateEmployee(ClientEntity orgDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            var accountid = objinfo.AccountId;
            int rolename = Convert.ToInt32(orgDtl.RoleName);

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {


                    UserProject up = new UserProject();
                    up.UProj_UserID = orgDtl.Usr_UserID;
                    up.UProj_ProjectID = orgDtl.Proj_ProjectID;
                    up.UProj_L1_ManagerId = orgDtl.ManagerName;
                    up.UProj_L2_ManagerId = orgDtl.Managername2;
                    up.UProj_StartDate = orgDtl.UProj_StartDate;
                    up.UProj_EndDate = orgDtl.UProj_EndDate;
                    up.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                    up.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                    up.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                    up.UProj_CreatedDate = DateTime.Now;
                    up.UProj_Version = 1;
                    up.UProj_isDeleted = false;
                    up.Is_L1_Manager = false;
                    up.Is_L2_Manager = false;
                    db.Set<UserProject>().Add(up);
                    db.SaveChanges();

                    response = "Successfully Added";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public string AssociateManager(ClientEntity orgDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            var accountid = objinfo.AccountId;
            int rolename = Convert.ToInt32(orgDtl.RoleName);

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    if (orgDtl.managertype == 1)
                    {
                        UserProject up = new UserProject();
                        up.UProj_UserID = orgDtl.Usr_UserID;
                        up.UProj_ProjectID = orgDtl.CL_ProjectID;
                        up.UProj_L1_ManagerId = orgDtl.ManagerName;
                        up.UProj_L2_ManagerId = orgDtl.Managername2;
                        up.UProj_StartDate = orgDtl.UProj_StartDate;
                        up.UProj_EndDate = orgDtl.UProj_EndDate;
                        up.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                        up.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        up.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                        up.UProj_CreatedDate = DateTime.Now;
                        up.UProj_Version = 1;
                        up.UProj_isDeleted = false;
                        up.Is_L1_Manager = true;
                        up.Is_L2_Manager = false;
                        db.Set<UserProject>().Add(up);
                        db.SaveChanges();
                    }
                    else if (orgDtl.managertype == 2)
                    {
                        UserProject up = new UserProject();
                        up.UProj_UserID = orgDtl.Usr_UserID;
                        up.UProj_ProjectID = orgDtl.CL_ProjectID;
                        up.UProj_L1_ManagerId = orgDtl.ManagerName;
                        up.UProj_L2_ManagerId = orgDtl.Managername2;
                        up.UProj_StartDate = orgDtl.UProj_StartDate;
                        up.UProj_EndDate = orgDtl.UProj_EndDate;
                        up.UProj_ParticipationPercentage = orgDtl.UProj_ParticipationPercentage;
                        up.UProj_ActiveStatus = orgDtl.UProj_ActiveStatus;
                        up.UProj_CreatedBy = orgDtl.UProj_CreatedBy;
                        up.UProj_CreatedDate = DateTime.Now;
                        up.UProj_Version = 1;
                        up.UProj_isDeleted = false;
                        up.Is_L1_Manager = false;
                        up.Is_L2_Manager = true;
                        db.Set<UserProject>().Add(up);
                        db.SaveChanges();
                    }



                    response = "Successfully Added";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }


        public List<ProjectAllocationEntities> GetGenders()
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from UT in db.UserGenders
                                     //where !db.LeaveSchemes.Any(l => l.LSchm_UserTypeID == UT.UsT_UserTypeID)
                                 select new ProjectAllocationEntities
                                 {
                                     Usr_GenderId = UT.Usr_GenderId,
                                     Gender = UT.Gender
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<ProjectAllocationEntities> Getaccountspecifictasks()
        {
            UserSessionInfo info = new UserSessionInfo();
            int accountId = info.AccountId;
            string RoleId = info.RoleName;

            try
            {
                if (RoleId == "Super Admin")
                {
                    using (var db = new EvolutyzCornerDataEntities())
                    {
                        var query = (from UT in db.AccountSpecificTasks
                                     where UT.isDeleted == false
                                     //where !db.LeaveSchemes.Any(l => l.LSchm_UserTypeID == UT.UsT_UserTypeID)
                                     select new ProjectAllocationEntities
                                     {
                                         Acc_SpecificTaskId = UT.Acc_SpecificTaskId,
                                         Acc_SpecificTaskName = UT.Acc_SpecificTaskName
                                     }).ToList();
                        return query;
                    }
                }
                else
                {
                    using (var db = new EvolutyzCornerDataEntities())
                    {
                        var query = (from UT in db.AccountSpecificTasks
                                     where UT.AccountID == accountId && UT.isDeleted == false
                                     //where !db.LeaveSchemes.Any(l => l.LSchm_UserTypeID == UT.UsT_UserTypeID)
                                     select new ProjectAllocationEntities
                                     {
                                         Acc_SpecificTaskId = UT.Acc_SpecificTaskId,
                                         Acc_SpecificTaskName = UT.Acc_SpecificTaskName
                                     }).ToList();
                        return query;
                    }
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ProjectAllocationEntities> GetTitle()
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from UT in db.UserTitles
                                     //where !db.LeaveSchemes.Any(l => l.LSchm_UserTypeID == UT.UsT_UserTypeID)
                                 select new ProjectAllocationEntities
                                 {
                                     Usr_Titleid = UT.Usr_Titleid,
                                     TitlePrefix = UT.TitlePrefix
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<ProjectAllocationEntities> GetCountryCodes()
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from UT in db.Countries
                                 where UT.StatusId == true
                                 select new ProjectAllocationEntities
                                 {
                                     CountryId = UT.CountryId,
                                     PhoneCode = UT.PhoneCode
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ProjectAllocationEntities> GetProjectUserDetails(int id)
        {
            //  ProjectAllocationEntities response = new ProjectAllocationEntities();
            UserSessionInfo info = new UserSessionInfo();
            int accountid = info.AccountId;
            int? ClientprojID = info.ClientprojID;
            var roleid = info.RoleName;
            if (roleid == "Super Admin")
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    try
                    {

                        var response = (from u in db.Users
                                        join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                        join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                        join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                        join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                        join cp in db.ClientProjects on p.Proj_ProjectID equals cp.Proj_ProjectID

                                        select new ProjectAllocationEntities
                                        {
                                            Usr_UserID = u.Usr_UserID,
                                            Usr_RoleID = u.Usr_RoleID,
                                            Email = u.Usr_LoginId,
                                            //  UsrP_EmployeeID = up.UsrP_EmployeeID,
                                            Proj_ProjectID = uf.UProj_ProjectID,
                                            Proj_ProjectName = p.Proj_ProjectName,
                                            UProj_ParticipationPercentage = uf.UProj_ParticipationPercentage,
                                            UProj_StartDate = uf.UProj_StartDate,
                                            UProj_EndDate = uf.UProj_EndDate,
                                            Usrp_DOJ = up.Usrp_DOJ,
                                            UsrP_FirstName = up.UsrP_FirstName + " " + up.UsrP_LastName,
                                            UsrP_LastName = up.UsrP_LastName,
                                            project = cp.ClientProjTitle,
                                            Usr_isDeleted = u.Usr_isDeleted,

                                            CL_ProjectId = cp.CL_ProjectID

                                        }).ToList();

                        // response.IsSuccessful = true;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        //  response.IsSuccessful = false;
                        //response.Message = "Error Occured in GetProjectDetailByID(ID)";
                        //  response.Detail = ex.Message.ToString();
                        throw ex;
                    }
                }
            }
            else if (roleid == "Admin")
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    try
                    {

                        var response = (from u in db.Users
                                        join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                        join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                        join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                        join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                        join cp in db.ClientProjects on uf.ClientprojID equals cp.CL_ProjectID
                                        join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                                        join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
                                        where u.Usr_AccountID == accountid && p.Proj_ProjectID == id
                                        select new ProjectAllocationEntities
                                        {
                                            Usr_UserID = u.Usr_UserID,
                                            Usr_RoleID = u.Usr_RoleID,
                                            Email = u.Usr_LoginId,
                                            // UsrP_EmployeeID = up.UsrP_EmployeeID,
                                            Proj_ProjectID = uf.UProj_ProjectID,
                                            Proj_ProjectName = p.Proj_ProjectName,
                                            project = cp.ClientProjTitle,
                                            RoleName = gr.Title,
                                            UProj_ParticipationPercentage = uf.UProj_ParticipationPercentage,
                                            UProj_StartDate = uf.UProj_StartDate,
                                            UProj_EndDate = uf.UProj_EndDate,
                                            Usrp_DOJ = up.Usrp_DOJ,
                                            UsrP_FirstName = up.UsrP_FirstName + " " + up.UsrP_LastName,
                                            UsrP_LastName = up.UsrP_LastName,
                                            Usr_isDeleted = u.Usr_isDeleted,
                                            CL_ProjectId = cp.CL_ProjectID
                                        }).Distinct().ToList();

                        // response.IsSuccessful = true;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        //  response.IsSuccessful = false;
                        //response.Message = "Error Occured in GetProjectDetailByID(ID)";
                        //  response.Detail = ex.Message.ToString();
                        throw ex;
                    }
                }
            }
            else
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    try
                    {

                        var response = (from u in db.Users
                                        join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                        join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                        join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                        join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                        join cp in db.ClientProjects on uf.ClientprojID equals cp.CL_ProjectID
                                        join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                                        join gr in db.GenericRoles on r.Rol_RoleName equals gr.GenericRoleID
                                        where u.Usr_AccountID == accountid && p.Proj_ProjectID == id && cp.CL_ProjectID == ClientprojID
                                        select new ProjectAllocationEntities
                                        {
                                            Usr_UserID = u.Usr_UserID,
                                            Usr_RoleID = u.Usr_RoleID,
                                            Email = u.Usr_LoginId,
                                            User_RoleId = r.Rol_RoleName,
                                            // UsrP_EmployeeID = up.UsrP_EmployeeID,
                                            Proj_ProjectID = uf.UProj_ProjectID,
                                            Proj_ProjectName = p.Proj_ProjectName,
                                            project = cp.ClientProjTitle,
                                            RoleName = gr.Title,
                                            UProj_ParticipationPercentage = uf.UProj_ParticipationPercentage,
                                            UProj_StartDate = uf.UProj_StartDate,
                                            UProj_EndDate = uf.UProj_EndDate,
                                            Usrp_DOJ = up.Usrp_DOJ,
                                            UsrP_FirstName = up.UsrP_FirstName + " " + up.UsrP_LastName,
                                            UsrP_LastName = up.UsrP_LastName,
                                            Usr_isDeleted = u.Usr_isDeleted

                                        }).ToList();

                        // response.IsSuccessful = true;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        //  response.IsSuccessful = false;
                        //response.Message = "Error Occured in GetProjectDetailByID(ID)";
                        //  response.Detail = ex.Message.ToString();
                        throw ex;
                    }
                }
            }

        }

        public ProjectAllocationEntities GetUserDetailById(int ID, string proid, int CL_ProjID)
        {
            int projectid = Convert.ToInt32(proid);
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    //Role GetRoleId =
                    var response = (from u in db.Users
                                    join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                    join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                    join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                    join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                    join cp in db.ClientProjects on uf.UProj_ProjectID equals cp.Proj_ProjectID
                                    join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID

                                    where u.Usr_UserID == ID && uf.UProj_ProjectID == projectid && cp.CL_ProjectID == CL_ProjID
                                    select new ProjectAllocationEntities
                                    {
                                        Usr_UserID = u.Usr_UserID,
                                        Usr_RoleID = r.Rol_RoleID,
                                        Email = u.Usr_LoginId,
                                        UsrP_EmployeeID = up.UsrP_EmployeeID,
                                        Proj_ProjectID = uf.UProj_ProjectID,
                                        Proj_ProjectName = p.Proj_ProjectName,
                                        UProj_ParticipationPercentage = uf.UProj_ParticipationPercentage,
                                        UProj_StartDate = uf.UProj_StartDate,
                                        UProj_EndDate = uf.UProj_EndDate,
                                        Usrp_DOJ = up.Usrp_DOJ,
                                        UsrP_FirstName = up.UsrP_FirstName,
                                        Usr_Username = u.Usr_Username,
                                        UsrP_LastName = up.UsrP_LastName,
                                        Usr_Password = (u.Usr_Password),
                                        Usr_Titleid = up.Usr_Titleid,
                                        Usrp_MobileNumber = up.Usrp_MobileNumber,
                                        Usrp_CountryCode = up.Usrp_CountryCode,
                                        Usr_GenderId = up.Usr_GenderId,
                                        Usrp_ProfilePicture = up.Usrp_ProfilePicture,
                                        Usr_UserTypeID = u.Usr_UserTypeID,
                                        Usr_TaskID = u.Usr_TaskID,
                                        ManagerName = uf.UProj_L1_ManagerId,
                                        Managername2 = uf.UProj_L2_ManagerId,
                                        LeadforManager = uf.UProj_L1_ManagerId,
                                        MName = db.Set<User>().Where(s => s.Usr_UserID == uf.UProj_UserID).FirstOrDefault<User>().Usr_Username,
                                        //db.Set<User>().Where(s=>s.Usr_UserID == uf.UProj_UserID).FirstOrDefault<User>().Usr_Username,
                                        Mname2 = db.Set<User>().Where(s => s.Usr_UserID == uf.UProj_UserID).FirstOrDefault<User>().Usr_Username,
                                        TimesheetMode_id = uf.TimesheetMode,
                                        Usr_isDeleted = u.Usr_isDeleted,
                                        Cl_projid = uf.ClientprojID,
                                        isdirectmanager = uf.IsDirectManager,
                                        isclientholiday = uf.isclientcalendar,
                                        ClientProjDesc = cp.ClientProjDesc,
                                        CL_ProjectId = cp.CL_ProjectID,
                                        roleid = db.Set<Role>().Where(s => s.Rol_RoleID == r.Rol_RoleID).FirstOrDefault<Role>().Rol_RoleName
                                    }).FirstOrDefault();
                    return response;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public ProjectAllocationEntities GetassUserDetailById(int ID)
        {
            //int projectid = Convert.ToInt32(proid);
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var response = (from u in db.Users
                                    join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                    join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                    join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                    join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                    join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID

                                    where u.Usr_UserID == ID && u.Usr_isDeleted == false
                                    select new ProjectAllocationEntities
                                    {
                                        Usr_UserID = u.Usr_UserID,
                                        Usr_RoleID = r.Rol_RoleID,
                                        Email = u.Usr_LoginId,
                                        UsrP_EmployeeID = up.UsrP_EmployeeID,
                                        Proj_ProjectID = uf.UProj_ProjectID,
                                        Proj_ProjectName = p.Proj_ProjectName,
                                        UProj_ParticipationPercentage = uf.UProj_ParticipationPercentage,
                                        UProj_StartDate = uf.UProj_StartDate,
                                        UProj_EndDate = uf.UProj_EndDate,
                                        Usrp_DOJ = up.Usrp_DOJ,
                                        UsrP_FirstName = up.UsrP_FirstName,
                                        Usr_Username = u.Usr_Username,
                                        UsrP_LastName = up.UsrP_LastName,
                                        Usr_Password = (u.Usr_Password),
                                        Usr_Titleid = up.Usr_Titleid,
                                        Usrp_MobileNumber = up.Usrp_MobileNumber,
                                        Usr_GenderId = up.Usr_GenderId,
                                        Usrp_ProfilePicture = up.Usrp_ProfilePicture,
                                        Usr_UserTypeID = u.Usr_UserTypeID,
                                        Usr_TaskID = u.Usr_TaskID,
                                        ManagerName = uf.UProj_L1_ManagerId,
                                        Managername2 = uf.UProj_L2_ManagerId,
                                        UProj_ActiveStatus = uf.UProj_ActiveStatus

                                    }).FirstOrDefault();
                    return response;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public int DeleteUser(int userid)
        {
            int retVal = 0;
            User userdtl = null;
            UsersProfile ufdtl = null;
            UserProject updtl = null;

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    userdtl = db.Set<User>().Where(s => s.Usr_UserID == userid).FirstOrDefault<User>();
                    if (userdtl == null)
                    {
                        return retVal;
                    }
                    userdtl.Usr_isDeleted = true;
                    db.Entry(userdtl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    retVal = 1;
                    ufdtl = db.Set<UsersProfile>().Where(s => s.UsrP_UserID == userid).FirstOrDefault<UsersProfile>();
                    if (ufdtl == null)
                    {
                        return retVal;
                    }
                    ufdtl.UsrP_isDeleted = true;
                    db.Entry(ufdtl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    retVal = 1;
                    updtl = db.Set<UserProject>().Where(s => s.UProj_UserID == userid).FirstOrDefault<UserProject>();
                    if (ufdtl == null)
                    {
                        return retVal;
                    }
                    updtl.UProj_isDeleted = true;
                    db.Entry(updtl).State = System.Data.Entity.EntityState.Modified;
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

        public List<ProjectAllocationEntities> GetHolidays(int projectid)
        {

            UserSessionInfo info = new UserSessionInfo();
            int accountid = info.AccountId;
            var roleid = info.RoleName;
            int userid = info.UserId;

            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    int holidaydtlid = (from temp in db.HolidayCalendars
                                        where temp.ProjectID == projectid && temp.AccountID == accountid
                                        select temp.HolidayCalendarID).FirstOrDefault();

                    if (holidaydtlid == 0)
                    {
                        int? OptionalHolidays = (from H in db.HolidayCalendars
                                                 where H.AccountID == accountid && H.isOptionalHoliday == true
                                                 select H.isOptionalHoliday).Count();
                        int? useroptholidays = (from temp in db.Acc_Spec_OptionalHolidays
                                                where temp.AccoutID == accountid
                                                select temp.NoofOptionalHolidays).FirstOrDefault();

                        var response = (from H in db.HolidayCalendars
                                        join F in db.FinancialYears on H.Year equals F.FinancialYearId
                                        //join cp in db.ClientProjects on H.CL_ProjectID equals cp.CL_ProjectID
                                        where H.AccountID == accountid && H.ProjectID == null && F.StartDate == 2021
                                        select new ProjectAllocationEntities
                                        {
                                            HolidayCalendarID = H.HolidayCalendarID,
                                            AccountID = H.AccountID,
                                            HolidayName = H.HolidayName,
                                            //HolidayProjectId = H.CL_ProjectID,
                                            FinancialYearId = F.FinancialYearId,
                                            Year = H.Year,
                                            HolidayDate = H.HolidayDate,
                                            financialyear = F.StartDate + "" /*+ F.EndDate*/,
                                            isOptionalHoliday = H.isOptionalHoliday,
                                            isDeleted = H.isDeleted,
                                            optionalholidays = OptionalHolidays,
                                            useroptionalholidays = useroptholidays

                                        }).ToList();


                        return response;
                    }
                    else if (roleid == "Admin")
                    {
                        int? OptionalHolidays = (from H in db.HolidayCalendars
                                                 where H.ProjectID == projectid && H.AccountID == accountid && H.isOptionalHoliday == true
                                                 select H.isOptionalHoliday).Count();
                        int? useroptholidays = (from temp in db.Acc_Spec_OptionalHolidays
                                                where temp.ProjectId == projectid && temp.AccoutID == accountid && temp.IsDeleted == false
                                                select temp.NoofOptionalHolidays).FirstOrDefault();
                        var response = (from H in db.HolidayCalendars
                                        join F in db.FinancialYears on H.Year equals F.FinancialYearId
                                        join P in db.Projects on H.ProjectID equals P.Proj_ProjectID
                                        join cp in db.ClientProjects on H.CL_ProjectID equals cp.CL_ProjectID
                                        //   join up in db.UserProjects on H.CL_ProjectID equals up.ClientprojID
                                        where H.AccountID == accountid && F.StartDate != 2018 && P.Proj_ProjectID == projectid
                                        select new ProjectAllocationEntities
                                        {
                                            HolidayCalendarID = H.HolidayCalendarID,
                                            AccountID = H.AccountID,
                                            HolidayName = H.HolidayName,
                                            HolidayProjectId = H.CL_ProjectID,
                                            ProjectName = cp.ClientProjTitle,
                                            Year = H.Year,
                                            HolidayDate = H.HolidayDate,
                                            FinancialYearId = F.FinancialYearId,
                                            financialyear = F.StartDate + "" /*+ F.EndDate*/,
                                            isOptionalHoliday = H.isOptionalHoliday,
                                            isDeleted = H.isDeleted,
                                            ProjectID = H.ProjectID,
                                            optionalholidays = OptionalHolidays,
                                            useroptionalholidays = useroptholidays

                                        }).Distinct().ToList();
                        return response;

                    }

                    else
                    {
                        int? OptionalHolidays = (from H in db.HolidayCalendars
                                                 where H.ProjectID == projectid && H.AccountID == accountid && H.isOptionalHoliday == true
                                                 select H.isOptionalHoliday).Count();
                        int? useroptholidays = (from temp in db.Acc_Spec_OptionalHolidays
                                                where temp.ProjectId == projectid && temp.AccoutID == accountid && temp.IsDeleted == false
                                                select temp.NoofOptionalHolidays).FirstOrDefault();
                        var response = (from H in db.HolidayCalendars
                                        join F in db.FinancialYears on H.Year equals F.FinancialYearId
                                        join cp in db.ClientProjects on H.CL_ProjectID equals cp.CL_ProjectID
                                        join up in db.UserProjects on H.CL_ProjectID equals up.ClientprojID
                                        where H.ProjectID == projectid && H.AccountID == accountid && F.StartDate == 2021 && up.UProj_UserID == userid
                                        select new ProjectAllocationEntities
                                        {
                                            HolidayCalendarID = H.HolidayCalendarID,
                                            AccountID = H.AccountID,
                                            HolidayName = H.HolidayName,
                                            HolidayProjectId = H.CL_ProjectID,
                                            ProjectName = cp.ClientProjTitle,
                                            Year = H.Year,
                                            HolidayDate = H.HolidayDate,
                                            FinancialYearId = F.FinancialYearId,
                                            financialyear = F.StartDate + "" /*+ F.EndDate*/,
                                            isOptionalHoliday = H.isOptionalHoliday,
                                            isDeleted = H.isDeleted,
                                            ProjectID = H.ProjectID,
                                            optionalholidays = OptionalHolidays,
                                            useroptionalholidays = useroptholidays

                                        }).ToList();
                        return response;
                    }
                    return null;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }



        }

        public List<ProjectAllocationEntities> Getprotasks(int projectid)
        {

            UserSessionInfo info = new UserSessionInfo();
            int accountid = info.AccountId;
            var roleid = info.RoleName;

            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    var response = (from H in db.ProjectSpecificTasks
                                    join ap in db.AccountSpecificTasks on H.tsk_TaskID equals ap.Acc_SpecificTaskId
                                    where H.ProjectId == projectid && H.isDeleted == false
                                    select new ProjectAllocationEntities
                                    {
                                        Proj_SpecificTaskId = H.Proj_SpecificTaskId,
                                        ProjectId = H.ProjectId,
                                        Proj_SpecificTaskName = H.Proj_SpecificTaskName,
                                        RTMId = H.RTMId,
                                        Actual_StartDate = H.Actual_StartDate,
                                        Actual_EndDate = H.Actual_EndDate,
                                        Plan_StartDate = H.Plan_StartDate,
                                        Plan_EndDate = H.Plan_EndDate,
                                        StatusId = H.StatusId,
                                        Acc_SpecificTaskId = ap.Acc_SpecificTaskId,
                                        Acc_SpecificTaskName = ap.Acc_SpecificTaskName,


                                    }).ToList();

                    // response.IsSuccessful = true;
                    return response;


                }
                catch (Exception ex)
                {
                    //  response.IsSuccessful = false;
                    //response.Message = "Error Occured in GetProjectDetailByID(ID)";
                    //  response.Detail = ex.Message.ToString();
                    throw ex;
                }
            }



        }

        public List<ProjectAllocationEntities> getprojecttaskbyid(int projectid)
        {

            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {




                    var response = (from H in db.ProjectSpecificTasks
                                    join ap in db.AccountSpecificTasks on H.tsk_TaskID equals ap.Acc_SpecificTaskId
                                    join a in db.Projects on H.ProjectId equals a.Proj_ProjectID
                                    where H.Proj_SpecificTaskId == projectid && H.isDeleted == false
                                    select new ProjectAllocationEntities
                                    {
                                        Proj_SpecificTaskId = H.Proj_SpecificTaskId,
                                        ProjectId = H.ProjectId,
                                        Proj_SpecificTaskName = H.Proj_SpecificTaskName,
                                        RTMId = H.RTMId,
                                        Actual_StartDate = H.Actual_StartDate,
                                        Actual_EndDate = H.Actual_EndDate,
                                        Plan_StartDate = H.Plan_StartDate,
                                        Plan_EndDate = H.Plan_EndDate,
                                        StatusId = H.StatusId,
                                        Acc_SpecificTaskId = ap.Acc_SpecificTaskId,
                                        Acc_SpecificTaskName = ap.Acc_SpecificTaskName,


                                    }).ToList();


                    return response;


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }



        }

        public string Addprotasks(string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            int acc_taskid = Convert.ToInt32(Acc_SpecificTaskName);
            int projectid = Convert.ToInt32(ProjectID);

            DateTime actualstartdate = Convert.ToDateTime(Actual_StartDate);
            DateTime actualenddate = Convert.ToDateTime(Actual_EndDate);
            DateTime planstartdate = Convert.ToDateTime(Plan_StartDate);
            DateTime planenddate = Convert.ToDateTime(Plan_EndDate);
            UserSessionInfo info = new UserSessionInfo();
            int userid = info.UserId;
            //  int sttus = Convert.ToInt32(StatusId);
            bool b = Convert.ToBoolean(StatusId);
            string strresponse = string.Empty;
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    db.Set<ProjectSpecificTask>().Add(new ProjectSpecificTask
                    {
                        ProjectId = projectid,
                        Proj_SpecificTaskName = Proj_SpecificTaskName,
                        RTMId = RTMId,
                        Actual_StartDate = actualstartdate,
                        Actual_EndDate = actualenddate,
                        Plan_StartDate = planstartdate,
                        Plan_EndDate = planenddate,
                        tsk_TaskID = acc_taskid,
                        StatusId = b,
                        isDeleted = false,
                        CreatedBy = userid,
                        CreatedDate = DateTime.Now

                    });
                    db.SaveChanges();
                    strresponse = "Successfully Added";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strresponse;
        }

        public string updatetasks(int id, string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            int acc_taskid = Convert.ToInt32(Acc_SpecificTaskName);
            int projectid = Convert.ToInt32(ProjectID);

            DateTime actualstartdate = Convert.ToDateTime(Actual_StartDate);
            DateTime actualenddate = Convert.ToDateTime(Actual_EndDate);
            DateTime planstartdate = Convert.ToDateTime(Plan_StartDate);
            DateTime planenddate = Convert.ToDateTime(Plan_EndDate);
            UserSessionInfo info = new UserSessionInfo();
            int userid = info.UserId;
            int sttus = Convert.ToInt32(StatusId);
            bool b = Convert.ToBoolean(sttus);
            string strresponse = string.Empty;


            try
            {


                using (var db = new EvolutyzCornerDataEntities())
                {
                    ProjectSpecificTask taskdetails = db.Set<ProjectSpecificTask>().Where(s => s.Proj_SpecificTaskId == id).FirstOrDefault<ProjectSpecificTask>();

                    if (taskdetails == null)
                    {
                        return null;
                    }


                    taskdetails.Proj_SpecificTaskName = Proj_SpecificTaskName;
                    taskdetails.tsk_TaskID = acc_taskid;
                    taskdetails.StatusId = b;
                    taskdetails.RTMId = RTMId;
                    taskdetails.Actual_StartDate = actualstartdate;
                    taskdetails.Actual_EndDate = actualenddate;
                    taskdetails.Plan_StartDate = planstartdate;
                    taskdetails.Plan_EndDate = planenddate;

                    taskdetails.ModifiedBy = userid;
                    taskdetails.ModifiedDate = System.DateTime.Now;

                    db.Entry(taskdetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    strresponse = "Task successfully updated";





                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strresponse;
        }

        public List<ProjectAllocationEntities> Associatemanagers(int projectid, int accountid)
        {
            // int ManagerType = Convert.ToInt32(managertype);
            List<ProjectAllocationEntities> response = new List<ProjectAllocationEntities>();
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    //if (ManagerType == 1)
                    //{
                    response = (from u in db.Users
                                join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                                where r.Rol_RoleName == 1007 && uf.UProj_ProjectID != projectid && u.Usr_isDeleted == false &&
a.Acc_AccountID == accountid && uf.Is_L1_Manager == true || uf.Is_L1_Manager == true
                                select new ProjectAllocationEntities
                                {
                                    Usr_UserID = u.Usr_UserID,

                                    Usr_Username = u.Usr_Username,

                                }).Distinct().ToList();

                    // response.IsSuccessful = true;

                    //}
                    //else if(ManagerType == 2)
                    //{
                    //     response = (from u in db.Users
                    //                    join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                    //                    join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                    //                    join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                    //                    join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                    //                    join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID

                    //                    where r.Rol_RoleName == 1007 && uf.UProj_ProjectID != projectid && a.Acc_AccountID == accountid && uf.Is_L2_Manager == true
                    //                    select new ProjectAllocationEntities
                    //                    {
                    //                        Usr_UserID = u.Usr_UserID,

                    //                        Usr_Username = u.Usr_Username,

                    //                    }).Distinct().ToList();

                    //    // response.IsSuccessful = true;

                    //}


                    return response;

                }
                catch (Exception ex)
                {
                    //  response.IsSuccessful = false;
                    //response.Message = "Error Occured in GetProjectDetailByID(ID)";
                    //  response.Detail = ex.Message.ToString();
                    throw ex;
                }
            }


        }

        public List<ProjectAllocationEntities> AssociateEmployees(int projectid, int accountid)
        {


            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var response = (from u in db.Users
                                    join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                    join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                    join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                    join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                    join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID

                                    where r.Rol_RoleName != 1001
                                    && r.Rol_RoleName != 1002
                                    && u.Usr_isDeleted == false
                                    && r.Rol_RoleName != 1007
                                    && uf.UProj_ProjectID != projectid
                                    && a.Acc_AccountID == accountid
                                    select new ProjectAllocationEntities
                                    {
                                        Usr_UserID = u.Usr_UserID,

                                        Usr_Username = u.Usr_Username,

                                    }).Distinct().ToList();


                    return response;


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }


        }

        public string DeleteProjecttask(string id)
        {
            int protaskid = Convert.ToInt32(id);
            string response = string.Empty;
            ProjectSpecificTask taskdtl = null;

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    taskdtl = db.Set<ProjectSpecificTask>().Where(s => s.Proj_SpecificTaskId == protaskid).FirstOrDefault<ProjectSpecificTask>();
                    if (taskdtl == null)
                    {
                        return null;
                    }
                    taskdtl.isDeleted = true;
                    db.Entry(taskdtl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response = "Successfully Deleted";


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public string ChangeStatus(string id, string status)
        {
            string strResponse = string.Empty;
            ProjectSpecificTask holidayData = null;
            bool Status = Convert.ToBoolean(status);
            int did = Convert.ToInt32(id);
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    holidayData = db.Set<ProjectSpecificTask>().Where(s => s.Proj_SpecificTaskId == did).FirstOrDefault<ProjectSpecificTask>();
                    if (holidayData == null)
                    {
                        return null;
                    }
                    holidayData.StatusId = Status;
                    // holidayData.isActive = false;
                    db.Entry(holidayData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    strResponse = "Status Changed Successfully";
                }
                catch (Exception ex)
                {
                    strResponse = ex.Message.ToString();
                }
            }
            return strResponse;
        }

        public string SavetimeSheetImages(int timesheetid, int? userid, List<string> news, int? upid)
        {
            string strresponse = string.Empty;
            EvolutyzCornerDataEntities db1 = new EvolutyzCornerDataEntities();
            var checkTimesheettypeMode = (from up in db1.UserProjects where up.UProj_UserID == userid select up.TimesheetMode).FirstOrDefault();
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {

                    List<AttachmentsinTimesheet> timesheets = db.Set<AttachmentsinTimesheet>().Where(s => s.TimeSheetID == timesheetid).ToList();

                    if (checkTimesheettypeMode == 2)
                    {
                        if (news.Count == 2)
                        {
                            if (timesheets.Count != 0)
                            {
                                timesheets.ToList().ForEach(u =>
                                {
                                    db.Set<AttachmentsinTimesheet>().Remove(u);

                                });

                                db.SaveChanges();

                            }
                        }
                        else if (news.Count == 1)
                        {
                            if (timesheets.Count == 2)
                            {
                                if (upid == 1)
                                {

                                    db.Set<AttachmentsinTimesheet>().Remove(timesheets[1]);



                                    db.SaveChanges();
                                }
                                else if (upid == 2)
                                {
                                    db.Set<AttachmentsinTimesheet>().Remove(timesheets[0]);
                                }
                            }
                        }

                        for (int i = 0; i <= news.Count - 1; i++)
                        {
                            var image = news[i];
                            db.Set<AttachmentsinTimesheet>().Add(new AttachmentsinTimesheet
                            {
                                //AttachmentId= 1,
                                TimeSheetID = timesheetid,
                                UserID = userid,
                                UploadedImages = image

                            });

                        }
                        db.SaveChanges();
                        strresponse = "Successfully images added";
                    }
                    else
                    {
                        if (news.Count != 0)
                        {
                            if (timesheets.Count != 0)
                            {
                                timesheets.ToList().ForEach(u =>
                                {
                                    db.Set<AttachmentsinTimesheet>().Remove(u);

                                });
                                // db.Set<RoleModule>().RemoveRange(_LeaveSchemeDtl);
                                db.SaveChanges();

                            }
                        }



                        for (int i = 0; i <= news.Count - 1; i++)
                        {
                            var image = news[i];
                            db.Set<AttachmentsinTimesheet>().Add(new AttachmentsinTimesheet
                            {
                                //AttachmentId= 1,
                                TimeSheetID = timesheetid,
                                UserID = userid,
                                UploadedImages = image

                            });

                        }
                        db.SaveChanges();
                        strresponse = "Successfully images added";
                    }


                }
                catch (Exception ex)
                {

                }

            }
            return strresponse;

        }
        public List<string> GetImages(int timesheetid)
        {
            List<string> strresponse = new List<string>();
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {
                    //strresponse = (from c in db.AttachmentsinTimesheets
                    //               where c.TimeSheetID == timesheetid
                    //               select new Attchments
                    //               {
                    //                  Attachmentid  = c.AttachmentId,
                    //                   Uploadedimages = c.UploadedImages

                    //               }).ToList();

                    List<AttachmentsinTimesheet> timesheets = db.Set<AttachmentsinTimesheet>().Where(s => s.TimeSheetID == timesheetid).ToList();
                    //foreach (imglist as timesheets)
                    //{

                    //}
                    timesheets.ToList().ForEach(u =>
                    {
                        strresponse.Add(u.UploadedImages);
                    });
                    return strresponse;

                }
                catch (Exception ex)
                {

                }

            }
            return strresponse;

        }



        #region kalyani

        public string AddSelectedManager(ClientEntity ManagerDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();
            bool Manager2;
            int Manager3;
            int? manager2 = ManagerDtl.Managername2;
            if (manager2 == 0 || manager2 == null)
            {
                Manager2 = false;
            }
            else
            {
                Manager2 = true;
            }
            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {

                    UserProject up = new UserProject();


                    up.TimesheetMode = ManagerDtl.TimesheetMode_id;
                    up.UProj_UserID = ManagerDtl.Usr_UserID;
                    up.UProj_ProjectID = ManagerDtl.Proj_ProjectID;
                    up.UProj_StartDate = System.DateTime.Now;
                    up.UProj_EndDate = System.DateTime.Now;
                    up.UProj_ParticipationPercentage = 0;
                    up.Is_L1_Manager = true;
                    up.Is_L2_Manager = Manager2;
                    up.UProj_CreatedBy = ManagerDtl.UProj_CreatedBy;
                    up.UProj_CreatedDate = DateTime.Now;
                    up.ClientprojID = ManagerDtl.clientProjId;
                    up.IsDirectManager = ManagerDtl.IsDirectManager;
                    up.isclientcalendar = ManagerDtl.isclientholidays;
                    db.Set<UserProject>().Add(up);
                    db.SaveChanges();



                    response = "1";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }


        public string AddSelectedEmployee(ClientEntity ManagerDtl)
        {
            string response = string.Empty;
            UserSessionInfo objinfo = new UserSessionInfo();

            using (var db = new DbContext(CONNECTION_NAME))
            {
                try
                {

                    UserProject up = new UserProject();
                    up.UProj_ProjectID = ManagerDtl.Proj_ProjectID;
                    up.UProj_UserID = ManagerDtl.Usr_UserID;
                    up.UProj_StartDate = System.DateTime.Now;
                    up.UProj_EndDate = System.DateTime.Now;
                    up.UProj_ParticipationPercentage = 0;
                    up.UProj_Version = 1;
                    up.UProj_CreatedBy = ManagerDtl.UProj_CreatedBy;
                    up.UProj_CreatedDate = DateTime.Now;
                    up.UProj_L1_ManagerId = ManagerDtl.ManagerName;
                    up.UProj_L2_ManagerId = ManagerDtl.Managername2;
                    up.TimesheetMode = ManagerDtl.TimesheetMode_id;
                    up.ClientprojID = ManagerDtl.clientProjId;
                    up.isclientcalendar = ManagerDtl.isclientholidays;
                    db.Set<UserProject>().Add(up);
                    db.SaveChanges();

                    // response = "Successfully Added";

                    response = "1";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public ProjectAllocationEntities GetManagerDetailByMId(int selectedManager)
        {
            //int projectid = Convert.ToInt32(proid);
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {



                    //Role GetRoleId =
                    var response = (from u in db.Users
                                    join a in db.Accounts on u.Usr_AccountID equals a.Acc_AccountID
                                    join up in db.UsersProfiles on u.Usr_UserID equals up.UsrP_UserID
                                    join uf in db.UserProjects on u.Usr_UserID equals uf.UProj_UserID
                                    join p in db.Projects on uf.UProj_ProjectID equals p.Proj_ProjectID
                                    join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID



                                    where uf.UProj_UserID == selectedManager
                                    select new ProjectAllocationEntities
                                    {
                                        Usr_UserID = u.Usr_UserID,

                                        Proj_ProjectID = uf.UProj_ProjectID,

                                        TimesheetMode_id = uf.TimesheetMode,

                                        Cl_projid = uf.ClientprojID,
                                        isdirectmanager = uf.IsDirectManager,




                                    }).FirstOrDefault();
                    return response;
                }
                catch (Exception ex)
                {



                    throw ex;
                }
            }
        }



        //public List<ProjectEntity> GetAllProjects(int AccountId)
        //{
        //    try
        //    {
        //        using (var db = new EvolutyzCornerDataEntities())
        //        {
        //            var query = (from P in db.Projects
        //                         join A in db.Accounts on P.Proj_AccountID equals A.Acc_AccountID
        //                         where A.Acc_AccountID == AccountId
        //                         //where c.Statusid == Convert.ToBoolean(StatusEnum.Active)
        //                         select new ProjectEntity
        //                         {
        //                             Proj_ProjectID = P.Proj_ProjectID,
        //                             Proj_ProjectName = P.Proj_ProjectName,
        //                         }).ToList();
        //            return query;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}


        public List<ProjectEntity> GetAllProjects(int Proj_ProjectID)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {
                    var query = (from P in db.ClientProjects
                                     //join A in db.Accounts on P.Accountid equals A.Acc_AccountID
                                     //where P.Accountid == AccountId && P.Proj_ProjectID == Proj_ProjectID
                                 where P.Proj_ProjectID == Proj_ProjectID
                                 //where c.Statusid == Convert.ToBoolean(StatusEnum.Active)
                                 select new ProjectEntity
                                 {
                                     clientprojId = P.CL_ProjectID,
                                     Proj_ProjectID = (int)P.Proj_ProjectID,
                                     Proj_ProjectName = P.ClientProjTitle,
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<UserEntity> getManagersBySelectProject(int ProjectId, int CL_ProjId, int AccountId/*,int UserID*/)
        {
            List<UserEntity> entity = new List<UserEntity>();
            using (var db = new EvolutyzCornerDataEntities())
            {
                var getusersdata = db.UserProjects.Where(a => a.UProj_ProjectID == ProjectId && a.ClientprojID == CL_ProjId).Select(a => a.UProj_UserID).ToList();


                var getuser = db.UserProjects.Where(a => a.UProj_ProjectID == ProjectId).FirstOrDefault();
                var query = (from u in db.Users
                             join up in db.UserProjects on u.Usr_UserID equals up.UProj_UserID
                             join us in db.UsersProfiles on u.Usr_UserID equals us.UsrP_UserID
                             join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                             where
                              //up.UProj_ProjectID != ProjectId
                              up.ClientprojID != CL_ProjId
                             && r.Rol_RoleName == 1007 && r.Rol_RoleName != 1002
                             && u.Usr_AccountID == AccountId
                             select new UserEntity
                             {
                                 Usr_UserID = us.UsrP_UserID,
                                 UsrP_FirstName = us.UsrP_FirstName + "" + us.UsrP_LastName,
                             }).Distinct().ToList();
                foreach (var id in getusersdata)
                {
                    query = query.Where(a => a.Usr_UserID != id).ToList();
                }
                return query;



                //if (getuser != null)
                //{

                //    try
                //    {
                //        SqlConnection conn = new SqlConnection(str);
                //        conn.Open();
                //        SqlCommand cmd = new SqlCommand("GetManagersByProjectId", conn);
                //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //        cmd.Parameters.AddWithValue("@ProjectID", ProjectId);
                //        cmd.Parameters.AddWithValue("@CL_ProjId", CL_ProjId);
                //        cmd.Parameters.AddWithValue("@AccountID", AccountId);

                //        SqlDataReader sdr = cmd.ExecuteReader();
                //        while (sdr.Read())
                //        {
                //            entity.Add(new UserEntity
                //            {
                //                Usr_UserID = Convert.ToInt32(sdr["Usr_UserID"]),
                //                UsrP_FirstName = sdr["UsrP_FirstName"].ToString() + "" + sdr["UsrP_LastName"].ToString(),
                //            });
                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //        throw ex;
                //    }


                //}
                //else
                //{

                //}


            }

            //return entity;


        }


        public List<UserEntity> getEmployeesBySelectProject(int ProjectId, int AccountId, int CL_ProjId)
        {
            List<UserEntity> entity = new List<UserEntity>();
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    //var test = (from u in db.Users
                    //            join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                    //            join upf in db.UsersProfiles on u.Usr_UserID equals upf.UsrP_UserID
                    //            join up in db.UserProjects on u.Usr_UserID equals up.UProj_UserID
                    //            // join cp in db.ClientProjects on u.Usr_AccountID equals cp.Accountid
                    //            join cp in db.ClientProjects on up.ClientprojID equals cp.CL_ProjectID


                    //            where
                    //            r.Rol_RoleName != 1007
                    //            && (cp.CL_ProjectID != CL_ProjId
                    //            || up.UProj_ProjectID != ProjectId)
                    //            && cp.Accountid == AccountId
                    //            select new UserEntity
                    //            {
                    //                Usr_UserID = u.Usr_UserID,
                    //                UsrP_FirstName = upf.UsrP_FirstName + "" + upf.UsrP_LastName,

                    //            }).Distinct().ToList();
                    SqlConnection conn = new SqlConnection(str);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GetEmployeesByProjectId", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectID", ProjectId);
                    cmd.Parameters.AddWithValue("@CL_ProjId", CL_ProjId);
                    cmd.Parameters.AddWithValue("@AccountID", AccountId);

                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        entity.Add(new UserEntity
                        {
                            Usr_UserID = Convert.ToInt32(sdr["Usr_UserID"]),
                            UsrP_FirstName = sdr["UsrP_FirstName"].ToString() + "" + sdr["UsrP_LastName"].ToString(),
                        });
                    }




                }
                catch (Exception ex)
                {
                    throw ex;

                }



            }

            return entity;


        }

        public List<UserEntity> GetL2ManagerNamesforNewEmp(int projid)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    var ManagerNames = (from UPF in db.UsersProfiles
                                        join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                        where UP.Is_L1_Manager == true && UP.UProj_ProjectID == projid && UPF.UsrP_isDeleted == false
                                        select new UserEntity
                                        {
                                            Usr_UserID = UPF.UsrP_UserID,
                                            UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                        }).ToList();
                    //ManagerNames.Add(new UserEntity
                    //{
                    //    //Usr_UserID = ,
                    //    UsrP_FirstName = "Select Manager"
                    //});
                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();
                    //ManagerNames.Add(new UserEntity
                    //{
                    //    Usr_UserID = 0,
                    //    UsrP_FirstName = "Select Manager"
                    //});


                    return ManagerNames;


                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }


        public List<OrganizationAccountEntity> GetRoleNamesbyemployee(int AccountId)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.GenericRoles
                                 join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                                 where UT.GenericRoleID != 1007 && r.Rol_AccountID == AccountId && UT.GenericRoleID != 1002
                                 select new OrganizationAccountEntity
                                 {
                                     GenericRoleID = r.Rol_RoleID,
                                     Title = r.Rol_RoleDescription
                                 }).ToList();

                    //var query = (from UT in db.GenericRoles
                    //             join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                    //             where UT.GenericRoleID != 1007 && r.Rol_AccountID == AccountId
                    //             select new OrganizationAccountEntity
                    //             {
                    //                 GenericRoleID = UT.GenericRoleID,
                    //                 Title = UT.Title
                    //             }).ToList();
                    //return query;
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<OrganizationAccountEntity> GetEmpRolenames(int AccountId)
        {
            try
            {
                using (var db = new EvolutyzCornerDataEntities())
                {

                    var query = (from UT in db.GenericRoles
                                 join r in db.Roles on UT.GenericRoleID equals r.Rol_RoleName
                                 where (UT.GenericRoleID == 1007 || UT.GenericRoleID == 1006 || UT.GenericRoleID == 1061) && r.Rol_AccountID == AccountId
                                 select new OrganizationAccountEntity
                                 {
                                     GenericRoleID = r.Rol_RoleID,
                                     Title = r.Rol_RoleDescription
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


        #region testmethod
        //public ProjectAllocationEntities getuserdatabyid(int userid, string proid)
        //{
        //    int projectid = Convert.ToInt32(proid);
        //    using (var db = new EvolutyzCornerDataEntities())
        //    {
        //        try
        //        {
        //            User users = null;
        //            UserProject uproject = null;
        //            Account accounts = null;
        //            UsersProfile uprofile = null;
        //            Project project = null;
        //            Role role = null;

        //            users = db.Users.Single(a => a.Usr_UserID == userid);
        //            uproject = db.UserProjects.Single(a => a.UProj_UserID == userid && a.UProj_ProjectID == projectid);
        //            accounts = db.Accounts.Single(a => a.Acc_AccountID == users.Usr_AccountID);
        //            uprofile = db.UsersProfiles.Single(a => a.UsrP_UserID == userid);
        //            project = db.Projects.Single(a => a.Proj_ProjectID == projectid);
        //            role = db.Roles.Single(a => a.Rol_RoleID == users.Usr_RoleID);
        //            ProjectAllocationEntities entities = new ProjectAllocationEntities();
        //            entities.Usr_UserID = users.Usr_UserID;
        //            entities.Usr_RoleID = role.Rol_RoleID;
        //            entities.Email = users.Usr_LoginId;
        //            entities.UsrP_EmployeeID = uprofile.UsrP_EmployeeID;
        //            entities.Proj_ProjectID = uproject.UProj_ProjectID;
        //            entities.Proj_ProjectName = project.Proj_ProjectName;
        //            entities.UProj_ParticipationPercentage = uproject.UProj_ParticipationPercentage;
        //            entities.UProj_StartDate = uproject.UProj_StartDate;
        //            entities.UProj_EndDate = uproject.UProj_EndDate;
        //            entities.Usrp_DOJ = uprofile.Usrp_DOJ;
        //            entities.UsrP_FirstName = uprofile.UsrP_FirstName;
        //            entities.Usr_Username = users.Usr_Username;
        //            entities.UsrP_LastName = uprofile.UsrP_LastName;
        //            entities.Usr_Password = (users.Usr_Password);
        //            entities.Usr_Titleid = uprofile.Usr_Titleid;
        //            entities.Usrp_MobileNumber = uprofile.Usrp_MobileNumber;
        //            entities.Usrp_CountryCode = uprofile.Usrp_CountryCode;
        //            entities.Usr_GenderId = uprofile.Usr_GenderId;
        //            entities.Usrp_ProfilePicture = uprofile.Usrp_ProfilePicture;
        //            entities.Usr_UserTypeID = users.Usr_UserTypeID;
        //            entities.Usr_TaskID = users.Usr_TaskID;
        //            entities.ManagerName = uproject.UProj_L1_ManagerId;
        //            entities.Managername2 = uproject.UProj_L2_ManagerId;
        //            entities.MName = db.Set<User>().Where(s => s.Usr_UserID == uproject.UProj_UserID).FirstOrDefault<User>().Usr_Username;
        //            entities.Mname2 = db.Set<User>().Where(s => s.Usr_UserID == uproject.UProj_UserID).FirstOrDefault<User>().Usr_Username;
        //            entities.TimesheetMode_id = uproject.TimesheetMode;
        //            entities.Usr_isDeleted = users.Usr_isDeleted;
        //            entities.Cl_projid = uproject.ClientprojID;
        //            entities.isdirectmanager = uproject.IsDirectManager;
        //            entities.isclientholiday = uproject.isclientcalendar;

        //            return entities;
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }
        //    }
        //} 
        #endregion



        public List<UserEntity> getusersforclient(int Projectid, int accountid, int clientprojectid)
        {
            // List<UserEntity> entity = new List<UserEntity>();
            using (var db = new EvolutyzCornerDataEntities())
            {
                var getprojects = db.Users.Where(a => a.Usr_AccountID == accountid).Select(a => a.Usr_UserID).ToList();

                var getusersdata = db.UserProjects.Where(a => a.UProj_ProjectID == Projectid && a.ClientprojID == clientprojectid).Select(a => a.UProj_UserID).ToList();


                var query = (from u in db.Users
                             join up in db.UserProjects on u.Usr_UserID equals up.UProj_UserID //into s
                             //from up in s.DefaultIfEmpty()
                             join Cp in db.ClientProjects on up.ClientprojID equals Cp.CL_ProjectID
                             join us in db.UsersProfiles on u.Usr_UserID equals us.UsrP_UserID
                             join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                             where //(up.UProj_ProjectID != Projectid || up.UProj_ProjectID == Projectid) &&

                              up.ClientprojID != clientprojectid
                             && r.Rol_RoleName != 1007 && r.Rol_RoleID != 1002 && r.Rol_RoleID != 1011 && r.Rol_RoleID != 1010
                             && u.Usr_AccountID == accountid
                             select new UserEntity
                             {
                                 Usr_UserID = us.UsrP_UserID,
                                 UsrP_FirstName = us.UsrP_FirstName + "" + us.UsrP_LastName + " - " + Cp.ClientProjTitle,
                             }).ToList();


                foreach (var id in getusersdata)
                {
                    query = query.Where(a => a.Usr_UserID != id).ToList();
                }




                return query;

            }

        }

        public List<UserEntity> BindManagersforProject(int CL_ProjectId, int userid, int AccountId)
        {
            using (var db = new EvolutyzCornerDataEntities())
            {
                List<UserEntity> ManagerNames = new List<UserEntity>();
                try
                {
                    dynamic Managers;


                    Managers = (from UPF in db.UsersProfiles
                                join UP in db.UserProjects on UPF.UsrP_UserID equals UP.UProj_UserID
                                where UP.Is_L1_Manager == true
                                && UP.ClientprojID == CL_ProjectId
                                && UPF.UsrP_UserID != userid
                                && UPF.UsrP_isDeleted == false
                                select new UserEntity
                                {
                                    Usr_UserID = UPF.UsrP_UserID,
                                    UsrP_FirstName = UPF.UsrP_FirstName + "" + UPF.UsrP_LastName,

                                }).ToList();


                    ManagerNames = Managers;

                    ManagerNames = ManagerNames.OrderBy(p => p.Usr_UserID).ToList();




                    return ManagerNames;


                }
                catch (Exception ex)
                {
                    return null;

                }

            }

        }

        public string DeleteProjectAssigned(int clientproj_id,int ProjectId)
        {
            Project proj = null;
            ClientProject clientProject = null;
            EvolutyzCornerDataEntities db = new EvolutyzCornerDataEntities();
            string strResponse = string.Empty;
            clientProject = db.ClientProjects.Where(a => a.CL_ProjectID == clientproj_id).FirstOrDefault();
            //proj = db.Projects.Where(s => s.Proj_ProjectID == ProjectId).FirstOrDefault();

            if (clientProject != null)
            {
                //db.Projects.Remove(proj);
                db.ClientProjects.Remove(clientProject);
                db.SaveChanges();
                strResponse = "Successfully Deleted";
            }
            return strResponse;
        }
    }
}
