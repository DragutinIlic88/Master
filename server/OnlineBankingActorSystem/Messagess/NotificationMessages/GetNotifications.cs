namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record GetNotifications(ulong RequestId, string UserToken)
	{
		public override string ToString()
		{
			return $"{nameof(GetNotifications)} message: request id: {RequestId}, user token: {UserToken}";
		}
	}
}
