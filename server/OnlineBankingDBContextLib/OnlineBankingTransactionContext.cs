using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;


namespace OnlineBankingDBContextLib
{
	public class OnlineBankingTransactionContext : DbContext
	{
		public DbSet<Transaction> Transactions { get; set; }

		public OnlineBankingTransactionContext(DbContextOptions<OnlineBankingTransactionContext> options) :base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Transaction>()
				.HasKey(t => t.TransactionId);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.UserId)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.AccountNumber)
				.IsRequired()
				.HasMaxLength(30);


			modelBuilder.Entity<Transaction>()
				.Property(t => t.AccountIban)
				.IsRequired()
				.HasMaxLength(34);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.BankIdentifierCode)
				.IsRequired()
				.HasMaxLength(256);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.CreationTime)
				.IsRequired()
				.HasColumnType("datetime");

			modelBuilder.Entity<Transaction>()
				.Property(t => t.EndTime)
				.HasColumnType("datetime");

			modelBuilder.Entity<Transaction>()
				.Property(t => t.TransactionStatus)
				.IsRequired()
				.HasConversion<string>();

			modelBuilder.Entity<Transaction>(t =>
			{
				t.OwnsOne(a=>a.TransactionAmount,
					amount=> {
						amount.Property(a => a.Currency)
						.HasColumnName("Currency")
						.IsRequired();
						amount.Property(a => a.Total)
						.HasColumnName("Total")
						.IsRequired()
						.HasColumnType("decimal");

					});
			});
		}
	}
}
