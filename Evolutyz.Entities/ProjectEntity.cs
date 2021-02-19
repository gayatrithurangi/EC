using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutyz.Entities
{


    public class ProjectEntity : ResponseHeader
    {
        public int Proj_ProjectID { get; set; }
        public int Proj_AccountID { get; set; }
        public string AccountName { get; set; }
        public string Proj_ProjectCode { get; set; }
        public string Proj_ProjectName { get; set; }
        public string Proj_ProjectDescription { get; set; }
        public System.DateTime Proj_StartDate { get; set; }
        public DateTime? Proj_EndDate { get; set; }
        public bool Proj_ActiveStatus { get; set; }
        public short Proj_Version { get; set; }
        public System.DateTime Proj_CreatedDate { get; set; }
        public int Proj_CreatedBy { get; set; }
        public Nullable<System.DateTime> Proj_ModifiedDate { get; set; }
        public Nullable<int> Proj_ModifiedBy { get; set; }
        public bool Proj_isDeleted { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string CountryName { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateName { get; set; }
        public int TimesheetMode_id { get; set; }
        public string TimeModeName { get; set; }
        public string WebUrl { get; set; }
        public bool? Is_Timesheet_ProjectSpecific { get; set; }
        public int clientprojId { get; set; }
        public string CL_ProjectTitle { get; set; }
    }




    public class ProjectAllocationEntities : ResponseHeader
    {

        public int CL_ProjectId { get; set; }

        public string ClientProjDesc { get; set; }

        public string ProjectName { get; set; }
        public string PhoneCode { get; set; }
        public int Proj_AccountID { get; set; }
        public int UProj_UserProjectID { get; set; }
        public int UProj_ProjectID { get; set; }
        public int UProj_UserID { get; set; }
        public string Usrp_ProfilePicture { get; set; }
        public int Proj_ProjectID { get; set; }
        [RegularExpression("[^ ]+ [^ ]+")]
        public string Email { get; set; }
        public int? TimesheetMode_id { get; set; }
        public string TimeModeName { get; set; }
        public string Usr_Password { get; set; }
        public string Usr_Username { get; set; }
        public string Usr_ConfirmPassword { get; set; }
        public bool Usr_ActiveStatus { get; set; }
        public Nullable<int> Usrp_CountryCode { get; set; }
        public int? Usr_Titleid { get; set; }
        public string UsrP_FirstName { get; set; }
        public string UsrP_LastName { get; set; }
        public string UsrP_EmployeeID { get; set; }
        public Nullable<System.DateTime> Usrp_DOJ { get; set; }
        public int Usr_UserTypeID { get; set; }
        public string RoleName { get; set; }
        public int? ManagerName { get; set; }
        public int? Managername2 { get; set; }
        public int? LeadforManager { get; set; }

        public string MName { get; set; }
        public string Mname2 { get; set; }
        public bool Proj_isDeleted { get; set; }
        public Nullable<int> Usr_TaskID { get; set; }
        public byte UProj_ParticipationPercentage { get; set; }
        public System.DateTime UProj_StartDate { get; set; }
        public System.DateTime? UProj_EndDate { get; set; }
        public bool UProj_ActiveStatus { get; set; }
        public int? Usr_GenderId { get; set; }
        public string Gender { get; set; }
        public int UProj_CreatedBy { get; set; }
        public string Proj_ProjectDescription { get; set; }
        public bool Proj_ActiveStatus { get; set; }
        public System.DateTime Proj_StartDate { get; set; }
        public Nullable<System.DateTime> Proj_EndDate { get; set; }
        public string WebUrl { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Proj_ProjectCode { get; set; }
        public string Proj_ProjectName { get; set; }
        public string AccountName { get; set; }
        public string TitlePrefix { get; set; }
        public int Usr_UserID { get; set; }
        public int? Usr_RoleID { get; set; }
        public int? roleid { get; set; }
        public int HolidayCalendarID { get; set; }
        public int AccountID { get; set; }
        public bool Usr_isDeleted { get; set; }
        public string HolidayName { get; set; }
        public int? Year { get; set; }
        public System.DateTime HolidayDate { get; set; }
        public string HolidayWeek { get; set; }
        public bool isOptionalHoliday { get; set; }
        public double? optionalholidays { get; set; }
        public int? useroptionalholidays { get; set; }
        public bool IsDirectManager { get; set; }
        public bool isActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public int? FinancialYearId { get; set; }
        public Nullable<int> StartDate { get; set; }
        public Nullable<int> EndDate { get; set; }
        public string financialyear { get; set; }

        public int Acc_SpecificTaskId { get; set; }
        public string Acc_SpecificTaskName { get; set; }

        public Nullable<bool> isdirectmanager { get; set; }
        public string Usrp_MobileNumber { get; set; }
        public int Proj_SpecificTaskId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Proj_SpecificTaskName { get; set; }
        public string RTMId { get; set; }
        public Nullable<System.DateTime> Actual_StartDate { get; set; }
        public Nullable<System.DateTime> Actual_EndDate { get; set; }
        public Nullable<System.DateTime> Plan_StartDate { get; set; }
        public Nullable<System.DateTime> Plan_EndDate { get; set; }
        public Nullable<bool> StatusId { get; set; }
        public Nullable<int> tsk_TaskID { get; set; }
        public bool Is_Timesheet_ProjectSpecific { get; set; }

        public int? managerslist { get; set; }
        public int? employeeslist { get; set; }

        public string clientprojecttitle { get; set; }
        public string clientprojectdescription { get; set; }
        public string client { get; set; }
        public string project { get; set; }
        public int? Cl_projid { get; set; }
        public int? Accid { get; set; }
        public Nullable<bool> isclientholiday { get; set; }
        public int? ManagerNames1 { get; set; }
        public int? UserNames { get; set; }
        public string EmpRoleName { get; set; }
        //public string EmpTaskName { get; set; }
        public Nullable<int> EmpTaskName { get; set; }
        public int? ManagerName1foremp { get; set; }
        public int? Managername2foremp { get; set; }
        public int? HolidayProjectId { get; set; }
        public int? User_RoleId { get; set; }
    }

    public class ClientEntity
    {
        public int TimesheetMode_id { get; set; }
        public string TimeModeName { get; set; }
        // public int UProj_ProjectID { get; set; }
        // public int UProj_UserID { get; set; }
        public string Usrp_ProfilePicture { get; set; }
        public int Proj_ProjectID { get; set; }
        public string Email { get; set; }
        public bool IsDirectManager { get; set; }
        public Nullable<int> Usrp_CountryCode { get; set; }
        public string Usr_Password { get; set; }
        public string Usr_Username { get; set; }
        public string Usr_ConfirmPassword { get; set; }
        public bool Usr_isDeleted { get; set; }
        public string Usrp_MobileNumber { get; set; }
        public int? Usr_Titleid { get; set; }
        public string UsrP_FirstName { get; set; }
        public string UsrP_LastName { get; set; }
        public string UsrP_EmployeeID { get; set; }
        public Nullable<System.DateTime> Usrp_DOJ { get; set; }
        public int Usr_UserTypeID { get; set; }
        public string RoleName { get; set; }
        public int? ManagerName { get; set; }
        public int? Managername2 { get; set; }
        public Nullable<int> Usr_TaskID { get; set; }
        public byte UProj_ParticipationPercentage { get; set; }
        public System.DateTime UProj_StartDate { get; set; }
        public System.DateTime? UProj_EndDate { get; set; }
        public bool UProj_ActiveStatus { get; set; }
        public int? Usr_GenderId { get; set; }
        public int UProj_CreatedBy { get; set; }
        public int Usr_UserID { get; set; }
        public Nullable<bool> Is_L1_Manager { get; set; }
        public Nullable<bool> Is_L2_Manager { get; set; }
        public int managertype { get; set; }
        public int Rol_RoleName { get; set; }
        public int CL_ProjectID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> Accountid { get; set; }
        public string ClientProjTitle { get; set; }
        public string ClientProjDesc { get; set; }
        public bool isclientholidays { get; set; }
        public int? Leadformanager { get; set; }
        public int clientProjId { get; set; }
        public int? ManagerName1foremp { get; set; }
        public int? ManagerName2foremp { get; set; }
        public string imgCropped { get; set; }
        public bool hasUsers { get; set; }

    }

    public class History_ProjectsEntity
    {
        public int History_Project_ID { get; set; }
        public int History_Proj_ProjectID { get; set; }
        public int History_Proj_AccountID { get; set; }
        public string AccountName { get; set; }
        public string History_Proj_ProjectCode { get; set; }
        public string History_Proj_ProjectName { get; set; }
        public string History_Proj_ProjectDescription { get; set; }
        public System.DateTime History_Proj_StartDate { get; set; }
        public Nullable<System.DateTime> History_Proj_EndDate { get; set; }
        public bool History_Proj_ActiveStatus { get; set; }
        public short History_Proj_Version { get; set; }
        public System.DateTime History_Proj_CreatedDate { get; set; }
        public int History_Proj_CreatedBy { get; set; }
        public Nullable<System.DateTime> History_Proj_ModifiedDate { get; set; }
        public Nullable<int> History_Proj_ModifiedBy { get; set; }
        public bool History_Proj_isDeleted { get; set; }
    }
}
