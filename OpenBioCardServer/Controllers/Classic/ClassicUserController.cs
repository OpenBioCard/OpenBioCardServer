using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models.DTOs.Classic;
using OpenBioCardServer.Services;
using OpenBioCardServer.Utilities.Mappers;

namespace OpenBioCardServer.Controllers.Classic;

[Route("classic/user")]
[ApiController]
public class ClassicUserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ClassicAuthService _authService;
    private readonly ILogger<ClassicUserController> _logger;
    
    // 缓存相关依赖
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    private readonly IConfiguration _configuration;
    
    // 缓存配置字段
    private readonly bool _useRedis;
    private readonly int _expirationMinutes;

    public ClassicUserController(
        AppDbContext context,
        ClassicAuthService authService,
        ILogger<ClassicUserController> logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
        _memoryCache = memoryCache;
        _configuration = configuration;
        
        // 读取缓存配置
        _useRedis = configuration.GetValue<bool>("CacheSettings:UseRedis");
        _expirationMinutes = configuration.GetValue<int>("CacheSettings:ExpirationMinutes", 5);
        
        // 如果启用了 Redis，尝试获取 IDistributedCache 服务
        if (_useRedis)
        {
            _distributedCache = serviceProvider.GetService<IDistributedCache>();
        }
    }
    
    // 生成统一的 Cache Key
    private static string GetProfileCacheKey(string username) => 
        $"Profile:{username.Trim().ToLowerInvariant()}";

    /// <summary>
    /// Get user profile (public endpoint)
    /// </summary>
    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        string cacheKey = GetProfileCacheKey(username);
        ClassicProfile? cachedProfile = null;
        
        // 读取缓存
        try
        {
            if (_useRedis && _distributedCache != null)
            {
                // Redis: 读取字符串并反序列化
                var jsonStr = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    cachedProfile = JsonSerializer.Deserialize<ClassicProfile>(jsonStr);
                }
            }
            else
            {
                // Memory: 直接读取对象引用
                _memoryCache.TryGetValue(cacheKey, out cachedProfile);
            }
            if (cachedProfile != null)
            {
                return Ok(cachedProfile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache read failed for {Key}", cacheKey);
        }

        
        try
        {
            var profile = await _context.Profiles
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Contacts)
                .Include(p => p.SocialLinks)
                .Include(p => p.Projects)
                .Include(p => p.WorkExperiences)
                .Include(p => p.SchoolExperiences)
                .Include(p => p.Gallery)
                .FirstOrDefaultAsync(p => p.Username == username);

            if (profile == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var classicProfile = ClassicMapper.ToClassicProfile(profile);
            
            // 写入缓存
            try 
            {
                if (_useRedis && _distributedCache != null)
                {
                    // Redis: 序列化为 JSON 存储
                    var jsonStr = JsonSerializer.Serialize(classicProfile);
                    await _distributedCache.SetStringAsync(cacheKey, jsonStr, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
                    });
                }
                else
                {
                    // Memory: 存储对象引用 (带 Size 限制)
                    _memoryCache.Set(cacheKey, classicProfile, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_expirationMinutes))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                        .SetSize(1));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache write failed for {Key}", cacheKey);
            }

            return Ok(classicProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user: {Username}", username);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Update user profile (requires authentication)
    /// </summary>
    [HttpPost("{username}")]
    public async Task<IActionResult> UpdateProfile(string username, [FromBody] ClassicProfile request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Extract token from Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { error = "Missing authentication token" });
            }

            var (isValid, account) = await _authService.ValidateTokenAsync(token);

            if (!isValid || account == null)
            {
                return Unauthorized(new { error = "Invalid token" });
            }

            if (account.UserName != username)
            {
                return Unauthorized(new { error = "Token does not match username" });
            }

            var profile = await _context.Profiles
                .AsTracking()
                .FirstOrDefaultAsync(p => p.Username == username);

            if (profile == null)
            {
                return NotFound(new { error = "Profile not found" });
            }

            // Update basic profile fields
            ClassicMapper.UpdateProfileFromClassic(profile, request);
            
            // Clear all existing collections using ExecuteDeleteAsync
            await _context.ContactItems
                .Where(c => c.ProfileId == profile.Id)
                .ExecuteDeleteAsync();
            
            await _context.SocialLinkItems
                .Where(s => s.ProfileId == profile.Id)
                .ExecuteDeleteAsync();
                
            await _context.ProjectItems
                .Where(p => p.ProfileId == profile.Id)
                .ExecuteDeleteAsync();
                
            await _context.WorkExperienceItems
                .Where(w => w.ProfileId == profile.Id)
                .ExecuteDeleteAsync();
                
            await _context.SchoolExperienceItems
                .Where(s => s.ProfileId == profile.Id)
                .ExecuteDeleteAsync();
                
            await _context.GalleryItems
                .Where(g => g.ProfileId == profile.Id)
                .ExecuteDeleteAsync();

            // Add new collections from request
            if (request.Contacts?.Any() == true)
            {
                var contacts = ClassicMapper.ToContactEntities(request.Contacts, profile.Id);
                await _context.ContactItems.AddRangeAsync(contacts);
            }

            if (request.SocialLinks?.Any() == true)
            {
                var socialLinks = ClassicMapper.ToSocialLinkEntities(request.SocialLinks, profile.Id);
                await _context.SocialLinkItems.AddRangeAsync(socialLinks);
            }

            if (request.Projects?.Any() == true)
            {
                var projects = ClassicMapper.ToProjectEntities(request.Projects, profile.Id);
                await _context.ProjectItems.AddRangeAsync(projects);
            }

            if (request.WorkExperiences?.Any() == true)
            {
                var workExperiences = ClassicMapper.ToWorkExperienceEntities(request.WorkExperiences, profile.Id);
                await _context.WorkExperienceItems.AddRangeAsync(workExperiences);
            }

            if (request.SchoolExperiences?.Any() == true)
            {
                var schoolExperiences = ClassicMapper.ToSchoolExperienceEntities(request.SchoolExperiences, profile.Id);
                await _context.SchoolExperienceItems.AddRangeAsync(schoolExperiences);
            }

            if (request.Gallery?.Any() == true)
            {
                var gallery = ClassicMapper.ToGalleryEntities(request.Gallery, profile.Id);
                await _context.GalleryItems.AddRangeAsync(gallery);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // 清除缓存
            string cacheKey = GetProfileCacheKey(username);
            try 
            {
                if (_useRedis && _distributedCache != null)
                {
                    await _distributedCache.RemoveAsync(cacheKey);
                }
                else
                {
                    _memoryCache.Remove(cacheKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache removal failed for {Key}", cacheKey);
            }

            _logger.LogInformation("Profile updated for user: {Username}", username);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating profile for user: {Username}", username);
            return StatusCode(500, new { error = "Profile update failed" });
        }
    }
}
