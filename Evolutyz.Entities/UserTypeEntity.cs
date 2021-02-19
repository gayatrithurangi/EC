using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutyz.Entities
{
    public class UserTypeEntity : ResponseHeader
    {
        public int UsT_UserTypeID { get; set; }
        public int UsT_AccountID { get; set; }
        public string AccountName { get; set; }
        public string UsT_UserTypeCode { get; set; }
        public string UsT_UserType { get; set; }
        public string UsT_UserTypeDescription { get; set; }
        public bool UsT_ActiveStatus { get; set; }
        public int UsT_Version { get; set; }
        public System.DateTime UsT_CreatedDate { get; set; }
        public int UsT_CreatedBy { get; set; }
       public Nullable<System.DateTime> UsT_ModifiedDate { get; set; }
        public Nullable<int> UsT_ModifiedBy { get; set; }
        public bool UsT_isDeleted { get; set; }
    }


    public class imagesviewmodel
    {
        public string Url { get; set; }
    }
    public partial class History_UserTypeEntity
    {
        public int History_UserType_ID { get; set; }
        public int History_UsT_UserTypeID { get; set; }
        public int History_UsT_AccountID { get; set; }
        public string AccountName { get; set; }
        public string History_UsT_UserTypeCode { get; set; }
        public string History_UsT_UserType { get; set; }
        public string History_UsT_UserTypeDescription { get; set; }
        public bool History_UsT_ActiveStatus { get; set; }
        public int History_UsT_Version { get; set; }
        public System.DateTime History_UsT_CreatedDate { get; set; }
        public int History_UsT_CreatedBy { get; set; }
        public Nullable<System.DateTime> History_UsT_ModifiedDate { get; set; }
        public Nullable<int> History_UsT_ModifiedBy { get; set; }
        public bool History_UsT_isDeleted { get; set; }
    }


    public  class GetTickets
    {
        public int? TID { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int? TypeOfIssue { get; set; }
        public string TypeOfIssueDescription { get; set; }

        public string Ticket_raise_date { get; set; }
        public string Ticket_Forecast_date { get; set; }

        public string Ticket_Closed_date { get; set; }
        public int? closedby { get; set; }
        public string closedbyName { get; set; }
        public int? Priority { get; set; }

        public string Prioritydescription { get; set; }

        public bool IsTicketsAuthority { get; set; }

        public List<Comments> comments { get; set; }

        public List<TicketImages> ticketImages { get; set; }
    }

    public class Comments
    {
        public int? Commentid { get; set; }

        public string CommentName { get; set; }

        public int? Userid { get; set; }

        public string UserName { get; set; }
        public int? Ticketid { get; set; }

        public bool Active { get; set; }
    }

    public class TicketImages
    {
        public int? Imageid { get; set; }

        public string ImageName { get; set; }
        public int? Ticketid { get; set; }

    }

    public class QuestionsEntity
    {
        public string Number { get; set; }

        public int index { get; set; }
        public int QBID { get; set; }
        public Nullable<int> TechnologyStackId { get; set; }

        public string Question1 { get; set; }
    
        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

       public int? Answer { get; set; }

        public string Solution { get; set; }

        public string Title { get; set; }

        public int Qid { get; set; }
    }


    public class examuserdetails
    {
        public int QuestionCount { get; set; }
        public int Assessment_TimePeriod_in_sec { get; set; }
        public int Actualtimeinsec { get; set; }

        public string Name { get; set; }

        public string InterviewForPositionname { get; set; }




    }

    public class Obj
    {
        public string QuesNo { get; set; }
        public string Ans { get; set; }
        public string Actualquesno { get; set; }

        public string Description { get; set; }
    }



    public class Questionsdata
    {
        public string Number { get; set; }

        public int index { get; set; }
        public int QBID { get; set; }
        public Nullable<int> TechnologyStackId { get; set; }

        public string Question { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

        public int? Answer { get; set; }

        public int? selectedanswer { get; set; }

        public bool isAttempted { get; set; }

        public bool isCorrectanswer { get; set; }

        public string Descriptionforoptionchoosen { get; set; }


    }

}
