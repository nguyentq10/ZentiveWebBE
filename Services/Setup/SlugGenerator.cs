using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Setup
{
    public static class SlugGenerator
    {
        // Hàm chính để tạo slug
        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            // 1. Loại bỏ dấu tiếng Việt và chuyển "đ" thành "d"
            string slug = RemoveDiacritics(title);

            // 2. Chuyển sang chữ thường
            slug = slug.ToLowerInvariant();

            // 3. Xóa các ký tự đặc biệt còn lại (chỉ giữ lại chữ, số, khoảng trắng, gạch nối)
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "", RegexOptions.Compiled);

            // 4. Thay thế một hoặc nhiều khoảng trắng bằng MỘT dấu gạch nối
            slug = Regex.Replace(slug, @"\s+", "-", RegexOptions.Compiled);

            // 5. (Tùy chọn) Xóa các dấu gạch nối trùng lặp (nếu có)
            // Bước này thường không cần thiết nếu bước 3 và 4 làm tốt
            // nhưng để đây cho chắc chắn
            slug = Regex.Replace(slug, @"-{2,}", "-", RegexOptions.Compiled);

            // 6. Cắt bỏ dấu gạch nối ở đầu và cuối chuỗi
            slug = slug.Trim('-');

            // (Tùy chọn) Giới hạn độ dài của slug
            // if (slug.Length > 100)
            //    slug = slug.Substring(0, 100);

            return slug;
        }

        // Hàm phụ trợ để loại bỏ dấu tiếng Việt
        private static string RemoveDiacritics(string text)
        {
            // Xử lý chữ 'đ' và 'Đ'
            text = text.Replace("đ", "d").Replace("Đ", "D");

            // Chuẩn hóa chuỗi về FormD (phân tách ký tự và dấu)
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            // Lặp qua từng ký tự và chỉ giữ lại các ký tự không phải là dấu
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Chuẩn hóa lại về FormC (dạng thông thường)
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
