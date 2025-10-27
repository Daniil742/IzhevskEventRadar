namespace IzhevskEventRadar.Contracts.Models;

public class PostDto
{
    public int Id { get; set; }
    public long PublishDate { get; set; }
    public string? Text { get; set; }
    public AttachmentDto[] Attachments { get; set; } = Array.Empty<AttachmentDto>();
}
