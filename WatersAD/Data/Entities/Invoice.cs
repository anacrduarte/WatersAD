﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WatersAD.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Data da fatura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; }

        public int ClientId { get; set; }

        [JsonIgnore]
        public Client Client { get; set; }

        [Display(Name = "Emitida")]
        public bool Issued { get; set; } = false;
        [Display(Name = "Enviada")]
        public bool Sent { get; set; } = false;

        [Display(Name = "Total")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Data limite de pagamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LimitDate { get; set; }
    }
}
