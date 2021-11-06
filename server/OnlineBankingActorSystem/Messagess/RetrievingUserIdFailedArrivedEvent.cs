namespace OnlineBankingActorSystem.Messagess
{
	public record RetrievingUserIdFailedArrivedEvent(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievingUserIdFailedArrivedEvent)} message: requestId: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
