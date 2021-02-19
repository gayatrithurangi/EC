using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutyz.Entities
{
    public class UniversaltopicEntity
    {
        public int? VideoID { get; set; }
        public int UniversalTopicid { get; set; }
        public string Topiccode { get; set; }
        public string Topic { get; set; }
        public string image { get; set; }
        public Nullable<int> view { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> StatusID { get; set; }
        public string Status { get; set; }
        public int? userid { get; set; }
        public string visibility { get; set; }
        public string FirstName { get; set; }
        public string Modify { get; set; }

    }
}
