namespace BusinessAccessLayer.ViewModel
{
    public class XeroPaymentInsertUpdateReqViewModel
    {
        public string TenantId { get; set; }
        public List<XeroPaymentModel> Payments { get; set; }
    }
    public class XeroPaymentModel
    {
        public string InvoiceId { get; set; }
        public string AccountId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
