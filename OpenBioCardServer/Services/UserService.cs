using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Data;
using OpenBioCardServer.Models;

namespace OpenBioCardServer.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserProfileAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> UpdateUserProfileAsync(string username, User updatedProfile)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return false;
        }

        // 更新资料字段
        user.Name = updatedProfile.Name;
        user.Pronouns = updatedProfile.Pronouns;
        user.Avatar = updatedProfile.Avatar;
        user.Bio = updatedProfile.Bio;
        user.Location = updatedProfile.Location;
        user.Website = updatedProfile.Website;
        user.Background = updatedProfile.Background;
        
        user.CurrentCompany = updatedProfile.CurrentCompany;
        user.CurrentCompanyLink = updatedProfile.CurrentCompanyLink;
        user.CurrentSchool = updatedProfile.CurrentSchool;
        user.CurrentSchoolLink = updatedProfile.CurrentSchoolLink;
        
        user.Contacts = updatedProfile.Contacts;
        user.SocialLinks = updatedProfile.SocialLinks;
        user.Projects = updatedProfile.Projects;
        user.Gallery = updatedProfile.Gallery;

        await _context.SaveChangesAsync();
        return true;
    }
}