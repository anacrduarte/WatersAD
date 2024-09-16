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

            await AddCountryAsync("Portugal", new List<(string CityName, string LocalityName, string PostalCode)>
                                    {
                                        // Cidades e localidades de Portugal
                                        ("Vila Nova de Famalicão", "Abade de Vermoim", "4770"),
                                        ("Vila Nova de Famalicão", "Pousada de Saramagos", "4770"),
                                        ("Vila Nova de Famalicão", "Fradelos", "4760"),
                                        ("Vila Nova de Famalicão", "Arnoso (Santa Maria)", "4760"),
                                        ("Vila Nova de Famalicão", "Bente", "4770"),
                                        ("Vila Nova de Famalicão", "Calendário", "4760"),
                                        ("Vila Nova de Famalicão", "Cavalões", "4760"),
                                        ("Vila Nova de Famalicão", "Esmeriz", "4760"),
                                        ("Vila Nova de Famalicão", "Gondifelos", "4760"),
                                        ("Vila Nova de Famalicão", "Lagoa", "4770"),
                                        ("Vila Nova de Famalicão", "Louro", "4760"),
                                        ("Vila Nova de Famalicão", "Mouquim", "4770"),

                                        ("Guimarães", "Moreira de Cónegos", "4815"),
                                        ("Guimarães", "Costa", "4810"),
                                        ("Guimarães", "Guardizela", "4765"),
                                        ("Guimarães", "Castelões", "4800"),
                                        ("Guimarães", "Nespereira", "4839"),
                                        ("Guimarães", "Lordelo", "4815"),
                                        ("Guimarães", "Mascotelos", "4835"),
                                        ("Guimarães", "Longos", "4805"),
                                        ("Guimarães", "Leitões", "4805"),

                                        ("Santo-Tirso", "Agrela", "4825"),
                                        ("Santo-Tirso", "Santo Tirso", "4780"),
                                        ("Santo-Tirso", "São Mamede Negrelos", "4795"),
                                        ("Santo-Tirso", "Monte Córdova", "4825"),
                                        ("Santo-Tirso", "Couto (Santa Cristina)", "4780"),
                                        ("Santo-Tirso", "Aves", "4796"),
                                        ("Santo-Tirso", "Roriz", "4795")


                                    });
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

        //private async Task AddClientsAsync()
        //{
        //    if (!_context.Clients.Any())
        //    {
        //        var waterMeters = new List<WaterMeterService>();

        //        for (int i = 0; i < 20; i++)
        //        {

        //            var serialNumber = Guid.NewGuid().ToString().Substring(0, 8);

        //            var waterMeter = new WaterMeterService
        //            {
        //                SerialNumber = serialNumber
        //            };

        //            waterMeters.Add(waterMeter);
        //        }

        //        foreach (var waterMeter in waterMeters)
        //        {
        //            await _context.WaterMeterServices.AddAsync(waterMeter);
        //        }
        //        await _context.SaveChangesAsync();

        //    }


        //}


        private async Task AddCountryAsync(string nameCountry, ICollection<(string cityName, string localityName, string postalCode)> citiesWithLocality)
        {
            var country = await _context.Countries
                                .Include(c => c.Cities)
                                .ThenInclude(c => c.Localities)
                                .FirstOrDefaultAsync(c => c.Name == nameCountry);
            if (country == null)
            {
                country = new Country
                {
                    Name = nameCountry,
                    Cities = new List<City>(),
                };

                _context.Countries.Add(country);

            }


            foreach (var cityWithLocality in citiesWithLocality)
            {
                var city = country.Cities
                           .FirstOrDefault(c => c.Name == cityWithLocality.cityName) ??
                           await _context.Cities
                           .Include(c => c.Localities)
                           .FirstOrDefaultAsync(c => c.Name == cityWithLocality.cityName);

                if (city == null)
                {
                    city = new City
                    {
                        Name = cityWithLocality.cityName,
                        Localities = new List<Locality>(),
                        CountryId = country.Id,
                       
                    };
                }

                if(!city.Localities.Any(l => l.Name == cityWithLocality.localityName && l.PostalCode == cityWithLocality.postalCode))
                {
                    city.Localities.Add(new Locality 
                    { 
                        Name = cityWithLocality.localityName,
                        PostalCode = cityWithLocality.postalCode
                    });
                }

                country.Cities.Add(city);

            }
            
            await _context.SaveChangesAsync();
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Customer.ToString());
            await _userHelper.CheckRoleAsync(UserType.Employee.ToString());

        }
    }
}
