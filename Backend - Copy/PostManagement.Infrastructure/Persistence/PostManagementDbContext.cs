using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Infrastructure.Persistence;

public class PostManagementDbContext : DbContext
{
    public PostManagementDbContext(DbContextOptions<PostManagementDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostStatusHistory> PostStatusHistories => Set<PostStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<Comment>()
    .HasOne(c => c.Post)
    .WithMany(p => p.Comments)
    .HasForeignKey(c => c.PostId)
    .OnDelete(DeleteBehavior.Cascade);

        

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = UserRoleName.User, Description = "Normal user" },
            new Role { Id = 2, Name = UserRoleName.Admin, Description = "Admin user" },
            new Role { Id = 3, Name = UserRoleName.SuperAdmin, Description = "Super administrator" }
        );

        // Seed a single superadmin with the default reset password
        var superAdminId = Guid.Parse("6d0f3bc0-0d9a-4d32-9ea4-2c3b63c0cf41");
        const string defaultPassword = "ChangeMyPa$$word1";
        var defaultPasswordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(defaultPassword)));
        var seededAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = superAdminId,
            Username = "mehul_superadmin",
            Email = "mehul.superadmin@infinite.com",
            PasswordHash = defaultPasswordHash,
            IsActive = true,
            CreatedAt = seededAt,
            IsAutoDeactivated = false
        });

        modelBuilder.Entity<UserRole>().HasData(new UserRole
        {
            UserId = superAdminId,
            RoleId = 3
        });
        // Post → CreatedByUser (1-to-many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post → AssignedToUser (optional)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.AssignedToUser)
            .WithMany()
            .HasForeignKey(p => p.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
