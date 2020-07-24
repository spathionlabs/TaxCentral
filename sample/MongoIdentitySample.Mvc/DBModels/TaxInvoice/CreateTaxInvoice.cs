using Main.Mvc.Models;
using System;
using System.Collections.Generic;


namespace Main.Mvc.DBModels.TaxInvoice
{
    enum TaxInvoiceStatus
    {
        Created,
        Rejected,
        MakePayment,
        SchedulePayment
    }
    public class CreateTaxInvoice : EntityBase

    {
        public string DateIssue { get; set; }
        public string SerialNumber { get; set; }
        public string NameoFSeller { get; set; }
        public string GSTIDSeller { get; set; }
        public string NameBuyer { get; set; }
        public string GSTIdBuyer { get; set; }
        public string RegisteNoBuyer { get; set; }

        public Guid MerchantId { get; set; }
        public string EmailBuyer { get; set; }
        public List<InvoiceItemDetail> itemDetails { get; set; }
        public double TotalAmount { get; set; }
        public double GSTAmount { get; set; }
        public int TaxStatus { get; set; }
        public string PONO { get; set; }

        public DateTime? PaymentOn { get; set; }

        public DateTime CreatedOn { get; set; }

    }
    public class InvoiceItemDetail
    {
        public string Product { get; set; }
        public double GST { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
    }

}
