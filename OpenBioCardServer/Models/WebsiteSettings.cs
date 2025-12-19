namespace OpenBioCardServer.Models;

public class WebsiteSettings
{
    public string Title { get; set; } = string.Empty;
    public Asset Logo { get; set; } = new();
}