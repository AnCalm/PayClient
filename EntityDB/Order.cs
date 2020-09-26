//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int OrderID { get; set; }
        public string OrderInsideID { get; set; }
        public string OrderExternalID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> ProductParValue { get; set; }
        public Nullable<decimal> ProductSalePrice { get; set; }
        public string TargetAccount { get; set; }
        public string TargetAccountType { get; set; }
        public string TargetAccountTypeName { get; set; }
        public Nullable<int> BuyAmount { get; set; }
        public Nullable<decimal> TotalSalePrice { get; set; }
        public string Game { get; set; }
        public string GameName { get; set; }
        public string Area { get; set; }
        public string AreaName { get; set; }
        public string Server { get; set; }
        public string ServerName { get; set; }
        public string RechargeMode { get; set; }
        public string RechargeModeName { get; set; }
        public string StockMerchantId { get; set; }
        public string StockMerchantName { get; set; }
        public string CustomerIp { get; set; }
        public string CustomerRegion { get; set; }
        public Nullable<System.DateTime> DealDateTime { get; set; }
        public Nullable<System.DateTime> StartDatetime { get; set; }
        public Nullable<System.DateTime> EndDatetime { get; set; }
        public Nullable<int> RechargeStatus { get; set; }
        public Nullable<decimal> SuccessfulAmount { get; set; }
        public string ChargeAccountInfo { get; set; }
        public string RechargeMsg { get; set; }
        public Nullable<bool> IsNotify { get; set; }
        public string MerchantCode { get; set; }
    }
}