
namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public class ValidateUserAuthentication : BaseMessage
	{
		public ValidateUserAuthentication(ulong requestId, string email, string password) :base(requestId)
		{
			Email = email;
			Password = password;
		}

		public ValidateUserAuthentication(Authenticate message) :this(message.RequestId, message.Email, message.Password)
		{
		}

		public string Email { get; }
		public string Password { get; }

		public override string ToString()
		{
			return $"{nameof(ValidateUserAuthentication)} message: email: {Email} , password: {Password}";
		}
	}
}
