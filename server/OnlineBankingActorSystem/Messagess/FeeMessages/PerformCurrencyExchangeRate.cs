namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record PerformCurrencyExchangeRate(ulong RequestId, string UserId, string FromCurrency, string ToCurrency)
	{
		public override string ToString()
		{
			return $"{nameof(PerformCurrencyExchangeRate)} message, request id: {RequestId}, userId: {UserId},  from curerncy: {FromCurrency} to currency: {ToCurrency}";
		}
	}
}
