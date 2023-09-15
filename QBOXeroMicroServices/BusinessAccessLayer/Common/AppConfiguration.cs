using Microsoft.Extensions.Configuration;

namespace BusinessAccessLayer.Common
{
    public class AppConfiguration
    {
        public static string HostEnvironment = string.Empty;
        public static string ConnectionString = string.Empty;
        public static string DatabaseName = string.Empty;

        public static string QBOClientID = string.Empty;
        public static string QBOClientSecret = string.Empty;
        public static string QBORedirectUrl = string.Empty;
        public static string QBOEnvironment = string.Empty;
        public static string QBOBaseUrl = string.Empty;

        public static string QBOCutomerGet = string.Empty;
        public static string QBOCustomerInsertUpdate = string.Empty;
        public static string QBOCustomerDelete = string.Empty;
        public static string QBOInvoiceGet = string.Empty;
        public static string QBOInvoiceInsertUpdate = string.Empty;
        public static string QBOInvoiceDelete = string.Empty;

        static AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var currentDirectory = "/home/site/wwwroot";
            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            if (isLocal)
            {
                currentDirectory = Directory.GetCurrentDirectory();
            }
            var path = Path.Combine(currentDirectory, "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            var appSetting = root.GetSection("ApplicationSettings");
            HostEnvironment = root.GetSection("HostEnvironment").Value;

            ConnectionString = root.GetSection("DatabaseSettings").GetSection(HostEnvironment).GetSection("ConnectionString").Value;
            DatabaseName = root.GetSection("DatabaseSettings").GetSection(HostEnvironment).GetSection("DatabaseName").Value;

            QBOClientID = root.GetSection("QBOConfig").GetSection(HostEnvironment).GetSection("QBOClientID").Value;
            QBOClientSecret = root.GetSection("QBOConfig").GetSection(HostEnvironment).GetSection("QBOClientSecret").Value;
            QBORedirectUrl = root.GetSection("QBOConfig").GetSection(HostEnvironment).GetSection("QBORedirectUrl").Value;
            QBOEnvironment = root.GetSection("QBOConfig").GetSection(HostEnvironment).GetSection("QBOEnvironment").Value;
            QBOBaseUrl = root.GetSection("QBOConfig").GetSection(HostEnvironment).GetSection("QBOBaseUrl").Value;

            QBOCutomerGet = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOCutomerGet").Value;
            QBOCustomerInsertUpdate = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOCustomerInsertUpdate").Value;
            QBOCustomerDelete = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOCustomerDelete").Value;
            QBOInvoiceGet = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOInvoiceGet").Value;
            QBOInvoiceInsertUpdate = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOInvoiceInsertUpdate").Value;
            QBOInvoiceDelete = root.GetSection("MicroServiceEndPoints").GetSection(HostEnvironment).GetSection("QBOInvoiceDelete").Value;
        }
    }
}
