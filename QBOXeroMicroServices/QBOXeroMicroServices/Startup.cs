using DataAccessLayer.DatabaseSettings;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace QBOXeroMicroServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {   
            // Swagger configuration
            services.AddSwaggerGen();

            services.AddControllersWithViews();
            services.AddCors();

            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(x => x.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<IConnectionService, ConnectionService>();

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // allow any origin
              .AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "QBOAuth",
                    pattern: "{controller=QBOAuth}/{action=Index}");
                endpoints.MapControllerRoute(
                    name: "Email",
                    pattern: "{controller=Email}/{action=Index}/{encryptionId}/{dueDateSort}/{invoiceNumber}");
            });
        }
    }
}
