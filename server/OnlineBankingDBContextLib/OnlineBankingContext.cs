using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;


namespace OnlineBankingDBContextLib
{

	public class OnlineBankingContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<UserToken> UserTokens { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Role> Roles { get; set; }

		public OnlineBankingContext(DbContextOptions<OnlineBankingContext> options) : base(options) 
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//users table configuration
			modelBuilder.Entity<User>()
				.HasKey(u => u.UserID);

			modelBuilder.Entity<User>()
				.Property(u => u.UserID)
				.IsRequired()
				.HasColumnName("Id")
				.HasMaxLength(450);

			modelBuilder.Entity<User>()
				.Property(u => u.UserName)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<User>()
				.Property(u => u.NormalizedUserName)
				.IsRequired()
				.HasMaxLength(256)
				.HasComputedColumnSql("upper([UserName])");

			modelBuilder.Entity<User>()
				.Property(u => u.Email)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.HasDatabaseName("Index_Email")
				.IsUnique();

			modelBuilder.Entity<User>()
				.Property(u => u.NormalizedEmail)
				.IsRequired()
				.HasMaxLength(256)
				.HasComputedColumnSql("upper([Email])");

			modelBuilder.Entity<User>()
				.Property(u => u.EmailConfirmed)
				.IsRequired()
				.HasDefaultValue(false);

			modelBuilder.Entity<User>()
				.Property(u => u.PasswordHash)
				.IsRequired()
				.HasColumnType("nvarchar(max)");

			modelBuilder.Entity<User>()
				.Property(u => u.MobileNumber);

			modelBuilder.Entity<User>()
				.Property(u => u.HomePhoneNumber);

			modelBuilder.Entity<User>()
				.Property(u => u.HomeAddress);

			modelBuilder.Entity<User>()
				.Property(u => u.ProfileImagePath);

			modelBuilder.Entity<User>()
				.Property(u => u.TwoFactorEnabled)
				.IsRequired()
				.HasDefaultValue(false);

			modelBuilder.Entity<User>()
				.Property(u => u.RegistrationDate)
				.IsRequired()
				.HasDefaultValueSql("CONVERT(date,GETDATE())");

			modelBuilder.Entity<User>()
				.Property(u => u.LastModifiedDate)
				.IsRequired();

			modelBuilder.Entity<User>()
				.HasOne(u => u.Token)
				.WithOne(ut => ut.User);

			//Roles table configuration
			modelBuilder.Entity<Role>()
				.HasKey(r => r.RoleID);

			modelBuilder.Entity<Role>()
				.Property(r => r.RoleID)
				.HasColumnName("Id")
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<Role>()
				.Property(r => r.Name)
				.HasMaxLength(256)
				.IsRequired();

			modelBuilder.Entity<Role>()
				.Property(r=>r.NormalizedName)
				.IsRequired()
				.HasMaxLength(256)
				.HasComputedColumnSql("upper([Name])");

			modelBuilder.Entity<Role>()
				.Property(r => r.CreationDate)
				.IsRequired()
				.HasDefaultValue("CONVERT(date,GETDATE())");

			//UserTokens table configuration
			modelBuilder.Entity<UserToken>()
				.HasKey(ut => ut.UserID);

			modelBuilder.Entity<UserToken>()
				.HasIndex(ut => ut.TokenValue)
				.IsUnique();

			modelBuilder.Entity<UserToken>()
				.Property(ut => ut.UserID)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<UserToken>()
				.Property(ut => ut.TokenValue)
				.HasColumnName("Value")
				.IsRequired();


			modelBuilder.Entity<UserToken>()
				.Property(ut => ut.TokenType)
				.HasColumnName("Type");

			modelBuilder.Entity<UserToken>()
				.Property(ut => ut.TokenGenerationTime)
				.HasColumnName("GenerationTime")
				.IsRequired();


			modelBuilder.Entity<UserToken>()
				.Property(ut => ut.TokenExpirationTime)
				.HasColumnName("ExpirationTime")
				.IsRequired();

			//UserRole table configuration; used for many-to-many relationship
			modelBuilder.Entity<UserRole>()
				.HasKey(ur => new { ur.UserID, ur.RoleID });

			modelBuilder.Entity<UserRole>()
				.Property(ur => ur.UserID)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<UserRole>()
				.Property(ur => ur.RoleID)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.User)
				.WithMany(u => u.UserRoles)
				.HasForeignKey(ur => ur.UserID);

			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.Role)
				.WithMany(r => r.UserRoles)
				.HasForeignKey(ur => ur.RoleID);



		}

	}
}
