using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess
{
	public record RetrieveUserId(ulong RequestId, string Token)
	{
		public override string ToString()
		{
			return $"{nameof(RetrieveUserId)} message: requestId: {RequestId} , token: {Token}"; ;
		}
	}
}
