using Intuit.Ipp.Data;
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
        public List<Customer> ReturnObject { get; set; }
    }
}