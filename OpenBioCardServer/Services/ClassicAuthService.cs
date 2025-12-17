using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models.Entities;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Services;

public class ClassicAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<ClassicAuthService> _logger;

    public ClassicAuthService(
        AppDbContext context, 
        IConfiguration config,
        ILogger<ClassicAuthService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<(bool isValid, Account? account)> ValidateTokenAsync(string token)
    {
        // Check if it's a root token
        if (token.StartsWith("root-"))
        {
            // For root, we don't store tokens in DB, just verify format and return root account
            var rootAccount = await GetRootAccountAsync();
            return (rootAccount != null, rootAccount);
        }

        var tokenEntity = await _context.Tokens
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.TokenValue == token);

        if (tokenEntity == null)
            return (false, null);

        // Update last used timestamp
        tokenEntity.LastUsed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, tokenEntity.Account);
    }

    public async Task<Account?> GetRootAccountAsync()
    {
        var rootUsername = _config["AuthSettings:RootUsername"];
        if (string.IsNullOrEmpty(rootUsername))
            return null;

        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserName == rootUsername && a.Type == UserType.Root);
    }

    public async Task<string> CreateTokenAsync(Account account)
    {
        string tokenValue;
        
        if (account.Type == UserType.Root)
        {
            // Root tokens have special format and are not stored in DB
            tokenValue = $"root-{Guid.NewGuid()}";
        }
        else
        {
            tokenValue = Guid.NewGuid().ToString();
            
            var token = new Token
            {
                TokenValue = tokenValue,
                AccountId = account.Id
            };

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
        }

        return tokenValue;
    }

    public async Task<bool> HasAdminPermissionAsync(Account account) =>
        account.Type == UserType.Admin || account.Type == UserType.Root;
}
