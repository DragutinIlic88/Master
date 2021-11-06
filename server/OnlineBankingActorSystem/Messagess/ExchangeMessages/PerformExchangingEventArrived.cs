using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{
	public record PerformExchangingEventArrived(ulong RequestId, PerformExchanging PerformExchanging)
	{
		public override string ToString()
		{
			return $"{nameof(PerformExchangingEventArrived)} message: requestId {RequestId}, perform exchanging message {PerformExchanging}";
		}
	}
}
