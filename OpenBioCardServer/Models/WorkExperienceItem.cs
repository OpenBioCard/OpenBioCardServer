namespace OpenBioCardServer.Models;

public class WorkExperienceItem
{
    public string Company { get; set; } = string.Empty;
    public string? CompanyUrl { get; set; }
    public string? Position { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public string? Description { get; set; }
    
    public Asset? Logo { get; set; }
}