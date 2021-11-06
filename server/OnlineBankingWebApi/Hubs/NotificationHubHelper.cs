using Contracts;
using Contracts.Models;
using Microsoft.AspNetCore.SignalR;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Hubs
{
	public class NotificationHubHelper : INotificationHubHelper
	{
		private readonly IHubContext<NotificationHub> _hub;

		public NotificationHubHelper(IHubContext<NotificationHub> hub)
		{
			_hub = hub;
		}

		public void SendNotification(NotificationModel notification, string connectionId) {
			_hub.Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
		}

		public void SendNotifications(NotificationModel[] notifications, string connectionId) {
			_hub.Clients.Client(connectionId).SendAsync("ReceiveNotifications", notifications);
		}
	}
}
