using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class ProjectSummaryResponse
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; } 
        public string MediaCoverUrl { get; set; } 
        public decimal CurrentAmount { get; set; }
        public decimal Goal { get; set; } 
        public DateTime? EndAt { get; set; } 
        public string CreatorName { get; set; }
    }
}
