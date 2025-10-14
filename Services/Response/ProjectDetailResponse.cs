using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class ProjectDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public decimal Goal { get; set; }
        public decimal CurrentAmount { get; set; }
        public double ProgressPercentage { get; set; } // Tiến độ (tính toán)
        public int BackerCount { get; set; } // Số người ủng hộ (tính toán)
        public string MediaCoverUrl { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorName { get; set; }

        // Dữ liệu lồng nhau
        public List<RewardTierDto> Tiers { get; set; }
        public List<MediaAssetDto> Media { get; set; }
    }
}
