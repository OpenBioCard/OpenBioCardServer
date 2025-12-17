using OpenBioCardServer.Models;
using OpenBioCardServer.Models.Entities;

namespace OpenBioCardServer.Utilities;

/// <summary>
/// User DTO 和 Entities 之间的映射工具
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// 将 Account 和 Profile 实体转换为 User DTO
    /// </summary>
    public static User ToDto(UserAccount account, UserProfile profile)
    {
        return new User
        {
            Id = account.Id,
            Username = account.Username,
            Type = account.Type,
            CreatedAt = account.CreatedAt,
            
            // Profile fields
            Name = profile.Name,
            Pronouns = profile.Pronouns,
            Avatar = profile.Avatar,
            Bio = profile.Bio,
            Location = profile.Location,
            Website = profile.Website,
            Background = profile.Background,
            
            CurrentCompany = profile.CurrentCompany,
            CurrentCompanyLink = profile.CurrentCompanyLink,
            CurrentSchool = profile.CurrentSchool,
            CurrentSchoolLink = profile.CurrentSchoolLink,
            
            Contacts = profile.Contacts,
            SocialLinks = profile.SocialLinks,
            Projects = profile.Projects,
            Gallery = profile.Gallery,
            
            WorkExperiences = profile.WorkExperiences,
            SchoolExperiences = profile.SchoolExperiences
        };
    }

    /// <summary>
    /// 从 User DTO 更新 Profile 实体
    /// </summary>
    public static void UpdateProfileFromDto(UserProfile profile, User dto)
    {
        profile.Name = dto.Name;
        profile.Pronouns = dto.Pronouns;
        profile.Avatar = dto.Avatar;
        profile.Bio = dto.Bio;
        profile.Location = dto.Location;
        profile.Website = dto.Website;
        profile.Background = dto.Background;
        
        profile.CurrentCompany = dto.CurrentCompany;
        profile.CurrentCompanyLink = dto.CurrentCompanyLink;
        profile.CurrentSchool = dto.CurrentSchool;
        profile.CurrentSchoolLink = dto.CurrentSchoolLink;
        
        profile.Contacts = dto.Contacts;
        profile.SocialLinks = dto.SocialLinks;
        profile.Projects = dto.Projects;
        profile.Gallery = dto.Gallery;
        
        profile.WorkExperiences = dto.WorkExperiences;
        profile.SchoolExperiences = dto.SchoolExperiences;
    }

    /// <summary>
    /// 创建新的 UserProfile
    /// </summary>
    public static UserProfile CreateProfile(Guid userId, User dto)
    {
        return new UserProfile
        {
            UserId = userId,
            Name = dto.Name,
            Pronouns = dto.Pronouns,
            Avatar = dto.Avatar,
            Bio = dto.Bio,
            Location = dto.Location,
            Website = dto.Website,
            Background = dto.Background,
            
            CurrentCompany = dto.CurrentCompany,
            CurrentCompanyLink = dto.CurrentCompanyLink,
            CurrentSchool = dto.CurrentSchool,
            CurrentSchoolLink = dto.CurrentSchoolLink,
            
            Contacts = dto.Contacts,
            SocialLinks = dto.SocialLinks,
            Projects = dto.Projects,
            Gallery = dto.Gallery,
            
            WorkExperiences = dto.WorkExperiences,
            SchoolExperiences = dto.SchoolExperiences
        };
    }
}
