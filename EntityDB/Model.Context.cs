﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DCLEntities : DbContext
    {
        public DCLEntities()
            : base("name=DCLEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Cards> Cards { get; set; }
        public virtual DbSet<ChargeAccountType> ChargeAccountType { get; set; }
        public virtual DbSet<ChargeClass> ChargeClass { get; set; }
        public virtual DbSet<ClientConfig> ClientConfig { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderChargeAccount> OrderChargeAccount { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ChargeClassInfo> ChargeClassInfo { get; set; }
        public virtual DbSet<ProductInfo> ProductInfo { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
