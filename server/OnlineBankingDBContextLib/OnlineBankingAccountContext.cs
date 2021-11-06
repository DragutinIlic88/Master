using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;

namespace OnlineBankingDBContextLib
{
	public class OnlineBankingAccountContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }

		public OnlineBankingAccountContext(DbContextOptions<OnlineBankingAccountContext> options) :base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Account>()
				.HasKey(a => a.Iban);

			modelBuilder.Entity<Account>()
				.Property(a => a.Iban)
				.IsRequired()
				.HasMaxLength(34);

			modelBuilder.Entity<Account>()
				.Property(a => a.AccountNumber)
				.IsRequired()
				.HasMaxLength(30);

			modelBuilder.Entity<Account>()
				.Property(a => a.UserId)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<Account>()
				.Property(a => a.BankIdentifierCode)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<Account>()
				.Property(a => a.CreationDate)
				.IsRequired()
				.HasColumnType("datetime");

			modelBuilder.Entity<Account>()
				.Property(a => a.Currency)
				.IsRequired()
				.HasMaxLength(3);

			modelBuilder.Entity<Account>()
				.Property(a => a.Amount)
				.IsRequired()
				.HasColumnType("decimal");

			modelBuilder.Entity<Account>()
				.Property(a => a.InstitutionName)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<Account>()
				.Property(a => a.CountryCode)
				.IsRequired()
				.HasMaxLength(3);

			modelBuilder.Entity<Account>()
				.Property(a => a.AccountType)
				.IsRequired()
				.HasConversion<string>();
		}
	}
}
