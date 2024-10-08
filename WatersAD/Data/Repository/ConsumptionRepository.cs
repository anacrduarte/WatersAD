﻿using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;
using WatersAD.Models;


namespace WatersAD.Data.Repository
{
    public class ConsumptionRepository : GenericRepository<Consumption>, IConsumptionRepository
    {
        private readonly DataContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;

        public ConsumptionRepository(DataContext context, IInvoiceRepository invoiceRepository, IClientRepository clientRepository, ITierRepository tierRepository,
            IWaterMeterRepository waterMeterRepository) : base(context)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _tierRepository = tierRepository;
            _waterMeterRepository = waterMeterRepository;
        }

        public IEnumerable<Consumption> GetAllWaterMeterAndClient()
        {
            return _context.Consumptions
                           .Include(c => c.WaterMeter)
                           .ThenInclude(wm => wm.Client)
                           .ToList();
        }
        public async Task<IEnumerable<Consumption>> GetAllConsumptionForWaterMeter(int waterMeterId)
        {
            return await _context.Consumptions
                           .Include(c => c.WaterMeter)
                           .ThenInclude(wm => wm.Client)
                           .Where(c => c.WaterMeterId == waterMeterId)
                           .ToListAsync();
        }
        public async Task<Consumption> GetWaterMeterAndClientAsync(int consumptionId)
        {
            return await _context.Consumptions
                           .Include(c => c.WaterMeter)
                           .ThenInclude(wm => wm.Client)
                           .FirstOrDefaultAsync(c => c.Id == consumptionId);
        }


        public Consumption GetPreviousConsumption(WaterMeter waterMeter)
        {
            return waterMeter.Consumptions
                .OrderByDescending(c => c.ConsumptionDate)
                .FirstOrDefault();
        }

        private decimal CalculateTotalAmount(double tierPrice, double currentValue, double previousValue)
        {

            return Convert.ToDecimal(tierPrice * (currentValue - previousValue) / 1000);
        }


        public async Task CreateConsumptionAndInvoiceAsync(ConsumptionViewModel model, WaterMeter waterMeter, Tier matchingTier, Consumption previousConsumption)
        {

            var newConsumption = new Consumption
            {
                ConsumptionDate = model.ConsumptionDate,
                ConsumptionValue = model.ConsumptionValue,
                RegistrationDate = model.RegistrationDate,
                WaterMeterId = model.WaterMeterId,
                TierId = matchingTier.Id,
            };


            await CreateAsync(newConsumption);



            var totalAmount = CalculateTotalAmount(matchingTier.TierPrice, model.ConsumptionValue, previousConsumption.ConsumptionValue);


            var invoice = new Invoice
            {
                TotalAmount = totalAmount,
                ClientId = waterMeter.ClientId,
                Client = waterMeter.Client,
                InvoiceDate = DateTime.Now,
                LimitDate = DateTime.Now.AddDays(30),
            };

            await _invoiceRepository.CreateAsync(invoice);

            newConsumption.Tier = matchingTier;
            newConsumption.WaterMeter = waterMeter;
            newConsumption.Invoice = invoice;

            var client = await _clientRepository.GetByIdAsync(waterMeter.ClientId);

            if (client == null)
            {
                return;
            }

            if (client.Invoices == null)
            {
                client.Invoices = new List<Invoice>();
            }

            client.Invoices.Add(invoice);

            await _clientRepository.UpdateAsync(client);

            await UpdateAsync(newConsumption);



            await _context.SaveChangesAsync();


        }

        public async Task<ICollection<Consumption>> GetAllInvoicesForClientAsync(int id)
        {
            return await _context.Consumptions
                 .Include(c => c.WaterMeter)
                 .Include(c => c.Invoice)
                 .Where(c => c.Invoice.ClientId == id)
                 .ToListAsync();
        }

        public async Task<Consumption> GetConsumptionAsync(int invoiceId)
        {
            return await _context.Consumptions
                .Include(c => c.WaterMeter)
                .FirstOrDefaultAsync(c => c.Invoice.Id == invoiceId);
        }
    }
}
