using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class CreateProjectRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Summary { get; set; } 

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Goal { get; set; } 

        [Required]
        public DateTime EndAt { get; set; } 
        [Required]
        public Guid CategoryId { get; set; }
    }
}
