using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Today.Model.Models;
using Today.Model.Repositories;
using Today.Web.Services.AccountService;
using Today.Web.Services.CityService;
using Today.Web.Services.locationService;
using Today.Web.Services.MemberService;
using Today.Web.Services.ProductService;
using Today.Web.Services.ClassifyService;
using Today.Web.Services.CheenkoutService;
using Today.Web.Services.EcpayService;
using Today.Web.Services.ProductInfoService;
using Microsoft.OpenApi.Models;
using Today.Web.Services.MemberCommentService;
using Today.Web.Services.ShopCartService;
using Today.Web.Services.OrderService;
using Today.Web.Services.BookService;
using Today.Web.Services.CollectService;
using Today.Web.Repository;

namespace Today.Web
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
            services.AddControllersWithViews();
            services.AddDbContext<TodayDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TodayDB"));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodayWeb", Version = "v1" });
            });
            services.AddTransient<IGenericRepository, GenericRepository>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<ICollectionService, CollectionService>();


            // ���UDI
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<GenericRepository>();
            services.AddScoped<IMemberService, MemberService>();
            //services.AddScoped<IHttpContextAccessor, HttpContextAccessor>(); //(���n�ΡA�Τ��ؤ�k�i�U������j)
            services.AddHttpContextAccessor();

            //Cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    //�����g�o�M�w�]�Ȥ@��

                    //�]�w�n�JAction�����|�G 
                    //options.LoginPath = new PathString("/Account/Login");

                    ////�]�w �ɦ^���} ��QueryString�ѼƦW�١G
                    //options.ReturnUrlParameter = "ReturnUrl";

                    ////�]�w�n�XAction�����|�G 
                    //options.LogoutPath = new PathString("/Account/Logout");

                    ////�Y�v�������A�|�ɦV��Action�����|
                    //options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                })
                //�[�U�aOAuth
                .AddGoogle(options => {
                    var provider = "Google";
                    options.ClientId = Configuration[$"Authentication:{provider}:ClientId"];
                    options.ClientSecret = Configuration[$"Authentication:{provider}:ClientSecret"];

                    //options.CallbackPath = "/signin-google";
                })
                .AddFacebook(options =>
                {
                    var provider = "FB";
                    options.AppId = Configuration[$"Authentication:{provider}:ClientId"];
                    options.AppSecret = Configuration[$"Authentication:{provider}:ClientSecret"];

                    //options.CallbackPath = "/signin-facebook";
                })
                .AddLine(options =>
                {
                    var provider = "Line";
                    options.ClientId = Configuration[$"Authentication:{provider}:ClientId"];
                    options.ClientSecret = Configuration[$"Authentication:{provider}:ClientSecret"];

                    //options.CallbackPath = "/signin-line";
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["RedisConfig:ToolManMemoryCache"];
            });


            services.AddTransient<ILocationService, LocationService>();


            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IClassifyService, ClassifyService>();
            services.AddTransient<IChenkoutService, ChenkoutService>();
            services.AddTransient<IEcpayService, EcpayService>();
            //services.AddScoped<IProductInfoService, ProductInfoService>();
            services.AddTransient<IProductInfoService, ProductInfoService>();
            services.AddTransient<IMemberCommentService, MemberCommentService>();
            services.AddTransient<IShopCartService, ShopCartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IMemoryCacheRepository, MemoryCacheRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodayWeb v1"));
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //�[
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //defaults: new { controller = "Products", action = "Index" ,id="Category"});
            });
        }
    }
}
