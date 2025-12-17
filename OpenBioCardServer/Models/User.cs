using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OpenBioCardServer.Models.Entities;

namespace OpenBioCardServer.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string PasswordSalt { get; set; } = string.Empty;

    [JsonIgnore]
    public string Token { get; set; } = Guid.NewGuid().ToString();

    public string Type { get; set; } = "user"; 

    // 基础资料
    public string Name { get; set; } = string.Empty;
    public string? Pronouns { get; set; }
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? Background { get; set; }

    public string? CurrentCompany { get; set; }
    public string? CurrentCompanyLink { get; set; }
    public string? CurrentSchool { get; set; }
    public string? CurrentSchoolLink { get; set; }

    // 集合
    public List<ContactItem> Contacts { get; set; } = new();
    public List<SocialLinkItem> SocialLinks { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();
    public List<GalleryItem> Gallery { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<SchoolExperiences> Educations { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}