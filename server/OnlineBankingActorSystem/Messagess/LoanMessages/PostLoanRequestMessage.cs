using System;


namespace OnlineBankingActorSystem.Messagess.Loan
{
	public record PostLoanRequestMessage(ulong RequestId, string UserToken, string FromAccount, string ReceiveAccount, decimal TotalAmount, string Currency,
		DateTime StartDate, DateTime EndDate, string Purpose, decimal? Participation, string Collateral)
	{
		public override string ToString()
		{
			return $"{nameof(PostLoanRequestMessage)} message: request id: {RequestId}, user token: {UserToken}, from account: {FromAccount}, to account: {ReceiveAccount}" +
				$"amount: {TotalAmount}, currency: {Currency}, start date: {StartDate}, end date: {EndDate}, purpose: {Purpose}, participation: {(Participation?? 0)}, " +
				$"collateral: {Collateral ?? "no-collateral"}";
		}
	}
}
