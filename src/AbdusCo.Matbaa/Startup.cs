using AbdusCo.Matbaa.Pdf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AbdusCo.Matbaa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBrowserless();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddTransient<IContentTypeProvider, FileExtensionContentTypeProvider>();
            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(description =>
                    description.ActionDescriptor is not ControllerActionDescriptor descriptor
                        ? null
                        : $"{descriptor.ControllerName}.{descriptor.ActionName}");
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Matbaa", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api";
                    c.DisplayOperationId();
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Matbaa v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}