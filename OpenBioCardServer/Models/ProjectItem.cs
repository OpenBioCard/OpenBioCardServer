namespace OpenBioCardServer.Models;

public class ProjectItem
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Description { get; set; }
    public Asset? Logo { get; set; }
}