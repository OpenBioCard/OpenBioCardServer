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
}
