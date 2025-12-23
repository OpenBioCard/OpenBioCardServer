using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

public class ProjectItemEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProfileId { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string? Url { get; set; }
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    // Logo Asset (optional)
    public AssetType? LogoType { get; set; }
    [MaxLength(64)] public string? LogoTag { get; set; }
    [MaxLength(512)] public string? LogoText { get; set; }
    public byte[]? LogoData { get; set; }
    
    // Navigation property
    public ProfileEntity Profile { get; set; } = null!;
}