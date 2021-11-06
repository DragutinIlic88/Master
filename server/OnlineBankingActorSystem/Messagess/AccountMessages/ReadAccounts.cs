

namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record ReadAccounts(ulong RequestId, string UserId)
	{
		public override string ToString()
		{
			return $"{nameof(GetAccounts)} message: requestId: {RequestId} , userId: {UserId}"; 
		}
	}
}
