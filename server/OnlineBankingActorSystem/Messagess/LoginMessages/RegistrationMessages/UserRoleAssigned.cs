using OnlineBankingActorSystem.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record UserRoleAssigned (ulong RequestId, Role Role)
	{
		public override string ToString()
		{
			return $"{nameof(UserRoleAssigned)} message: requestId: {RequestId}, role: {Role}";
		}
	}
}
