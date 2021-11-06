using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.SignalR;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Hubs
{
	public class NotificationHub : Hub
	{
		private readonly ILoggerManager _logger;
		private readonly IActorRef _notificationActor;
		private readonly IIncrement _notificationIncrementor;
		public NotificationHub(ILoggerManager logger, NotificationActorProvider notificationActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_notificationActor = notificationActorProvider();
			_notificationIncrementor = incrementor;
		}
		public void ReceivedUserToken(string UserToken)
		{
			_logger.LogInfo($"{nameof(ReceivedUserToken)}, user token: {UserToken} sent to notification hub ");
			_notificationActor.Tell(new SaveUserConnectionString(_notificationIncrementor.Increment(nameof(ReceivedUserToken)), UserToken, Context.ConnectionId));
		}
	}
}
