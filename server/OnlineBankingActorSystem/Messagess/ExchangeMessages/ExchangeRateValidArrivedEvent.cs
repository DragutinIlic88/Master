

namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{
	public record ExchangeRateValidArrivedEvent(ulong RequestId)
	{
		public override string ToString() => $"{nameof(ExchangeRateValidArrivedEvent)} message : request id {RequestId}";
	}
}
