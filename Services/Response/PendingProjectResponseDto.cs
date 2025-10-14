using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class PendingProjectResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CreatorName { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
