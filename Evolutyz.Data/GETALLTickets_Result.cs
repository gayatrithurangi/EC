//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Evolutyz.Data
{
    using System;
    
    public partial class GETALLTickets_Result
    {
        public int TID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public Nullable<int> TypeOfIssue { get; set; }
        public string TypeOfIssueDescription { get; set; }
        public string Ticket_raise_date { get; set; }
        public Nullable<System.DateTime> Ticket_Forecast_date { get; set; }
        public string Ticket_Closed_date { get; set; }
        public Nullable<int> ClosedBy { get; set; }
        public string closedbyName { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Prioritydescription { get; set; }
    }
}
