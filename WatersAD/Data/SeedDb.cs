using System;
using WatersAD.Data.Entities;

namespace WatersAD.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Clients.Any())
            {
                AddClient("Ana", "Duarte");
                AddClient("Susana", "Duarte");
                

                await _context.SaveChangesAsync();
            }
        }

        private void AddClient(string firstName,string lastName)
        {
            _context.Clients.Add(new Client
            {
                FirstName = firstName,
                LastName = lastName,
                NIF = _random.Next(1000),
                Address = "Rua da flores",
                PhoneNumber = _random.Next(1000),

            });
        }
    }
}
