using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class RejectProjectRequest
    {
        [Required(ErrorMessage = "Lý do từ chối là bắt buộc.")]
        public string Note { get; set; }
    }
}
