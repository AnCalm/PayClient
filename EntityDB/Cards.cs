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
    
    public partial class Cards
    {
        public int CardsID { get; set; }
        public int ChargeAccountTypeID { get; set; }
        public string CardNumber { get; set; }
        public string CardPassWord { get; set; }
        public decimal Price { get; set; }
        public Nullable<int> ReChargeStatus { get; set; }
        public string ReChargeMsg { get; set; }
        public bool IsAvailable { get; set; }
        public Nullable<System.DateTime> CreatTime { get; set; }
        public Nullable<System.DateTime> UseTime { get; set; }
    }
}
