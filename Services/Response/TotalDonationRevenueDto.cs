using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class TotalDonationRevenueDto
    {
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "VND"; // Mặc định là VND
    }
}
