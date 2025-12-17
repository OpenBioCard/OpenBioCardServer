using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models;
using OpenBioCardServer.Models.Entities;
using OpenBioCardServer.Utilities;

namespace OpenBioCardServer.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    public async Task<(bool success, string? token, string? error)> SignupAsync(
        string username, 
        string password, 
        string type)
    {
        // 不允许注册 root 类型用户
        if (type == "root")
        {
            return (false, null, "Cannot register root user");
        }

        // 验证用户类型
        if (type != "user" && type != "admin")
        {
            return (false, null, "Invalid user type");
        }

        // 检查用户名是否已存在
        if (await _context.UserAccounts.AnyAsync(u => u.Username == username))
        {
            return (false, null, "Username already exists");
        }

        var (hash, salt) = PasswordHasher.HashPassword(password);

        var account = new UserAccount
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Type = type,
            Token = Guid.NewGuid().ToString()
        };

        var profile = new UserProfile
        {
            UserId = account.Id,
            Name = username
        };

        _context.UserAccounts.Add(account);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return (true, account.Token, null);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<(bool success, string? token, string? error)> SigninAsync(
        string username, 
        string password)
    {
        var account = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == username);
            
        if (account == null)
        {
            return (false, null, "Invalid username or password");
        }

        // 验证密码
        if (!PasswordHasher.VerifyPassword(password, account.PasswordHash, account.PasswordSalt))
        {
            return (false, null, "Invalid username or password");
        }

        return (true, account.Token, null);
    }

    /// <summary>
    /// 验证 Token 并获取用户（返回 DTO）
    /// </summary>
    public async Task<User?> ValidateTokenAsync(string token)
    {
        var account = await _context.UserAccounts
            .Include(a => a.Profile)
            .FirstOrDefaultAsync(u => u.Token == token);

        if (account == null)
        {
            return null;
        }

        return UserMapper.ToDto(account, account.Profile);
    }

    /// <summary>
    /// 删除用户账号
    /// </summary>
    public async Task<bool> DeleteAccountAsync(string username)
    {
        var account = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == username);
            
        if (account == null)
        {
            return false;
        }

        // 不允许删除 root 用户
        if (account.Type == "root")
        {
            return false;
        }

        _context.UserAccounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
}
