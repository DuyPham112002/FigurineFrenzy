using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBAccess.Entites;

public partial class FigurineFrenzyContext : DbContext
{
    public FigurineFrenzyContext()
    {
    }

    public FigurineFrenzyContext(DbContextOptions<FigurineFrenzyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Auction> Auctions { get; set; }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<ImageSet> ImageSets { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=FigurineFrenzy;User Id=Test;Password=test;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A607C2A26A");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Phone, "UQ__Account__5C7E359E446EDB61").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RoleId)
                .HasMaxLength(400)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__RoleId__628FA481");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admin__3214EC0798AE729D");

            entity.ToTable("Admin");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(400);

            entity.HasOne(d => d.Account).WithMany(p => p.Admins)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Admin__AccountId__71D1E811");
        });

        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasKey(e => e.AuctionId).HasName("PK__Auction__51004A4C4DA83BEB");

            entity.ToTable("Auction");

            entity.Property(e => e.AuctionId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.CategoryId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.OwnerId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(400);
            entity.Property(e => e.WinnerId)
                .HasMaxLength(400)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Auctions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Auction__Categor__6FB49575");

            entity.HasOne(d => d.Owner).WithMany(p => p.AuctionOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Auction__OwnerId__6EC0713C");

            entity.HasOne(d => d.Winner).WithMany(p => p.AuctionWinners)
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("FK__Auction__WinnerI__6CD828CA");
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bid__3214EC0767048B19");

            entity.ToTable("Bid");

            entity.Property(e => e.Id)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.AuctionId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.BidTime).HasColumnType("datetime");
            entity.Property(e => e.Bidder)
                .HasMaxLength(400)
                .IsUnicode(false);

            entity.HasOne(d => d.Auction).WithMany(p => p.Bids)
                .HasForeignKey(d => d.AuctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bid__AuctionId__72910220");

            entity.HasOne(d => d.BidderNavigation).WithMany(p => p.Bids)
                .HasForeignKey(d => d.Bidder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bid__Bidder__73852659");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC074D730CF8");

            entity.ToTable("Category");

            entity.Property(e => e.Id)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(400);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC07A6CC461F");

            entity.ToTable("Image");

            entity.Property(e => e.Id)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.ImageSetId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(400)
                .IsUnicode(false);

            entity.HasOne(d => d.ImageSet).WithMany(p => p.Images)
                .HasForeignKey(d => d.ImageSetId)
                .HasConstraintName("FK__Image__ImageSetI__10216507");
        });

        modelBuilder.Entity<ImageSet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImageSet__3214EC07171B7A4B");

            entity.ToTable("ImageSet");

            entity.Property(e => e.Id)
                .HasMaxLength(400)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Item__727E838B85727F2C");

            entity.ToTable("Item");

            entity.Property(e => e.ItemId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.AuctionId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageSetId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.NameOfProduct).HasMaxLength(400);

            entity.HasOne(d => d.Auction).WithMany(p => p.Items)
                .HasForeignKey(d => d.AuctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Auction1");

            entity.HasOne(d => d.ImageSet).WithMany(p => p.Items)
                .HasForeignKey(d => d.ImageSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Item__ImageSetId__11158940");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A16A6DED5");

            entity.Property(e => e.RoleId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(40);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Token__658FEEEA4977F402");

            entity.ToTable("Token");

            entity.Property(e => e.TokenId).HasMaxLength(400);
            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(400);

            entity.HasOne(d => d.Account).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Token__AccountId__6C190EBB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07BC39CAEA");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccountId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(400);
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(400);
            entity.Property(e => e.FullName).HasMaxLength(400);

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__User__AccountId__778AC167");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
