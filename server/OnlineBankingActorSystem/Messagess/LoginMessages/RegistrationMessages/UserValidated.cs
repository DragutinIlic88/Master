using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record UserValidated(ulong RequestId, bool IsValid = false, bool SameEmail = false, bool SameMobile = false, string ErrorMessage = "")
	{

		public override string ToString()
		{
			return $"{nameof(UserValidated)} message: requestId: {RequestId}, is valid: {IsValid}, same mobile: {SameMobile}, same email: {SameEmail}";
		}
	}
}
