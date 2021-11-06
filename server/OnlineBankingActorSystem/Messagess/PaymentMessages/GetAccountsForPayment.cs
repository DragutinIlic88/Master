namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record GetAccountsForPayment(ulong RequestId, string UserId, string AccountNumber, string BeneficiaryAccountNumber)
	{
		public override string ToString()
		{
			return $"{nameof(GetAccountsForPayment)} message: request id: {RequestId}, user id {UserId}, user account {AccountNumber}, payment account: {BeneficiaryAccountNumber}";
		}
	}
}
