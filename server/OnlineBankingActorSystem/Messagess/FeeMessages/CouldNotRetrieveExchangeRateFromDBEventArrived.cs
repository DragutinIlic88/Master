namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record CouldNotRetrieveExchangeRateFromDBEventArrived(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotRetrieveExchangeRateFromDBEventArrived)} message: request id: {RequestId}";
		}
	}
}
