using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record HashPassword (ulong RequestId, string Password)
	{

		public override string ToString()
		{
			return $"{nameof(HashPassword)} message: requestId: {RequestId}, password: *****";
		}
	}
}
