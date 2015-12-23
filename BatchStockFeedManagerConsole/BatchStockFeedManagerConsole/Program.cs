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

using System.Configuration;
using System.Collections.Specialized;
using System.IO;



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
                // Read excel sheet
                IList<ProductStockUserDefinedType> currentStockCounts = ReadExcelSheet();               
                string result = new DataAccessor().BatchUpload(currentStockCounts);
                Console.WriteLine("result : " + result);                
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        /// <summary>
        /// A helper method to read the excel sheet
        /// </summary>
        private static IList<ProductStockUserDefinedType> ReadExcelSheet()
        {
            IList<ProductStockUserDefinedType> excelData = new List<ProductStockUserDefinedType>();
            try
            {
                // get file locations
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                string fileLocation = appSettings["laserVirgin"];                       // laser virgin
                excelData = ReadExcelToAList(excelData, fileLocation);

                fileLocation = appSettings["inkjetVirgin"];                             // inkjet virgin
                excelData = ReadExcelToAList(excelData, fileLocation);

                fileLocation = appSettings["inktankVirgin"];                            // inktank virgin
                excelData = ReadExcelToAList(excelData, fileLocation);

                fileLocation = appSettings["laserNonVirgin"];                           // laser non virgin
                excelData = ReadExcelToAList(excelData, fileLocation);

                fileLocation = appSettings["inkjetNonVirgin"];                          // inkjet non virgin
                excelData = ReadExcelToAList(excelData, fileLocation);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return excelData;
        }

        /// <summary>
        /// A helper method to read excel sheet and create a list and return it
        /// </summary>
        private static IList<ProductStockUserDefinedType> ReadExcelToAList(IList<ProductStockUserDefinedType> excelData, string laserVirginExcelLoc)
        {
            try
            {
                FileInfo file = new FileInfo(laserVirginExcelLoc);
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorkbook workbook = package.Workbook;
                    if (workbook != null)
                    {
                        ExcelWorksheet firstWorksheet = workbook.Worksheets.First();
                        for (int rowNumber = 2; rowNumber <= firstWorksheet.Dimension.End.Row; rowNumber++)
                        {
                            ExcelRange row = firstWorksheet.Cells[rowNumber, 1, rowNumber, firstWorksheet.Dimension.End.Column];

                            ProductStockUserDefinedType product = new ProductStockUserDefinedType();
                            // productId
                            string cellName = string.Format("E{0}", rowNumber);
                            product.productId = int.Parse(firstWorksheet.Cells[cellName].Value.ToString().Trim());

                            // quantity
                            cellName = string.Format("F{0}", rowNumber);
                            product.quantity = int.Parse(firstWorksheet.Cells[cellName].Value.ToString().Trim());

                            // stockCountAmended, lastAmendedDate, lastIncrementDate
                            product.stockCountAmended = "yes";
                            product.lastAmendedDate = DateTime.Now;
                            product.lastIncrementDate = null;

                            excelData.Add(product);
                        }
                    }                
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }

            return excelData;
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

                // inkjet non virgin
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
                    Random rnd = new Random();
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

                        // add quantities - for testing
                        //cellName = string.Format("F{0}", rowNumExcel);
                        //worksheet.Cells[cellName].Value = rnd.Next(500, 1000);
                        
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
