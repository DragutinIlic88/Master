namespace OnlineBankingActorSystem.Messagess.CurrencyMessages
{
	public record CouldNotReadCurrenices(ulong RequestId, string UserId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotReadCurrenices)} message: requestId: {RequestId} , userId: {UserId}, error message {ErrorMessage}";
		}
	}
}
