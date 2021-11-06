using Akka.Actor;
using OnlineBankingEntitiesLib;

namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record PayOnLocalAccount(ulong RequestId, string UserId, Account UserAccount, Account BeneficieryAccount, decimal Amount, string BeneficiaryCustomer,
		int? Model, string Reference, string PaymentCode, string PaymentPurpose,   IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(PayOnLocalAccount)} message: request id: {RequestId}, user id: {UserId}, user account: {UserAccount}, beneficiary account: {BeneficieryAccount}" +
				$" amount: {Amount} beneficiary customer: { BeneficiaryCustomer} reference: { Reference}, payment code: { PaymentCode}" +
				$" payment purpose: {PaymentPurpose} model: {(Model.HasValue ? Model : "no model")}, sender: {Sender}";
		}
		
	}
}
