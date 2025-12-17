using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Models.Entities;

namespace OpenBioCardServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<MediaAsset> MediaAssets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 UserAccount
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("UserAccounts");
            entity.HasIndex(u => u.Username).IsUnique();
            
            // 一对一关系：Account <-> Profile
            entity.HasOne(a => a.Profile)
                .WithOne(p => p.Account)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // 一对多关系：Account -> MediaAssets
            entity.HasMany(a => a.MediaAssets)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 UserProfile
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfiles");
            
            // 配置 JSON 列
            entity.OwnsMany(p => p.Contacts, b => b.ToJson());
            
            // SocialLinks 需要特殊配置，因为包含嵌套对象
            entity.OwnsMany(p => p.SocialLinks, nav =>
            {
                nav.ToJson();
                nav.OwnsOne(s => s.GithubData);
            });
            
            entity.OwnsMany(p => p.Projects, b => b.ToJson());
            entity.OwnsMany(p => p.Gallery, b => b.ToJson());
            
            entity.OwnsMany(p => p.WorkExperiences, b => b.ToJson());
            entity.OwnsMany(p => p.Educations, b => b.ToJson());
        });

        // 配置 MediaAsset
        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.ToTable("MediaAssets");
            entity.HasIndex(m => m.UserId);
            entity.HasIndex(m => m.Type);
            entity.HasIndex(m => m.CreatedAt);
        });
    }
}
