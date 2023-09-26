using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;
using InsertUpdateXeroInvoice;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using XeroCore;

[assembly: FunctionsStartup(typeof(Startup))]
namespace InsertUpdateXeroInvoice
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionService, ConnectionService>();
            builder.Services.AddSingleton<XeroAuthClient>();
            builder.Services.AddSingleton<XeroClients>();
        }
    }
}
