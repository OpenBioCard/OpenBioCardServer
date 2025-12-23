using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

/// <summary>
/// 用户资料实体 - 与Account分离
/// </summary>
public class ProfileEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string AccountName { get; set; } = string.Empty;
    
    /// <summary>
    /// 语言代码 (ISO 639-1). 
    /// null = 默认/主 Profile
    /// "en", "ja" = 特定语言变体
    /// </summary>
    [MaxLength(16)]
    public string? Language { get; set; }
    
    // Avatar (required Asset) - 扁平化存储
    public AssetType AvatarType { get; set; } = AssetType.Text;
    [MaxLength(512)] public string? AvatarTag { get; set; }
    [MaxLength(512)] public string? AvatarText { get; set; }
    public byte[]? AvatarData { get; set; }
    
    [MaxLength(128)]
    public string? Nickname { get; set; }
    
    [MaxLength(64)]
    public string? Pronouns { get; set; }
    
    [MaxLength(2000)]
    public string? Biography { get; set; } // Bio
    
    [MaxLength(256)]
    public string? Location { get; set; }
    
    [MaxLength(512)]
    public string? Website { get; set; }
    
    // Background (optional Asset)
    public AssetType? BackgroundType { get; set; }
    [MaxLength(64)] public string? BackgroundTag { get; set; }
    [MaxLength(512)] public string? BackgroundText { get; set; }
    public byte[]? BackgroundData { get; set; }
    
    [MaxLength(256)]
    public string? CurrentCompany { get; set; }
    
    [MaxLength(512)]
    public string? CurrentCompanyLink { get; set; }
    
    [MaxLength(256)]
    public string? CurrentSchool { get; set; }
    
    [MaxLength(512)]
    public string? CurrentSchoolLink { get; set; }
    
    // Navigation properties
    public Account Account { get; set; } = null!;
    public ICollection<ContactItemEntity> Contacts { get; set; } = new List<ContactItemEntity>();
    public ICollection<SocialLinkItemEntity> SocialLinks { get; set; } = new List<SocialLinkItemEntity>();
    public ICollection<ProjectItemEntity> Projects { get; set; } = new List<ProjectItemEntity>();
    public ICollection<WorkExperienceItemEntity> WorkExperiences { get; set; } = new List<WorkExperienceItemEntity>();
    public ICollection<SchoolExperienceItemEntity> SchoolExperiences { get; set; } = new List<SchoolExperienceItemEntity>();
    public ICollection<GalleryItemEntity> Gallery { get; set; } = new List<GalleryItemEntity>();
}
