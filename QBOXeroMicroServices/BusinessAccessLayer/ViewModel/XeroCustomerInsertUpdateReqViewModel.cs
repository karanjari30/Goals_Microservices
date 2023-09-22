using Xero.NetStandard.OAuth2.Model.Accounting;

namespace BusinessAccessLayer.ViewModel
{
    public class XeroCustomerInsertUpdateReqViewModel
    {
        public string TenantId { get; set; }
        public List<XeroContactModel> Contacts { get; set; }
    }
    public class XeroContactModel
    {
        public Guid? ContactID { get; set; }

        public string? ContactNumber { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public Address? Address { get; set; }
    }
}
