using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages
{
	public record Logout (ulong RequestId, string Token)
	{
		public override string ToString()
		{
			return $"{nameof(Logout)} messages: requestId: {RequestId}, token: {Token}";
		}
	}
}
