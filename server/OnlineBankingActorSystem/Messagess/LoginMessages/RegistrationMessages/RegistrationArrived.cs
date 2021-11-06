
namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record RegistrationArrived (ulong RequestId, string Email, string Password, string ConfirmPassword, string BankId, string Mobile, string UserName, string Address, string HomePhone)
	{
		public override string ToString()
		{
			return $"{nameof(RegistrationArrived)} message: request id {RequestId}, email: {Email}, password: **** , confirm password: ****, bank id: {BankId}, mobile: {Mobile}, user name: {UserName}, address {Address}, home phone: {HomePhone}";
		}
	}
}
