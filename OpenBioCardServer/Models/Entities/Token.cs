using System.ComponentModel.DataAnnotations;

namespace OpenBioCardServer.Models.Entities;

/// <summary>
/// 访问令牌 - 支持多设备同时登录
/// </summary>
public class Token
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(128)]
    public string TokenValue { get; set; } = string.Empty;
    
    [Required]
    public Guid AccountId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 设备信息（可选），用于标识设备
    /// </summary>
    [MaxLength(256)]
    public string? DeviceInfo { get; set; }
    
    // Navigation property
    public Account Account { get; set; } = null!;
}