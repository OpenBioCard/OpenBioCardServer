using System.ComponentModel.DataAnnotations;

namespace OpenBioCardServer.Models.Entities;

public class SocialLinkItemEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProfileId { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(512)]
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展属性（如GithubData）存储为JSON
    /// PostgreSQL: jsonb, SQLite: text
    /// </summary>
    public string? AttributesJson { get; set; }
    
    // Navigation property
    public ProfileEntity Profile { get; set; } = null!;
}