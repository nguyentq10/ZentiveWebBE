using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class AdminDonationDetailsDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } // Trạng thái thanh toán
        public DonorInfoDto? Donor { get; set; } // Thông tin người ủng hộ (nếu không ẩn danh)
    }

    public class DonorInfoDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
    }
}
