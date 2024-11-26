using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryCityController: Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountryCityController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }


   

		[HttpGet("GetCitiesByCountryId/{countryId}")]
		public async Task<IActionResult> GetCitiesByCountryId(int countryId)
		{
			if (countryId <= 0)
			{
				return BadRequest(new { ErrorMessage = "Invalid country ID." });
			}

			try
			{
				var cities = await _countryRepository.GetCitiesByCountryIdAsync(countryId);

				if (cities == null || !cities.Any())
				{
					return NotFound(new { ErrorMessage = "No cities found for the specified country." });
				}

				return Ok(cities);
			}
			catch (Exception ex)
			{
				// Log the exception
				return StatusCode(500, new { ErrorMessage = "An error occurred while processing your request.", Details = ex.Message });
			}
		}


		[HttpGet("GetAllCountry")]
		public async Task<IActionResult> GetAllCountry()
		{
			try
			{
				var countries = await _countryRepository.GetAllAsync(); 
				return Ok(countries);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Erro ao buscar os dados dos países", error = ex.Message });
			}
		}

		[HttpGet("GetLocalityByCityId/{cityId}")]
		public async Task<IActionResult> GetLocalityByCityId(int cityId)
		{
			if (cityId <= 0)
			{
				return BadRequest(new { ErrorMessage = "Invalid city ID." });
			}

			try
			{
				var localities = await _countryRepository.GetLocalitiesByCityIdAsync(cityId);

				if (localities == null || !localities.Any())
				{
					return NotFound(new { ErrorMessage = "No localities found for the specified country." });
				}

				return Ok(localities);
			}
			catch (Exception ex)
			{
				
				return StatusCode(500, new { ErrorMessage = "An error occurred while processing your request.", Details = ex.Message });
			}
		}

	}
}
