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
        [Required(ErrorMessage = "Tiêu đề dự án là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự.")]
        public string Title { get; set; }

        [StringLength(255, ErrorMessage = "Tóm tắt không được vượt quá 255 ký tự.")]
        public string Summary { get; set; }

        public string Description { get; set; }

        public string MediaCoverUrl { get; set; }

        [Required(ErrorMessage = "Mục tiêu kêu gọi là bắt buộc.")]
        [Range(1, double.MaxValue, ErrorMessage = "Mục tiêu kêu gọi phải lớn hơn 0.")]
        public decimal Goal { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        public DateTime EndAt { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public Guid CategoryId { get; set; }
    }
}
