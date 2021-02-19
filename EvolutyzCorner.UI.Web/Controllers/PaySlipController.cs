using Evolutyz.Business;
using Evolutyz.Data;
using Evolutyz.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using EvolutyzCorner.UI.Web.Models;
using System.Configuration;
using evolCorner.Models;
using System.Data.Entity.SqlServer;
using Excel.Helper;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Data.OleDb;




namespace EvolutyzCorner.UI.Web.Controllers
{
    public class PaySlipController : Controller
    {

        public static string host = System.Web.HttpContext.Current.Request.Url.Host;
        public static string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
        public static string port1 = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];


        DataTable dt = new DataTable();
        public static string s;
        public static int[][] numbers;
        public static string s1;
        public static string s2;
        public static object[][] values;
        public static Order[] orders;
        public static Employee[] employees;
        static void ReadExcelExample(ExcelDataReaderHelper excelHelper)
        {
            // worksheet info
            System.Diagnostics.Debug.WriteLine("\nNumber of Worksheets: {0} ({1})", excelHelper.WorksheetCount, string.Join(", ", excelHelper.WorksheetNames));


            employees = excelHelper.GetRange<Employee>("Sheet1", 1, 1);
            //   System.Diagnostics.Debug.WriteLine(string.Join("\n", orders.Select(x => x.ToString())));

            s2 = string.Join("\n", employees.Select(x => x.ToString()));

        }




       
        static void ReadExcelFileExample(string filename)

        {
           
            try
            {
                using (ExcelDataReaderHelper excelHelper = new ExcelDataReaderHelper(filename))
                {
                    ReadExcelExample(excelHelper);
                }
            }
            catch (Exception ex)
            {



            }


            // }
        }

       
        public ActionResult SalarySlips()
        {
          
            return View();
        }
        static string month;
            static string Year;

