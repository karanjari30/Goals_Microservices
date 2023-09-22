namespace BusinessAccessLayer.Common
{
    public class Message
    {
        public const string CompanyIdMessage = "CompanyId is empty or null";
        public const string TenantIdMessage = "TenantId is empty or null";
        public const string OrganizationNotFoundMessage = "Connection not found";
        public const string OrganizationFoundMessage = "Connection found";
        public const string OrganizationDisconnectMessage = "QuickBooks disconnect successfully";
        public const string OrganizationAlreadyDisconnectMessage = "QuickBooks company already disconnected";
        public const string QuickBookCompanyConnected = "QuickBooks company connected";
        public const string QuickBookCompanyAlreadyConnected = "QuickBooks company already connected";
        public const string XeroCompanyConnected = "Xero company connected";
        public const string XeroCompanyAlreadyConnected = "Xero company already connected";

        public const string FetchRecords = "Records fetched Successfully";
        public const string UnAuthorizedUser = "UnAuthorized User";
        public const string InvoicesFoundMessage = "Invoices found";
        public const string InvoicesNotFoundMessage = "Invoices not found";
        public const string CustomersFoundMessage = "Customers found";
        public const string CustomersNotFoundMessage = "Customers not found";
        public const string QBOTokenExpire = "QuickBooks access token expire or not found";
        public const string XeroTokenExpire = "Xero access token expire or not found";
        public const string QueueItemNotFound = "Queue Item Not Found.";
        public const string InvalidData = "Invalid data";
        public const string CustomerIdMessage = "Customer is empty or null";
        public const string CustomerDeleteMessage = "Customer is deleted";
        public const string CustomerInsertUpdateMessage = "Customer is insert or update successfully";
        public const string InvoiceIdMessage = "Invoice is empty or null";
        public const string InvoiceInsertUpdateMessage = "Invoice is insert or update successfully";
        public const string InvoiceDeleteMessage = "Invoice is deleted";
        public const string PaymentsFoundMessage = "Payments found";
        public const string PaymentsNotFoundMessage = "Payments not found";
        public const string PaymentInsertUpdateMessage = "Payment is insert or update successfully";
        public const string PaymentIdMessage = "Payment is empty or null";
        public const string PaymentDeleteMessage = "Payment is deleted";
        public const string XeroCustomerIdMessage = "ContactId is null or empty";
    }
    public static class GetCommonMessage
    {
        public static string GetExceptionMessage(Exception ex)
        {
            string retMessage = string.Empty;
            retMessage = ex.Message;
            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
            {
                retMessage = ex.InnerException.Message;
            }
            return retMessage;
        }
    }
}
