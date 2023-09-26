using Xero.NetStandard.OAuth2.Model.Accounting;

namespace BusinessAccessLayer.ViewModel
{
    public class XeroInvoiceInsertUpdateReqViewModel
    {
        public string TenantId { get; set; }
        public List<XeroInvoiceModel> Invoices { get; set; }
    }

    public class XeroInvoiceModel
    {
        public Guid? InvoiceID { get; set; }
        public string Reference { get; set; }
        public Invoice.StatusEnum Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ContactId { get; set; }
       public List<XeroInvoiceLineReq> LineItems { get; set; }
    }

    public class XeroInvoiceLineReq
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public string AccountCode { get; set; }
    }
}
