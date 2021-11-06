using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record GetRoleForUser (ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(GetRoleForUser)} message: requestId:{RequestId}";
		}
	}
}
