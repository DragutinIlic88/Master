using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record RetrievedToAndFromAccounts(ulong RequestId, string UserId, Account FromAccount,Account ToAccount)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievedToAndFromAccounts)} message: requestId: {RequestId} , userId: {UserId}, from account {FromAccount.AccountNumber}, to account {ToAccount.AccountNumber}";
		}
	}
}
