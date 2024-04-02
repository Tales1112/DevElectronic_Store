using DevElectronic_Store.Identidade.API.Services.Interface;
using DevElectronic_Store.WebApi.Core.Identidade;
using DevElectronic_Store.WebApi.Core.Usuario;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDevPack.Security.Jwt.AspNetCore;

namespace DevElectronic_Store.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            return services;
        }
        public static IApplicationBuilder UserApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
           
            app.UseAuthConfiguration();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseJwksDiscovery();
            return app;
        }
    }
}
