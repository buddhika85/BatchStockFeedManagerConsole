//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BatchStockFeedManagerConsole.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class vProductInfo
    {
        public int productlistid { get; set; }
        public string model { get; set; }
        public int quantity { get; set; }
        public string Description { get; set; }
        public int weight_grams { get; set; }
        public int volume_cm3 { get; set; }
        public string product_image { get; set; }
        public string abbr { get; set; }
        public string model_public { get; set; }
        public string description_public { get; set; }
        public Nullable<System.DateTime> dateUpdated { get; set; }
        public string status { get; set; }
        public int productbrandid { get; set; }
        public string productbrandname { get; set; }
        public decimal marketvalue { get; set; }
        public Nullable<int> actionID { get; set; }
        public string productActionName { get; set; }
        public string producttypeid { get; set; }
        public Nullable<int> productcategory { get; set; }
        public string ProductCatergoryName { get; set; }
        public Nullable<int> productcondition { get; set; }
        public string conditionName { get; set; }
    }
}
