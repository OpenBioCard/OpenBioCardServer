namespace OpenBioCardServer.Models.DTOs;

public class ProfileDto
{
    public string Username { get; set; } = string.Empty;
    
    // Asset DTOs
    public AssetDto Avatar { get; set; } = new();
    public string? NickName { get; set; }
    public string? Pronouns { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public AssetDto? Background { get; set; }
    
    public string? CurrentCompany { get; set; }
    public string? CurrentCompanyLink { get; set; }
    public string? CurrentSchool { get; set; }
    public string? CurrentSchoolLink { get; set; }

    public List<ContactItemDto> Contacts { get; set; } = new();
    public List<SocialLinkItemDto> SocialLinks { get; set; } = new();
    public List<ProjectItemDto> Projects { get; set; } = new();
    public List<WorkExperienceItemDto> WorkExperiences { get; set; } = new();
    public List<SchoolExperienceItemDto> SchoolExperiences { get; set; } = new();
    public List<GalleryItemDto> Gallery { get; set; } = new();
}