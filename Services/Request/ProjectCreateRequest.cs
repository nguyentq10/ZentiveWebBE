namespace Service.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace Repository.DTOs.Requests
    {
        public class ProjectCreateRequest
        {
            [Required]
            public string Title { get; set; }

            public string Summary { get; set; }

            public string Description { get; set; }

            [Range(0, double.MaxValue)]
            public decimal Goal { get; set; }

            public DateTime? StartAt { get; set; }
            public DateTime? EndAt { get; set; }

            // Thông tin người tạo dự án (tạm test, sau này JWT sẽ xác định)
            [Required]
            public string FullName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Phone]
            public string Phone { get; set; }

            public Guid CategoryId { get; set; }

            // Danh sách phần thưởng
            public List<ProjectCreateRequest> RewardTiers { get; set; } = new();
        }

        public class RewardTierCreateRequest
        {
            public string Title { get; set; }

            public string Description { get; set; }

            [Range(1, double.MaxValue)]
            public decimal Amount { get; set; }

            public int? Quantity { get; set; }

            public DateOnly? DeliveryDate { get; set; }
        }
    }

}
