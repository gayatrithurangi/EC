using evolCorner.Models;
using Evolutyz.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Evolutyz.Data
{
    public class LookUpDAC
    {
        public List<TaskLookupEntity> GetLookUp()
        {
            UserSessionInfo obj = new UserSessionInfo();
            int objaccountid = obj.AccountId;
            int userid = obj.UserId;
            int roleid = obj.RoleId;


            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {

                    //var Roleid = db.Roles.Where(d => d.Rol_RoleID == roleid).FirstOrDefault().Rol_RoleName;
                    // var Roleid = db.Users.Where(d => d.Usr_UserID == userid).FirstOrDefault().Usr_RoleID;
                    //var query = (from p in db.ClientProjectsTasks
                    //             join a in db.Accounts on p.Accountid equals a.Acc_AccountID
                    //             join r in db.Roles on p.rol_roleid equals r.Rol_RoleID
                    //             join at in db.AccountSpecificTasks on p.acc_specifictaskid equals at.Acc_SpecificTaskId
                    //             where p.Accountid == objaccountid && p.rol_roleid == Roleid
                    //             select new TaskLookupEntity
                    //             {
                    //                 tsk_TaskID = p.acc_specifictaskid,
                    //                 tsk_TaskName = at.Acc_SpecificTaskName
                    //             }).ToList();
                    //// }
                    /// var query = (from AT in db.ClientProjectsTasks
                    //join ut in db.GenericTasks on AT.acc_specifictaskid equals ut.tsk_TaskID
                    //where AT.Accountid == objaccountid && AT.rol_roleid == roleid


                    //             select new TaskLookupEntity
                    //             {
                    //                 Proj_SpecificTaskId = AT.CL_ProjectsTasksID,
                    //                 Proj_SpecificTaskName = ut.tsk_TaskName
                    //             }).ToList();
                    //return query;
                    var query = (from UT in db.ClientProjectsTasks
                                 join cp in db.GenericTasks on UT.acc_specifictaskid equals cp.tsk_TaskID
                                 where UT.rol_roleid == roleid && UT.Accountid == objaccountid
                                 select new TaskLookupEntity
                                 {
                                     tsk_TaskID = UT.CL_ProjectsTasksID,
                                     tsk_TaskName = cp.tsk_TaskName
                                 }).ToList();
                    return query;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<ProjectEntity> GetLoadProjects()
        {
            UserSessionInfo obj = new UserSessionInfo();
            int objaccountid = obj.AccountId;
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {


                    var query1 = (from p in db.Projects
                                  join up in db.UserProjects on p.Proj_ProjectID equals up.UProj_ProjectID
                                  join u in db.Users on up.UProj_UserID equals u.Usr_UserID
                                  join r in db.Roles on u.Usr_RoleID equals r.Rol_RoleID
                                  where p.Proj_AccountID == objaccountid
                                  select new ProjectEntity
                                  {

                                      Proj_ProjectID = p.Proj_ProjectID,
                                      Proj_ProjectName = p.Proj_ProjectName,
                                  }).GroupBy(n => new { n.Proj_ProjectID, n.Proj_ProjectName })
                                     .Select(p => p.FirstOrDefault()).ToList();
                    return query1;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }



        public List<TaskLookupEntity> GetLookUpByEmpId(int Userid)
        {
            UserSessionInfo obj = new UserSessionInfo();
            int objaccountid = obj.AccountId;
            int roleid = obj.RoleId;
            using (var db = new EvolutyzCornerDataEntities())
            {
                try
                {
                    var Roleid = db.Users.Where(d => d.Usr_UserID == Userid).FirstOrDefault().Usr_RoleID;
                    var Roleid2 = db.Roles.Where(d => d.Rol_RoleID == Roleid).FirstOrDefault().Rol_RoleName;

                    var query1 = (from UT in db.AccountSpecificTasks
                                  join cp in db.ClientProjectsTasks on UT.Acc_SpecificTaskId equals cp.acc_specifictaskid
                                  where cp.rol_roleid == Roleid2 && cp.Accountid == objaccountid
                                  select new TaskLookupEntity
                                  {
                                      tsk_TaskID = cp.acc_specifictaskid,
                                      tsk_TaskName = UT.Acc_SpecificTaskName
                                  }).ToList();
                    return query1;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


    }
}
