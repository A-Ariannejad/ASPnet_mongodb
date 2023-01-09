namespace ASPTemplate.Dtos
{
    public class InstaPostUpdateRequest
    {
        public string PostId { get; set; } 
        public IFormFile Image { get; set; }
        public string? Caption { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
    }
}
