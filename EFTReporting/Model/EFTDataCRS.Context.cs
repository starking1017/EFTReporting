﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFTReporting.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PCLAW_FRSReportsEntities : DbContext
    {
        public PCLAW_FRSReportsEntities()
            : base("name=PCLAW_FRSReportsEntities")
        {
        }

        public PCLAW_FRSReportsEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<APVendLi_tbl_vw> APVendLi_tbl_vw { get; set; }
        public virtual DbSet<EFT_Reporting_VendorList_vw> EFT_Reporting_VendorList_vw { get; set; }
    
        public virtual ObjectResult<EFT_Reporting_AccountPaymentDetail_Result> EFT_Reporting_AccountPaymentDetail(Nullable<int> vendorID, Nullable<long> payDate)
        {
            var vendorIDParameter = vendorID.HasValue ?
                new ObjectParameter("VendorID", vendorID) :
                new ObjectParameter("VendorID", typeof(int));
    
            var payDateParameter = payDate.HasValue ?
                new ObjectParameter("PayDate", payDate) :
                new ObjectParameter("PayDate", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EFT_Reporting_AccountPaymentDetail_Result>("EFT_Reporting_AccountPaymentDetail", vendorIDParameter, payDateParameter);
        }
    }
}
