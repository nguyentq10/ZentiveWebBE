using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Models;
using System;
using System.Collections.Generic;

namespace DAL.DBcontext;

public partial class ZenthicDBContext : DbContext
{
    public ZenthicDBContext()
    {
    }

    public ZenthicDBContext(DbContextOptions<ZenthicDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<MediaAsset> MediaAssets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PayoutAccount> PayoutAccounts { get; set; }

    public virtual DbSet<Pledge> Pledges { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectApproval> ProjectApprovals { get; set; }

    public virtual DbSet<RewardTier> RewardTiers { get; set; }

    public virtual DbSet<SiteDonation> SiteDonations { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ZenthicDB;Persist Security Info=True;User ID=sa;Password=12345;Encrypt=False");
    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC07B1FE1720");

            entity.HasIndex(e => e.Email, "UQ__Accounts__A9D10534B8E27D25").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Backer");
            entity.Property(e => e.School).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07E461AC58");

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB65679812A").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MediaAss__3214EC07694C6141");

            entity.HasIndex(e => new { e.ProjectId, e.SortOrder }, "UX_MediaAssets_Project_Sort").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Image");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(d => d.Project).WithMany(p => p.MediaAssets)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaAssets_Project");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC076DA50C66");

            entity.HasIndex(e => e.Status, "IX_Payments_Status");

            entity.HasIndex(e => new { e.Provider, e.ExternalId }, "UX_Payments_Provider_ExternalId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("VND")
                .IsFixedLength();
            entity.Property(e => e.ExternalId).HasMaxLength(100);
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.Property(e => e.Provider)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<PayoutAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayoutAc__3214EC07509681C8");

            entity.HasIndex(e => new { e.UserId, e.Provider, e.AccountNumber }, "UQ_PayoutAccounts_User_Provider_Acc").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountName).HasMaxLength(150);
            entity.Property(e => e.AccountNumber)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.BankName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Provider)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.PayoutAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PayoutAccounts_User");
        });

        modelBuilder.Entity<Pledge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pledges__3214EC078140ACDD");

            entity.HasIndex(e => new { e.ProjectId, e.Status }, "IX_Pledges_Project_Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Backer).WithMany(p => p.Pledges)
                .HasForeignKey(d => d.BackerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pledges_Backer");

            entity.HasOne(d => d.Payment).WithMany(p => p.Pledges)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pledges_Payment");

            entity.HasOne(d => d.Project).WithMany(p => p.Pledges)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pledges_Project");

            entity.HasOne(d => d.RewardTier).WithMany(p => p.Pledges)
                .HasForeignKey(d => d.RewardTierId)
                .HasConstraintName("FK_Pledges_Reward");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC0708C56CDA");

            entity.HasIndex(e => new { e.Status, e.CategoryId, e.EndAt }, "IX_Projects_Status_Category_EndAt");

            entity.HasIndex(e => e.Slug, "UQ__Projects__BC7B5FB6D1CC442A").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndAt).HasColumnType("datetime");
            entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MediaCoverUrl).HasMaxLength(500);
            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.StartAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Category");

            entity.HasOne(d => d.Creator).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Creator");
        });

        modelBuilder.Entity<ProjectApproval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectA__3214EC075CF95D09");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DecidedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Decision)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.ProjectApprovals)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectApprovals_Admin");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectApprovals)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectApprovals_Project");
        });

        modelBuilder.Entity<RewardTier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RewardTi__3214EC07473CAB15");

            entity.HasIndex(e => new { e.ProjectId, e.Title }, "UQ_RewardTiers_Project_Title").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.RewardTiers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RewardTiers_Project");
        });

        modelBuilder.Entity<SiteDonation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SiteDona__3214EC07680E3A1F");

            entity.HasIndex(e => e.Status, "IX_SiteDonations_Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(300);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Donor).WithMany(p => p.SiteDonations)
                .HasForeignKey(d => d.DonorId)
                .HasConstraintName("FK_SiteDonations_Donor");

            entity.HasOne(d => d.Payment).WithMany(p => p.SiteDonations)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SiteDonations_Payment");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}