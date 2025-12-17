using System.Text.Json;
using OpenBioCardServer.Models.DTOs;
using OpenBioCardServer.Models.Entities;
using OpenBioCardServer.Models.Enums;

namespace OpenBioCardServer.Utilities.Mappers;

public static class DataMapper
{
    // === Entity to DTO ===
    
    public static ProfileDto ToProfileDto(ProfileEntity profile)
    {
        var dto = new ProfileDto
        {
            Username = profile.Username,
            Avatar = ToAssetDto(profile.AvatarType, profile.AvatarText, profile.AvatarData),
            NickName = profile.NickName,
            Pronouns = profile.Pronouns,
            Description = profile.Description,
            Location = profile.Location,
            Website = profile.Website,
            Background = profile.BackgroundType.HasValue
                ? ToAssetDto(profile.BackgroundType.Value, profile.BackgroundText, profile.BackgroundData)
                : null,
            CurrentCompany = profile.CurrentCompany,
            CurrentCompanyLink = profile.CurrentCompanyLink,
            CurrentSchool = profile.CurrentSchool,
            CurrentSchoolLink = profile.CurrentSchoolLink,
            Contacts = profile.Contacts.Select(ToContactItemDto).ToList(),
            SocialLinks = profile.SocialLinks.Select(ToSocialLinkItemDto).ToList(),
            Projects = profile.Projects.Select(ToProjectItemDto).ToList(),
            WorkExperiences = profile.WorkExperiences.Select(ToWorkExperienceItemDto).ToList(),
            SchoolExperiences = profile.SchoolExperiences.Select(ToSchoolExperienceItemDto).ToList(),
            Gallery = profile.Gallery.Select(ToGalleryItemDto).ToList()
        };
        
        return dto;
    }

    public static AssetDto ToAssetDto(AssetType type, string? text, byte[]? data) => new()
    {
        Type = type,
        Text = text,
        DataBase64 = data != null ? Convert.ToBase64String(data) : null
    };

    public static ContactItemDto ToContactItemDto(ContactItemEntity entity) => new()
    {
        Type = entity.Type,
        Text = entity.Text,
        Image = entity.ImageType.HasValue
            ? ToAssetDto(entity.ImageType.Value, entity.ImageText, entity.ImageData)
            : null
    };

    public static SocialLinkItemDto ToSocialLinkItemDto(SocialLinkItemEntity entity)
    {
        var dto = new SocialLinkItemDto
        {
            Type = entity.Type,
            Value = entity.Value
        };

        if (!string.IsNullOrEmpty(entity.AttributesJson))
        {
            try
            {
                var attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.AttributesJson);
                if (attributes != null)
                {
                    dto.Attributes = attributes;
                }
            }
            catch { /* Ignore deserialization errors */ }
        }

