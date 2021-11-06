using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record AuthenticationAllowed(ulong RequestId, string Email, string Password, string PasswordHash)
	{
		public override string ToString()
		{
			return $"{nameof(AuthenticationAllowed)} message: requestId: {RequestId}, email: {Email}, password: {PasswordHash}";
		}
	}
}
