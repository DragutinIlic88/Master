using Akka.Actor;
namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{

	public record ConfirmExchangeArrivedEvent(ulong RequestId, ConfirmExchange ConfirmExchange, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(ConfirmExchangeArrivedEvent)} message: requestId {RequestId}, confirm exchange message {ConfirmExchange}, sender {Sender}";
		}
	}
}
