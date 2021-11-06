
namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record CouldNotReadAccounts(ulong RequestId, string UserId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotReadAccounts)} message: requestId: {RequestId} , userId: {UserId}, error message {ErrorMessage}";
		}
	}
}
