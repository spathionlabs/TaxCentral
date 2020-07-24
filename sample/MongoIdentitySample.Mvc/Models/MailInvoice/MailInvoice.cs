using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Mvc.Models.MailInvoice
{
    public class MailInvoice: EntityBase
    {
        // public ObjectId Id { get; set; }
        public string ToEmail { get; set; }

        public String FromEmail { get; set; }
        // public ObjectId MerchantId { get; set; }
        public InvoiceJSON invoiceJSON { get; set; }
        public DateTime CreatedOn { get; set; }
        public MailInvoiceStatus InvoiceStatus { get; set; }

        public TaxPractitionerStatus taxpractitionerstatus { get; set; }

        public string TaxPractioner { get; set; }

        public MailInvoiceCreationType mailInvoiceCreationType { get; set; }

        //public string Hash { get; set; }
        public bool IsSend { get; set; }
        public string HashUrl { get; set; }

    }

    public enum MailInvoiceCreationType
    {
        Created,
        Received,
        Uploaded,
        SMTP
    }
    public enum MailInvoiceStatus
    {
        Created,
        Accepted,
        Rejected
    }
    public enum TaxPractitionerStatus
    { None, Validated, Rejected }

    public class InvoiceJSON
    {
        public string version { get; set; }
        public billLists[] billLists { get; set; }
    }
    public class billLists
    {
        public string userGstin { get; set; }
        public string supplyType { get; set; }
        public int subSupplyType { get; set; }
        public string docType { get; set; }
        public string docNo { get; set; }

        public string docDate { get; set; }
        public int transType { get; set; }
        public string fromGstin { get; set; }
        public string fromTrdName { get; set; }
        public string fromAddr1 { get; set; }
        public string fromAddr2 { get; set; }
        public string fromPlace { get; set; }
        public int fromStateCode { get; set; }
        public int actualFromStateCode { get; set; }
        public string toGstin { get; set; }
        public string toTrdName { get; set; }
        public string toAddr1 { get; set; }
        public string toAddr2 { get; set; }
        public string toPlace { get; set; }
        public int toStateCode { get; set; }
        public int actualToStateCode { get; set; }
        public double totalValue { get; set; }
        public double cgstValue { get; set; }
        public double sgstValue { get; set; }
        public double igstValue { get; set; }
        public double cessValue { get; set; }
        public double TotNonAdvolVal { get; set; }
        public double OthValue { get; set; }
        public double totInvValue { get; set; }
        public int transMode { get; set; }
        public int transDistance { get; set; }
        public string transporterName { get; set; }
        public string transporterId { get; set; }
        public string transDocNo { get; set; }
        public string transDocDate { get; set; }
        public string vehicleNo { get; set; }
        public string vehicleType { get; set; }
        public ItemLists[] itemList { get; set; }
    }
    public class ItemLists
    {
        public int itemNo { get; set; }
        public string productName { get; set; }
        public string productDesc { get; set; }
        public double quantity { get; set; }
        public string qtyUnit { get; set; }
        public double taxableAmount { get; set; }
        public int sgstRate { get; set; }
        public int cgstRate { get; set; }
        public int igstRate { get; set; }
        public int cessRate { get; set; }
        public int cessNonAdvol { get; set; }


    }
}
