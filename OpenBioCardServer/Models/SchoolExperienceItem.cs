namespace OpenBioCardServer.Models;

public class SchoolExperienceItem
{
    public string School { get; set; } = string.Empty;
    public string? SchoolLink { get; set; }
    public string? Degree { get; set; }
    public string? Major { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public string? Description { get; set; }
    
    public Asset? Logo { get; set; }
}