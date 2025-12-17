using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models;
using OpenBioCardServer.Utilities;

namespace OpenBioCardServer.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取用户公开资料
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

        return UserMapper.ToDto(account, account.Profile);
    }

    /// <summary>
    /// 更新用户资料
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

        UserMapper.UpdateProfileFromDto(account.Profile, updatedProfile);

        await _context.SaveChangesAsync();
        return true;
    }
}