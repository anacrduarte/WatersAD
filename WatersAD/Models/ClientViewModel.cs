using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ClientViewModel : Client 
    {
        [Display(Name="Image")]
        public IFormFile ImageFile { get; set; }
    }
}
