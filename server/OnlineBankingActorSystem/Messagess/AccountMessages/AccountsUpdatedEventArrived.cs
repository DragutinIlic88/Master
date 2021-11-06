namespace OnlineBankingActorSystem.Messagess.AccountMessages
{

	public record AccountsUpdatedEventArrived(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(AccountsUpdatedEventArrived)} message: requestId: {RequestId} ";
		}
	}
}
