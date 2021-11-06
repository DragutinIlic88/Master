
namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record GetAccounts(ulong RequestId, string Token)
	{
		public override string ToString()
		{
			return $"{nameof(GetAccounts)} message: requestId: {RequestId} , token: {Token}";
		}
	}
}
