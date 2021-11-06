
namespace OnlineBankingActorSystem.Messagess.AccountMessages
{

	public record GetFromAndToAccount(ulong RequestId, string UserId, string FromAccount, string ToAccount)
	{
		public override string ToString()
		{
			return $"{nameof(GetFromAndToAccount)} message: requestId: {RequestId} , userId: {UserId}, from account {FromAccount}, to account {ToAccount}";
		}
	}
}
