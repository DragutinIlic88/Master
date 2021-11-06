
namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{
	public record ExchangeRateInvalidArrivedEvent(ulong RequestId, string ErrorMessage)
	{
		public override string ToString() => $"{nameof(ExchangeRateInvalidArrivedEvent)} message: request id: {RequestId}, error message: {ErrorMessage}";
	}
}
