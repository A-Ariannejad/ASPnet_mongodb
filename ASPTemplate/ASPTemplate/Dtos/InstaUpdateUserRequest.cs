namespace ASPTemplate.Dtos
{
    public class InstaUpdateUserRequest
    {
        public string? Gender { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public IFormFile? Image { get; set; } = null;
    }
}
