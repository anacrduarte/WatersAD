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
            await CheckUserAsync("João", "Silva", "joao.silva@example.com", "912345678", "Rua das Flores, 123, Lisboa", UserType.Employee, "~/image/noimage.png");
            await CheckUserAsync("Maria", "Santos", "maria.santos@example.com", "923456789", "Avenida da Liberdade, 456, Porto", UserType.Customer, "~/image/noimage.png");
            await CheckUserAsync("Pedro", "Oliveira", "pedro.oliveira@example.com", "934567890", "Praça do Comércio, 789, Coimbra", UserType.Customer, "~/image/noimage.png");
            //await CheckUserAsync("Ana", "Pereira", "ana.pereira@example.com", "945678901", "Rua da Baixa, 159, Braga", UserType.Customer, null);
            //await CheckUserAsync("Luís", "Ferreira", "luis.ferreira@example.com", "956789012", "Avenida da República, 987, Faro", UserType.Employee, null);
            //await CheckUserAsync("Tiago", "Costa", "tiago.costa@example.com", "961234567", "Rua das Amoreiras, 1, Lisboa", UserType.Customer, null);
            //await CheckUserAsync("Rita", "Gomes", "rita.gomes@example.com", "962345678", "Avenida das Nações, 2, Porto", UserType.Customer, null);
            //await CheckUserAsync("Filipe", "Almeida", "filipe.almeida@example.com", "963456789", "Praça do Rossio, 3, Coimbra", UserType.Customer, null);
            //await CheckUserAsync("Beatriz", "Silva", "beatriz.silva@example.com", "964567890", "Rua do Comércio, 4, Braga", UserType.Customer, null);
            //await CheckUserAsync("Gustavo", "Pereira", "gustavo.pereira@example.com", "965678901", "Avenida de Roma, 5, Faro", UserType.Customer, null);
            //await CheckUserAsync("Laura", "Martins", "laura.martins@example.com", "966789012", "Rua das Laranjeiras, 6, Évora", UserType.Customer, null);
            //await CheckUserAsync("Hugo", "Mendes", "hugo.mendes@example.com", "967890123", "Praça do Marquês, 7, Setúbal", UserType.Customer, null);
            //await CheckUserAsync("Inês", "Ramos", "ines.ramos@example.com", "968901234", "Avenida da Liberdade, 8, Lisboa", UserType.Customer, null);
            //await CheckUserAsync("Nuno", "Figueiredo", "nuno.figueiredo@example.com", "969012345", "Rua do Pôr do Sol, 9, Porto", UserType.Customer, null);
            //await CheckUserAsync("Sofia", "Teixeira", "sofia.teixeira@example.com", "970123456", "Praça de Camões, 10, Coimbra", UserType.Customer, null);
            //await CheckUserAsync("André", "Cunha", "andre.cunha@example.com", "971234567", "Rua dos Três Castelos, 11, Braga", UserType.Customer, null);
            //await CheckUserAsync("Clara", "Vasconcelos", "clara.vasconcelos@example.com", "972345678", "Avenida de França, 12, Faro", UserType.Customer, null);
            //await CheckUserAsync("Ricardo", "Pinto", "ricardo.pinto@example.com", "973456789", "Rua de São Bento, 13, Évora", UserType.Customer, null);
            //await CheckUserAsync("Patrícia", "Barbosa", "patricia.barbosa@example.com", "974567890", "Praça da Alegria, 14, Setúbal", UserType.Customer, null);
            //await CheckUserAsync("Samuel", "Alves", "sa@mail.com", "975678901", "Rua Nova, 15, Lisboa", UserType.Employee, null);


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
                    MustChangePassword = false,
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
