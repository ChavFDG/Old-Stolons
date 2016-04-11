using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using Stolons.Models;
using System.Threading;
using Microsoft.Data.Entity;
using System.Globalization;

namespace Stolons.Tools
{
    public static  class BillGenerator
    {

        public static void ManageBills(ApplicationDbContext dbContext)
        {
            ApplicationConfig.Modes lastMode = Configurations.Mode;
            do
            {
                ApplicationConfig.Modes currentMode = Configurations.Mode;
                if (lastMode == ApplicationConfig.Modes.Order && currentMode == ApplicationConfig.Modes.Preparation)
                {
                    //We moved form Order to Preparation, create and send bills
                    List<Bill> bills = new List<Bill>();
                    Dictionary<Producer, List<BillEntry>> producerBills = new Dictionary<Producer, List<BillEntry>>();
                    //Consumer (create bills)
                    foreach (var weekBasket in dbContext.ValidatedWeekBaskets.Include(x => x.Products).Include(x=>x.Consumer))
                    {
                        //Generate bill for consumer
                        bills.Add(GenerateBill(weekBasket, dbContext));
                        //Add to producer bill entry
                        foreach (var tmpBillEntry in weekBasket.Products)
                        {
                            var billEntry = dbContext.BillEntrys.Include(x=>x.User).Include(x => x.Product).ThenInclude(x => x.Producer).First(x=>x.Id == tmpBillEntry.Id);
                            Producer producer = billEntry.Product.Producer;
                            if (producerBills.ContainsKey(producer))
                            {
                                producerBills.Add(producer, new List<BillEntry>());
                            }
                            producerBills[producer].Add(billEntry);
                        }
                    }
                    //Producer (creates bills)
                    foreach (var producerBill in producerBills)
                    {
                        //Generate bill for producer
                        bills.Add(GenerateBill(producerBill.Key, producerBill.Value, dbContext));
                    }
                    //Bills (save bills and send mails to user)
                    foreach(var bill in bills)
                    {
                        dbContext.Add(bill);
                        //Send mail to user with bill
                        //TODO
                    }
                    //Remove week basket
                    dbContext.TempsWeekBaskets.Clear();
                    dbContext.ValidatedWeekBaskets.Clear();
                    //Move product to, to validate
                    dbContext.Products.ToList().ForEach(x => x.State = Product.ProductState.Stock);
                    dbContext.SaveChanges();
                }
                lastMode = currentMode;
                Thread.Sleep(5000);
            } while (true);
        }
        /*
        *BILL NAME INFORMATION
        *Bills are stored like that : bills\UserId\Year_WeekNumber
        */


        private static Bill GenerateBill(Producer producer, List<BillEntry> billEntry, ApplicationDbContext dbContext)
        {
            Bill bill = CreateBill(producer);
            //Generate exel file with bill number for producer
            throw new NotImplementedException();
        }

