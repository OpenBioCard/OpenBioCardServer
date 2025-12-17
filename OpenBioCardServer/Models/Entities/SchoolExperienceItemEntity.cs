using System.ComponentModel.DataAnnotations;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Models.Entities;

public class SchoolExperienceItemEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProfileId { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string School { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string? SchoolLink { get; set; }
    
    [MaxLength(128)]
    public string? Degree { get; set; }
    
    [MaxLength(128)]
    public string? Major { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    // Logo Asset (optional)
    public AssetType? LogoType { get; set; }
    [MaxLength(512)]
    public string? LogoText { get; set; }
    public byte[]? LogoData { get; set; }
    
    // Navigation property
    public ProfileEntity Profile { get; set; } = null!;
}