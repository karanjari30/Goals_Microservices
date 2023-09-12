using Intuit.Ipp.Data;

namespace BusinessAccessLayer.ViewModel
{
    public class QBOCustomerInsertUpdateReqViewModel
    {
        public string AccountingCompanyId { get; set; }
        public List<Customer> customers { get; set; }
    }
}
