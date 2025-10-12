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
        public string Subtitle { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal CurrentPledgeAmount { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreatorName { get; set; }
    }
}
