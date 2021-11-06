namespace OnlineBankingActorSystem.Messagess.Loan
{
	public record ReadLoanTransactions(ulong RequestId, string UserId, string AccountNumber=null)
	{
		public override string ToString()
		{
			return $"{nameof(ReadLoanTransactions)} message: request id: {RequestId}, user id: {UserId}, account number: {AccountNumber?? "no account number"}";
		}
	}
}
