using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Core
{
    public static class StatusConstants
    {
        // Trạng thái chung của Payment/Pledge
        // QUAN TRỌNG: Hãy đảm bảo các giá trị chuỗi này
        // khớp chính xác với CHECK constraint trong database của bạn.
        public const string PaymentPending = "Pending";
        public const string PaymentCompleted = "Paid"; // Hoặc "Paid" tùy database
        public const string PaymentFailed = "Failed";

        // Trạng thái dự án
        public const string ProjectPublished = "Published";
        public const string ProjectDraft = "Draft";
        public const string ProjectPending = "Pending";
        // ... (Thêm các trạng thái khác nếu cần)
    }
}
