namespace OpenBioCardServer.Models;

public class WorkExperience
{
    public string Position { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? CompanyLink { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrent { get; set; }
}