using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class PreparePledgeRequest
    {
       
        public Guid? RewardTierId { get; set; }

      
        [Range(10000, 1000000000, ErrorMessage = "Amount must be at least 10,000 VND.")]
        public decimal? Amount { get; set; }
    }
}
