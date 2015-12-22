using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchStockFeedManagerConsole.DAL.ViewModels
{
    /// <summary>
    /// A class which model the user defined table type of the database - ProductStockUserDefinedType
    /// </summary>
    public class ProductStockUserDefinedType
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public string stockCountAmended { get; set; }
        public DateTime? lastAmendedDate { get; set; }
        public DateTime? lastIncrementDate { get; set; }
    }
}
