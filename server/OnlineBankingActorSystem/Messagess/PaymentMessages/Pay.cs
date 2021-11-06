
namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record Pay(ulong RequestId, string UserToken, string AccountNumber, 
		string BeneficieryCustomer, string BenericieryCustomerAccount, decimal Amount, string Currency,
		int? Model, string Reference, string PaymentCode, string PaymentPurpose)
	{
		public override string ToString()
		{
			return $"{nameof(Pay)} message: requestId: {RequestId}, user token: {UserToken}, account number: {AccountNumber}" +
				$"beneficiary customer: {BeneficieryCustomer}, beneficiary customer account: {BenericieryCustomerAccount}, amount: {Amount}, currency: {Currency}" +
				$"model: {Model}, reference: {Reference}, payment code: {PaymentCode}, payment purpose: {PaymentPurpose}";
		}
	}
}
