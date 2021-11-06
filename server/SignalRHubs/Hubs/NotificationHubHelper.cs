using Microsoft.AspNetCore.SignalR;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Hubs
{
	public class NotificationHubHelper
	{
		private readonly IHubContext<NotificationHub> _hub;

		public NotificationHubHelper(IHubContext<NotificationHub> hub)
		{
			_hub = hub;
		}

		public void SendNotification(NotificationModel notification) {
			_hub.Clients.Client("").SendAsync("ReceiveNotification", notification);
		}

		public void SendNotifications(NotificationModel[] notifications) { 
			_hub.Clients.Client("").SendAsync("ReceiveNotifications", notifications);
		}
	}
}
