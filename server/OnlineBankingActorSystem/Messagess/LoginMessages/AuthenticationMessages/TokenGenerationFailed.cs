using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record  TokenGenerationFailed(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(TokenGenerationFailed)} message: requestId: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
