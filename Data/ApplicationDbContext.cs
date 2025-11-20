using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // === CUSTOM DB SETS ===
    public DbSet<User> CustomUsers { get; set; }
    public DbSet<GameLevel> GameLevels { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<LevelResult> LevelResults { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Role> CustomRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // REGION
        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.regionId);
            entity.Property(e => e.Name).IsRequired();
        });

        // ROLE
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.roleId);
            entity.Property(e => e.Name).IsRequired();
        });

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.userId);
            entity.Property(e => e.username).IsRequired();
            entity.Property(e => e.linkAvatar);
            entity.Property(e => e.otp);

            entity.HasOne(e => e.region)
                .WithMany(r => r.Users)
                .HasForeignKey("regionId")
                .IsRequired();

            entity.HasOne(e => e.role)
                .WithMany(r => r.Users)
                .HasForeignKey("roleId")
                .IsRequired();
        });

        // GAME LEVEL
        modelBuilder.Entity<GameLevel>(entity =>
        {
            entity.HasKey(e => e.levelId);
            entity.Property(e => e.title).IsRequired();
            entity.Property(e => e.description).IsRequired();
        });

        // QUESTION
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.questionId);

            entity.Property(e => e.contentQuestion).IsRequired();
            entity.Property(e => e.answer).IsRequired();
            entity.Property(e => e.option1).IsRequired();
            entity.Property(e => e.option2).IsRequired();
            entity.Property(e => e.option3).IsRequired();
            entity.Property(e => e.option4).IsRequired();

            entity.HasOne(q => q.gameLevel)
                .WithMany(gl => gl.questions)
                .HasForeignKey(q => q.levelId)
                .IsRequired();
        });

        // LEVEL RESULT
        modelBuilder.Entity<LevelResult>(entity =>
        {
            entity.HasKey(e => e.quizResultId);

            entity.Property(e => e.score).IsRequired();
            entity.Property(e => e.completionDate).IsRequired();

            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.userId)
                .IsRequired();

            entity.HasOne(e => e.gameLevel)
                .WithMany()
                .HasForeignKey(e => e.levelId)
                .IsRequired();
        });

        // SEED DATA
        modelBuilder.Entity<Region>().HasData(
            new Region(1, "Region1"),
            new Region(2, "Region2")
        );

        modelBuilder.Entity<Role>().HasData(
            new Role(1, "Admin"),
            new Role(2, "User")
        );

        modelBuilder.Entity<User>().HasData(
            new { userId = 1, username = "user1", linkAvatar = "avatar1.png", otp = 123456, regionId = 1, roleId = 1 },
            new { userId = 2, username = "user2", linkAvatar = "avatar2.png", otp = 654321, regionId = 2, roleId = 2 }
        );
    }
}
