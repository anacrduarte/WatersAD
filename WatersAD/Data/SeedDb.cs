using Microsoft.AspNetCore.Identity;
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

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CreateTiersAsync();
            await CreateCountyAndCityAndLocalityAsync();
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
            if (!_context.WaterMeterServices.Any())
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

        private async Task CreateTiersAsync()
        {
            if (!_context.Tiers.Any())
            {
                var tiers = new List<Tier>
                {
                   new Tier {
                        TierName = "Escalão 1",
                        TierNumber = 1,
                        TierPrice = 0.3,
                        UpperLimit = 5000,
                    },
                   new Tier {
                        TierName = "Escalão 2",
                        TierNumber = 2,
                        TierPrice = 0.8,
                        UpperLimit = 15000,
                   },
                   new Tier {
                        TierName = "Escalão 3",
                        TierNumber = 3,
                        TierPrice = 1.2,
                        UpperLimit = 25000,
                   },
                   new Tier {
                        TierName = "Escalão 4",
                        TierNumber = 4,
                        TierPrice = 1.6,
                        UpperLimit = 2147483647,
                   }
                };
                _context.Tiers.AddRange(tiers);
               await _context.SaveChangesAsync();
            }
        }

        private async Task CreateCountyAndCityAndLocalityAsync()
        {
            if (!_context.Countries.Any())
            {
                var country = new Country
                {
                    Name = "Portugal",
                    Cities = new List<City>
                    {
                        new City
                        {
                            Name = "Braga",
                            Localities = new List<Locality>
                            {
                               new Locality { Name = "Maximinos" },
                               new Locality { Name = "São José de São Lázaro" },
                               new Locality { Name = "Braga (São João)" },
                               new Locality { Name = "S. Vicente" },
                               new Locality { Name = "Esporões" },
                               new Locality { Name = "Gualtar" }
                            }
                        },
                        new City
                        {
                            Name = "Vila Nova de Famalicão",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Famalicão" },
                                new Locality { Name = "Ribeirão" },
                                new Locality { Name = "Lousado" },
                                new Locality { Name = "Caldelas" },
                                new Locality { Name = "Telhado" },
                                new Locality { Name = "Rato" },
                                new Locality { Name = "Fradelos" },
                                new Locality { Name = "Santa Maria de Famalicão" },
                                new Locality { Name = "Cruz" },
                                new Locality { Name = "Delães" },
                                new Locality { Name = "Chafé" },
                                new Locality { Name = "Vila Nova" },
                                new Locality { Name = "Alberque" },
                                new Locality { Name = "Azevedo" },
                                new Locality { Name = "Antas" },
                                new Locality { Name = "Landim" },
                                new Locality { Name = "Gavião" },
                                new Locality { Name = "Guilhabreu" },
                                new Locality { Name = "Joane" },
                                new Locality { Name = "Lamas" }
                            }
                        },
                        new City
                        {
                            Name = "Vizela",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Vizela (Santo Adrião)" },
                                new Locality { Name = "Infesta" },
                                new Locality { Name = "Caldas de Vizela" },
                                new Locality { Name = "São Miguel" },
                                new Locality { Name = "São João" },
                                new Locality { Name = "Silvares" },
                                new Locality { Name = "Serra" },
                                new Locality { Name = "Parque da Cidade" },
                                new Locality { Name = "Cano" },
                                new Locality { Name = "Arco de Baúlhe" }
                            }
                        },
                        new City
                        {
                            Name = "Guimarães",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Centro Histórico" },
                                new Locality { Name = "Serra de Santa Catarina" },
                                new Locality { Name = "São Torcato" },
                                new Locality { Name = "Pevidém" },
                                new Locality { Name = "Gonçalves" },
                                new Locality { Name = "Azevedo" },
                                new Locality { Name = "Selho S. Jorge" },
                                new Locality { Name = "Atães" },
                                new Locality { Name = "Cavalões" },
                                new Locality { Name = "Moreira de Cónegos" },
                                new Locality { Name = "Ruas" },
                                new Locality { Name = "São Paio" },
                                new Locality { Name = "Sande" },
                                new Locality { Name = "Creixomil" },
                                new Locality { Name = "Ribeirão" },
                                new Locality { Name = "Caldeiro" },
                                new Locality { Name = "Caldas" },
                                new Locality { Name = "Rendufinho" },
                                new Locality { Name = "Brito" },
                                new Locality { Name = "Bico" }
                            }
                        },
                        new City
                        {
                            Name = "Santo-Tirso",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Santo Tirso" },
                                new Locality { Name = "Roriz" },
                                new Locality { Name = "Souto" },
                                new Locality { Name = "S. Miguel de Lema" },
                                new Locality { Name = "S. Martinho do Campo" },
                                new Locality { Name = "S. Salvador do Campo" },
                                new Locality { Name = "Alvito" },
                                new Locality { Name = "Riba de Ave" },
                                new Locality { Name = "S. Bento da Vitória" },
                                new Locality { Name = "São Mamede" }
                            }
                        },
                        new City
                        {
                            Name = "Póvoa de Varzim",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Póvoa de Varzim" },
                                new Locality { Name = "Aguçadoura" },

                            }
                        },
                        new City
                        {
                            Name = "Esposende",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Esposende" },
                                new Locality { Name = "Marinhas" },

                            }
                        },
                        new City
                        {
                            Name = "Barcelos",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Barcelos" },
                                new Locality { Name = "Vila de Punhe" },

                            }
                        },
                        new City
                        {
                            Name = "Fafe",
                            Localities = new List<Locality>
                            {
                                new Locality { Name = "Fafe" },
                                new Locality { Name = "Aboim da Nóbrega" },

                            }
                        }

                    }
                };
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();

            }
        }


    }
}
