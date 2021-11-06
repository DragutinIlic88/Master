namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record CouldNotDeleteNotification(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotDeleteNotification)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
