using static BusinessAccessLayer.Common.Enums;

namespace BusinessAccessLayer.Common
{
    public class ResultPT
    {
        public ResponseStatus ResponseStatus { get; set; }
        public string Message { get; set; }
        public dynamic Result { get; set; }
    }

    public class DownloadResultPT
    {
        public object ReturnObject { get; set; } //= null;
        public ResponseStatus TransactionStatus { get; set; }
        public string ResultMsg { get; set; }
    }
    public class CustomerManaualSyncResult : DownloadResultPT
    {
        public List<Intuit.Ipp.Data.Customer> ReturnObject { get; set; }
    }
    public class InvoiceSyncResult : DownloadResultPT
    {
        public List<Intuit.Ipp.Data.Invoice> ReturnObject { get; set; }
    }
    public class PaymentSyncResult : DownloadResultPT
    {
        public List<Intuit.Ipp.Data.Payment> ReturnObject { get; set; }
    }
    public class XeroCustomerSyncResult : DownloadResultPT
    {
        public List<Xero.NetStandard.OAuth2.Model.Accounting.Contact> ReturnObject { get; set; }
    }
    public class XeroInvoiceSyncResult : DownloadResultPT
    {
        public List<Xero.NetStandard.OAuth2.Model.Accounting.Invoice> ReturnObject { get; set; }
    }
    public class XeroPaymentSyncResult : DownloadResultPT
    {
        public List<Xero.NetStandard.OAuth2.Model.Accounting.Payment> ReturnObject { get; set; }
    }
}