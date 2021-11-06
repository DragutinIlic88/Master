namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record NotificationDeleted(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(NotificationDeleted)} message: request id: {RequestId}";
		}
	}
}
