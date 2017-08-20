using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWork.Customer;
using UnitOfWork.Repositories;

namespace UnitOfWork.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            //These are two services available at constructor
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //This is the only service available at ConfigureServices

            //var connection = new SqliteConnection(Configuration.GetConnectionString("InMemoryConnection"));
            //connection.Open();

            //services.AddDbContext<UnitOfWorkDbContext>(options =>
            //    options.UseSqlite(connection));

            services.AddDbContext<UnitOfWorkDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(EfCoreRepository<,>));

            services.AddTransient<ICustomerAppService, CustomerAppService>();

            services.AddMvc();

            //var serviceProvider = services.BuildServiceProvider();

            //var repository = serviceProvider.GetService<ICustomerAppService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!" );
            });
        }
    }
}
