using BankUsersDBEntitiesLib;
using Microsoft.EntityFrameworkCore;
using System;

namespace BankUsersDBContextLib
{
	public class BankUsersContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<UserBankIds> UserBankIds { get; set; }

		public BankUsersContext(DbContextOptions<BankUsersContext> options) :base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.HasKey(u => u.UniquePersonalIdentificationNumber);

			modelBuilder.Entity<User>()
				.HasMany(u => u.UserBanksIds)
				.WithOne(ub => ub.User);


			modelBuilder.Entity<User>()
				.Property(u => u.UniquePersonalIdentificationNumber)
				.IsRequired()
				.HasColumnName("UPIN")
				.HasMaxLength(450);

			modelBuilder.Entity<User>()
				.Property(u => u.FirstName)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<User>()
				.Property(u => u.LastName)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<User>()
				.Property(u => u.MobileNumber)
				.IsRequired();

			modelBuilder.Entity<User>()
				.Property(u => u.Email);

			modelBuilder.Entity<User>()
				.HasData(new User { UniquePersonalIdentificationNumber = "0410988710273", FirstName = "Dragutin", LastName = "Ilic", Email = "dragutinilic88@gmail.com", MobileNumber = "0645869478" },
				new User { UniquePersonalIdentificationNumber="2312991720274", FirstName="Natalija" , LastName="Ilic" ,Email="natalijailic91@gmail.com",MobileNumber="0698877661"},
				new User { UniquePersonalIdentificationNumber = "1122333445555", FirstName = "Test", LastName = "Testic", Email = "test1234@test.com", MobileNumber = "1122334455" });

			modelBuilder.Entity<UserBankIds>()
				.HasKey(ub => ub.BankId);

			modelBuilder.Entity<UserBankIds>()
				.HasOne(ub => ub.User)
				.WithMany(u => u.UserBanksIds)
				.HasForeignKey(ub => ub.UniquePersonalIdentificationNumber);

			modelBuilder.Entity<UserBankIds>()
				.Property(ub => ub.BankId)
				.IsRequired()
				.HasMaxLength(18);

			modelBuilder.Entity<UserBankIds>()
				.Property(ub => ub.UniquePersonalIdentificationNumber)
				.IsRequired()
				.HasColumnName("UPIN")
				.HasMaxLength(450);

			modelBuilder.Entity<UserBankIds>()
				.HasData(new UserBankIds { BankId = "112233445566", UniquePersonalIdentificationNumber = "0410988710273" },
				new UserBankIds { BankId="111222333444555666", UniquePersonalIdentificationNumber = "0410988710273" },
				new UserBankIds { BankId = "555555555555", UniquePersonalIdentificationNumber= "2312991720274" },
				new UserBankIds { BankId = "666666666668", UniquePersonalIdentificationNumber = "2312991720274" },
				new UserBankIds { BankId = "777777777777", UniquePersonalIdentificationNumber = "2312991720274" },
				new UserBankIds { BankId = "666666666666", UniquePersonalIdentificationNumber = "2312991720274" },
				new UserBankIds { BankId = "666666666666778899", UniquePersonalIdentificationNumber = "2312991720274" },
				new UserBankIds { BankId = "555666666666778899", UniquePersonalIdentificationNumber = "1122333445555" },
				new UserBankIds { BankId = "555666666666", UniquePersonalIdentificationNumber = "1122333445555" }
				);
		}
	}
}
