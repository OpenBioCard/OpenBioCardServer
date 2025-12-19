using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

/// <summary>
/// 系统设置实体 - 单例配置
/// </summary>
public class SystemSettingsEntity
{
    [Key]
    public int Id { get; set; } = 1; // Singleton, always 1
    
    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = "OpenBioCard (Server)";
    
    // Logo Asset (optional)
    public AssetType? LogoType { get; set; }
    [MaxLength(512)]
    public string? LogoText { get; set; }
    public byte[]? LogoData { get; set; }
}