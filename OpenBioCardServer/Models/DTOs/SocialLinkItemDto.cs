namespace OpenBioCardServer.Models.DTOs;

public class SocialLinkItemDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new();
}