using System.ComponentModel.DataAnnotations;

namespace OpenBioCardServer.Models.Entities;

/// <summary>
/// 媒体资源实体 - 独立存储 BASE64 图片等二进制数据
/// </summary>
public class MediaAsset
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 资源类型：avatar, background, qrcode, project_logo, gallery_image 等
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 存储类型：base64, url, cloud_storage
    /// 便于将来迁移到云存储
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string StorageType { get; set; } = "base64";

    /// <summary>
    /// 实际数据：BASE64 字符串或 URL
    /// 将来迁移到云存储时，这里存储 URL
    /// </summary>
    [Required]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// MIME 类型：image/png, image/jpeg 等
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; set; }

    /// <summary>
    /// 文件大小（字节）- 用于未来的配额管理
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 所属用户 ID
    /// </summary>
    public Guid UserId { get; set; }

    // 导航属性
    public UserAccount User { get; set; } = null!;
}