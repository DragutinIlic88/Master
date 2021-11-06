using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public class UnhashPasswordForAuthenticationValidation : BaseMessage
	{
		public UnhashPasswordForAuthenticationValidation(ulong requestId,string email,  string password, string passwordHash) : base(requestId)
		{
			Email = email;
			Password = password;
			PasswordHash = passwordHash;
		}

		public UnhashPasswordForAuthenticationValidation(ValidateUserAuthentication msg, string passwordHash): this(msg.RequestId, msg.Email, msg.Password, passwordHash)
		{
		}

		public string Email { get; }
		public string Password { get; }
		public string PasswordHash { get; }

		public override string ToString()
		{
			return $"{nameof(UnhashPasswordForAuthenticationValidation)} message: requestId: {RequestId},  email: {Email}, password hash: {PasswordHash} password: *****";
		}
	}
}
