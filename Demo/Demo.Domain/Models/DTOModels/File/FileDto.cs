namespace Demo.Domain.Models.DTOModels.File;

public class FileDto
{
    public byte[] File { get; set; }
    public string MimeType { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public int Size { get; set; }
}