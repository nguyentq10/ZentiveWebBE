using System.ComponentModel.DataAnnotations;

namespace Services.Request
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string ConfirmPassword { get; set; }

        // --- Bổ sung các trường mới (tùy chọn) ---

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; } // Dấu ? cho biết trường này không bắt buộc

        public string? Address { get; set; }

        public string? School { get; set; }
    }
}
