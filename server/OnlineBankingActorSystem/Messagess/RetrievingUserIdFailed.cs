
namespace OnlineBankingActorSystem.Messagess
{
	public record RetrievingUserIdFailed(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievingUserIdFailed)} message: requestId: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
