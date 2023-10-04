using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;
using InsertUpdateXeroPayment;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using XeroCore;

[assembly: FunctionsStartup(typeof(Startup))]
namespace InsertUpdateXeroPayment
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
