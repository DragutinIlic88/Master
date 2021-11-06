namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record PayOnRemoteAccountEventArrived(ulong RequestId, PayOnRemoteAccount Message)
	{
		public override string ToString()
		{
			return $"{nameof(PayOnRemoteAccountEventArrived)} message: request id: {RequestId}, message: {Message}";
		}
	}
}
