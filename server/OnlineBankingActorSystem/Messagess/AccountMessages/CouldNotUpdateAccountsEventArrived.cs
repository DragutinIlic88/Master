
namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record CouldNotUpdateAccountsEventArrived(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotUpdateAccountsEventArrived)} message: requestId: {RequestId}";
		}
	}
}
