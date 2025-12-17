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

        // 更新 Profile 数据
        UserMapper.UpdateProfileFromDto(account.Profile, updatedProfile);

        // 处理所有图片：将 BASE64 转存到 MediaAsset 表
        await _mediaAssetService.ProcessProfileImagesAsync(account.Id, account.Profile);

        await _context.SaveChangesAsync();
        return true;
    }
}