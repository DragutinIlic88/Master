using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages
{
	public record LogoutAllowed (ulong RequestId )
	{
		public override string ToString()
		{
			return $"{nameof(LogoutAllowed)} message: request id: {RequestId}";
		}
	}
}
