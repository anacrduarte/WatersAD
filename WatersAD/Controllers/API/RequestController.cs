using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class RequestController : Controller
	{
		private readonly IWaterMeterRepository _waterMeterRepository;
		private readonly ICountryRepository _countryRepository;
		private readonly INotificationRepository _notificationRepository;
		private readonly IUserHelper _userHelper;

		public RequestController(IWaterMeterRepository waterMeterRepository, IUserHelper userHelper, ICountryRepository countryRepository, INotificationRepository notificationRepository)
        {
			_waterMeterRepository = waterMeterRepository;
			_countryRepository = countryRepository;
			_notificationRepository = notificationRepository;
			_userHelper = userHelper;
		}

		[HttpPost("RequestWaterMeter")]
		public async Task<IActionResult> RequestWaterMeter([FromBody] RequestWaterMeterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Message = "Modelo inválido.", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
			}

			try
			{
				// Verifica se o usuário já existe pelo e-mail
				var user = await _userHelper.GetUserByEmailAsync(model.Email);
				if (user != null)
				{
					return Conflict(new { Message = "Esse e-mail já está registrado. Se você já é cliente, faça o pedido na sua área de cliente." });
				}

				// Valida a localidade principal
				var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);
				if (locality == null)
				{
					return NotFound(new { Message = "Localidade principal inválida." });
				}

				// Valida a localidade do medidor de água
				var waterMeterLocality = await _countryRepository.GetLocalityAsync(model.LocalityWaterMeterId);
				if (waterMeterLocality == null)
				{
					return NotFound(new { Message = "Localidade do medidor de água inválida." });
				}

				// Chama o serviço que cria o pedido
				var response = await CreateWaterRequest(model);

				if (response.IsSuccess)
				{
					return Ok(new { Message = "O seu pedido foi encaminhado com sucesso. Seremos o mais breves possível!" });
				}

				// Caso a criação falhe sem detalhes específicos
				return StatusCode(500, new { Message = "Erro ao criar o pedido de medidor de água." });
			}
			catch (Exception ex)
			{
				// Tratamento de exceções
				return StatusCode(500, new { Message = $"Ocorreu um erro ao processar a requisição.", Error = ex.Message });
			}
		}

		public async Task<Response> CreateWaterRequest(RequestWaterMeterViewModel model)
		{
			try
			{
				var user = await _userHelper.GetUserByEmailAsync(model.Email);

				var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);


				var waterMeterLocality = await _countryRepository.GetLocalityAsync(model.LocalityWaterMeterId);



				var request = new RequestWaterMeter
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Address = model.Address,
					NIF = model.NIF,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					HouseNumber = model.HouseNumber,
					PostalCode = model.PostalCode,
					RemainPostalCode = model.RemainPostalCode,
					AddressWaterMeter = model.AddressWaterMeter,
					PostalCodeWaterMeter = model.PostalCodeWaterMeter,
					HouseNumberWaterMeter = model.HouseNumberWaterMeter,
					RemainPostalCodeWaterMeter = model.RemainPostalCodeWaterMeter,
					LocalityId = model.LocalityId,
					LocalityWaterMeterId = model.LocalityWaterMeterId,
					ClientId = model.ClientId,
					InstallationDate = DateTime.Now,
				};

				request.Locality = locality;
				request.WaterMeterLocality = waterMeterLocality;

				await _waterMeterRepository.AddRequestWaterMeterAsync(request);

				var notification = new Notification
				{
					Message = $"Novo pedido de {model.FirstName} {model.LastName}",
					CreatedAt = DateTime.UtcNow,
					IsRead = false,
					RequestWaterMeter = request,
				};

				await _notificationRepository.CreateAsync(notification);
				return new Response { IsSuccess = true };
			}
			catch (Exception ex)
			{
				return new Response
				{
					IsSuccess = false,
					Message = ex.Message,
					Result = ex
				};
			}

		}

	}
}