        [HttpPost]
        public JsonResult btnUpload_Click(PaySlip formData)
        {
            string Res = string.Empty;
            month = formData.Month;
            Year = formData.Year;
            try
            {
                string connectionString = "";
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(file.FileName);
                    string fileLocation = Server.MapPath("~/Content/" + fileName);
                    
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    file.SaveAs(fileLocation);
                    ReadExcelFileExample(fileLocation);
                   
                    foreach (var i in employees)
                    {
                        

                        Document document = new Document(PageSize.A4, 88f, 88f, 10f, 10f);
                        iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                            Phrase phrase = null;
                            PdfPCell cell = null;
                            PdfPTable table = null;
                            iTextSharp.text.BaseColor color = null;
                            document.Open();
                            //#statregion  heading 
                            table = new PdfPTable(2);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;
                            //table.SetWidths(new float[] { 0.3f, 0.3f });
                            phrase = new Phrase();
                            phrase.Add(new Chunk("M/S.EVOLUTYZ IT SERVICES PVT LTD", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                            table.AddCell(cell);



                            cell = ImageCell("~/PDFImages/img.png", 50f, PdfPCell.ALIGN_RIGHT);
                            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                            table.AddCell(cell);
                            document.Add(table);
                            //#endregion




                            table = new PdfPTable(1);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;

                            //Name
                            phrase = new Phrase();

                            //phrase.Add(new Chunk(" Pay Slip for February 2018", FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                            phrase.Add(new Chunk(" Pay Slip for " + formData.Month + " " + formData.Year, FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                            table.AddCell(cell);



                            document.Add(table);

                            document.Add(new Paragraph("  "));
                            // document.Add(new Paragraph("  "));




                            table = new PdfPTable(2);
                            table.TotalWidth = 500;
                            table.SetWidths(new float[] { 300f, 200f });
                            table.LockedWidth = true;
                            PdfPTable tableinner;
                            tableinner = new PdfPTable(2);
                            // PdfPTable table2 = new PdfPTable(2);

                            tableinner.HorizontalAlignment = Element.ALIGN_LEFT;
                            //table2.HorizontalAlignment = Element.ALIGN_RIGHT;

                            //1

                            tableinner.AddCell(PhraseCell(new Phrase("Employee Name\n" + "\n"
                                                            + "Designation    \n" + "\n"
                                                            + "Department     \n" + "\n"
                                                            + "Date of Joining  \n" + "\n"
                                                            + "Working Days    \n" + "\n"
                                                            + "Days Worked     \n" + "\n"
                                                            + "Payable Days    \n" + "\n"
                                                            + "LOP Days",
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT)).BorderColor = BaseColor.WHITE;

                            tableinner.AddCell(PhraseCell(new Phrase(i.EmployeeName + "\n"
                                                         + "\n" + i.Designation + "\n"
                                                             + "\n" + i.Department + "\n"
                                                             + "\n" + Convert.ToDateTime(i.DateOfJoining).ToShortDateString() + "\n"
                                                             + "\n" + i.WorkingDays + "\n"
                                                             + "\n" + i.DaysWorked + "\n"
                                                             + "\n" + i.PayableDays + "\n"
                                                             + "\n" + i.LOPDays,
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT)).BorderColor = BaseColor.WHITE;

                            table.AddCell(tableinner);
                            //2
                            tableinner = new PdfPTable(1);
                            tableinner.AddCell(PhraseCell(new Phrase("Employee No                              " + i.EmployeeNo + "\n"
                                                         + "\n" + "BankAcNO:   " + i.BankAcNO + "\n"
                                                         + "\n" + "PAN No:     " + i.PANNo + "\n"
                                                         + "\n" + "UAN No                             " + i.UANNo + "\n"
                                                         + "\n" + "PF No      " + i.PFNo + "\n"
                                                         + "\n" + "ESI No                               " + i.ESINo + "\n"
                                                         + "\n" + "Pay Mode                          " + i.PayMode + "\n"
                                                         + "\n" + "Pay Date                           " + Convert.ToDateTime(i.PayDate.ToString().Trim()).ToShortDateString() + "\n",
                           FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED_ALL)).BorderColor = BaseColor.WHITE; ;
                            table.AddCell(tableinner);

                            //table 2
                            document.Add(table);
                            table = new PdfPTable(5);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                            //table.SpacingBefore = 30f;

                            table.AddCell(PhraseCell(new Phrase("EARNINGS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase("GROSS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase("EARNED", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase("DEDUCTIONS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase("AMOUNT", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_LEFT));
                            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 2;
                            cell.PaddingBottom = 10f;
                            table.AddCell(cell);

                            //table3
                            document.Add(table);
                            table = new PdfPTable(5);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                            // table.SpacingBefore = 30f;
                            table.AddCell(PhraseCell(new Phrase("Basic" + "\n"
                                                     //+ "\n" + "HRA" + "\n"
                                                     + "\n" + "House Rent Allow" + "\n"
                                                      + "\n" + "Conveyance Allow" + "\n"
                                                     + "\n" + "Education Allow" + "\n"
                                                     + "\n" + "Medical Allow" + "\n"
                                                     + "\n" + "LTA" + "\n"
                                                     + "\n" + "Special Allowance" + "\n"
                                                     + "\n" + "Food Allow" + "\n"
                                                      + "\n" + "Incentives/Bonus" + "\n"
                                                     + "\n" + "Others" + "\n",
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));

                            table.AddCell(PhraseCell(new Phrase(i.Basic1 + "\n"
                                                                 + "\n" + i.HRA1 + "\n"
                                                                  + "\n" + i.Convey1 + "\n"
                                                                 + "\n" + i.Education1 + "\n"
                                                                 + "\n" + i.Medical1 + "\n"
                                                                 + "\n" + i.LTA1 + "\n"
                                                                 + "\n" + i.SpecialAllowance1 + "\n"
                                                                 + "\n" + i.FoodAllowance1 + "\n",
                                //+ "\n" + dr["Others"] + "\n",
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));

                            table.AddCell(PhraseCell(new Phrase(i.Basic2 + "\n"
                                                               + "\n" + i.HRA2 + "\n"
                                                                + "\n" + i.Convey2 + "\n"
                                                               + "\n" + i.Education2 + "\n"
                                                               + "\n" + i.Medical2 + "\n"
                                                               + "\n" + i.LTA2 + "\n"
                                                               + "\n" + i.SpecialAllowance2 + "\n"
                                                               + "\n" + i.FoodAllowance2 + "\n"
                                                             + "\n" + i.IncentivesBonus2 + "\n"
                                                              + "\n" + i.Others + "\n",
                              FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));

                            table.AddCell(PhraseCell(new Phrase("EPF" + "\n"
                                                                       + "\n" + "Professional Tax" + "\n"
                                                                       + "\n" + "TDS" + "\n"
                                                                       + "\n" + "ESI" + "\n"
                                                                       + "\n" + "Advance" + "\n"
                                                                       + "\n" + "LTA" + "\n"
                                                                       + "\n" + "Other" + "\n",

                                //+ "\n" + "LTA" + "\n",
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase(i.EPF + "\n"
                                                                          + "\n" + i.ProfessionalTax + "\n"
                                                                          + "\n" + i.TDS + "\n"
                                                                          + "\n" + i.ESI + "\n"
                                                                          + "\n" + i.Advance + "\n"
                                                                          + "\n" + i.LTA3 + "\n"
                                                                          + "\n" + i.Other + "\n",
                                //+ "\n" + dr["LTA"] + "\n",
                                FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 2;
                            cell.PaddingBottom = 10f;
                            table.AddCell(cell).BorderWidthLeft = 0;

                            //table4

                            document.Add(table);
                            table = new PdfPTable(5);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                            //table.SpacingBefore = 30f;
                            table.AddCell(PhraseCell(new Phrase("TOTAL EARNINGS:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase(i.TotalGrossSalary + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                            table.AddCell(PhraseCell(new Phrase(i.TOTALEARNINGS + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                            table.AddCell(PhraseCell(new Phrase("TOTAL DEDUCTIONS:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(PhraseCell(new Phrase(i.TOTALDEDUCTIONS + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                            cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                            cell.Colspan = 2;
                            cell.PaddingBottom = 10f;
                            table.AddCell(cell);

                            //table4
                            document.Add(table);
                            table = new PdfPTable(1);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.SetWidths(new float[] { 0.3f });

                            //var amount = Convert.ToDecimal(dr["NET SALARY"]);
                            //NumberToWord toWord = new NumberToWord();

                            var amount = Convert.ToInt32(i.NETSALARY);
                            string result = NumberToWords(amount) + " " + "Rupees" + " " + "Only";
                            //var result = toWord.AmtInWord(amount);


                            table.AddCell(PhraseCell(new Phrase("NET SALARY:              " + i.NETSALARY + "  " + "(" + result + ")" + "\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            // table.AddCell(PhraseCell(new Phrase("NET SALARY:              " + dr["NET SALARY"] + "\n" + wordTo(), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                            table.AddCell(cell);



                            document.Add(table);
                            //#endregion

                            table = new PdfPTable(1);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.SetWidths(new float[] { 0.3f });
                            table.SpacingBefore = 5f;


                            phrase = new Phrase();

                            //phrase.Add(new Chunk("*** This is a computer generated Payslip. Signature not required***", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                            phrase.Add(new Chunk("*** This is a computer generated Payslip. Signature not required***", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                            table.AddCell(cell);

                            // table.AddCell(cell);

                            document.Add(table);
                            document.Close();

                            memoryStream.Close();



                            //----------Saving in Particular location ---------

                            //string value = ConfigurationManager.AppSettings["Pdf"];
                            string value = Server.MapPath("~/PaySlipPdf");
                            string dirPath = value;
                            string name = i.EmployeeName + ".pdf";

                            string filName = name;


                            // using (FileStream fs = new FileStream(dirPath, FileMode.OpenOrCreate, FileAccess.Write))
                            using (FileStream fs = new FileStream(dirPath + "\\" + filName, FileMode.Create))
                            // using (BinaryWriter sw = new BinaryWriter(File.Open(dirPath + "\\" + fileName, FileMode.Create)))
                            using (BinaryWriter sw = new BinaryWriter(fs))
                            {
                                byte[] bytes = memoryStream.ToArray();

                                sw.Write(bytes);

                                sw.Close();
                                sw.Dispose();

                            }
                            Res = "Saved Successfully in Folder";
                            

                        }




                    }
                    
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(Res, JsonRequestBehavior.AllowGet);
        }

        public string PdfGen(int Id)
        {
            string Res = string.Empty;
            try
            {

                DataRow dr = GetData("SELECT * FROM [Sheet1$]").Select("Id=" + Id)[0];


                //DataTable dtt = new DataTable();
                //DataRow stt = dr;
                //dtt.Rows.Add(stt);
                Document document = new Document(PageSize.A4, 88f, 88f, 10f, 10f);
                iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    Phrase phrase = null;
                    PdfPCell cell = null;
                    PdfPTable table = null;
                    iTextSharp.text.BaseColor color = null;
                    document.Open();
                    //#statregion  heading 
                    table = new PdfPTable(2);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    //table.SetWidths(new float[] { 0.3f, 0.3f });
                    phrase = new Phrase();
                    phrase.Add(new Chunk("M/S.EVOLUTYZ IT SERVICES PVT LTD", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    table.AddCell(cell);



                    cell = ImageCell("~/PDFImages/img.png", 50f, PdfPCell.ALIGN_RIGHT);
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    table.AddCell(cell);
                    document.Add(table);
                    //#endregion




                    table = new PdfPTable(1);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;

                    //Name
                    phrase = new Phrase();

                    //phrase.Add(new Chunk("                                         Pay Slip for February 2018", FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    phrase.Add(new Chunk("                                         Pay Slip for " + DateTime.Now.AddMonths(-1).ToString("MMMMMMMMMMMMM") + " " + DateTime.Now.AddMonths(-1).Year, FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    table.AddCell(cell);



                    document.Add(table);

                    document.Add(new Paragraph("  "));
                    // document.Add(new Paragraph("  "));




                    table = new PdfPTable(2);
                    table.TotalWidth = 500;
                    table.SetWidths(new float[] { 300f, 200f });
                    table.LockedWidth = true;
                    PdfPTable tableinner;
                    tableinner = new PdfPTable(2);
                    // PdfPTable table2 = new PdfPTable(2);

                    tableinner.HorizontalAlignment = Element.ALIGN_LEFT;
                    //table2.HorizontalAlignment = Element.ALIGN_RIGHT;

                    //1

                    tableinner.AddCell(PhraseCell(new Phrase("Employee Name\n" + "\n"
                                                    + "Designation    \n" + "\n"
                                                    + "Department     \n" + "\n"
                                                    + "Date of Joining  \n" + "\n"
                                                    + "Working Days    \n" + "\n"
                                                    + "Days Worked     \n" + "\n"
                                                    + "Payable Days    \n" + "\n"
                                                    + "LOP Days",
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT)).BorderColor = BaseColor.WHITE;

                    tableinner.AddCell(PhraseCell(new Phrase(dr["Employee Name"].ToString().Trim() + "\n"
                                                 + "\n" + dr["Designation"].ToString().Trim() + "\n"
                                                     + "\n" + dr["Department"].ToString().Trim() + "\n"
                                                     + "\n" + Convert.ToDateTime(dr["Date Of Joining"].ToString().Trim()).ToShortDateString() + "\n"
                                                     + "\n" + dr["Working Days"].ToString().Trim() + "\n"
                                                     + "\n" + dr["Days Worked"].ToString().Trim() + "\n"
                                                     + "\n" + dr["Payable Days"].ToString().Trim() + "\n"
                                                     + "\n" + dr["LOP Days"].ToString().Trim(),
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT)).BorderColor = BaseColor.WHITE;

                    table.AddCell(tableinner);
                    //2
                    tableinner = new PdfPTable(1);
                    tableinner.AddCell(PhraseCell(new Phrase("Employee No                              " + dr["Employee No"] + "\n"
                                                 + "\n" + "BankAcNO:   " + dr["BankAcNO"] + "\n"
                                                 + "\n" + "PAN No:     " + dr["PAN No"] + "\n"
                                                 + "\n" + "UAN No                             " + dr["UAN No"] + "\n"
                                                 + "\n" + "PF No      " + dr["PF No"] + "\n"
                                                 + "\n" + "ESI No                               " + dr["ESI No"] + "\n"
                                                 + "\n" + "Pay Mode                          " + dr["Pay Mode"] + "\n"
                                                 + "\n" + "Pay Date                           " + Convert.ToDateTime(dr["Pay Date"].ToString().Trim()).ToShortDateString() + "\n",
                   FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_JUSTIFIED_ALL)).BorderColor = BaseColor.WHITE; ;
                    table.AddCell(tableinner);

                    //table 2
                    document.Add(table);
                    table = new PdfPTable(5);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                    //table.SpacingBefore = 30f;

                    table.AddCell(PhraseCell(new Phrase("EARNINGS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase("GROSS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase("EARNED", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase("DEDUCTIONS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase("AMOUNT", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_LEFT));
                    cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                    cell.Colspan = 2;
                    cell.PaddingBottom = 10f;
                    table.AddCell(cell);

                    //table3
                    document.Add(table);
                    table = new PdfPTable(5);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                    // table.SpacingBefore = 30f;
                    table.AddCell(PhraseCell(new Phrase("Basic" + "\n"
                                             //+ "\n" + "HRA" + "\n"
                                             + "\n" + "House Rent Allow" + "\n"
                                              + "\n" + "Conveyance Allow" + "\n"
                                             + "\n" + "Education Allow" + "\n"
                                             + "\n" + "Medical Allow" + "\n"
                                             + "\n" + "LTA" + "\n"
                                             + "\n" + "Special Allowance" + "\n"
                                             + "\n" + "Food Allow" + "\n"
                                              + "\n" + "Incentives/Bonus" + "\n"
                                             + "\n" + "Others" + "\n",
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));

                    table.AddCell(PhraseCell(new Phrase(dr["Basic"] + "\n"
                                                         + "\n" + dr["HRA"] + "\n"
                                                          + "\n" + dr["Conveyance Allow"] + "\n"
                                                         + "\n" + dr["Education"] + "\n"
                                                         + "\n" + dr["Medical"] + "\n"
                                                         + "\n" + dr["LTA"] + "\n"
                                                         + "\n" + dr["Special Allowance"] + "\n"
                                                         + "\n" + dr["Food Allowance"] + "\n",
                        //+ "\n" + dr["Others"] + "\n",
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));

                    table.AddCell(PhraseCell(new Phrase(dr["Basic"] + "\n"
                                                       + "\n" + dr["HRA"] + "\n"
                                                        + "\n" + dr["Conveyance Allow"] + "\n"
                                                       + "\n" + dr["Education"] + "\n"
                                                       + "\n" + dr["Medical"] + "\n"
                                                       + "\n" + dr["LTA1"] + "\n"
                                                       + "\n" + dr["Special Allowance"] + "\n"
                                                       + "\n" + dr["Food Allowance"] + "\n"
                                                     + "\n" + dr["Incentives/Bonus"] + "\n"
                                                      + "\n" + dr["Others"] + "\n",
                      FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));

                    table.AddCell(PhraseCell(new Phrase("EPF" + "\n"
                                                               + "\n" + "Professional Tax" + "\n"
                                                               + "\n" + "TDS" + "\n"
                                                               + "\n" + "ESI" + "\n"
                                                               + "\n" + "Advance" + "\n"
                                                               + "\n" + "Other" + "\n",
                        //+ "\n" + "LTA" + "\n",
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase(dr["EPF"] + "\n"
                                                                  + "\n" + dr["Professional Tax"] + "\n"
                                                                  + "\n" + dr["TDS"] + "\n"
                                                                  + "\n" + dr["ESI"] + "\n"
                                                                  + "\n" + dr["Advance"] + "\n"
                                                                  + "\n" + dr["Other"] + "\n",
                        //+ "\n" + dr["LTA"] + "\n",
                        FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                    cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                    cell.Colspan = 2;
                    cell.PaddingBottom = 10f;
                    table.AddCell(cell).BorderWidthLeft = 0;

                    //table4

                    document.Add(table);
                    table = new PdfPTable(5);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SetWidths(new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f });
                    //table.SpacingBefore = 30f;
                    table.AddCell(PhraseCell(new Phrase("TOTAL EARNINGS:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase(dr["Total Gross Salary"] + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                    table.AddCell(PhraseCell(new Phrase(dr["TOTAL EARNINGS"] + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                    table.AddCell(PhraseCell(new Phrase("TOTAL DEDUCTIONS:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(PhraseCell(new Phrase(dr["TOTAL DEDUCTIONS"] + "", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_RIGHT));
                    cell = PhraseCell(new Phrase(), PdfPCell.ALIGN_CENTER);
                    cell.Colspan = 2;
                    cell.PaddingBottom = 10f;
                    table.AddCell(cell);

                    //table4
                    document.Add(table);
                    table = new PdfPTable(1);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SetWidths(new float[] { 0.3f });

                    //var amount = Convert.ToDecimal(dr["NET SALARY"]);
                    //NumberToWord toWord = new NumberToWord();

                    var amount = Convert.ToInt32(dr["NET SALARY"]);
                    string result = NumberToWords(amount) + " " + "Rupees" + " " + "Only";
                    //var result = toWord.AmtInWord(amount);


                    table.AddCell(PhraseCell(new Phrase("NET SALARY:              " + dr["NET SALARY"] + "  " + "(" + result + ")" + "\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    // table.AddCell(PhraseCell(new Phrase("NET SALARY:              " + dr["NET SALARY"] + "\n" + wordTo(), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)), PdfPCell.ALIGN_LEFT));
                    table.AddCell(cell);



                    document.Add(table);
                    //#endregion

                    table = new PdfPTable(1);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.SetWidths(new float[] { 0.3f });
                    table.SpacingBefore = 5f;


                    phrase = new Phrase();

                    //phrase.Add(new Chunk("*** This is a computer generated Payslip. Signature not required***", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                    phrase.Add(new Chunk("*** This is a computer generated Payslip. Signature not required***", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    table.AddCell(cell);

                    // table.AddCell(cell);

                    document.Add(table);
                    document.Close();

                    memoryStream.Close();



                    //----------Saving in Particular location ---------

                    //string value = ConfigurationManager.AppSettings["Pdf"];
                    string value = Server.MapPath("~/PaySlipPdf");
                    string dirPath = value;
                    string name = dr["Employee Name"] + ".pdf";

                    string fileName = name;


                    // using (FileStream fs = new FileStream(dirPath, FileMode.OpenOrCreate, FileAccess.Write))
                    using (FileStream fs = new FileStream(dirPath + "\\" + fileName, FileMode.Create))
                    // using (BinaryWriter sw = new BinaryWriter(File.Open(dirPath + "\\" + fileName, FileMode.Create)))
                    using (BinaryWriter sw = new BinaryWriter(fs))
                    {
                        byte[] bytes = memoryStream.ToArray();

                        sw.Write(bytes);

                        sw.Close();
                        sw.Dispose();

                    }
                    Res = "Saved Successfully in Folder";
                    //lblMes.Text = "Saved Successfully in Folder";
                    //btnShowPDf.Visible = true;
                    //btnSendmail.Visible = true;

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return Res;
        }

        protected DataTable GetData(string query)
        {
            try
            {
                string conStr = "";

                string FilePath = Path.GetFullPath(Server.MapPath("~/Content/PaySlip.xlsx"));
                string Extension = Path.GetExtension(FilePath);
                string strConn = "";
                switch (Extension)
                {
                    case ".xls":
                        //Excel 1997-2003  
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        break;
                    case ".xlsx":
                        //Excel 2007-2010  
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0 xml;HDR=Yes;IMEX=1\"";
                        break;
                }
                //File.Delete(FilePath);
                conStr = String.Format(strConn, FilePath);
                OleDbConnection connExcel = new OleDbConnection(conStr);
                OleDbConnection conn = new OleDbConnection(conStr);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                OleDbCommand cmd = new OleDbCommand(query, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);

                da.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = iTextSharp.text.BaseColor.BLACK;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }

        private static PdfPCell ImageCell(string path, float scale, int align)
        {
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath(path));
            image.ScalePercent(scale);
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }

        private string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";
            if ((number / 1000000000) > 0)
            {
                words += NumberToWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }

            if ((number / 10000000) > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }


            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }


            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        [HttpGet]
        public void btnShowPDf_Click()
        {
            System.Diagnostics.Process.Start("explorer.exe", Server.MapPath("~/PaySlipPdf"));
        }

        public JsonResult btnSendmail_Click()
        {

            //string connectionString = "";
            string fileLocation = Server.MapPath("~/Content/PaySlip.xlsx");
           

            foreach (var item in employees)
            {
                Sendemail(item.Email, item.EmployeeName);

            }

            System.IO.File.Delete(fileLocation);
            string[] files = Directory.GetFiles(Server.MapPath("~/PaySlipPdf"));
            foreach (string file in files)
            {
                System.IO.File.Delete(file);

            }
         
            return Json("Mail Is Successfully Sent to the Employee", JsonRequestBehavior.AllowGet);

        }


        public void Sendemail(string email, string employeeName)
        {

            string UrlEmailAddress = string.Empty;
            string UrlEmailImage = string.Empty;

            if (host == "localhost")
            {
                UrlEmailAddress = host + ":" + port;
            }
            else
            {
                UrlEmailAddress = port1;
            }
            //   Server.MapPath("~/PaySlipPdf");
            var path1 = Server.MapPath("~/PaySlipPdf/");
            var fileName = employeeName + ".pdf";
            var attchment = path1 + fileName;
            StringBuilder mailbody = new StringBuilder();
            //StringBuilder mailbody1 = new StringBuilder();
            mailbody.AppendLine();
            mailbody.Append("Dear " + employeeName + ",");
            mailbody.AppendLine();

            mailbody.AppendLine();
            mailbody.Append("<div style='padding-top:30px'>Please find the attached payslip for the month of " + month + " " + Year + "</div>");

            mailbody.AppendLine();

            mailbody.AppendLine();
            mailbody.Append("<div style='padding-top:20px'>Warm Regards,</div><div>Naveen Kumar.</div><div>Accounts Executive.</div><div>Evolutyz IT Services Pvt Ltd.</div>Phone -7989020756.");

            //var emailcontent = "<html><body>" +
            //    "<div style='margin-bottom:30px'>Hi&nbsp;" + employeeName + ",</div><div  style='margin-bottom:20px'>Please find the attached payslip for the month of " + DateTime.Now.AddMonths(-1).ToString("MMMMMMMMMMMMM") + " " + DateTime.Now.AddMonths(-1).Year +
            //"</div><div>Thanks & Regards,</div><div>" + "\n" + "Naveen Kumar Pilli," + "\n" + "</div><div>Accounts Executive,</div><div>" + "\n" + "Evolutyz IT Services Pvt Ltd,</div><div>" + "\n" + "Phone - 7989020756 .</div>" +
            //"</body></html>";



            var Subject = "Payslip - " + month + " " + Year;
          

            var client = new SendGridClient("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");

            
            try
            {


                var msgs = new SendGridMessage();
                msgs.From = new EmailAddress("accounts@evolutyz.in");
                msgs.Subject = Subject;
                msgs.HtmlContent = mailbody.ToString();
                var bytes = System.IO.File.ReadAllBytes(attchment);
                var file = Convert.ToBase64String(bytes);
                msgs.AddAttachment(fileName, file);
                // msgs.AddAttachment(path1, fileName);


                // msgs.AddAttachment(path1, fileName);
                // msgs.AddAttachment(attchment);
                msgs.AddTo(new EmailAddress(email));
                var responses = client.SendEmailAsync(msgs);
            }
            catch (Exception ex)
            {

            }
            //var responses = client.SendEmailAsync(msgs);
            //return "";
        }
    }
}