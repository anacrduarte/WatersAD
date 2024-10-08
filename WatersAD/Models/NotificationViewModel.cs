using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class NotificationViewModel
    {
       public int RequestWaterMeterId { get; set; }

       public int NotificationId { get; set; }

        [Display(Name = "Data de Instalação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InstallationDate { get; set; }
    }
}
