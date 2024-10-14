using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Data;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class InvoicesController : Controller
    {

        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IMailHelper _mailHelper;

        public InvoicesController(IInvoiceRepository invoiceRepository, IFlashMessage flashMessage, IUserHelper userHelper,
            IClientRepository clientRepository, IWaterMeterRepository waterMeterRepository, IConsumptionRepository consumptionRepository, ICountryRepository countryRepository,
            ITierRepository tierRepository, IMailHelper mailHelper)
        {

            _invoiceRepository = invoiceRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _waterMeterRepository = waterMeterRepository;
            _consumptionRepository = consumptionRepository;
            _countryRepository = countryRepository;
            _tierRepository = tierRepository;
            _mailHelper = mailHelper;
        }
        [Authorize(Roles = "Employee")]
        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            return View(await _invoiceRepository.GetAllInvoicesAndClientAsync());
        }

        public async Task<IActionResult> GetInvoiceClient()
        {
            try
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
                if (user == null)
                {
                    return NotFound();
                }
                var client = await _clientRepository.GetClientByUserEmailAsync(user.Email);

                if (client == null)
                {
                    return NotFound();
                }
                var consumption = await _consumptionRepository.GetAllInvoicesForClientAsync(client.Id);
                if (consumption.Count < 1)
                {
                    _flashMessage.Warning("Ainda não tem faturas associadas");
                    return RedirectToAction("Index", "Home");
                }

                var invoices = consumption.Select(c => c.Invoice).Where(i => i.Issued && i.Sent).Distinct().ToList();
              

                var model = new InvoicesClientViewModel
                {
                    ClientId = client.Id,
                    WaterMeters = consumption.Select(c => c.WaterMeter).Distinct().ToList(),
                    Consumptions = consumption,
                    Invoices = invoices,
                   
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Invoice", "GetInvoiceClient");
            }

        }
        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            try
            {
                var invoice = await _invoiceRepository.GetDetailsInvoiceAsync(id.Value);
                if (invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");
                }
                var consumption = await _consumptionRepository.GetConsumptionAsync(invoice.Id);

                var waterMeter = await _waterMeterRepository.GetWaterMeterWithCityAndCountryAsync(consumption.WaterMeter.Id);

                var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(consumption.WaterMeter.Id);

                var tier = await _tierRepository.GetByIdAsync(consumption.TierId);

                var model = new InvoiceDetailsViewModel
                {
                    Client = invoice.Client,
                    WaterMeter = waterMeter,
                    Invoice = invoice,
                    Consumption = consumption,
                    WaterMeterService = waterMeterService,
                    Tier = tier,



                };
                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> SendAndIssue(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
                if(invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");
                }
                if(invoice.Issued || invoice.Sent)
                {
                    _flashMessage.Warning($"A fatura já foi emitida e enviada.");

                    return RedirectToAction(nameof(Index));
                }
                var client = await _clientRepository.GetByIdAsync(invoice.ClientId);
                if (client == null) { return NotFound(); }

                var locality = await _countryRepository.GetLocalityAsync(client.LocalityId);
                if (locality == null) { return NotFound(); }

                var city = await _countryRepository.GetCityAsync(locality.CityId);
                if (city == null) { return NotFound(); }

                locality.City = city;

                var country = await _countryRepository.GetCountryAsync(city);
                if (country == null) { return NotFound(); }

                var consumption = await _consumptionRepository.GetConsumptionAsync(invoice.Id);
                if (consumption == null)
                {
                    return NotFound();
                }

                var waterMeter = await _waterMeterRepository.GetWaterMeterWithCityAndCountryAsync(consumption.WaterMeter.Id);

                var waterMeterLocality = await _countryRepository.GetLocalityAsync(waterMeter.LocalityId);
                if (waterMeterLocality == null) { return NotFound(); }

                var waterMeterCity = await _countryRepository.GetCityAsync(waterMeterLocality.CityId);
                if (waterMeterCity == null)
                {
                    return NotFound();
                }




                var tier = await _tierRepository.GetByIdAsync(consumption.TierId);
                if (tier == null) { return NotFound(); }

                PdfDocument document = CreatePdfDocument();
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                DrawHeader(graphics, page, client, locality, country, invoice, waterMeter);


                DrawBody(graphics, page, consumption, tier, invoice, waterMeter, waterMeterLocality, waterMeterCity);


                AddFooter(graphics, page, invoice);

                AddPageNumber(document);

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                invoice.Issued = true;
                invoice.Sent = true;
                await _invoiceRepository.UpdateAsync(invoice);
                var response = await _mailHelper.SendMail(client.FullName, client.Email, $"Fatura referente ao mês de {invoice.InvoiceDate.ToString("MMMM")}", "Caro cliente, em anexo enviamos a fatura da água.Obrigado.", stream);

                if (response.IsSuccess)
                {
                    _flashMessage.Info($"Fatura enviada e emitida com sucesso.");

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _flashMessage.Info($"Erro ao enviar a fatura.");

                    return RedirectToAction(nameof(Index));
                }
               

            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }



        }


        public async Task<IActionResult> InvoiceHistory(int clientId, int waterMeterId)
        {
            try
            {

                var client = await _clientRepository.GetByIdAsync(clientId);

                if (client == null)
                {
                    return NotFound();
                }

                var consumption = await _consumptionRepository.GetAllInvoicesForClientAsync(client.Id);

                var invoices = consumption.Where(c => c.WaterMeterId == waterMeterId).Select(c => c.Invoice).ToList();

                var waterMeter = await _waterMeterRepository.GetByIdAsync(waterMeterId);

                var model = new InvoicesClientViewModel
                {
                    Invoices = invoices,
                    WaterMeter = waterMeter,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Invoice", "GetInvoiceClient");
            }

        }
        public async Task<IActionResult> Pdf(int? clientId, int? invoiceId, int? waterMeterId, int? consumptionId)
        {

            try
            {
                if (clientId == null || invoiceId == null || waterMeterId == null || consumptionId == null)
                { return NotFound(); }

                var client = await _clientRepository.GetByIdAsync(clientId.Value);
                if (client == null) { return NotFound(); }

                var locality = await _countryRepository.GetLocalityAsync(client.LocalityId);
                if (locality == null) { return NotFound(); }

                var city = await _countryRepository.GetCityAsync(locality.CityId);
                if (city == null) { return NotFound(); }

                locality.City = city;

                var country = await _countryRepository.GetCountryAsync(city);
                if (country == null) { return NotFound(); }

                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId.Value);
                if (invoice == null) { return NotFound(); }

                var waterMeter = await _waterMeterRepository.GetWaterMeterWithConsumptionsAsync(waterMeterId.Value);

                if (waterMeter == null) { return NotFound(); }

                var waterMeterLocality = await _countryRepository.GetLocalityAsync(waterMeter.LocalityId);
                if (waterMeterLocality == null) { return NotFound(); }

                var waterMeterCity = await _countryRepository.GetCityAsync(waterMeterLocality.CityId);
                if (waterMeterCity == null)
                {
                    return NotFound();
                }

                var consumption = await _consumptionRepository.GetByIdAsync(consumptionId.Value);
                if (consumption == null)
                {
                    return NotFound();
                }

                var tier = await _tierRepository.GetByIdAsync(consumption.TierId);
                if (tier == null) { return NotFound(); }

                PdfDocument document = CreatePdfDocument();
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;


                DrawHeader(graphics, page, client, locality, country, invoice, waterMeter);


                DrawBody(graphics, page, consumption, tier, invoice, waterMeter, waterMeterLocality, waterMeterCity);


                AddFooter(graphics, page, invoice);

                AddPageNumber(document);

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0; // Redefine a posição do stream


                return new FileStreamResult(stream, "application/pdf") { FileDownloadName = "Sample.pdf" };


            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Invoice", "GetInvoiceClient");
            }



        }

        static void AddPageNumber(PdfDocument document)
        {
            for (int i = 0; i < document.Pages.Count; i++)
            {
                PdfPage page = document.Pages[i];
                PdfGraphics graphics = page.Graphics;


                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
                PdfBrush brush = PdfBrushes.Black;


                float pageWidth = page.GetClientSize().Width;
                float pageHeight = page.GetClientSize().Height;
                string pageNumberText = $"Página {i + 1}";


                SizeF textSize = font.MeasureString(pageNumberText);


                graphics.DrawString(pageNumberText, font, brush, new PointF(pageWidth - textSize.Width - 10, pageHeight - textSize.Height - 10));
            }
        }

        static PdfImage LoadImage(string relativePath)
        {

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);


            using (FileStream imageStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return PdfImage.FromStream(imageStream);
            }
        }

        private PdfDocument CreatePdfDocument()
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            document.PageSettings.Margins.All = 10;
            return document;
        }

        private void DrawHeader(PdfGraphics graphics, PdfPage page, Client client, Locality locality, Country country, Invoice invoice, WaterMeter waterMeter)
        {
            float pageWidth = page.GetClientSize().Width;
            float startX1 = 10;
            float startY1 = 10;


            PdfImage logoImage = LoadImage("wwwroot/image/layout/logoPdf.png");
            PdfImage waterImage = LoadImage("wwwroot/image/layout/aguanatural.jpg");
            float imageWidth = 70;
            float imageHeight = 70;


            RectangleF logoRect = new RectangleF(startX1, startY1, imageWidth, imageHeight);
            page.Graphics.DrawImage(logoImage, logoRect);


            float waterWidth = page.GetClientSize().Width;
            float waterHeight = 100;
            RectangleF waterRect = new RectangleF(0, 250, waterWidth, waterHeight);
            page.Graphics.DrawImage(waterImage, waterRect);


            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
            PdfFont fontClient = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Bold);


            DrawCompanyInfo(graphics, font, startY1);


            DrawClientInfo(graphics, fontClient, pageWidth, startY1, client, locality, country);
        }

        private void DrawCompanyInfo(PdfGraphics graphics, PdfFont font, float startY)
        {
            string companyName = "Águas Duarte";
            string address = "Rua das Flores, nº 125";
            string address1 = "4700 - 000, Braga";
            string contact = "912 345 678";

            graphics.DrawString(companyName, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(85, startY + 5));
            graphics.DrawString(address, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(85, startY + 20));
            graphics.DrawString(address1, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(85, startY + 35));
            graphics.DrawString(contact, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(85, startY + 50));
        }

        private void DrawClientInfo(PdfGraphics graphics, PdfFont fontClient, float pageWidth, float startY1, Client client, Locality locality, Country country)
        {
            graphics.DrawString($"{client.FullName}", fontClient, PdfBrushes.Black, new Syncfusion.Drawing.PointF(pageWidth - 200, startY1));
            graphics.DrawString($"{client.FullAdress}", fontClient, PdfBrushes.Black, new Syncfusion.Drawing.PointF(pageWidth - 200, startY1 + 15));
            graphics.DrawString($"{client.FullPostalCode} - {locality.Name}", fontClient, PdfBrushes.Black, new Syncfusion.Drawing.PointF(pageWidth - 200, startY1 + 30));
            graphics.DrawString($"{locality.City.Name}, {country.Name}", fontClient, PdfBrushes.Black, new Syncfusion.Drawing.PointF(pageWidth - 200, startY1 + 45));
        }

        private void DrawBody(PdfGraphics graphics, PdfPage page, Consumption consumption, Tier tier, Invoice invoice, WaterMeter waterMeter, Locality waterMeterLocality, City waterMeterCity)
        {
            float pageWidth = page.GetClientSize().Width;
            float startY = 140;
            float lineSpacing = 25;


            RectangleF box = new RectangleF(0, startY, pageWidth, 140);
            PdfPen boxPen = new PdfPen(Color.White, 0);
            page.Graphics.DrawRectangle(boxPen, box);

            float boxStartX = box.X + 10;
            float boxStartY = box.Y + 10;


            PdfFont fontText = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            PdfBrush textBrush = PdfBrushes.Black;


            graphics.DrawString($"Contador:", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX, boxStartY));
            graphics.DrawString($"Morada:", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX, boxStartY + lineSpacing));


            graphics.DrawString($"{waterMeter.Id}", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX + 100, boxStartY));
            graphics.DrawString($"{waterMeter.FullAdress}", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX + 100, boxStartY + lineSpacing));
            graphics.DrawString($"{waterMeter.RemainPostalCode} - {waterMeterLocality.Name}", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX + 100, boxStartY + 2 * lineSpacing));
            graphics.DrawString($"{waterMeterCity.Name}", fontText, textBrush, new Syncfusion.Drawing.PointF(boxStartX + 100, boxStartY + 3 * lineSpacing));


            DrawInvoiceDetails(graphics, page, consumption, tier, invoice, lineSpacing);
        }

        private void DrawInvoiceDetails(PdfGraphics graphics, PdfPage page, Consumption consumption, Tier tier, Invoice invoice, float lineSpacing)
        {
            float startX = 200;
            float startY = 120;

            PdfFont fontTitle = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            PdfFont fontText = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            PdfBrush textBrush = PdfBrushes.Black;


            string startTitle = $"Fatura nº {invoice.Id}";
            float pageWidth = page.GetClientSize().Width;
            SizeF textSize = fontTitle.MeasureString(startTitle);
            float startTitleX = pageWidth - textSize.Width - 20;

            graphics.DrawString(startTitle, fontTitle, PdfBrushes.DarkBlue, new Syncfusion.Drawing.PointF(startTitleX, startY));


            startY += 40;


            graphics.DrawString($"Data do consumo:", fontText, textBrush, new Syncfusion.Drawing.PointF(startX, startY + 9 * lineSpacing));
            graphics.DrawString($"Consumo:", fontText, textBrush, new Syncfusion.Drawing.PointF(startX, startY + 10 * lineSpacing));
            graphics.DrawString("Escalão:", fontText, textBrush, new Syncfusion.Drawing.PointF(startX, startY + 11 * lineSpacing));
            graphics.DrawString("Preço escalão:", fontText, textBrush, new Syncfusion.Drawing.PointF(startX, startY + 12 * lineSpacing));

            page.Graphics.DrawLine(new PdfPen(Color.Black, 2), 0, startY + 8 * lineSpacing, pageWidth, startY + 8 * lineSpacing);

            float valueX = startX + 200;

            graphics.DrawString($"{consumption.ConsumptionDate:dd/MM/yyyy}", fontText, textBrush, new Syncfusion.Drawing.PointF(valueX, startY + 9 * lineSpacing));
            graphics.DrawString($"{consumption.ConsumptionValue} litros", fontText, textBrush, new Syncfusion.Drawing.PointF(valueX, startY + 10 * lineSpacing));
            graphics.DrawString($"{tier.TierName}", fontText, textBrush, new Syncfusion.Drawing.PointF(valueX, startY + 11 * lineSpacing));
            graphics.DrawString($"{tier.TierPrice:C2} Euros", fontText, textBrush, new Syncfusion.Drawing.PointF(valueX, startY + 12 * lineSpacing));


            DrawPaymentInfo(graphics, page, invoice, valueX, startY, lineSpacing);
        }

        private void DrawPaymentInfo(PdfGraphics graphics, PdfPage page, Invoice invoice, float valueX, float startY, float lineSpacing)
        {
            graphics.DrawString("Valor a Pagar:", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new Syncfusion.Drawing.PointF(page.GetClientSize().Width / 2, page.GetClientSize().Height - 240));
            graphics.DrawString("Data Limite de Pagamento:", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, startY + page.GetClientSize().Height - 280));
            graphics.DrawString($"{invoice.TotalAmount:C2} Euros", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new Syncfusion.Drawing.PointF(page.GetClientSize().Width - 100, page.GetClientSize().Height - 240));
            graphics.DrawString($"{invoice.LimitDate:dd/MM/yyyy}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new Syncfusion.Drawing.PointF(valueX - 100, startY + page.GetClientSize().Height - 280));

            page.Graphics.DrawLine(new PdfPen(Color.Black, 2), page.GetClientSize().Width / 2, page.GetClientSize().Height - 260, page.GetClientSize().Width, page.GetClientSize().Height - 260);
            page.Graphics.DrawLine(new PdfPen(Color.Black, 2), 0, page.GetClientSize().Height - 210, page.GetClientSize().Width, page.GetClientSize().Height - 210);
        }

        private void AddFooter(PdfGraphics graphics, PdfPage page, Invoice invoice)
        {
            float footerHeight = 40;
            float imageWidth = 70;
            float imageHeight = 70;


            RectangleF imageRect = new RectangleF(page.GetClientSize().Width - imageWidth - 50, page.GetClientSize().Height - imageHeight - footerHeight - 90, imageWidth, imageHeight);


            PdfImage mbWayImage = LoadImage("wwwroot/image/layout/MbWay.png");
            page.Graphics.DrawImage(mbWayImage, imageRect);


            PdfFont fontMbWay = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
            PdfBrush brush = PdfBrushes.Black;

            float textYPosition = imageRect.Bottom + 5;
            page.Graphics.DrawString("Entidade: 9999999", fontMbWay, brush, new PointF(imageRect.X, textYPosition));
            page.Graphics.DrawString("Referência: 11111111", fontMbWay, brush, new PointF(imageRect.X, textYPosition + 20));
            page.Graphics.DrawString($"Valor: {invoice.TotalAmount:C2} Euros", fontMbWay, brush, new PointF(imageRect.X, textYPosition + 40));

            page.Graphics.DrawLine(new PdfPen(Color.Black, 2), 0, page.GetClientSize().Height - footerHeight, page.GetClientSize().Width, page.GetClientSize().Height - footerHeight);
        }

    }
}

