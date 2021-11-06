using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.CurrencyMessages
{
	public record GetCurrenciesArrivedEvent(ulong RequestId, string Token, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(GetCurrenciesArrivedEvent)} message: requestId: {RequestId} , token: {Token}, sender {Sender}";
		}
	}
}
