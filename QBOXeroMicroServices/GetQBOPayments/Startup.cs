using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;
using GetQBOPayments;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using QBOCore;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GetQBOPayments
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionService, ConnectionService>();
            builder.Services.AddSingleton<QBOAuthClient>();
            builder.Services.AddSingleton<QBOClients>();
        }
    }
}
