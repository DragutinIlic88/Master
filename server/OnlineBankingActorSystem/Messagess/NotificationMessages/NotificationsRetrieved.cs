using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record NotificationsRetrieved(ulong RequestId,string UserId,IList <Notification> Notifications)
	{
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(NotificationsRetrieved)} message: requestId: {RequestId} , userId: {UserId} {Environment.NewLine}");
			text.Append($"Accounts: {Environment.NewLine}");
			foreach (var notification in Notifications)
			{
				text.Append($"notification id: {notification.MessageId}, notification title: {notification.Title} {Environment.NewLine}");
			}
			return text.ToString();
		}
	}
}
