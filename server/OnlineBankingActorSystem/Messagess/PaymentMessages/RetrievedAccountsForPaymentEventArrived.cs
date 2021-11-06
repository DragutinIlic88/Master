using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record RetrievedAccountsForPaymentEventArrived(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievedAccountsForPaymentEventArrived)} message: request id: {RequestId}";
		}
	}
}
