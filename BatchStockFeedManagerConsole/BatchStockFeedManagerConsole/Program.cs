using BatchStockFeedManagerConsole.DAL;
using BatchStockFeedManagerConsole.DAL.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BatchStockFeedManagerConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("What do want to do? input for 1 for create input 2 for read");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    Create();
                }
                else if (input == "2")
                {
                    ReadAndBatchUpload();
                }
                else {
                    Console.WriteLine("\nInvalid input - input can only be 1 or 2");
                }
            }
            catch (Exception ex)
            {                
                Console.Write(string.Format("Exception : {0} | Source : {1} | Inner exception : {2}", 
                        ex.Message, ex.Source ,ex.InnerException));
            }
            Console.WriteLine("\nPress any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// A helper which reads excel file
        /// </summary>
        private static void ReadAndBatchUpload()
        {
            try
            {                
                IList<ProductStockUserDefinedType> currentStockCounts = new List<ProductStockUserDefinedType>() { 
                    new ProductStockUserDefinedType() {
                        productId = 1,
                        quantity = 50,
                        stockCountAmended = "yes",
                        lastAmendedDate = DateTime.Now,
                        lastIncrementDate = null
                    },
                    new ProductStockUserDefinedType() {
                        productId = 2,
                        quantity = 60,
                        stockCountAmended = "no",
                        lastAmendedDate = DateTime.Now,
                        lastIncrementDate = null
                    },
                };
                int result = new DataAccessor().BatchUpload(currentStockCounts);
                Console.WriteLine("result : " + result);
                //foreach (ProductStockUserDefinedType item in result)
                //{
                //    Console.WriteLine(item.productId + " | " + item.quantity + " | " + item.stockCountAmended
                //            + item.lastAmendedDate + " | " + item.lastIncrementDate);
                //}
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        /// <summary>
        /// A helper wich creates excel files
        /// </summary>
        private static void Create()
        {
            try
            {
                // laser virgin
                bool wasCreated = WriteExcelSheet("laser", "virgin", string.Format("laser_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Laser - Virgin - excel file created" :
                                    "Error - Laser - Virgin - excel file not created");

                // inkjet virgin
                wasCreated = WriteExcelSheet("inkjet", "virgin", string.Format("inkjet_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inkjet - Virgin - excel file created" :
                                    "Error - Inkjet - Virgin - excel file not created");

                // inktank virgin
                wasCreated = WriteExcelSheet("inktank", "virgin", string.Format("inktank_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inktank - Virgin - excel file created" :
                                    "Error - Inktank - Virgin - excel file not created");

                // laser non virgin
                wasCreated = WriteExcelSheet("laser", "no", string.Format("laser_nonvirgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Laser - Non Virgin - excel file created" :
                                    "Error - Laser - Non Virgin - excel file not created");

                // inkjet virgin
                wasCreated = WriteExcelSheet("inkjet", "no", string.Format("inkjet_nonvirgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inkjet - Non Virgin - excel file created" :
                                    "Error - Inkjet - Non Virgin - excel file not created");
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        /// <summary>
        /// Helper method which creates excel sheets
        /// </summary>
        private static bool WriteExcelSheet(string category, string virginOrNot, string fileName)
        {
            bool wasCreated = false;
            try
            {
                IList<SP_FiterProducts_Result> result = new DataAccessor().GetFilteredProducts(category, virginOrNot);

                using (var package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // set up headers                   
                    worksheet.Cells["A1"].Value = "Condition";
                    worksheet.Cells["B1"].Value = "Brand";
                    worksheet.Cells["C1"].Value = "Model";
                    worksheet.Cells["D1"].Value = "Action";
                    worksheet.Cells["E1"].Value = "Product Id";
                    worksheet.Cells["F1"].Value = "Total stock quantity";
                    worksheet.Cells["G1"].Value = "Size of box";
                    worksheet.Cells["H1"].Value = "Qty per box";

                    int rowNumExcel = 2;
                    foreach (SP_FiterProducts_Result item in result)
                    {
                        // condition
                        string cellName = string.Format("A{0}", rowNumExcel);
                        worksheet.Cells[cellName].Value = item.conditionName;

                        // brand
                        cellName = string.Format("B{0}", rowNumExcel);
                        worksheet.Cells[cellName].Value = item.productbrandname;

                        // model
                        cellName = string.Format("C{0}", rowNumExcel);
                        worksheet.Cells[cellName].Value = item.model;

                        // action
                        cellName = string.Format("D{0}", rowNumExcel);
                        worksheet.Cells[cellName].Value = item.productActionName;

                        // product Id
                        cellName = string.Format("E{0}", rowNumExcel);
                        worksheet.Cells[cellName].Value = item.productlistid;
                        
                        ++rowNumExcel;
                    }                 


                    string fileLocationWithName = string.Format(@"E:\KTP Project\Dropbox\Stage 5 - Live Stock Feed\BatchStockFeedManagerConsole\BatchStockFeedManagerConsole\BatchStockFeedManagerConsole\ExcelFiles\{0}.xlsx", fileName);
                    package.SaveAs(new System.IO.FileInfo(fileLocationWithName));
                }

                wasCreated = true;
            }
            catch (Exception ex)
            {
                wasCreated = false;
                throw ex;
            }
            return wasCreated;
        }

    }
}
