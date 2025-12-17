using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

public class ContactItemEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProfileId { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string Type { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string? Text { get; set; }
    
    // Image Asset (optional) - 扁平化存储
    public AssetType? ImageType { get; set; }
    [MaxLength(512)]
    public string? ImageText { get; set; }
    public byte[]? ImageData { get; set; }
    
    // Navigation property
    public ProfileEntity Profile { get; set; } = null!;
}