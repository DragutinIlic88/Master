using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record FetchUserFailed(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(FetchUserFailed)} message: requestId: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
