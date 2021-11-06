namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record UserAccountRetirievedEventArrived(ulong RequestId )
	{
		public override string ToString()
		{
			return $"{nameof(UserAccountRetirievedEventArrived)} message: request id: {RequestId}";
		}
	}
}
