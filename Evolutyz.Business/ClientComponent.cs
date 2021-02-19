using Evolutyz.Data;
using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
namespace Evolutyz.Business
{
    public class ClientComponent
    {

        public List<OrganizationAccountEntity> GetAllRoles()
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetAllRoles();
        }
        public List<OrganizationAccountEntity> GetAllNewRoles()
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetAllNewRoles();
        }
        public List<ClientEntity> getclientprojects(int projid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.getclientprojects(projid);
        }
        public List<ClientEntity> getclientprojectsdropdown(int projid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.getclientprojectsdropdown(projid);
        }
        public ProjectAllocationEntities GetClientProjbyid(int orgID)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.GetClientProjbyid(orgID);
        }

        public ProjectAllocationEntities GetUserDetailById(int orgID, string proid, int CL_ProjID)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.GetUserDetailById(orgID, proid,  CL_ProjID);
            //return ProjectDAC.getuserdatabyid(orgID, proid);

            
        }
        public ProjectAllocationEntities GetassUserDetailById(int orgID)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.GetassUserDetailById(orgID);
        }
        public string updateuserdetails(ClientEntity projectdtl)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.updateuserdetails(projectdtl);
        }

        public string Savemanager(ClientEntity orgAccount)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.Savemanager(orgAccount);
        }
        public string SaveEmployee(ClientEntity orgAccount)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.SaveEmployee(orgAccount);
        }

        public string AssociateEmployee(ClientEntity orgAccount)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.AssociateEmployee(orgAccount);
        }
        public string AssociateManager(ClientEntity orgAccount)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.AssociateManager(orgAccount);
        }

        public List<ProjectAllocationEntities> GetGenders()
        {
            var count = new ClientDAC();
            return count.GetGenders();
        }
        public List<ProjectAllocationEntities> Getaccountspecifictasks()
        {
            var count = new ClientDAC();
            return count.Getaccountspecifictasks();
        }

        public List<ProjectAllocationEntities> GetTitle()
        {
            var count = new ClientDAC();
            return count.GetTitle();
        }
        public List<ProjectAllocationEntities> GetCountryCodes()
        {
            var count = new ClientDAC();
            return count.GetCountryCodes();
        }
        public List<ProjectAllocationEntities> GetProjectUserDetails(int id)
        {
            var count = new ClientDAC();
            return count.GetProjectUserDetails(id);
        }


        public List<ProjectAllocationEntities> GetHolidays(int projectid)
        {
            var count = new ClientDAC();
            return count.GetHolidays(projectid);
        }
        public List<ProjectAllocationEntities> Getprotasks(int projectid)
        {
            var count = new ClientDAC();
            return count.Getprotasks(projectid);
        }
        public List<ProjectAllocationEntities> getprojecttaskbyid(int projectid)
        {
            var count = new ClientDAC();
            return count.getprojecttaskbyid(projectid);
        }

        public List<ProjectAllocationEntities> GetAlltasknames(int projectid, int Roleid)
        {
            var count = new ClientDAC();
            return count.GetAlltasknames(projectid, Roleid);
        }
        public List<ProjectAllocationEntities> GetAllEmptasknames(int projectid, int Roleid)
        {
            var count = new ClientDAC();
            return count.GetAllEmptasknames(projectid, Roleid);
        }
        public List<ProjectAllocationEntities> gettasknames(int projectid)
        {
            var count = new ClientDAC();
            return count.gettasknames(projectid);
        }


        public List<UserEntity> GetManagerNames2(int projid/*, int CL_Projid*/)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetManagerNames2(projid/*,CL_Projid*/);
        }

        public List<UserEntity> GetL2ManagerNamesforNewEmp(int projid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetL2ManagerNamesforNewEmp(projid);
        }
        public List<UserEntity> GetManagerByRole(int projid, int userid, int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.ManagerByRole(projid, userid, AccountId);
        }

        public List<UserEntity> BindManagerByProject(int projid, int userid, int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.BindManagersforProject(projid, userid, AccountId);
        }



        public List<UserEntity> GetManagerOnChange(int projid, int ManagerID,int Client_ProjId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetManagerOnChange(projid, ManagerID, Client_ProjId);
        }
        public List<UserEntity> GetManager1onProjectChange(int CL_ProjectId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetManager1onProjectChange(CL_ProjectId);
        }

        public List<UserEntity> GetManagerNames(int projid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetManagerNames(projid);
        }

        #region To get all countryNames from data access layer

        public List<ProjectEntity> GetCountryNames()
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetCountryNames();

        }

        public List<ProjectEntity> GetTimeSheetModes(int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetTimeSheetModes(AccountId);
        }

        #region To get all details of Role Names Table from Data access layer

        public List<OrganizationAccountEntity> GetUserRolenames(int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetRoleNames(AccountId);
        }
        public List<OrganizationAccountEntity> GetRoleNamesbyemp(int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetRoleNamesbyemp(AccountId);
        }
        #endregion
        #endregion
        #region To call add method of ProjectAllocation from Data access layer
        public int AddManager(ProjectAllocationEntity _ProjectAllocation)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.AddManager(_ProjectAllocation);
        }
        #endregion
        #region To get all StateNames from data access layer

        public List<ProjectEntity> GetStateNames(int CountryId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetStateNames(CountryId);

        }

        #endregion

        #region To call add method of Project from Data access layer
        public ProjectEntity AddProject(ProjectEntity _Project)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.AddProject(_Project);
        }
        #endregion

        #region To get all details of Account Table from Data access layer
        public ProjectEntity GetClientDetailByID(int orgID)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.GetClientDetailByID(orgID);
        }
        #endregion

        #region To call update method of UserType Table from Data access layer
        public int UpdateclientDetails(ProjectEntity _uesrtype)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.UpdateclientDetails(_uesrtype);
        }
        #endregion

        public int DeleteUser(int userid)
        {
            var count = new ClientDAC();
            return count.DeleteUser(userid);
        }

        public string Addprotasks(string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            var count = new ClientDAC();
            return count.Addprotasks(Acc_SpecificTaskName, ProjectID, Proj_SpecificTaskName, RTMId, Actual_StartDate, Actual_EndDate, Plan_StartDate, Plan_EndDate, StatusId);
        }
        public string updatetasks(int id, string Acc_SpecificTaskName, string ProjectID, string Proj_SpecificTaskName, string RTMId, string Actual_StartDate, string Actual_EndDate, string Plan_StartDate, string Plan_EndDate, string StatusId)
        {
            var count = new ClientDAC();
            return count.updatetasks(id, Acc_SpecificTaskName, ProjectID, Proj_SpecificTaskName, RTMId, Actual_StartDate, Actual_EndDate, Plan_StartDate, Plan_EndDate, StatusId);
        }

        public string DeleteProjecttask(string id)
        {
            ClientDAC dacobj = new ClientDAC();
            return dacobj.DeleteProjecttask(id);
        }

        public List<ProjectAllocationEntities> Associatemanagers(int projectid, int accountid)
        {
            var count = new ClientDAC();
            return count.Associatemanagers(projectid, accountid);
        }
        public List<ProjectAllocationEntities> AssociateEmployees(int projectid, int accountid)
        {
            var count = new ClientDAC();
            return count.AssociateEmployees(projectid, accountid);
        }


        public string ChangeStatus(string id, string status)
        {
            var LeaveTypeDAC = new ClientDAC();
            return LeaveTypeDAC.ChangeStatus(id, status);
        }

        public string SavetimeSheetImages(int timesheetid, int? userid, List<string> news,int? upid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.SavetimeSheetImages(timesheetid, userid, news,upid);
        }
        public List<string> GetImages(int timesheetid)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetImages(timesheetid);
        }

        #region Kalyani 

        public string AddSelectedManager(ClientEntity formdata)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.AddSelectedManager(formdata);
        }
        public string AddSelectedEmployee(ClientEntity formdata)
        {
            var clientDAC = new ClientDAC();
            return clientDAC.AddSelectedEmployee(formdata);
        }


        public ProjectAllocationEntities GetManagerDetailByMId(int selectedManager)
        {
            var ProjectDAC = new ClientDAC();
            return ProjectDAC.GetManagerDetailByMId(selectedManager);
        }

        public List<ProjectEntity> GetAllProjects(int Proj_ProjectID)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetAllProjects(Proj_ProjectID);
        }
        public List<UserEntity> getManagersBySelectProject(int ProjectId, int CL_ProjId, int AccountId/*,int UserID*/)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.getManagersBySelectProject(ProjectId, CL_ProjId, AccountId/*, UserID*/);

        }
        public List<UserEntity> getEmployeesBySelectProject(int ProjectId, int AccountId, int CL_ProjId)
        {
            var ClientDAC = new ClientDAC();
           //return ClientDAC.getEmployeesBySelectProject(ProjectId, AccountId, CL_ProjId);
            return ClientDAC.getusersforclient(ProjectId, AccountId, CL_ProjId);
            

        }
        public List<OrganizationAccountEntity> GetRoleNamesbyemployee(int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetRoleNamesbyemployee(AccountId);
        }

        public List<OrganizationAccountEntity> GetEmpRolenames(int AccountId)
        {
            var ClientDAC = new ClientDAC();
            return ClientDAC.GetEmpRolenames(AccountId);
        }
        public string DeleteProjectsData(int client_id,int Projectid)
        {
            var count = new ClientDAC();
            return count.DeleteProjectAssigned(client_id, Projectid);
        }
        #endregion
    }
}
