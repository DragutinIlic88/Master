namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record PayOnLocalAccountEventArrived(ulong RequestId, PayOnLocalAccount Message)
	{
		public override string ToString()
		{
			return $"{nameof(PayOnLocalAccountEventArrived)} message: request id: {RequestId}, message: {Message}";
		}
	}
}
