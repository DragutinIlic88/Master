using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record Registered (ulong RequestId)
	{

		public override string ToString()
		{
			return $"{nameof(Registered)} message: request id: {RequestId}";
		}
	}
}
