namespace Store.webApi.Dtos.Common
{
    public class FileUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public string? Folder { get; set; }
    }
}