        private static Bill GenerateBill(ValidatedWeekBasket weekBasket, ApplicationDbContext dbContext)
        {
            Bill bill = CreateBill(weekBasket.Consumer);
            //Generate exel file with bill number for user
            string filePath = Path.Combine(Configurations.Environment.WebRootPath, Configurations.BillsStockagePath,bill.User.Id.ToString());
            FileInfo newFile = new FileInfo(filePath + @"\"+ bill .BillNumber+ ".xlsx");
            if (newFile.Exists)
            {
                //Normaly impossible
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filePath + @"\" + bill.BillNumber + ".xlsx");
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Facture");
                //Add global informations
                worksheet.Cells[0, 0].Value = "Numéro de facture :";
                worksheet.Cells[0, 1].Value = bill.BillNumber;
                worksheet.Cells[1,0].Value = "Année :";
                worksheet.Cells[1, 1].Value = DateTime.Now.Year;
                worksheet.Cells[2, 0].Value = "Semaine :";
                worksheet.Cells[2, 1].Value = DateTime.Now.GetIso8601WeekOfYear();
                //Add product informations
                worksheet.Cells[4, 0].Value = "PRODUITS :";

                // - Add the headers
                worksheet.Cells[5, 0].Value = "Nom";
                worksheet.Cells[5, 1].Value = "Famille";
                worksheet.Cells[5, 2].Value = "Type";
                worksheet.Cells[5, 3].Value = "Prix unitaire";
                worksheet.Cells[5, 4].Value = "Quantité";
                worksheet.Cells[5, 5].Value = "Prix total";
                // - Add products
                int row = 6;
                foreach(var tmpBillEntry in weekBasket.Products)
                {
                    var billEntry = dbContext.BillEntrys.Include(x => x.User).Include(x => x.Product).ThenInclude(x => x.Producer).First(x => x.Id == tmpBillEntry.Id);
                    worksheet.Cells[row, 0].Value = billEntry.Product.Name;
                    worksheet.Cells[row, 1].Value = billEntry.Product.Familly.FamillyName;
                    worksheet.Cells[row, 2].Value = billEntry.Product.Type;
                    worksheet.Cells[row, 3].Value = billEntry.Product.Price;
                    worksheet.Cells[row, 4].Value = billEntry.Quantity;
                    worksheet.Cells[row, 5].Formula = new ExcelCellAddress(row,3).Address +"*" + new ExcelCellAddress(row, 4).Address;
                    row++;
                }
                //- Add TOTAL
                worksheet.Cells[row, 4].Value = "TOTAL : ";
                worksheet.Cells[row, 5].Formula = "TOTAL : ";

                //Add a formula for the value-column
                worksheet.Cells["E2:E4"].Formula = string.Format("SUBTOTAL(9,{0})", new OfficeOpenXml.ExcelAddress(6, 5, row, 5).Address);

                //Format values :
                /*
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new OfficeOpenXml.ExcelAddress(2, 3, 4, 3).Address);
                worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";

                //Create an autofilter for the range
                worksheet.Cells["A1:E4"].AutoFilter = true;

                worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                
                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.RightAlignedText =
                    string.Format("Page {0} of {1}", OfficeOpenXml.ExcelHeaderFooter.PageNumber, OfficeOpenXml.ExcelHeaderFooter.NumberOfPages);
                // add the sheet name to the footer
                worksheet.HeaderFooter.OddFooter.CenteredText = OfficeOpenXml.ExcelHeaderFooter.SheetName;
                // add the file path to the footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = OfficeOpenXml.ExcelHeaderFooter.FilePath + OfficeOpenXml.ExcelHeaderFooter.FileName;

                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                // Change the sheet view to show it in page layout mode
                worksheet.View.PageLayoutView = true;
                */

                // Document properties
                package.Workbook.Properties.Title = "Facture : " + bill.BillNumber;
                package.Workbook.Properties.Author = "Stolons";
                package.Workbook.Properties.Comments = "Facture de la semaine " + bill.BillNumber;

                // Extended property values
                package.Workbook.Properties.Company = "Association Stolons";

                // save our new workbook and we are done!
                package.Save();

            }


            //
            return bill;
        }

        private static Bill CreateBill(User user)
        {
            Bill bill = new Bill();
            bill.BillNumber = DateTime.Now.Year + "_" + DateTime.Now.GetIso8601WeekOfYear();
            bill.User = user;
            bill.State = Bill.BillState.Pending;
            return bill;
        }

        public static void GenerateExel(string filePath)
        {
            FileInfo newFile = new FileInfo(filePath + @"\sample1.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filePath + @"\sample1.xlsx");
            }
            using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Inventory");
                //Add the headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Product";
                worksheet.Cells[1, 3].Value = "Quantity";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "Value";

                //Add some items...
                worksheet.Cells["A2"].Value = 12001;
                worksheet.Cells["B2"].Value = "Nails";
                worksheet.Cells["C2"].Value = 37;
                worksheet.Cells["D2"].Value = 3.99;

                worksheet.Cells["A3"].Value = 12002;
                worksheet.Cells["B3"].Value = "Hammer";
                worksheet.Cells["C3"].Value = 5;
                worksheet.Cells["D3"].Value = 12.10;

                worksheet.Cells["A4"].Value = 12003;
                worksheet.Cells["B4"].Value = "Saw";
                worksheet.Cells["C4"].Value = 12;
                worksheet.Cells["D4"].Value = 15.37;

                //Add a formula for the value-column
                worksheet.Cells["E2:E4"].Formula = "C2*D2";

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new OfficeOpenXml.ExcelAddress(2, 3, 4, 3).Address);
                worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";

                //Create an autofilter for the range
                worksheet.Cells["A1:E4"].AutoFilter = true;

                worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.RightAlignedText =
                    string.Format("Page {0} of {1}", OfficeOpenXml.ExcelHeaderFooter.PageNumber, OfficeOpenXml.ExcelHeaderFooter.NumberOfPages);
                // add the sheet name to the footer
                worksheet.HeaderFooter.OddFooter.CenteredText = OfficeOpenXml.ExcelHeaderFooter.SheetName;
                // add the file path to the footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = OfficeOpenXml.ExcelHeaderFooter.FilePath + OfficeOpenXml.ExcelHeaderFooter.FileName;

                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                // Change the sheet view to show it in page layout mode
                worksheet.View.PageLayoutView = true;

                // set some document properties
                package.Workbook.Properties.Title = "Invertory";
                package.Workbook.Properties.Author = "Jan Källman";
                package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";

                // set some extended property values
                package.Workbook.Properties.Company = "AdventureWorks Inc.";

                // set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // save our new workbook and we are done!
                package.Save();

            }
        }

        private static int GetIso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
    }
}
