using Contracts.Models;

namespace Contracts
{
	public interface INotificationHubHelper
	{
		void SendNotification(NotificationModel notification, string connectionId);
		void SendNotifications(NotificationModel[] notifications, string connectionId);
	}
}
