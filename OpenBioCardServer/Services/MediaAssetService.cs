using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenBioCardServer.Configuration;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models.Entities;

namespace OpenBioCardServer.Services;

/// <summary>
/// 媒体资源服务 - 管理 BASE64 图片等媒体资源
/// </summary>
public class MediaAssetService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MediaAssetService> _logger;
    private readonly AssetSettings _assetSettings;
    
    // 资产引用格式前缀
    private const string AssetRefPrefix = "asset:";

    public MediaAssetService(
        AppDbContext context, 
        ILogger<MediaAssetService> logger,
        IOptions<AssetSettings> assetSettings)
    {
        _context = context;
        _logger = logger;
        _assetSettings = assetSettings.Value;
    }

    /// <summary>
    /// 处理用户资料中的所有图片 - 将 BASE64 转存到 MediaAsset 表
    /// </summary>
    public async Task ProcessProfileImagesAsync(Guid userId, UserProfile profile)
    {
        // 1. 处理头像（如果是BASE64图片）
        if (!string.IsNullOrEmpty(profile.Avatar) && IsBase64Image(profile.Avatar))
        {
            var result = await SaveMediaAssetAsync(userId, "avatar", profile.Avatar);
            if (result.success && result.assetId.HasValue)
            {
                profile.Avatar = $"{AssetRefPrefix}{result.assetId.Value}";
            }
        }

        // 2. 处理背景图
        if (!string.IsNullOrEmpty(profile.Background) && IsBase64Image(profile.Background))
        {
            var result = await SaveMediaAssetAsync(userId, "background", profile.Background);
            if (result.success && result.assetId.HasValue)
            {
                profile.Background = $"{AssetRefPrefix}{result.assetId.Value}";
            }
        }

        // 3. 处理联系方式中的二维码图片
        for (int i = 0; i < profile.Contacts.Count; i++)
        {
            var contact = profile.Contacts[i];
            if (!string.IsNullOrEmpty(contact.Value) && IsBase64Image(contact.Value))
            {
                var result = await SaveMediaAssetAsync(userId, $"contact_qr_{contact.Type}", contact.Value);
                if (result.success && result.assetId.HasValue)
                {
                    profile.Contacts[i].Value = $"{AssetRefPrefix}{result.assetId.Value}";
                }
            }
        }

        // 4. 处理项目Logo
        for (int i = 0; i < profile.Projects.Count; i++)
        {
            var project = profile.Projects[i];
            if (!string.IsNullOrEmpty(project.Logo) && IsBase64Image(project.Logo))
            {
                var result = await SaveMediaAssetAsync(userId, "project_logo", project.Logo);
                if (result.success && result.assetId.HasValue)
                {
                    profile.Projects[i].Logo = $"{AssetRefPrefix}{result.assetId.Value}";
                }
            }
        }

        // 5. 处理相册图片
        for (int i = 0; i < profile.Gallery.Count; i++)
        {
            var item = profile.Gallery[i];
            if (!string.IsNullOrEmpty(item.Image) && IsBase64Image(item.Image))
            {
                var result = await SaveMediaAssetAsync(userId, "gallery", item.Image);
                if (result.success && result.assetId.HasValue)
                {
                    profile.Gallery[i].Image = $"{AssetRefPrefix}{result.assetId.Value}";
                }
            }
        }

        // 6. 处理工作经历Logo（必须是图片）
        for (int i = 0; i < profile.WorkExperiences.Count; i++)
        {
            var work = profile.WorkExperiences[i];
            if (!string.IsNullOrEmpty(work.Logo))
            {
                // Logo 字段必须是图片格式
                if (!IsBase64Image(work.Logo))
                {
                    throw new ArgumentException($"Work experience logo must be a valid image format");
                }
                
                var result = await SaveMediaAssetAsync(userId, "work_logo", work.Logo);
                if (result.success && result.assetId.HasValue)
                {
                    profile.WorkExperiences[i].Logo = $"{AssetRefPrefix}{result.assetId.Value}";
                }
            }
        }

        // 7. 处理教育经历Logo（必须是图片）
        for (int i = 0; i < profile.SchoolExperiences.Count; i++)
        {
            var edu = profile.SchoolExperiences[i];
            if (!string.IsNullOrEmpty(edu.Logo))
            {
                // Logo 字段必须是图片格式
                if (!IsBase64Image(edu.Logo))
                {
                    throw new ArgumentException($"School experience logo must be a valid image format");
                }
                
                var result = await SaveMediaAssetAsync(userId, "school_logo", edu.Logo);
                if (result.success && result.assetId.HasValue)
                {
                    profile.SchoolExperiences[i].Logo = $"{AssetRefPrefix}{result.assetId.Value}";
                }
            }
        }
    }

    /// <summary>
    /// 还原用户资料中的所有图片引用 - 将 "asset:{GUID}" 还原为 BASE64
    /// </summary>
    public async Task RestoreProfileImagesAsync(UserProfile profile)
    {
        // 1. 还原头像
        if (!string.IsNullOrEmpty(profile.Avatar) && IsAssetReference(profile.Avatar))
        {
            var data = await GetMediaAssetDataAsync(ExtractAssetId(profile.Avatar));
            if (data != null)
            {
                profile.Avatar = data;
            }
        }

        // 2. 还原背景图
        if (!string.IsNullOrEmpty(profile.Background) && IsAssetReference(profile.Background))
        {
            var data = await GetMediaAssetDataAsync(ExtractAssetId(profile.Background));
            if (data != null)
            {
                profile.Background = data;
            }
        }

        // 3. 还原联系方式二维码
        for (int i = 0; i < profile.Contacts.Count; i++)
        {
            var contact = profile.Contacts[i];
            if (!string.IsNullOrEmpty(contact.Value) && IsAssetReference(contact.Value))
            {
                var data = await GetMediaAssetDataAsync(ExtractAssetId(contact.Value));
                if (data != null)
                {
                    profile.Contacts[i].Value = data;
                }
            }
        }

        // 4. 还原项目Logo
        for (int i = 0; i < profile.Projects.Count; i++)
        {
            var project = profile.Projects[i];
            if (!string.IsNullOrEmpty(project.Logo) && IsAssetReference(project.Logo))
            {
                var data = await GetMediaAssetDataAsync(ExtractAssetId(project.Logo));
                if (data != null)
                {
                    profile.Projects[i].Logo = data;
                }
            }
        }

        // 5. 还原相册图片
        for (int i = 0; i < profile.Gallery.Count; i++)
        {
            var item = profile.Gallery[i];
            if (!string.IsNullOrEmpty(item.Image) && IsAssetReference(item.Image))
            {
                var data = await GetMediaAssetDataAsync(ExtractAssetId(item.Image));
                if (data != null)
                {
                    profile.Gallery[i].Image = data;
                }
            }
        }

        // 6. 还原工作经历Logo
        for (int i = 0; i < profile.WorkExperiences.Count; i++)
        {
            var work = profile.WorkExperiences[i];
            if (!string.IsNullOrEmpty(work.Logo) && IsAssetReference(work.Logo))
            {
                var data = await GetMediaAssetDataAsync(ExtractAssetId(work.Logo));
                if (data != null)
                {
                    profile.WorkExperiences[i].Logo = data;
                }
            }
        }

        // 7. 还原教育经历Logo
        for (int i = 0; i < profile.SchoolExperiences.Count; i++)
        {
            var edu = profile.SchoolExperiences[i];
            if (!string.IsNullOrEmpty(edu.Logo) && IsAssetReference(edu.Logo))
            {
                var data = await GetMediaAssetDataAsync(ExtractAssetId(edu.Logo));
                if (data != null)
                {
                    profile.SchoolExperiences[i].Logo = data;
                }
            }
        }
    }

    /// <summary>
    /// 保存媒体资源（当前实现：BASE64）
    /// 将来可扩展：上传到云存储并返回 URL
    /// </summary>
    public async Task<(bool success, Guid? assetId, string? error)> SaveMediaAssetAsync(
        Guid userId, 
        string type, 
        string base64Data)
    {
        try
        {
            // 验证 BASE64 数据格式
            if (!IsValidBase64Image(base64Data))
            {
                return (false, null, "Invalid BASE64 image format");
            }

            // 提取 MIME 类型和实际数据
            var (mimeType, actualData) = ExtractBase64Data(base64Data);
            
            // 验证 MIME 类型
            if (!_assetSettings.AllowedImageTypes.Contains(mimeType))
            {
                return (false, null, $"Image type not allowed: {mimeType}");
            }
            
            // 估算文件大小（BASE64 编码后约为原始大小的 4/3）
            var estimatedSize = (long)(actualData.Length * 0.75);
            
            // 使用配置的最大文件大小
            if (estimatedSize > _assetSettings.MaxFileSizeBytes)
            {
                var maxSizeMB = _assetSettings.MaxFileSizeMB;
                var actualSizeMB = estimatedSize / 1024.0 / 1024.0;
                return (false, null, 
                    $"File size ({actualSizeMB:F2}MB) exceeds maximum allowed size ({maxSizeMB}MB)");
            }

            var asset = new MediaAsset
            {
                UserId = userId,
                Type = type,
                StorageType = "base64",
                Data = base64Data,
                MimeType = mimeType,
                FileSize = estimatedSize
            };

            _context.MediaAssets.Add(asset);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Media asset saved: UserId={UserId}, Type={Type}, Size={Size}KB, AssetId={AssetId}", 
                userId, type, estimatedSize / 1024, asset.Id);

            return (true, asset.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save media asset: UserId={UserId}, Type={Type}", userId, type);
            return (false, null, "Failed to save media asset");
        }
    }

    /// <summary>
    /// 获取媒体资源数据
    /// </summary>
    public async Task<string?> GetMediaAssetDataAsync(Guid assetId)
    {
        var asset = await _context.MediaAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == assetId);

        return asset?.Data;
    }

    /// <summary>
    /// 删除用户的特定类型媒体资源
    /// </summary>
    public async Task<bool> DeleteUserMediaAssetAsync(Guid userId, string type)
    {
        try
        {
            var assets = await _context.MediaAssets
                .Where(m => m.UserId == userId && m.Type == type)
                .ToListAsync();

            if (assets.Any())
            {
                _context.MediaAssets.RemoveRange(assets);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Deleted {Count} media assets: UserId={UserId}, Type={Type}", 
                    assets.Count, userId, type);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete media assets: UserId={UserId}, Type={Type}", userId, type);
            return false;
        }
    }

    /// <summary>
    /// 清理用户的所有媒体资源
    /// </summary>
    public async Task<bool> CleanupUserMediaAssetsAsync(Guid userId)
    {
        try
        {
            var assets = await _context.MediaAssets
                .Where(m => m.UserId == userId)
                .ToListAsync();

            if (assets.Any())
            {
                _context.MediaAssets.RemoveRange(assets);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Cleaned up {Count} media assets for user: UserId={UserId}", 
                    assets.Count, userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup media assets: UserId={UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// 获取用户使用的存储空间统计
    /// </summary>
    public async Task<(long totalBytes, int fileCount)> GetUserStorageStatsAsync(Guid userId)
    {
        var stats = await _context.MediaAssets
            .Where(m => m.UserId == userId)
            .GroupBy(m => m.UserId)
            .Select(g => new
            {
                TotalBytes = g.Sum(m => m.FileSize ?? 0),
                FileCount = g.Count()
            })
            .FirstOrDefaultAsync();

        return stats != null 
            ? (stats.TotalBytes, stats.FileCount) 
            : (0, 0);
    }
    
    #region Profile Asset Management

    /// <summary>
    /// 从 UserProfile 中提取所有资产引用（asset:{GUID}）
    /// </summary>
    public List<Guid> ExtractAllAssetReferences(UserProfile profile)
    {
        var assetIds = new List<Guid>();

        // 1. 头像
        if (!string.IsNullOrEmpty(profile.Avatar) && IsAssetReference(profile.Avatar))
        {
            assetIds.Add(ExtractAssetId(profile.Avatar));
        }

        // 2. 背景图
        if (!string.IsNullOrEmpty(profile.Background) && IsAssetReference(profile.Background))
        {
            assetIds.Add(ExtractAssetId(profile.Background));
        }

        // 3. 联系方式二维码
        foreach (var contact in profile.Contacts)
        {
            if (!string.IsNullOrEmpty(contact.Value) && IsAssetReference(contact.Value))
            {
                assetIds.Add(ExtractAssetId(contact.Value));
            }
        }

        // 4. 项目Logo
        foreach (var project in profile.Projects)
        {
            if (!string.IsNullOrEmpty(project.Logo) && IsAssetReference(project.Logo))
            {
                assetIds.Add(ExtractAssetId(project.Logo));
            }
        }

        // 5. 相册图片
        foreach (var item in profile.Gallery)
        {
            if (!string.IsNullOrEmpty(item.Image) && IsAssetReference(item.Image))
            {
                assetIds.Add(ExtractAssetId(item.Image));
            }
        }

        // 6. 工作经历Logo
        foreach (var work in profile.WorkExperiences)
        {
            if (!string.IsNullOrEmpty(work.Logo) && IsAssetReference(work.Logo))
            {
                assetIds.Add(ExtractAssetId(work.Logo));
            }
        }

        // 7. 教育经历Logo
        foreach (var edu in profile.SchoolExperiences)
        {
            if (!string.IsNullOrEmpty(edu.Logo) && IsAssetReference(edu.Logo))
            {
                assetIds.Add(ExtractAssetId(edu.Logo));
            }
        }

        return assetIds;
    }

    /// <summary>
    /// 删除指定的媒体资源（不自动保存更改，由调用者统一保存）
    /// </summary>
    public async Task<bool> DeleteMediaAssetAsync(Guid assetId)
    {
        try
        {
            var asset = await _context.MediaAssets.FindAsync(assetId);
            if (asset != null)
            {
                _context.MediaAssets.Remove(asset);
                
                _logger.LogInformation(
                    "Marked media asset for deletion: AssetId={AssetId}, UserId={UserId}, Type={Type}", 
                    assetId, asset.UserId, asset.Type);
                
                return true;
            }
            
            _logger.LogWarning("Media asset not found: AssetId={AssetId}", assetId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete media asset: AssetId={AssetId}", assetId);
            return false;
        }
    }

    #endregion

    #region Utilities

    /// <summary>
    /// 判断字符串是否为 BASE64 图片
    /// </summary>
    private static bool IsBase64Image(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;

        return data.StartsWith("data:image/") && data.Contains("base64,");
    }

    /// <summary>
    /// 判断字符串是否为资产引用格式 "asset:{GUID}"
    /// </summary>
    private static bool IsAssetReference(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;

        return data.StartsWith(AssetRefPrefix) && 
               Guid.TryParse(data.Substring(AssetRefPrefix.Length), out _);
    }

    /// <summary>
    /// 从资产引用中提取 GUID
    /// </summary>
    private static Guid ExtractAssetId(string assetRef)
    {
        return Guid.Parse(assetRef.Substring(AssetRefPrefix.Length));
    }

    /// <summary>
    /// 验证是否为有效的 BASE64 图片
    /// </summary>
    private static bool IsValidBase64Image(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;

        return data.StartsWith("data:image/") && data.Contains("base64,");
    }

    /// <summary>
    /// 从 BASE64 数据中提取 MIME 类型和实际数据
    /// </summary>
    private static (string mimeType, string actualData) ExtractBase64Data(string base64Data)
    {
        // 格式：data:image/png;base64,iVBORw0KG...
        var parts = base64Data.Split(',');
        if (parts.Length != 2)
            return ("image/png", base64Data);

        var header = parts[0]; // data:image/png;base64
        var data = parts[1];   // iVBORw0KG...

        // 提取 MIME 类型
        var mimeType = "image/png";
        if (header.Contains(':') && header.Contains(';'))
        {
            var start = header.IndexOf(':') + 1;
            var end = header.IndexOf(';');
            mimeType = header.Substring(start, end - start);
        }

        return (mimeType, data);
    }

    #endregion
}
