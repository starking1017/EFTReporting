//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class EFTAdmin
    {
        public string PclawDb { get; set; }
        public string PclawVendorNo { get; set; }
        public string PclawVendorName { get; set; }
        public Nullable<System.DateTime> EFTStartupDate { get; set; }
        public string LegalCompanyName { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorProvince { get; set; }
        public string VendorPostal { get; set; }
        public string BankAddress1 { get; set; }
        public string BankAddress2 { get; set; }
        public string BankCity { get; set; }
        public string BankProvince { get; set; }
        public string BankPostal { get; set; }
        public string FinancialInstitute { get; set; }
        public string TransitNo { get; set; }
        public string AccountNo { get; set; }
        public string RemittanceEmail { get; set; }
        public bool IsDeActivate { get; set; }
    }
}
