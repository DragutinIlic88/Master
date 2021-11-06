using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;


namespace OnlineBankingDBContextLib
{
	public class OnlineBankingCurrencyContext : DbContext
	{
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<UserCurrency> UserCurrencies { get; set; }
		public DbSet<ExchangeRate> ExchangeRates { get; set; }
		public OnlineBankingCurrencyContext(DbContextOptions<OnlineBankingCurrencyContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Currencies table configuration
			modelBuilder.Entity<Currency>()
				.HasKey(c => c.CurrencyId);

			modelBuilder.Entity<Currency>()
				.Property(c => c.CurrencyId)
				.IsRequired()
				.HasColumnName("Id")
				.HasMaxLength(450);

			modelBuilder.Entity<Currency>()
				.Property(c => c.Code)
				.IsRequired()
				.HasColumnName("Code")
				.HasMaxLength(3);

			modelBuilder.Entity<Currency>()
				.Property(c => c.Name)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<Currency>()
				.Property(c => c.Country)
				.IsRequired()
				.HasMaxLength(256);

			//UserCurrencies table configuration
			modelBuilder.Entity<UserCurrency>()
				.HasKey(uc => new { uc.UserId, uc.CurrencyId });

			modelBuilder.Entity<UserCurrency>()
				.Property(uc => uc.CurrencyId)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<UserCurrency>()
				.Property(uc => uc.UserId)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<UserCurrency>()
				.HasOne(uc => uc.Currency)
				.WithMany(c => c.UserCurrencies)
				.HasForeignKey(uc => uc.CurrencyId);

			//ExchangeRates table configuration
			modelBuilder.Entity<ExchangeRate>()
				.HasKey(er => new { er.FromCurrencyId, er.ToCurrencyId });

			modelBuilder.Entity<ExchangeRate>()
				.Property(er => er.FromCurrencyId)
				.IsRequired()
				.HasMaxLength(450);


			modelBuilder.Entity<ExchangeRate>()
				.Property(er => er.ToCurrencyId)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<ExchangeRate>()
				.HasOne(er => er.FromCurrency)
				.WithMany()
				.HasForeignKey(er=>er.FromCurrencyId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ExchangeRate>()
				.HasOne(er => er.ToCurrency)
				.WithMany()
				.HasForeignKey(er=>er.ToCurrencyId)
				.OnDelete(DeleteBehavior.Restrict);



			modelBuilder.Entity<ExchangeRate>()
				.Property(er => er.DateOfInsert)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<ExchangeRate>()
				.Property(er => er.Rate)
				.IsRequired()
				.HasMaxLength(256);

		}
	}
}