        return dto;
    }

    public static ProjectItemDto ToProjectItemDto(ProjectItemEntity entity) => new()
    {
        Name = entity.Name,
        Url = entity.Url,
        Description = entity.Description,
        Logo = entity.LogoType.HasValue
            ? ToAssetDto(entity.LogoType.Value, entity.LogoText, entity.LogoData)
            : null
    };

    public static WorkExperienceItemDto ToWorkExperienceItemDto(WorkExperienceItemEntity entity) => new()
    {
        Company = entity.Company,
        CompanyUrl = entity.CompanyUrl,
        Position = entity.Position,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Description = entity.Description,
        Logo = entity.LogoType.HasValue
            ? ToAssetDto(entity.LogoType.Value, entity.LogoText, entity.LogoData)
            : null
    };

    public static SchoolExperienceItemDto ToSchoolExperienceItemDto(SchoolExperienceItemEntity entity) => new()
    {
        School = entity.School,
        SchoolLink = entity.SchoolLink,
        Degree = entity.Degree,
        Major = entity.Major,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Description = entity.Description,
        Logo = entity.LogoType.HasValue
            ? ToAssetDto(entity.LogoType.Value, entity.LogoText, entity.LogoData)
            : null
    };

    public static GalleryItemDto ToGalleryItemDto(GalleryItemEntity entity) => new()
    {
        Image = entity.ImageType.HasValue
            ? ToAssetDto(entity.ImageType.Value, entity.ImageText, entity.ImageData)
            : null,
        Caption = entity.Caption
    };

    // === DTO to Entity ===
    
    public static void UpdateProfileEntity(ProfileEntity entity, ProfileDto dto)
    {
        entity.Username = dto.Username;
        entity.NickName = dto.NickName;
        entity.Pronouns = dto.Pronouns;
        entity.Description = dto.Description;
        entity.Location = dto.Location;
        entity.Website = dto.Website;
        entity.CurrentCompany = dto.CurrentCompany;
        entity.CurrentCompanyLink = dto.CurrentCompanyLink;
        entity.CurrentSchool = dto.CurrentSchool;
        entity.CurrentSchoolLink = dto.CurrentSchoolLink;

        UpdateAssetInEntity(dto.Avatar, 
            type => entity.AvatarType = type,
            text => entity.AvatarText = text,
            data => entity.AvatarData = data);

        if (dto.Background != null)
        {
            entity.BackgroundType = dto.Background.Type;
            entity.BackgroundText = dto.Background.Text;
            entity.BackgroundData = !string.IsNullOrEmpty(dto.Background.DataBase64)
                ? Convert.FromBase64String(dto.Background.DataBase64)
                : null;
        }
        else
        {
            entity.BackgroundType = null;
            entity.BackgroundText = null;
            entity.BackgroundData = null;
        }
    }

    private static void UpdateAssetInEntity(
        AssetDto asset,
        Action<AssetType> setType,
        Action<string?> setText,
        Action<byte[]?> setData)
    {
        setType(asset.Type);
        setText(asset.Text);
        setData(!string.IsNullOrEmpty(asset.DataBase64)
            ? Convert.FromBase64String(asset.DataBase64)
            : null);
    }

    public static ContactItemEntity ToContactItemEntity(ContactItemDto dto, Guid profileId)
    {
        var entity = new ContactItemEntity
        {
            ProfileId = profileId,
            Type = dto.Type,
            Text = dto.Text
        };

        if (dto.Image != null)
        {
            entity.ImageType = dto.Image.Type;
            entity.ImageText = dto.Image.Text;
            entity.ImageData = !string.IsNullOrEmpty(dto.Image.DataBase64)
                ? Convert.FromBase64String(dto.Image.DataBase64)
                : null;
        }

        return entity;
    }

    public static SocialLinkItemEntity ToSocialLinkItemEntity(SocialLinkItemDto dto, Guid profileId) => new()
    {
        ProfileId = profileId,
        Type = dto.Type,
        Value = dto.Value,
        AttributesJson = dto.Attributes.Any()
            ? JsonSerializer.Serialize(dto.Attributes)
            : null
    };

    public static ProjectItemEntity ToProjectItemEntity(ProjectItemDto dto, Guid profileId)
    {
        var entity = new ProjectItemEntity
        {
            ProfileId = profileId,
            Name = dto.Name,
            Url = dto.Url,
            Description = dto.Description
        };

        if (dto.Logo != null)
        {
            entity.LogoType = dto.Logo.Type;
            entity.LogoText = dto.Logo.Text;
            entity.LogoData = !string.IsNullOrEmpty(dto.Logo.DataBase64)
                ? Convert.FromBase64String(dto.Logo.DataBase64)
                : null;
        }

        return entity;
    }

    public static WorkExperienceItemEntity ToWorkExperienceItemEntity(WorkExperienceItemDto dto, Guid profileId)
    {
        var entity = new WorkExperienceItemEntity
        {
            ProfileId = profileId,
            Company = dto.Company,
            CompanyUrl = dto.CompanyUrl,
            Position = dto.Position,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description
        };

        if (dto.Logo != null)
        {
            entity.LogoType = dto.Logo.Type;
            entity.LogoText = dto.Logo.Text;
            entity.LogoData = !string.IsNullOrEmpty(dto.Logo.DataBase64)
                ? Convert.FromBase64String(dto.Logo.DataBase64)
                : null;
        }

        return entity;
    }

    public static SchoolExperienceItemEntity ToSchoolExperienceItemEntity(SchoolExperienceItemDto dto, Guid profileId)
    {
        var entity = new SchoolExperienceItemEntity
        {
            ProfileId = profileId,
            School = dto.School,
            SchoolLink = dto.SchoolLink,
            Degree = dto.Degree,
            Major = dto.Major,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description
        };

        if (dto.Logo != null)
        {
            entity.LogoType = dto.Logo.Type;
            entity.LogoText = dto.Logo.Text;
            entity.LogoData = !string.IsNullOrEmpty(dto.Logo.DataBase64)
                ? Convert.FromBase64String(dto.Logo.DataBase64)
                : null;
        }

        return entity;
    }

    public static GalleryItemEntity ToGalleryItemEntity(GalleryItemDto dto, Guid profileId)
    {
        var entity = new GalleryItemEntity
        {
            ProfileId = profileId,
            Caption = dto.Caption
        };

        if (dto.Image != null)
        {
            entity.ImageType = dto.Image.Type;
            entity.ImageText = dto.Image.Text;
            entity.ImageData = !string.IsNullOrEmpty(dto.Image.DataBase64)
                ? Convert.FromBase64String(dto.Image.DataBase64)
                : null;
        }

        return entity;
    }
}
