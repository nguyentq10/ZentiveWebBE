using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class CreateMediaAssetRequestDto
    {
        [Required]
        [Url] // Đảm bảo đây là một URL hợp lệ
        public string Url { get; set; }

        [Required]
        [StringLength(50)] // Giới hạn độ dài loại media
        public string Type { get; set; } // Ví dụ: "Image", "Video"

        // Có thể không bắt buộc, service sẽ tự gán nếu cần
        public int SortOrder { get; set; } = 0;
    }
}
