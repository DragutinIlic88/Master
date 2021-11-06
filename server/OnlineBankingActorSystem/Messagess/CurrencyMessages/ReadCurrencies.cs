namespace OnlineBankingActorSystem.Messagess.CurrencyMessages
{
	public record ReadCurrencies(ulong RequestId, string UserId)
	{
		public override string ToString()
		{
			return $"{nameof(ReadCurrencies)} message: requestId: {RequestId} , userId: {UserId}";
		}
	}
}
