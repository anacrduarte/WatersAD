using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Helpers;

namespace WatersAD.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper )
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("Ana", "Duarte", "ana@mail.com", "987654321", "Rua das Flores", UserType.Admin, "~/image/user/Account.png");

       
           await AddWaterMeterServiceAsync();


           
        }

        private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, string address, UserType userType, string image)
        {
            User user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    UserType = userType,
                    ImageUrl = image,
                    EmailConfirmed = true,
                };
                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }

        private async Task AddWaterMeterServiceAsync()
        {
            if(!_context.WaterMeterServices.Any())
            {
                var waterMeters = new List<WaterMeterService>();

                for (int i = 0; i < 20; i++)
                {

                    var serialNumber = Guid.NewGuid().ToString().Substring(0, 8);

                    var waterMeter = new WaterMeterService
                    {
                        SerialNumber = serialNumber
                    };

                    waterMeters.Add(waterMeter);
                }

                foreach (var waterMeter in waterMeters)
                {
                    await _context.WaterMeterServices.AddAsync(waterMeter);
                }
                await _context.SaveChangesAsync();

            }
           

        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Customer.ToString());
            await _userHelper.CheckRoleAsync(UserType.Employee.ToString());

        }
    }
}
