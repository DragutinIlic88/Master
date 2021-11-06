using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record SendSavedNotifications(ulong RequestId, string UserId)
	{
		public override string ToString()
		{
			return $"{nameof(SendSavedNotifications)} message: request id: {RequestId}, user id: {UserId}";
		}
	}
}
