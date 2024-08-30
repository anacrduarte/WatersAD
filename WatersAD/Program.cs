using Microsoft.EntityFrameworkCore;
using WatersAD.Data;
using WatersAD.Data.Repository;

namespace WatersAD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Inject datacontext
            builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));

            builder.Services.AddScoped<IClientRepository, ClientRepository>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add runtime compilation
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
