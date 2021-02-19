using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.SessionState;

namespace evolCorner.Models
{
    public class UserProjectdetailsEntity
    {

        private static HttpSessionState session { get { return HttpContext.Current.Session; } }


        public string AccountName { get; set; }

        public int User_ID
        {
            get
            {
                return (int)session["userID"];
            }
            set
            {
                session["userID"] = value;
            }
        }
        public int Account_ID
        {
            get
            {
                return (int)session["AccountID"];
            }
            set
            {
                session["AccountID"] = value;
            }
        }


        public string Usr_Username
        {
            get
            {
                return (string)session["Usr_Username"];
            }
            set
            {
                session["Usr_Username"] = value;
            }
        }


        public string UserType { get; set; }

        public string Usr_LoginId
        {
            get
            {
                return (string)session["Usr_LoginId"];
            }
            set
            {
                session["Usr_LoginId"] = value;
            }
        }

        public string Usr_Password
        {
            get
            {
                return (string)session["Usr_Password"];
            }
            set
            {
                session["Usr_Password"] = value;
            }
        }


        // public string Usr_Username { get; set; }
        //  public string Usr_LoginId { get; set; }
        // public string Usr_Password { get; set; }
        public int Proj_ProjectID { get; set; }
        public string projectName { get; set; }
        public string ProjectClientName { get; set; }
        public int tsktaskID { get; set; }

        public int? ClientProjectId  { get; set; }

        public string RoleCode
        {
            get
            {
                return (string)session["RoleCode"];
            }
            set
            {
                session["RoleCode"] = value;
            }
        }

    }


    public class ProductViewModel
    {
        public int? departmentId { get; set; }

        public string TicketDescription { get; set; }

        public int? IssueType { get; set; }

        public int? priority { get; set; }

        public string comments { get; set; }

        public DateTime? forecastdate { get; set; }
        public string ProductName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> ImageId { get; set; }
        public int? Tid { get; set; }
        public HttpPostedFileWrapper ImageFile { get; set; }

        public List<TImages> TImages { get; set; }
    }

    public class TImages
    {
        public string Imagename { get; set; }
    }


    public class AssessmentForPositions
    {
        public int InterviewForPositionId { get; set; }
        public string InterviewForPositionname { get; set; }

        public string Description { get; set; }

        public List<Position_TechnologyStack_List> Position_TechnologyStack_List { get; set; }
    }




    public class Position_TechnologyStack_List
    {

        public int Technologyid { get; set; }

        public string Title { get; set; }
      
        public bool Ischecked { get; set; }

        public int? NoofQuestions { get; set; }

        public int? Timeinseconds { get; set; }
    }


    public class GetallPositions
    {
        public int APID { get; set; }

        public string InterviewForPositionname { get; set; }

        public string Description { get; set; }
    }

}