using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumptionInfoController : Controller
    {
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;

        public ConsumptionInfoController(IConsumptionRepository consumptionRepository,
        ITierRepository tierRepository, IWaterMeterRepository waterMeterRepository, IUserHelper userHelper, IClientRepository clientRepository)
        {
            _consumptionRepository = consumptionRepository;
            _tierRepository = tierRepository;
            _waterMeterRepository = waterMeterRepository;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
        }


        [HttpGet("[action]/{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetAllInvoices(string email)
        {
            try
            {
                var user = await _userHelper.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "Utilizador não encontrado com o email fornecido." });
                }


                var client = await _clientRepository.GetClientByUserEmailAsync(user.Email);


                if (client == null)
                {
                    return NotFound(new { message = "Nenhum cliente associado ao utilizador encontrado." });
                }
                var clientId = client.Id;

                var invoice = await _consumptionRepository.GetAllInvoicesForClientAsync(clientId);
                return Ok(invoice);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Erro ao buscar os dados dados das faturas", error = ex.Message });
            }
        }



        [HttpGet("[action]/{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetWaterMetersClient(string email)
        {
            try
            {
                var user = await _userHelper.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "Utilizador não encontrado com o email fornecido." });
                }


                var client = await _clientRepository.GetClientByUserEmailAsync(user.Email);


                if (client == null)
                {
                    return NotFound(new { message = "Nenhum cliente associado ao utilizador encontrado." });
                }

                var waterMeters = await _waterMeterRepository.GetWaterMeterClientAsync(client.Id);

                if (waterMeters == null || !waterMeters.Any())
                {
                    return NotFound("No water meters found for the specified client.");
                }

                return Ok(waterMeters);

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    message = "Erro ao buscar os dados contadores",
                    error = ex.Message
                });
            }
        }


        [HttpPost("CreateConsumption")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] ConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Por favor, corrija os erros no formulário.");
            }

            try
            {
                var waterMeter = await _waterMeterRepository.GetWaterMeterWithConsumptionsAsync(model.WaterMeterId);

                if (waterMeter == null)
                {
                    return NotFound("Contador não encontrado.");
                }

                var consumption = await _consumptionRepository.GetByIdAsync(model.Id);
                if (consumption != null)
                {
                    return Conflict("Este consumo já existe.");
                }

                var matchingTier = await _tierRepository.GetMatchingTierAsync(model.ConsumptionValue);

                if (matchingTier == null)
                {
                    return NotFound("Escalão não encontrado.");
                }

                var previousConsumption = _consumptionRepository.GetPreviousConsumption(waterMeter);

                    if (previousConsumption != null && model.ConsumptionValue <= previousConsumption.ConsumptionValue)
                    {
                        return BadRequest("O valor de consumo atual deve ser maior do que o último valor registrado.");
                    }
                


                await _consumptionRepository.CreateConsumptionAndInvoiceAsync(model, waterMeter, matchingTier, previousConsumption);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar a requisição: {ex.Message}");
            }
        }
    }
}
