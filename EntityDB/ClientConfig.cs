//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
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
