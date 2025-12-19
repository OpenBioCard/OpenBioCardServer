namespace OpenBioCardServer.Models;

public class SystemSettings
{
    public string Title { get; set; } = string.Empty;
    public Asset Logo { get; set; } = new();
}