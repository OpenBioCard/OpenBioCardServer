using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public ClassicUserController(
        AppDbContext context,
        ClassicAuthService authService,
        ILogger<ClassicUserController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get user profile (public endpoint)
    /// </summary>
    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        try
        {
            var profile = await _context.Profiles
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
                .Include(p => p.Contacts)
                .Include(p => p.SocialLinks)
                .Include(p => p.Projects)
                .Include(p => p.WorkExperiences)
                .Include(p => p.SchoolExperiences)
                .Include(p => p.Gallery)
                .FirstOrDefaultAsync(p => p.Username == username);

            if (profile == null)
            {
                return NotFound(new { error = "Profile not found" });
            }

            // Update basic profile fields
            ClassicMapper.UpdateProfileFromClassic(profile, request);

            // Clear all existing collections (we'll replace them completely)
            _context.ContactItems.RemoveRange(profile.Contacts);
            _context.SocialLinkItems.RemoveRange(profile.SocialLinks);
            _context.ProjectItems.RemoveRange(profile.Projects);
            _context.WorkExperienceItems.RemoveRange(profile.WorkExperiences);
            _context.SchoolExperienceItems.RemoveRange(profile.SchoolExperiences);
            _context.GalleryItems.RemoveRange(profile.Gallery);

            // Add new collections from request
            if (request.Contacts?.Any() == true)
            {
                var contacts = ClassicMapper.ToContactEntities(request.Contacts, profile.Id);
                _context.ContactItems.AddRange(contacts);
            }

            if (request.SocialLinks?.Any() == true)
            {
                var socialLinks = ClassicMapper.ToSocialLinkEntities(request.SocialLinks, profile.Id);
                _context.SocialLinkItems.AddRange(socialLinks);
            }

            if (request.Projects?.Any() == true)
            {
                var projects = ClassicMapper.ToProjectEntities(request.Projects, profile.Id);
                _context.ProjectItems.AddRange(projects);
            }

            if (request.WorkExperiences?.Any() == true)
            {
                var workExperiences = ClassicMapper.ToWorkExperienceEntities(request.WorkExperiences, profile.Id);
                _context.WorkExperienceItems.AddRange(workExperiences);
            }

            if (request.SchoolExperiences?.Any() == true)
            {
                var schoolExperiences = ClassicMapper.ToSchoolExperienceEntities(request.SchoolExperiences, profile.Id);
                _context.SchoolExperienceItems.AddRange(schoolExperiences);
            }

            if (request.Gallery?.Any() == true)
            {
                var gallery = ClassicMapper.ToGalleryEntities(request.Gallery, profile.Id);
                _context.GalleryItems.AddRange(gallery);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Profile updated for user: {Username}", username);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user: {Username}", username);
            return StatusCode(500, new { error = "Profile update failed" });
        }
    }
}
