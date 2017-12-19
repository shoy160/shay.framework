using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shay.Core.Web;
using Shay.Framework;
using System;
using System.Reflection;

namespace Shay.Web.Tests
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
            services.AddMvc();
            services.AddHttpContextAccessor();
            DBootstrap.Instance.Initialize(Assembly.GetExecutingAssembly());            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            var logging = Configuration.GetSection("Logging");
            app.UseStaticFiles();
            app.UseStaticHttpContext();
            app.UseErrorHanleMiddleware();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action=Index}/{id?}");
            });
        }
    }
}
