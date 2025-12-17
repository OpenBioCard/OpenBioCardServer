using Microsoft.EntityFrameworkCore;
using OpenBioCardServer.Models.Entities;

namespace OpenBioCardServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 UserAccount
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("UserAccounts");
            entity.HasIndex(u => u.Username).IsUnique();
            
            // 一对一关系配置
            entity.HasOne(a => a.Profile)
                .WithOne(p => p.Account)
                .HasForeignKey<UserProfile>(p => p.UserId)
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
        });
    }
}