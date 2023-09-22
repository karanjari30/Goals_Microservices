namespace BusinessAccessLayer.ViewModel
{
    public class XeroCustomerReqViewModel
    {
        //public string EntityName { get; set; }
        public string TenantId { get; set; }
        public XeroFilterDataModel? Data { get; set; }
    }
    public class XeroFilterDataModel
    {
        public DateTime? LastModifiedDate { get; set; }
        public string? Where { get; set; }
        public string? OrderBy { get; set; }
        public List<Guid>? ContactIDs { get; set; }
        public int? Page { get; set; }
        public bool? IncludeArchived { get; set; }
        public bool? SummaryOnly { get; set; }
        public string? SearchTerm { get; set; }
    }
}
