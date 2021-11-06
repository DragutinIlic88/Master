using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record AccountsUpdated(ulong RequestId, int NumberOfUpdatedAccounts)
	{
		public override string ToString()
		{
			return $"{nameof(AccountsUpdated)} message: requestId: {RequestId}, number of updated accounts: {NumberOfUpdatedAccounts}";
		}
	}
}
