namespace OpenBioCardServer.Models;

public class SystemSettings
{
    public const string DefaultTitle = "OpenBioCard (Server)";
    
    public string Title { get; set; } = string.Empty;
    public Asset Logo { get; set; } = new();
}