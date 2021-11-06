using OnlineBankingEntitiesLib;

namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record RetrievedAccountsForPayment(ulong RequestId, string UserId, Account UserAccount, Account BeneficieryAccount = null)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievedAccountsForPayment)} message: request id: {RequestId}, user id: {UserId}, user account: {UserAccount.AccountNumber}, beneficiary account: {(BeneficieryAccount !=null ?BeneficieryAccount.AccountNumber : "no beneficiary account")}";
		}
	}
}
