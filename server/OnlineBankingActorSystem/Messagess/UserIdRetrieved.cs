using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess
{
	public record UserIdRetrieved(ulong RequestId, string Token , string UserId)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievingUserIdFailed)} message: requestId: {RequestId}, token: {Token}, userId: {UserId}"; 
		}
	}
}
