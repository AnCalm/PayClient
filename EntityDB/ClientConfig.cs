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
    
    public partial class ClientConfig
    {
        public int ClientConfigID { get; set; }
        public string MerchantID { get; set; }
        public string MerchantKey { get; set; }
        public string GetOrdersURL { get; set; }
        public string NotifyOrderURL { get; set; }
        public Nullable<int> GetOrderCount { get; set; }
        public Nullable<int> GetOrderTime { get; set; }
        public string MerchantCode { get; set; }
        public string Description { get; set; }
        public string WinSkin { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
    }
}