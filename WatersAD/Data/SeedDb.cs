using Microsoft.AspNetCore.Identity;
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
            
            //var user = await _userHelper.GetUserByEmailAsync("anaduarte@gmail.com");

            //if (user == null)
            //{
            //    user = new User
            //    {
            //        FirstName = "Ana",
            //        LastName = "Duarte",
            //        Email = "anaduarte@gmail.com",
            //        UserName = "anaduarte@gmail.com",
            //        PhoneNumber = "123456789",
            //    };
            //    var result = await _userHelper.AddUserAsync(user, "123456");
            //    if (result != IdentityResult.Success)
            //    {
            //        throw new InvalidOperationException("Could not create the user in seeder");
            //    }

            //    await _userHelper.AddUserToRoleAsync(user, "Admin");
            //}

            //var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            //if (!isInRole)
            //{
            //    await _userHelper.AddUserToRoleAsync(user, "Admin");
            //}


            //if (!_context.Clients.Any())
            //{
            //    AddClient("Ana", "Duarte", user);
            //    AddClient("Susana", "Duarte", user);
                

            //    await _context.SaveChangesAsync();
            //}
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

        private void AddClient(string firstName,string lastName, User user)
        {
            _context.Clients.Add(new Client
            {
                FirstName = firstName,
                LastName = lastName,
                NIF = _random.Next(1000),
                Address = "Rua da flores",
                PhoneNumber = "99999999",
                User = user,

            });
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Customer.ToString());
            await _userHelper.CheckRoleAsync(UserType.Employee.ToString());

        }
    }
}
