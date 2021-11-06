using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record Authenticated(ulong RequestId, string UserToken)
	{
		public override string ToString()
		{
			return $"{nameof(Authenticated)} message: requestId: {RequestId}, user token: {UserToken}";
		}

	}
}
