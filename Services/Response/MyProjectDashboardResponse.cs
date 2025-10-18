using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class MyProjectDashboardResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string MediaCoverUrl { get; set; }
        public string Status { get; set; }

        // --- Các trường theo dõi tiến độ ---
        public decimal CurrentAmount { get; set; }
        public decimal Goal { get; set; }
        public double ProgressPercentage { get; set; }
        public int BackerCount { get; set; }

        // --- Các trường thông tin bổ sung ---
        public string CategoryName { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
