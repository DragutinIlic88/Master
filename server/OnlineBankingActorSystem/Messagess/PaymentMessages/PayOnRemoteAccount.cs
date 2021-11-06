using Akka.Actor;
using OnlineBankingEntitiesLib;

namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record PayOnRemoteAccount(ulong RequestId, Account UserAccount, decimal Amount, string BeneficiaryCustomer, string BeneficiaryCustomerAccount,
		int? Model, string Reference, string PaymentCode, string PaymentPurpose, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(PayOnRemoteAccount)} message: request id: {RequestId}, user account: {UserAccount}, amount: {Amount} " +
				$"beneficiary customer: {BeneficiaryCustomer} , acocunt of beneficiary: {BeneficiaryCustomerAccount}, reference: {Reference}, payment code: {PaymentCode}" +
				$" payment purpose: {PaymentPurpose} model: {(Model.HasValue ? Model : "no model")}, sender: {Sender}";
		}
	}
}
