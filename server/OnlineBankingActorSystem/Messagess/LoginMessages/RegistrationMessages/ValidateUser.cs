

namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record ValidateUser (ulong RequestId, string BankId, string Email, string Mobile)
	{
		public override string ToString() {
			return $"{nameof(ValidateUser)} message: requestId: {RequestId} bankId: {BankId} , email: {Email} , mobile: {Mobile}";
		}

	}

	
}
