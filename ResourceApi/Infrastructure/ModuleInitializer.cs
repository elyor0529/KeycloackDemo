using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResourceApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResourceApi.Infrastructure
{
    public class ModuleInitializer
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<HttpClient>();
        }
    }
}
