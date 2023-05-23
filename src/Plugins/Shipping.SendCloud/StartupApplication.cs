using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipping.SendCloud.Services;

namespace Shipping.SendCloud
{
    public class StartupApplication : IStartupApplication
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IShippingSendCloudService, ShippingSendCloudService>();
            services.AddScoped<IShippingRateCalculationProvider, SendCloudShippingCalcPlugin>();

            //services.AddHttpClient("SendCloudUrl",
            //options =>
            //{
            //    var authenticationString = $"{configuration.GetValue<string>("SendCloudApi:ClientId")}:{configuration.GetValue<string>("SendCloudApi:ClientSecret")}";
            //    var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString));
            //    options.BaseAddress = new Uri(configuration.GetValue<string>("SendCloudApi:SendCloudUrl"));
            //    options.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            //});
        }

        public int Priority => 10;
        public void Configure(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {

        }
        public bool BeforeConfigure => false;

    }
}
