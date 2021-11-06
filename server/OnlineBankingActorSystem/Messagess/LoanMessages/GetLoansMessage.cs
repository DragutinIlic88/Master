

namespace OnlineBankingActorSystem.Messagess.Loan
{
	public record GetLoansMessage(ulong RequestId, string UserToken)
	{
		public override string ToString()
		{
			return $"{nameof(PostLoanRequestMessage)} message: request id: {RequestId}, user token: {UserToken}";
		}
	}
}
