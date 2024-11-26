using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public ClientsController(IClientRepository clientRepository, IUserHelper userHelper, IConverterHelper converterHelper)
        {
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        [HttpGet]
        [Route("api/clients/{id}")]
        public async Task<IActionResult> GetClientDetails(int? id)
        {
     
            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

         
            if (client == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Client not found."
                });
            }

     
            var user = await _userHelper.GetUserByEmailAsync(client.Email);
            client.User = user;

       
            var model = _converterHelper.ToClientViewModel(client);

            return Ok(new
            {
                Success = true,
                ClientDetails = model
            });
        }

    }
}
