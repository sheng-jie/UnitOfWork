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

            //使用内存数据库
            //var connection = new SqliteConnection(Configuration.GetConnectionString("InMemoryConnection"));
            //connection.Open();
            //services.AddDbContext<UnitOfWorkDbContext>(options =>
            //    options.UseSqlite(connection));

            services.AddDbContext<UnitOfWorkDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            //使用扩展方法注入Uow依赖
            services.AddUnitOfWork<UnitOfWorkDbContext>();
            //使用默认方法注入Uow依赖
            //services.AddScoped<IUnitOfWork, UnitOfWork<UnitOfWorkDbContext>>();

            //注入泛型仓储
            services.AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(EfCoreRepository<,>));

            services.AddTransient<ICustomerAppService, CustomerAppService>();

            //注入MVC
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //使得webroot（默认为wwwroot）下的文件可以被访问
            app.UseStaticFiles();

            //配置MVC路由
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //配置默认请求响应
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!" );
            });
        }
    }
}
