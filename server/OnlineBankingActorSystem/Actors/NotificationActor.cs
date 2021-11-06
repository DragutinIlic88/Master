using Akka.Actor;
using Akka.DependencyInjection;
using Contracts;
using Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using OnlineBankingEntitiesLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	class NotificationActor : BaseReceiveActor, ILogReceive
	{
		private static readonly ConcurrentDictionary<string, string> userTokenConnectionIds = new();
		private static readonly ConcurrentDictionary<string, string> userIdConnectionIds = new();
		private readonly INotificationHubHelper _notificationHubHelper;
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props(), "userIdRetriever");

		public NotificationActor(INotificationHubHelper notificationHubHelper) : base(nameof(NotificationActor))
		{
			_notificationHubHelper = notificationHubHelper;

			Receive<SaveUserConnectionString>(message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				userTokenConnectionIds.TryAdd(message.UserToken, message.ConnectionId);
				userIdRetrieverActor.Tell(new RetrieveUserId(message.RequestId, message.UserToken), Self);
			});

			Receive<UserIdRetrieved>(message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				userTokenConnectionIds.TryRemove(message.Token, out var connectionId);
				userIdConnectionIds.TryAdd(message.UserId, connectionId);
			});

			Receive<SendNotification>(message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				SendNotificationToClient(message);
			});

			ReceiveAsync<SendSavedNotifications>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await SendNotificationsToClientAsync(message);
			});

		}

		private async Task SendNotificationsToClientAsync(SendSavedNotifications msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var context = serviceScope.ServiceProvider.GetService<OnlineBankingNotificationContext>();
			try {
				var notifications = await context.Notifications.Where(n => n.UserId == msg.UserId).ToListAsync();
				if (notifications.Count > 0)
				{
					var notificationModels = notifications.Select(n => new NotificationModel
					{
						Content = n.Content,
						MessageId = n.MessageId,
						IsRead = n.IsRead,
						Date = n.Date,
						Time = n.Time,
						Title = n.Title,
						Type = n.Type
					}).ToArray();

					_notificationHubHelper.SendNotifications(notificationModels, userIdConnectionIds[msg.UserId]);
				}
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, notifications could not be sent , due to {e.GetBaseException()}");
			}
		}

		private void SendNotificationToClient(SendNotification msg) {
			try {
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingNotificationContext>();
				var notification = new Notification
				{
					MessageId = Guid.NewGuid(),
					Content = msg.MessageContent,
					UserId = msg.UserId,
					IsRead = false,
					Title = msg.MessageTitle,
					Date = DateTime.Now.ToString("MM/dd/yyyy"),
					Time = DateTime.Now.ToString("HH:mm:ss"),
					Type = msg.MessageType
				};

				context.Notifications.Add(notification);
				var saved = context.SaveChanges();

				if (saved > 0)
				{
					logger.Info($"{ActorName}, notification succesfully saved to database");
				}
				else {
					logger.Error($"{ActorName}, notification could not be saved to database");
				}

				_notificationHubHelper.SendNotification(new NotificationModel { 
				Content =notification.Content,
				MessageId = notification.MessageId,
				IsRead = notification.IsRead,
				Date = notification.Date,
				Time = notification.Time,
				Title = notification.Title,
				Type = notification.Type
				}, userIdConnectionIds[msg.UserId]);

			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, notification could not be sent , due to {e.GetBaseException()}");
			}

		}

	}
}
