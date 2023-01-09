using System.ComponentModel.DataAnnotations;

namespace ASPTemplate.Dtos
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Fullname { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required,DataType(DataType.Password), Compare(nameof(Password), ErrorMessage ="Passwords do not match !!!")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
