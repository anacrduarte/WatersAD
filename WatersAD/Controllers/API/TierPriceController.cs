using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TierPriceController : Controller
    {
        private readonly ITierRepository _tierRepository;

        public TierPriceController(ITierRepository tierRepository)
        {
            _tierRepository = tierRepository;
        }

        [HttpGet("GetAllTiers")]
        public ActionResult<IEnumerable<Tier>> GetAllTiers()
        {
            try
            {
                var tiers = _tierRepository.GetAll().OrderBy(t => t.TierNumber);
                return Ok(tiers); 
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "Erro ao buscar os dados dos tiers", error = ex.Message });
            }
        }
    }
}
