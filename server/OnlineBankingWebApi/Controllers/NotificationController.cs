using Akka.Actor;
using Contracts;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/notifications")]
	[ApiController]
	public class NotificationController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _notificationActor;
		private readonly IIncrement _notificationIncrementor;
		
		public NotificationController(ILoggerManager logger, NotificationStorageActorPorvider noitifcationStorageActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_notificationActor = noitifcationStorageActorProvider();
			_notificationIncrementor = incrementor;
		}

		[HttpGet("getNotifications/{userToken}")]
		public async Task<IActionResult> GetNotifications(string userToken)
		{ 
			_logger.LogInfo($"{nameof(GetNotifications)}, notifications will be retrieved for user with token {userToken}");
			var result = await _notificationActor.Ask(new GetNotifications(_notificationIncrementor.Increment(nameof(GetNotifications)), userToken));
			return Ok(result);
		}

		[HttpDelete("")]
		public async Task<IActionResult> DeleteNotification(NotificationModel notificationModel)
		{
			_logger.LogInfo($"{nameof(DeleteNotification)}, notification with id {notificationModel.MessageId} will be deleted");
			var result =  await _notificationActor.Ask(new DeleteNotification(_notificationIncrementor.Increment(nameof(DeleteNotification)),
				notificationModel.MessageId, notificationModel.Title, notificationModel.IsRead, notificationModel.Content, notificationModel.Date, notificationModel.Time));
			return Ok(result);
		}

		[HttpPost("")]
		public IActionResult MarkNotificationIsRead(NotificationModel notificationModel)
		{

			_logger.LogInfo($"{nameof(MarkNotificationIsRead)}, notification with id {notificationModel.MessageId} will be processed as read");
			_notificationActor.Tell(new MarkNotificationAsRead(_notificationIncrementor.Increment(nameof(MarkNotificationIsRead)), 
				notificationModel.MessageId, notificationModel.Title, notificationModel.IsRead, notificationModel.Content, notificationModel.Date, notificationModel.Time));
			 return NoContent();
		}
	}
}
