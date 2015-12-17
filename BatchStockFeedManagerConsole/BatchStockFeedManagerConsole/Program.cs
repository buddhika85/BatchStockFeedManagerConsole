using BatchStockFeedManagerConsole.DAL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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
                // laser virgin
                bool wasCreated = CreateExcelSheet("laser", "virgin", string.Format("laser_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ? 
                                    "Laser - Virgin - excel file created" : 
                                    "Error - Laser - Virgin - excel file not created");

                // inkjet virgin
                wasCreated = CreateExcelSheet("inkjet", "virgin", string.Format("inkjet_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inkjet - Virgin - excel file created" :
                                    "Error - Inkjet - Virgin - excel file not created");

                // inktank virgin
                wasCreated = CreateExcelSheet("inktank", "virgin", string.Format("inktank_virgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inktank - Virgin - excel file created" :
                                    "Error - Inktank - Virgin - excel file not created");

                // laser non virgin
                wasCreated = CreateExcelSheet("laser", "no", string.Format("laser_nonvirgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Laser - Non Virgin - excel file created" :
                                    "Error - Laser - Non Virgin - excel file not created");

                // inkjet virgin
                wasCreated = CreateExcelSheet("inkjet", "no", string.Format("inkjet_nonvirgin_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture)));
                Console.WriteLine(wasCreated ?
                                    "Inkjet - Non Virgin - excel file created" :
                                    "Error - Inkjet - Non Virgin - excel file not created");
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
        /// Helper method which creates excel sheets
        /// </summary>
        private static bool CreateExcelSheet(string category, string virginOrNot, string fileName)
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
            }
            return wasCreated;
        }

    }
}
