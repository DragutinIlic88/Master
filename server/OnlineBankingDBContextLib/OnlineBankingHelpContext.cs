using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;

namespace OnlineBankingDBContextLib
{
	public class OnlineBankingHelpContext : DbContext
	{

		public DbSet<HelpInformations> HelpInformations { get; set; }

		public OnlineBankingHelpContext(DbContextOptions<OnlineBankingHelpContext> options):base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<HelpInformations>().HasKey(h => new { h.PhoneNumber, h.EmailAddress });

			modelBuilder.Entity<HelpInformations>()
				.Property(t => t.PhoneNumber)
				.IsRequired()
				.HasMaxLength(450);

			modelBuilder.Entity<HelpInformations>()
				.Property(t => t.EmailAddress)
				.IsRequired()
				.HasMaxLength(256);

		}

	}
}
