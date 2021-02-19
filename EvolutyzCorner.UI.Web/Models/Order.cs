using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvolutyzCorner.UI.Web.Models
{
    public class Order
    {

        public DateTime OrderDate { get; set; }
        public string Region { get; set; }
        public string Rep { get; set; }
        public string Item { get; set; }
        public int Units { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }

        public override string ToString()
        {
            return string.Format("Order {0} rep: {1,8} ({2,7}) item: {3,7} {4,2} x {6,7} = {6:c2}", OrderDate.ToString("yyyy-MM-dd"), Rep, Region, Item, Units, UnitCost, Total);
        }

    }
    public class Employee
    {

        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        //  Designation
        public DateTime DateOfJoining { get; set; }
        public string Department { get; set; }
        public string WorkingDays { get; set; }
        public string DaysWorked { get; set; }
        public string PayableDays { get; set; }
        public string LOPDays { get; set; }
        public string EmployeeNo { get; set; }
        public string BankAcNO { get; set; }
        public string PANNo { get; set; }
        public string UANNo { get; set; }
        public string PFNo { get; set; }
        public string ESINo { get; set; }
        public string PayMode { get; set; }
        public string PayDate { get; set; }
        public string LTA1 { get; set; }
        public string LTA2 { get; set; }
        public string LTA3 { get; set; }

        public string Basic1 { get; set; }
        public string HRA1 { get; set; }
        public string Convey1 { get; set; }
        public string Education1 { get; set; }
        public string Medical1 { get; set; }
        public string SpecialAllowance1 { get; set; }
        public string FoodAllowance1 { get; set; }
        public string IncentivesBonus1 { get; set; }

        
        public string Basic2 { get; set; }
        public string HRA2 { get; set; }
        public string Convey2 { get; set; }
        public string Education2 { get; set; }
        public string Medical2 { get; set; }
        public string SpecialAllowance2 { get; set; }
        public string FoodAllowance2 { get; set; }
        public string IncentivesBonus2 { get; set; }
        public string TotalGrossSalary { get; set; }

        public string HouseRentAllow { get; set; }
        public string ConveyanceAllow { get; set; }
        public string EducationAllow { get; set; }
        public string MedicalAllow { get; set; }
        public string SpecialAllow { get; set; }
        public string FoodAllow { get; set; }
        public string Others { get; set; }
        public string TOTALEARNINGS { get; set; }
        public string EPF { get; set; }
        public string ProfessionalTax { get; set; }
        public string TDS { get; set; }
        // public string LTA { get; set; }
        public string ESI { get; set; }
        public string Advance { get; set; }
        public string Other { get; set; }
        public string TOTALDEDUCTIONS { get; set; }
        public string NETSALARY { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
    }
}































