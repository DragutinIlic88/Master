namespace OnlineBankingActorSystem.Messagess.NotificationMessages
{
	public record SendNotification(ulong RequestId, string UserId, string MessageContent, string MessageTitle, string MessageType)
	{
		public override string ToString()
		{
			return $"{nameof(SendNotification)} message: request id: {RequestId}, user id: {UserId}, message content: {MessageContent}, message title: {MessageTitle}, message type: {MessageType}";
		}
	}
}
