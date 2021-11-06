namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record GetPaymentFeeRateFromDB(ulong RequestId, string FromCurrency, string ToCurrency)
	{
		public override string ToString()
		{
			return $"{nameof(GetPaymentFeeRateFromDB)} message: request id: {RequestId}, from currency: {FromCurrency}, to currency: {ToCurrency}";
		}
	}
}
