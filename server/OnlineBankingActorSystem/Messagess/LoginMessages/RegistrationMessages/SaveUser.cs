
namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record SaveUser (ulong RequestId, string Email, string Password, string ConfirmPassword, string BankId, string Mobile, string UserName, string Address, string HomePhone)
	{

		public override string ToString()
		{
			return $"{nameof(SaveUser)} message: email: {Email}, password: {Password}, confirmPassword: {ConfirmPassword}, " +
				$"bankId: {BankId}, mobile: {Mobile}, username: {UserName}, address: {Address}, home phone: {HomePhone}";
		}

	}
}
