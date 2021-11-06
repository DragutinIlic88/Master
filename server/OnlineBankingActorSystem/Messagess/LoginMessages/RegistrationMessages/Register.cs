

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record Register(ulong RequestId, string Email, string Password, string ConfirmPassword, string BankId, string Mobile, string UserName, string Address="", string HomePhone="")
	{

		public override string ToString()
		{
			return $"{nameof(Register)} message: requestId: {RequestId} , email: {Email} , username: {UserName}";
		}

	}
}
