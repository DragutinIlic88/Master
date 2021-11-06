using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record CouldNotGetNotifications(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotGetNotifications)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
