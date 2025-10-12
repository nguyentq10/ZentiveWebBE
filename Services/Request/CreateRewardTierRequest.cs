using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class CreateRewardTierRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; } // Renamed from PledgeAmount

        public int? Quantity { get; set; }

        public DateOnly? DeliveryDate { get; set; } // Added this new field
    }
}
