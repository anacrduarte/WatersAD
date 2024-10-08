using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;
using WatersAD.Helpers;

namespace WatersAD.Models
{
    public class ClientViewModel :BasicInformationHelper
    {
      

        public Client Client { get; set; }

        public int ClientId { get; set; }

        public IEnumerable<SelectListItem> WaterMeterServices { get; set; }

        [Display(Name = "Data de Instalação")]
        public DateTime InstallationDate { get; set; }

        

       
    }
}
