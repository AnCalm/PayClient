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
    
    public partial class ChargeAccountType
    {
        public int ChargeAccountTypeID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsCard { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
