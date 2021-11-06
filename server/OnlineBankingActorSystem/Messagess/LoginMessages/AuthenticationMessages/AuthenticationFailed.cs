using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record AuthenticationFailed(ulong RequestId, string ErrorMessage = "")
	{
		public override string ToString()
		{
			return $"{nameof(AuthenticationFailed)} message: RequestId: {RequestId}, ErrorMessage: {ErrorMessage}";
		}
	}
}
