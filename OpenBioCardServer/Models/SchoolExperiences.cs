namespace OpenBioCardServer.Models;

public class SchoolExperiences
{
    public string School { get; set; } = string.Empty;
    public string? SchoolLink { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? Major { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrent { get; set; }
}