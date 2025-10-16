using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class CreateDonationRequestDto
    {
        [Required]
        [Range(10000, 1000000000, ErrorMessage = "Amount must be at least 10,000 VND.")]
        public decimal Amount { get; set; }

        // Cho phép người dùng chọn ẩn danh, ngay cả khi họ đã đăng nhập
        public bool IsAnonymous { get; set; } = false;

        // Lời nhắn tùy chọn
        public string? Message { get; set; }
    }
}
