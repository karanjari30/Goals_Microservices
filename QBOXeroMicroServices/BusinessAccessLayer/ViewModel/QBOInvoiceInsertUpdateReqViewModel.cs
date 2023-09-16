using Intuit.Ipp.Data;

namespace BusinessAccessLayer.ViewModel
{
    public class QBOInvoiceInsertUpdateReqViewModel
    {
        public string AccountingCompanyId { get; set; }
        public List<InvoiceReqViewModel> Invoices { get; set; }
    }

    public class InvoiceReqViewModel
    {
        public string DocNumber { get; set; }
        public DateTime TxnDate { get; set; }
        public string CustomerRefValue { get; set; }
        public string Id { get; set; }
        public List<InvoiceReqLine> Line { get; set; }
    }
    public class InvoiceReqLine
    {
        public string DetailType { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public InvoiceReqSalesItemLineDetail SalesItemLineDetail { get; set; }
    }
    public class InvoiceReqSalesItemLineDetail
    {
        public string ItemRefValue { get; set; }
    }
}
