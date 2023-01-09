using Microsoft.Build.Framework;

namespace ASPTemplate.Dtos
{
    public class InstaPostRequest
    {
        public IFormFile Image { get; set; }
        public string? Caption { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;

    }
}
