using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class ProjectDetailResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public decimal Goal { get; set; }
        public decimal CurrentAmount { get; set; }
        public string MediaCoverUrl { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; }
        public Guid CreatorId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
