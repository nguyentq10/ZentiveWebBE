using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class RewardTierResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; } // Renamed from PledgeAmount
        public int? Quantity { get; set; }
        public DateOnly? DeliveryDate { get; set; } // Added this new field
    }
}
