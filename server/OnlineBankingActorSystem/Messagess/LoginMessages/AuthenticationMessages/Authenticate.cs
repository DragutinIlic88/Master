using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{

	public record Authenticate(ulong RequestId, string Email, string Password)
	{
		public override string ToString()
		{
			return $"{nameof(Authenticate)} message: requestId: {RequestId} , email: {Email}";
		}
	}
}
