namespace OpenBioCardServer.Models;

public class Profile
{
    public string UserName { get; set; } = string.Empty;
    
    public Asset Avatar { get; set; } = new();
    public string? NickName { get; set; }
    public string? Pronouns { get; set; }
    public string? Description { get; set; } // BIO
    public string? Location { get; set; }
    public string? Website { get; set; }
    public Asset? Background { get; set; }
    
    public string? CurrentCompany { get; set; }
    public string? CurrentCompanyLink { get; set; }
    public string? CurrentSchool { get; set; }
    public string? CurrentSchoolLink { get; set; }

    public List<ContactItem> Contacts { get; set; } = new();
    public List<SocialLinkItem> SocialLinks { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();

    public List<WorkExperienceItem> WorkExperiences { get; set; } = new();
    public List<SchoolExperienceItem> SchoolExperiences { get; set; } = new();

    public List<GalleryItem> Gallery { get; set; } = new();
}