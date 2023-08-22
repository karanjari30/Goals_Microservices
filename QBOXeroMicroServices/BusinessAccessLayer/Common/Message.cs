using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Common
{
    public class Message
    {
        public const string CompanyIdMessage = "CompanyId is empty or null";
        public const string OrganizationNotFoundMessage = "Connection not found";
        public const string OrganizationFoundMessage = "Connection found";
        public const string OrganizationDisconnectMessage = "QuickBooks disconnect successfully";
        public const string OrganizationAlreadyDisconnectMessage = "QuickBooks company already disconnected";

        public const string FetchRecords = "Records fetched Successfully";
        public const string UnAuthorizedUser = "UnAuthorized User";
        public const string InvoicesFoundMessage = "Invoices found";
        public const string InvoicesNotFoundMessage = "Invoices not found";
        public const string CustomersFoundMessage = "Customers found";
        public const string CustomersNotFoundMessage = "Customers not found";
        public const string QBOTokenExpire = "QuickBooks access token expire or not found";
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
