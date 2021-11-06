namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record SaveUserConnectionString(ulong RequestId, string UserToken , string ConnectionId)
	{
		public override string ToString()
		{
			return $"{nameof(SaveUserConnectionString)} message: request id: {RequestId}, user token: {UserToken}, connection id: {ConnectionId}";
		}
	}
}
