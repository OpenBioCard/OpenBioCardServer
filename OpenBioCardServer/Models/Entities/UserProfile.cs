using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenBioCardServer.Models.Entities;

/// <summary>
/// 用户资料实体 - 存储公开的个人资料
/// </summary>
public class UserProfile
{
    [Key]
    [ForeignKey(nameof(Account))]
    public Guid UserId { get; set; }

    // Basic profile fields
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

    // Collections
    public List<ContactItem> Contacts { get; set; } = new();
    public List<SocialLinkItem> SocialLinks { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();
    public List<GalleryItem> Gallery { get; set; } = new();

    // 导航属性
    public UserAccount Account { get; set; } = null!;
}