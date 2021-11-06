using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages
{
	public record LogoutFailed(ulong RequestId, string ErrorMessage) 
	{
		public override string ToString()
		{
			return $"{nameof(LogoutFailed) } message: requestId: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
