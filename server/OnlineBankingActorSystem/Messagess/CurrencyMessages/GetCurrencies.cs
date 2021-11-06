using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.CurrencyMessages
{
	public record GetCurrencies(ulong RequestId, string Token) : IConsistentHashable
	{
		public object ConsistentHashKey => RequestId %10;

		public override string ToString()
		{
			return $"{nameof(GetCurrencies)} message: requestId: {RequestId} , token: {Token}";
		}
	}
}
