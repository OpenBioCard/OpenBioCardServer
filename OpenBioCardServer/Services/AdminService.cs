using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models.Entities;
using OpenBioCardServer.Utilities;

namespace OpenBioCardServer.Services;

public class AdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    public async Task<List<object>> GetAllUsersAsync()
    {
        return await _context.UserAccounts
            .Select(u => new 
            { 
                username = u.Username, 
                type = u.Type 
            })
            .Cast<object>()
            .ToListAsync();
    }

    /// <summary>
    /// 创建新用户（管理员操作）
    /// </summary>
    public async Task<(bool success, string? token, string? error)> CreateUserAsync(
        string newUsername, 
        string password, 
        string type)
    {
        // 不允许创建 root 用户
        if (type == "root")
        {
            return (false, null, "Cannot create root user");
        }

        if (type != "user" && type != "admin")
        {
            return (false, null, "Invalid user type");
        }

        if (await _context.UserAccounts.AnyAsync(u => u.Username == newUsername))
        {
            return (false, null, "Username already exists");
        }

        var (hash, salt) = PasswordHasher.HashPassword(password);

        var account = new UserAccount
        {
            Username = newUsername,
            PasswordHash = hash,
            PasswordSalt = salt,
            Type = type,
            Token = Guid.NewGuid().ToString()
        };

        var profile = new UserProfile
        {
            UserId = account.Id,
            Name = newUsername
        };

        _context.UserAccounts.Add(account);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return (true, account.Token, null);
    }

    /// <summary>
    /// 删除用户（管理员操作）
    /// </summary>
    public async Task<(bool success, string? error)> DeleteUserAsync(
        string usernameToDelete, 
        string currentUsername)
    {
        if (usernameToDelete == currentUsername)
        {
            return (false, "Cannot delete yourself");
        }

        var account = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == usernameToDelete);
            
        if (account == null)
        {
            return (false, "User not found");
        }

        // 不允许删除 root 用户
        if (account.Type == "root")
        {
            return (false, "Cannot delete root user");
        }

        _context.UserAccounts.Remove(account);
        await _context.SaveChangesAsync();

        return (true, null);
    }
}
