using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	class NotificationStorageActor : BaseReceiveActor, ILogReceive
	{
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props(), "userIdRetriever");
		private ConcurrentDictionary<ulong, IActorRef> controllers = new();

		public NotificationStorageActor() :base(nameof(NotificationStorageActor))
		{
			Receive<GetNotifications>(message =>
			{
				logger.Info($"{ActorName}, message received with data: {message}");
				controllers.TryAdd(message.RequestId, Sender);
				userIdRetrieverActor.Tell(new RetrieveUserId(message.RequestId, message.UserToken), Self);
			});
			ReceiveAsync<UserIdRetrieved>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await GetNotificationsAsync(message);
			});

			Receive<RetrievingUserIdFailed>(message=> { 
				logger.Info($"{ActorName}, message received with data: {message}");
				controllers.TryRemove(message.RequestId, out var controller);
				controller.Tell(message, Self);
			});

			ReceiveAsync<DeleteNotification>(async message =>
			{
				logger.Info($"{ActorName}, message received with data: {message}");
				await DeleteNotificationAsync(message);
			});

			ReceiveAsync<MarkNotificationAsRead>(async message => {
				logger.Info($"{ActorName}, message received with data: {message}");
				await MarkNotificationAsReadAsync(message);
			});
		}

		private async Task MarkNotificationAsReadAsync(MarkNotificationAsRead msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var context = serviceScope.ServiceProvider.GetService<OnlineBankingNotificationContext>();
			try
			{
				var notification = await context.Notifications.SingleAsync(n => n.MessageId == msg.MessageId);
				notification.IsRead = true;
				context.Update(notification);
				var updated = await context.SaveChangesAsync();
				if (updated != 1)
				{
					logger.Error($"{ActorName}, incorrect number of notification marked as read");
				}
				else
				{
					logger.Info($"{ActorName}, notification with id {notification.MessageId} updated correctly");
				}
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, notification could not be marked as read , due to {e.GetBaseException()}");
			}
		}

		private async Task GetNotificationsAsync(UserIdRetrieved msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var context = serviceScope.ServiceProvider.GetService<OnlineBankingNotificationContext>();
			controllers.TryRemove(msg.RequestId, out var controller);
			try
			{
				var notifications = await context.Notifications.Where(n => n.UserId == msg.UserId).ToListAsync();
				controller.Tell(new NotificationsRetrieved(msg.RequestId, msg.UserId, notifications),Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, notification could not be deleted , due to {e.GetBaseException()}");
				controller.Tell(new CouldNotGetNotifications(msg.RequestId, NotificationError.DatabaseDeleteError), Self);
			}
		}

		private async Task DeleteNotificationAsync(DeleteNotification msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var context = serviceScope.ServiceProvider.GetService<OnlineBankingNotificationContext>();
			try
			{
				var notification = await context.Notifications.SingleAsync(n => n.MessageId == msg.MessageId);
				context.Remove(notification);
				var changed = await context.SaveChangesAsync();
				if (changed != 1)
				{
					logger.Error($"{ActorName}, incorrect number of notification deleted");
					Sender.Tell(new CouldNotDeleteNotification(msg.RequestId, NotificationError.DatabaseDeleteError), Self);
				}
				else
				{
					logger.Info($"{ActorName}, notification with id {notification.MessageId} delted successfuly");
					Sender.Tell(new NotificationDeleted(msg.RequestId));
				}
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, notification could not be deleted , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotDeleteNotification(msg.RequestId, NotificationError.DatabaseDeleteError), Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new NotificationStorageActor());
	}
}
