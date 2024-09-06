using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DataContext>();

            //Inject datacontext
            builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));

            builder.Services.AddFlashMessage();

            builder.Services.AddTransient<SeedDb>();

            builder.Services.AddScoped<IUserHelper, UserHelper>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IImageHelper, ImageHelper>();
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

        
            //Add runtime compilation
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();



            var app = builder.Build();

            SeedData();

            void SeedData()
            {
                IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

                using (IServiceScope? scope = scopedFactory?.CreateScope())
                {
                    SeedDb? service = scope?.ServiceProvider.GetService<SeedDb>();
                    service?.SeedAsync().Wait();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

           

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

      
    }
}
