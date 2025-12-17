namespace OpenBioCardServer.Models.DTOs;

public class ProjectItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Description { get; set; }
    public AssetDto? Logo { get; set; }
}