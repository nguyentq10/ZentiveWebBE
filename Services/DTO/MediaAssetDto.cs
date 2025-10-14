using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class MediaAssetDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; } // Ví dụ: "Image", "Video"
        public int SortOrder { get; set; }
    }
}
