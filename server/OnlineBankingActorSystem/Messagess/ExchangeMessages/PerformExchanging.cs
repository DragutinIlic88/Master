using Akka.Actor;
using OnlineBankingEntitiesLib;

namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{

	public record PerformExchanging(ulong RequestId,  Account FromAccount, Account ToAccount,decimal ExchangeRate, decimal Amount,  IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(PerformExchanging)} message: requestId: {RequestId},  from account: {FromAccount},amount: {Amount}, exchange rate: {ExchangeRate}, to account: {ToAccount}, sender {Sender}";
		}
	}
}
