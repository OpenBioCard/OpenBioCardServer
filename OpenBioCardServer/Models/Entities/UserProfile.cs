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

    // 基础资料
    public string Name { get; set; } = string.Empty;
    public string? Pronouns { get; set; }
    
    /// <summary>
    /// 头像 - 可以是 emoji/字符 或图片
    /// 存储格式：
    /// - emoji 或纯文本：直接存储（如 "👨"）
    /// - BASE64 图片（前端提交）：转存到 MediaAsset 后存为 "asset:{GUID}"
    /// </summary>
    public string? Avatar { get; set; }
    
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    
    /// <summary>
    /// 背景图 - 仅支持图片
    /// 存储格式：
    /// - 空值：不显示背景
    /// - BASE64 图片（前端提交）：转存到 MediaAsset 后存为 "asset:{GUID}"
    /// </summary>
    public string? Background { get; set; }

    public string? CurrentCompany { get; set; }
    public string? CurrentCompanyLink { get; set; }
    public string? CurrentSchool { get; set; }
    public string? CurrentSchoolLink { get; set; }

    // 集合（嵌套对象，存储为 JSON）
    public List<ContactItem> Contacts { get; set; } = new();
    public List<SocialLinkItem> SocialLinks { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();
    public List<GalleryItem> Gallery { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<SchoolExperiences> SchoolExperiences { get; set; } = new();

    // 导航属性
    public UserAccount Account { get; set; } = null!;
}