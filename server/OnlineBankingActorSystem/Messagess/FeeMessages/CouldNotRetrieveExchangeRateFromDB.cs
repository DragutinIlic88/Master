namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record CouldNotRetrieveExchangeRateFromDB(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotRetrieveExchangeRateFromDB) } message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
