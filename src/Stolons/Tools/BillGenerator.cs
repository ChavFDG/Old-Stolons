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
using OfficeOpenXml.Style;
using Stolons.Helpers;

namespace Stolons.Tools
{
    public static  class BillGenerator
    {
        public class BillEntryConsumer
        {
            public BillEntry BillEntry { get; set; }
            public Consumer Consumer { get; set; }

            public BillEntryConsumer(BillEntry billEntry, Consumer consumer)
            {
                BillEntry = billEntry;
                Consumer = consumer;
            }
        }

        public static void ManageBills(ApplicationDbContext dbContext)
        {
            ApplicationConfig.Modes lastMode = Configurations.Mode;
            do
            {
                ApplicationConfig.Modes currentMode = Configurations.Mode;
                if (lastMode == ApplicationConfig.Modes.Order && currentMode == ApplicationConfig.Modes.Preparation)
                {
                    //We moved form Order to Preparation, create and send bills
                    List<IBill> bills = new List<IBill>();
                    Dictionary<Producer, List<BillEntryConsumer>> producerBills = new Dictionary<Producer, List<BillEntryConsumer>>();
                    //Consumer (create bills)
                    List<ValidatedWeekBasket> consumerWeekBaskets = dbContext.ValidatedWeekBaskets.Include(x => x.Products).Include(x => x.Consumer).ToList();
                    GenerateBill(consumerWeekBaskets,dbContext);
                    foreach (var weekBasket in consumerWeekBaskets)
                    {
                        //Generate bill for consumer
                        ConsumerBill consumerBill = GenerateBill(weekBasket, dbContext);
                        bills.Add(consumerBill);
                        dbContext.Add(consumerBill);
                        //Add to producer bill entry
                        foreach (var tmpBillEntry in weekBasket.Products)
                        {
                            var billEntry = dbContext.BillEntrys.Include(x => x.Product).ThenInclude(x => x.Producer).First(x=>x.Id == tmpBillEntry.Id);
                            Producer producer = billEntry.Product.Producer;
                            if (!producerBills.ContainsKey(producer))
                            {
                                producerBills.Add(producer, new List<BillEntryConsumer>());
                            }
                            producerBills[producer].Add(new BillEntryConsumer(billEntry,weekBasket.Consumer));
                        }
                    }
                    //Producer (creates bills)
                    foreach (var producerBill in producerBills)
                    {
                        //Generate bill for producer
                        ProducerBill bill = GenerateBill(producerBill.Key, producerBill.Value, dbContext);
                        bills.Add(bill);
                        dbContext.Add(bill);
                        //Send email to producer
                        //TODO

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
                    dbContext.BillEntrys.Clear();
                    //Move product to, to validate
                    dbContext.Products.ToList().ForEach(x => x.State = Product.ProductState.Stock);
                    dbContext.SaveChanges();
                }
                lastMode = currentMode;
                Thread.Sleep(5000);
            } while (true);
        }

        private static void GenerateBill(List<ValidatedWeekBasket> consumerWeekBaskets, ApplicationDbContext dbContext)
        {
            //Generate exel file with bill number for user
            #region File creation
            string filePath = Path.Combine(Configurations.Environment.WebRootPath, Configurations.StolonsBillsStockagePath);
            string billNumber = DateTime.Now.Year + "_" + DateTime.Now.GetIso8601WeekOfYear();
            FileInfo newFile = new FileInfo(filePath + @"\" + billNumber + ".xlsx");
            if (newFile.Exists)
            {
                //Normaly impossible
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filePath + @"\" + billNumber + ".xlsx");
            }
            else
            {
                Directory.CreateDirectory(filePath);
            }
            #endregion File creation
            //
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                if(!consumerWeekBaskets.Any())
                {
                    //Rien de commander cette semaine :'(
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Rien :'(");
                    worksheet.Cells[1, 1].Value = "Rien cette semaine ! C'est trop triste :'(";
                }
                else
                {
                    foreach (ValidatedWeekBasket weekBasket in consumerWeekBaskets.OrderBy(x => x.Consumer.Id))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(weekBasket.Consumer.Id.ToString() + " (" + weekBasket.Consumer.Name + ")");
                        int row = 1;
                        //Add global informations
                        worksheet.Cells[row, 1, 8, 3].Merge = true;
                        worksheet.Cells[row, 1, 8, 3].Value = weekBasket.Consumer.Id;
                        worksheet.Cells[row, 1, 8, 3].Style.Font.Size = 100;
                        worksheet.Cells[row, 1, 8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 1, 8, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        worksheet.Cells[row, 4].Value = "Facture";
                        worksheet.Cells[row, 5].Value = billNumber;
                        row++;
                        worksheet.Cells[row, 4].Value = "Semaine";
                        worksheet.Cells[row, 5].Value = DateTime.Now.GetIso8601WeekOfYear();
                        row++;
                        worksheet.Cells[row, 4].Value = "Nom";
                        worksheet.Cells[row, 5].Value = weekBasket.Consumer.Name;
                        row++;
                        worksheet.Cells[row, 4].Value = "Prénom";
                        worksheet.Cells[row, 5].Value = weekBasket.Consumer.Surname;
                        row++;
                        worksheet.Cells[row, 4].Value = "Téléphone";
                        worksheet.Cells[row, 5].Value = weekBasket.Consumer.PhoneNumber;
                        worksheet.Cells[1, 4, row, 4].Style.Font.Bold = true;
                        worksheet.Cells[1, 5, row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        row++;
                        worksheet.Cells[row, 4, row + 1, 5].Merge = true;
                        var total = worksheet.Cells[row, 6, row + 1, 6];
                        total.Merge = true;
                        worksheet.Cells[row, 4, row + 1, 5].Value = "TOTAL";
                        worksheet.Cells[row, 4, row + 1, 6].Style.Font.Size = 18;
                        worksheet.Cells[row, 4, row + 1, 6].Style.Font.Bold = true;
                        worksheet.Cells[row, 4, row + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 4, row + 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //Add product informations
                        row++;
                        row++;
                        row++;
                        worksheet.Cells[row, 1, row, 2].Merge = true;
                        worksheet.Cells[row, 1, row, 2].Value = "NOM";
                        worksheet.Cells[row, 3].Value = "TYPE";
                        worksheet.Cells[row, 4].Value = "PRIX UNITAIRE";
                        worksheet.Cells[row, 5].Value = "QUANTITE";
                        worksheet.Cells[row, 6].Value = "PRIX TOTAL";

                        //Create list of bill entry by product
                        Dictionary<Producer, List<BillEntry>> producersProducts = new Dictionary<Producer, List<BillEntry>>();
                        foreach (var billEntryConsumer in weekBasket.Products)
                        {
                            var billEntry = dbContext.BillEntrys.Include(x => x.Product).ThenInclude(x=>x.Producer).First(x => x.Id == billEntryConsumer.Id);
                            if (!producersProducts.ContainsKey(billEntry.Product.Producer))
                            {
                                producersProducts.Add(billEntry.Product.Producer, new List<BillEntry>());
                            }
                            producersProducts[billEntry.Product.Producer].Add(billEntry);
                        }
                        List<int> rowsTotal = new List<int>();
                        // - Add products by producer
                        foreach (var producer in producersProducts.Keys.OrderBy(x => x.Id))
                        {
                            row++;
                            worksheet.Cells[row, 1, row + 1, 6].Merge = true;
                            worksheet.Cells[row, 1, row + 1, 6].Value = producer.CompanyName;
                            worksheet.Cells[row, 1, row + 1, 6].Style.Font.Size = 22;
                            worksheet.Cells[row, 1, row + 1, 6].Style.Font.Bold = true;
                            row++;
                            row++;
                            int startRow = row;
                            foreach (var billEntry in producersProducts[producer].OrderBy(x => x.Product.Name))
                            {
                                worksheet.Cells[row, 1, row, 2].Merge = true;
                                worksheet.Cells[row, 1, row, 2].Value = billEntry.Product.Name; ;
                                worksheet.Cells[row, 3].Value = EnumHelper<Product.SellType>.GetDisplayValue(billEntry.Product.Type);
                                worksheet.Cells[row, 4].Value = billEntry.Product.Price;
                                worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00€";
                                worksheet.Cells[row, 5].Value = billEntry.Quantity;
                                worksheet.Cells[row, 6].Formula = new ExcelCellAddress(row, 4).Address + "*" + new ExcelCellAddress(row, 5).Address;
                                worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00€";
                                worksheet.Cells[row, 1, row, 6].Style.Font.Bold = true;
                                worksheet.Cells[row, 1, row, 6].Style.Font.Size = 13;
                                worksheet.Cells[row, 1, row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                row++;
                            }
                            //Total
                            worksheet.Cells[row, 5].Value = "TOTAL : ";
                            worksheet.Cells[row, 6].Formula = string.Format("SUBTOTAL(9,{0})", new OfficeOpenXml.ExcelAddress(startRow, 6, row - 1, 6).Address);
                            worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00€";
                            worksheet.Cells[row, 5, row, 6].Style.Font.Bold = true;
                            worksheet.Cells[row, 5, row, 6].Style.Font.Size = 13;
                            worksheet.Cells[row, 5, row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            rowsTotal.Add(row);
                        }

                        //Super total
                        string totalFormula = "";
                        for (int cpt = 0; cpt < rowsTotal.Count; cpt++)
                        {
                            totalFormula += new ExcelCellAddress(rowsTotal[cpt], 6).Address;
                            if (cpt != rowsTotal.Count - 1)
                            {
                                totalFormula += "+";
                            }
                        }
                        total.Formula = totalFormula;
                        total.Style.Numberformat.Format = "0.00€";
                        //
                        worksheet.View.PageLayoutView = true;
                        worksheet.Column(1).Width = (98 - 12 + 5) / 7d + 1;
                        worksheet.Column(2).Width = (98 - 12 + 5) / 7d + 1;
                        worksheet.Column(3).Width = (98 - 12 + 5) / 7d + 1;
                        worksheet.Column(4).Width = (98 - 12 + 5) / 7d + 1;
                        worksheet.Column(5).Width = (98 - 12 + 5) / 7d + 1;
                        worksheet.Column(6).Width = (98 - 12 + 5) / 7d + 1;
                    }
                }
             


                // Document properties
                package.Workbook.Properties.Title = "Factures : " + billNumber;
                package.Workbook.Properties.Author = "Stolons";
                package.Workbook.Properties.Comments = "Factures des adhérants de la semaine " + billNumber;

                // Extended property values
                package.Workbook.Properties.Company = "Association Stolons";
                // save our new workbook and we are done!
                package.Save();
            }
        }

        /*
        *BILL NAME INFORMATION
        *Bills are stored like that : bills\UserId\Year_WeekNumber_UserId
        */


        private static ProducerBill GenerateBill(Producer producer, List<BillEntryConsumer> billEntries, ApplicationDbContext dbContext)
        {
            ProducerBill bill = CreateBill<ProducerBill>(producer);
            //Generate exel file with bill number for user
            string filePath = Path.Combine(Configurations.Environment.WebRootPath, Configurations.ProducersBillsStockagePath, bill.User.Id.ToString());
            FileInfo newFile = new FileInfo(filePath + @"\" + bill.BillNumber + ".xlsx");
            if (newFile.Exists)
            {
                //Normaly impossible
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filePath + @"\" + bill.BillNumber + ".xlsx");
            }
            else
            {
                Directory.CreateDirectory(filePath);
            }
            
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Facture");
                int row = 1;
                //Add global informations
                worksheet.Cells[row, 1].Value = "Producteur :";
                worksheet.Cells[row, 2].Value = producer.CompanyName;
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row ,2].Style.Font.Size = 14;
                row++;
                worksheet.Cells[row, 1].Value = "Numéro de facture :";
                worksheet.Cells[row, 2].Value = bill.BillNumber;
                row++;
                worksheet.Cells[row, 1].Value = "Année :";
                worksheet.Cells[row, 2].Value = DateTime.Now.Year;
                row++;
                worksheet.Cells[row, 1].Value = "Semaine :";
                worksheet.Cells[row, 2].Value = DateTime.Now.GetIso8601WeekOfYear();
                row++;
                worksheet.Cells[1, 1, row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[1, 1, row, 1].Style.Font.Bold = true;
                //Add product informations
                row++;
                row++;
                worksheet.Cells[row, 1].Value = "PRODUITS :";
                worksheet.Cells[row, 1].Style.Font.Size = 18;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                //Create list of bill entry by product
                Dictionary<Product, List<BillEntryConsumer>> products = new Dictionary<Product, List<BillEntryConsumer>>();
                foreach (var billEntryConsumer in billEntries)
                {
                    if(!products.ContainsKey(billEntryConsumer.BillEntry.Product))
                    {
                        products.Add(billEntryConsumer.BillEntry.Product, new List<BillEntryConsumer>());
                    }
                    products[billEntryConsumer.BillEntry.Product].Add(billEntryConsumer);
                }
                List<int> rowsTotal = new List<int>();
                // - Add products
                foreach (var prod in products)
                {
                    // - Add the headers
                    worksheet.Cells[row, 2].Value = EnumHelper<Product.SellType>.GetDisplayValue(prod.Key.Type) + ( prod.Key.Type == Product.SellType.Piece ? "" : " (" + prod.Key.ProductUnit.ToString() + ")"  );
                    worksheet.Cells[row, 3].Value = prod.Key.Price ;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "0.00€";
                    worksheet.Cells[row, 2, row, 3].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 3].Style.Font.Size = 12;
                    worksheet.Cells[row, 2, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2, row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    int unitPriceRow = row;
                    row++;
                    worksheet.Cells[row - 1, 1, row, 1].Merge = true;
                    worksheet.Cells[row - 1, 1, row, 1].Value = prod.Key.Name;
                    worksheet.Cells[row - 1, 1, row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row - 1, 1, row, 1].Style.Font.Size = 16;
                    worksheet.Cells[row - 1, 1, row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row - 1, 1, row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row - 1, 1, row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells[row, 2].Value = "Quantité";
                    worksheet.Cells[row, 3].Value = "Prix total";
                    worksheet.Cells[row, 2, row, 3].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 3].Style.Font.Size = 12;
                    worksheet.Cells[row, 2, row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row++;
                    int productStartRow = row;
                    foreach (var billEntryConsumer in prod.Value.OrderBy(x=>x.Consumer.Id))
                    {
                        worksheet.Cells[row, 1].Value = "• " + billEntryConsumer.Consumer.Id;
                        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[row, 2].Value = billEntryConsumer.BillEntry.Quantity;
                        worksheet.Cells[row, 3].Formula = new ExcelCellAddress(row, 2).Address + "*" + new ExcelCellAddress(unitPriceRow, 3).Address;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "0.00€";
                        worksheet.Cells[row, 4].Value = "☐";
                        worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        row++;
                    }
                    worksheet.Cells[productStartRow, 1,row,1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[productStartRow, 2, row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[productStartRow, 2, row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rowsTotal.Add(row);
                    //Total
                    worksheet.Cells[row, 1].Value = "TOTAL";
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //Quantity
                    worksheet.Cells[row, 2].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(productStartRow, 2, row - 1, 2).Address);
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //Prix
                    worksheet.Cells[row, 3].Formula = new ExcelCellAddress(row, 2).Address + "*" + new ExcelCellAddress(unitPriceRow, 3).Address;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "0.00€";
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //
                    worksheet.Cells[row, 1, row, 3].Style.Font.Bold = true;
                    worksheet.Cells[row, 1, row, 3].Style.Font.Size = 14;
                    worksheet.Cells[row, 1, row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //Next product
                    row++;
                    row++;
                }
                //Super total
                string totalFormula ="";
                for(int cpt = 0; cpt<rowsTotal.Count;cpt++)
                {
                    totalFormula += new ExcelCellAddress(rowsTotal[cpt], 3).Address;
                    if(cpt != rowsTotal.Count -1)
                    {
                        totalFormula += "+";
                    }
                }
                worksheet.Cells[row, 2].Value = "TOTAL :";
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[row, 3].Formula = totalFormula;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "0.00€";
                worksheet.Cells[row, 2, row, 3].Style.Font.Bold = true;
                worksheet.Cells[row, 2, row ,3].Style.Font.Size = 18;
                
                // Document properties
                package.Workbook.Properties.Title = "Facture : " + bill.BillNumber;
                package.Workbook.Properties.Author = "Stolons";
                package.Workbook.Properties.Comments = "Facture de la semaine " + bill.BillNumber;

                // Extended property values
                package.Workbook.Properties.Company = "Association Stolons";

                //
                worksheet.View.PageLayoutView = true;
                worksheet.Column(1).Width = (300 - 12 + 5) / 7d + 1;
                worksheet.Column(2).Width = (125 - 12 + 5) / 7d + 1;
                worksheet.Column(3).Width = (125 - 12 + 5) / 7d + 1;
                worksheet.Column(4).Width = (40 - 12 + 5) / 7d + 1;

                // save our new workbook and we are done!
                package.Save();

            }
        

            //
            return bill;
        }

        private static ConsumerBill GenerateBill(ValidatedWeekBasket weekBasket, ApplicationDbContext dbContext)
        {
            ConsumerBill bill = CreateBill<ConsumerBill>(weekBasket.Consumer);
            //Generate exel file with bill number for user
            string filePath = Path.Combine(Configurations.Environment.WebRootPath, Configurations.ConsumersBillsStockagePath,bill.User.Id.ToString());
            FileInfo newFile = new FileInfo(filePath + @"\"+ bill .BillNumber+ ".xlsx");
            if (newFile.Exists)
            {
                //Normaly impossible
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filePath + @"\" + bill.BillNumber + ".xlsx");
            }
            else
            {
                Directory.CreateDirectory(filePath);
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Facture");
                //Add global informations
                int row = 1;
                worksheet.Cells[row, 1].Value = Configurations.ApplicationConfig.StolonsLabel;
                row++;
                worksheet.Cells[row, 1].Value = Configurations.ApplicationConfig.StolonsMailAdress;
                row++;
                worksheet.Cells[row, 1].Value = Configurations.ApplicationConfig.StolonsPhoneNumber;
                row++;
                worksheet.Cells[row, 1].Value = Configurations.ApplicationConfig.StolonsAddress;
                row++;
                worksheet.Cells[row, 5].Value = weekBasket.Consumer.Surname + ", " + weekBasket.Consumer.Name;
                row++;
                worksheet.Cells[row, 5].Value = weekBasket.Consumer.Email;
                row++;
                worksheet.Cells[row, 1].Value = "Numéro d'adhérent :";
                worksheet.Cells[row, 2].Value = weekBasket.Consumer.Id;
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                row++;
                worksheet.Cells[row, 1].Value = "Numéro de facture :";
                worksheet.Cells[row, 2].Value = bill.BillNumber;
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                row++;
                worksheet.Cells[row, 1].Value = "Année :";
                worksheet.Cells[row, 2].Value = DateTime.Now.Year;
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                row++;
                worksheet.Cells[row, 1].Value = "Semaine :";
                worksheet.Cells[row, 2].Value = DateTime.Now.GetIso8601WeekOfYear();
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                row++;
                worksheet.Cells[row, 1].Value = "Edité le :";
                worksheet.Cells[row, 2].Value = DateTime.Now.ToString();
                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                row++;
                row++;
                //Add product informations
                worksheet.Cells[row, 1, row + 1, 6].Merge = true;
                worksheet.Cells[row, 1, row + 1, 6].Value = "Produits de votre panier de la semaine";
                worksheet.Cells[row, 1, row + 1, 6].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row + 1, 6].Style.Font.Size = 14;
                row++;
                row++;
                // - Add the headers
                worksheet.Cells[row, 1].Value = "NOM";
                worksheet.Cells[row, 2].Value = "FAMILLE";
                worksheet.Cells[row, 3].Value = "TYPE";
                worksheet.Cells[row, 4].Value = "PRIX UNITAIRE";
                worksheet.Cells[row, 5].Value = "QUANTITE ";
                worksheet.Cells[row, 6].Value = "MONTANT";
                worksheet.Cells[row, 1, row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1, row, 6].Style.Font.Bold = true;
                row++;
                int startRow = row;
                // - Add products
                foreach (var tmpBillEntry in weekBasket.Products)
                {
                    var billEntry = dbContext.BillEntrys.Include(x => x.Product).ThenInclude(x => x.Familly).First(x => x.Id == tmpBillEntry.Id);
                    worksheet.Cells[row, 1].Value = billEntry.Product.Name;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 2].Value = billEntry.Product.Familly.FamillyName;
                    worksheet.Cells[row, 3].Value = EnumHelper<Product.SellType>.GetDisplayValue(billEntry.Product.Type);
                    worksheet.Cells[row, 4].Value = billEntry.Product.Price;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00€";
                    worksheet.Cells[row, 5].Value = billEntry.Quantity;
                    worksheet.Cells[row, 6].Formula = new ExcelCellAddress(row,4).Address +"*" + new ExcelCellAddress(row, 5).Address;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00€";
                    row++;
                }
                worksheet.Cells[startRow - 1, 1, row - 1, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //- Add TOTAL
                worksheet.Cells[row, 5].Value = "TOTAL : ";
                worksheet.Cells[row, 5].Style.Font.Bold = true;

                //Add a formula for the value-column
                worksheet.Cells[row, 6].Formula = string.Format("SUBTOTAL(9,{0})", new OfficeOpenXml.ExcelAddress(7, 6, row -1, 6).Address);
                worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00€";
                worksheet.Cells[row, 6].Style.Font.Bold = true;

                row++;
                row++;
                worksheet.Cells[row, 1].Value = Configurations.ApplicationConfig.OrderDeliveryMessage;
                worksheet.Cells[row, 1].Style.Font.Bold = true;

                // Document properties
                package.Workbook.Properties.Title = "Facture : " + bill.BillNumber;
                package.Workbook.Properties.Author = "Stolons";
                package.Workbook.Properties.Comments = "Facture de la semaine " + bill.BillNumber;

                // Extended property values
                package.Workbook.Properties.Company = "Association Stolons";
                //Column size
                worksheet.View.PageLayoutView = true;
                worksheet.Column(1).Width = (138 - 12 + 5) / 7d + 1;
                worksheet.Column(2).Width = (90 - 12 + 5) / 7d + 1;
                worksheet.Column(3).Width = (90 - 12 + 5) / 7d + 1;
                worksheet.Column(4).Width = (90 - 12 + 5) / 7d + 1;
                worksheet.Column(5).Width = (90 - 12 + 5) / 7d + 1;
                worksheet.Column(6).Width = (90 - 12 + 5) / 7d + 1;
                // save our new workbook and we are done!
                package.Save();
            }
            //
            return bill;
        }

        private static T CreateBill<T>(User user) where T : class, IBill , new()
        {
            IBill bill = new T();
            bill.BillNumber = DateTime.Now.Year + "_" + DateTime.Now.GetIso8601WeekOfYear() +"_" + user.Id;
            bill.User = user;
            bill.State = BillState.Pending;
            bill.EditionDate = DateTime.Now;
            return bill as T;
        }

        //Exemple de comment générer un ficher exel
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

        public static int GetIso8601WeekOfYear(this DateTime time)
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
