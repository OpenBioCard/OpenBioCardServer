using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

public class GalleryItemEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProfileId { get; set; }
    
    // Image Asset (optional but expected)
    public AssetType? ImageType { get; set; }
    [MaxLength(512)]
    public string? ImageText { get; set; }
    public byte[]? ImageData { get; set; }
    
    [MaxLength(500)]
    public string? Caption { get; set; }
    
    // Navigation property
    public ProfileEntity Profile { get; set; } = null!;
}