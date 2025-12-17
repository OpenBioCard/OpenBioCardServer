using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models;
using OpenBioCardServer.Utilities;

namespace OpenBioCardServer.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly MediaAssetService _mediaAssetService;

    public UserService(
        AppDbContext context,
        MediaAssetService mediaAssetService)
    {
        _context = context;
        _mediaAssetService = mediaAssetService;
    }

    /// <summary>
    /// 获取用户公开资料
    /// 自动还原所有图片引用为 BASE64 格式（保持前端兼容性）
    /// </summary>
    public async Task<User?> GetUserProfileAsync(string username)
    {
        var account = await _context.UserAccounts
            .Include(a => a.Profile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (account == null)
        {
            return null;
        }

        // 还原所有图片引用为 BASE64
        await _mediaAssetService.RestoreProfileImagesAsync(account.Profile);

        return UserMapper.ToDto(account, account.Profile);
    }

    /// <summary>
    /// 更新用户资料
    /// 自动将前端提交的 BASE64 图片存储到 MediaAsset 表
    /// 并删除不再使用的旧图片资源
    /// </summary>
    public async Task<bool> UpdateUserProfileAsync(string username, User updatedProfile)
    {
        var account = await _context.UserAccounts
            .Include(a => a.Profile)
            .FirstOrDefaultAsync(u => u.Username == username);
            
        if (account == null)
        {
            return false;
        }

        // 1. 收集更新前的所有资产引用
        var oldAssetIds = _mediaAssetService.ExtractAllAssetReferences(account.Profile);

        // 2. 更新 Profile 数据
        UserMapper.UpdateProfileFromDto(account.Profile, updatedProfile);

        // 3. 处理所有图片：将 BASE64 转存到 MediaAsset 表
        await _mediaAssetService.ProcessProfileImagesAsync(account.Id, account.Profile);

        // 4. 收集更新后的所有资产引用
        var newAssetIds = _mediaAssetService.ExtractAllAssetReferences(account.Profile);

        // 5. 删除不再使用的资产
        var unusedAssetIds = oldAssetIds.Except(newAssetIds).ToList();
        foreach (var assetId in unusedAssetIds)
        {
            await _mediaAssetService.DeleteMediaAssetAsync(assetId);
        }

        // 6. 统一保存所有更改（包括 Profile 更新、新 MediaAsset、删除的 MediaAsset）
        await _context.SaveChangesAsync();
        
        return true;
    }
}
