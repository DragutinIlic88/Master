using Akka.Actor;
namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record GetFeeArrivedEvent(ulong RequestId, string Token, string FromCurrency, string ToCurrency, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(GetFeeArrivedEvent)} message: requestId: {RequestId} , token: {Token}, from currency: {FromCurrency}, to currency {ToCurrency} sender {Sender}";
		}
	}
}
