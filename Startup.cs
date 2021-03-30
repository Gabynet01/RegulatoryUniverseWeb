using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RegulatoryUniverse.Models;
using Microsoft.Extensions.Logging;
using RegulatoryUniverse.Services;
using Microsoft.AspNetCore.Http;

namespace RegulatoryUniverse
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var loginConnection = Configuration.GetConnectionString("loginDatabaseConnect");
            var appConnection = Configuration.GetConnectionString("appDatabaseConnect");
            services.AddDbContext<UserProfileServiceDbContext>(options => options.UseSqlServer(loginConnection));
            services.AddDbContext<RegulatoryUniverseContext>(options => options.UseSqlServer(appConnection));
            services.Configure<Users>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<ReportSchedule>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession();
          
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<IHelperServices, Services.HelperServices>();
            services.AddTransient<ISessionManagerService, Services.SessionManagerService>();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //log to file
            var currentDate = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log" + currentDate+".txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });

        }
    }
}
