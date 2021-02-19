using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutyz.Entities
{
    public class managercomments
    {
        public string AccntMail { get; set; }
        public string Userid { get; set; }
        public string Statuses { get; set; }
        public string LeaveId { get; set; }
        public string ManagerMail { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string ManagerLevel { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class managerwfhcomments
    {
        public string AccntMail { get; set; }
        public string WfhUserid { get; set; }
        public string WfhStatuses { get; set; }
        public string userWFHId { get; set; }
        public string WfhManagerMail { get; set; }
        public string WfhManagerId { get; set; }
        public string WfhManagerName { get; set; }
        public string WfhManagerLevel { get; set; }
        public string WfhStartDate { get; set; }
        public string WfhEndDate { get; set; }
    }
}
