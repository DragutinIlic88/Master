using Microsoft.EntityFrameworkCore;
using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingDBContextLib
{
	public class OnlineBankingNotificationContext  :DbContext
	{
		public DbSet<Notification> Notifications { get; set; }

		public OnlineBankingNotificationContext(DbContextOptions<OnlineBankingNotificationContext> options) :base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Notification>().HasKey(n => n.MessageId);

			modelBuilder.Entity<Notification>().Property(n => n.MessageId).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.UserId).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.Content).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.IsRead).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.Date).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.Time).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.Title).IsRequired();

			modelBuilder.Entity<Notification>().Property(n => n.Type).IsRequired();





		}
	}
}
