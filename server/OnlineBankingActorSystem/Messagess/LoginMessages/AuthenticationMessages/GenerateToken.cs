using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record GenerateToken (ulong RequestId, string Email, string PasswordHash)
	{
		public override string ToString()
		{
			return $"{nameof(GenerateToken)} message: requestId: {RequestId}, email: {Email}, password: {PasswordHash}";
		}
	}
}
