namespace OpenBioCardServer.Models.DTOs;

public class ContactItemDto
{
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; }
    public AssetDto? Image { get; set; }
}