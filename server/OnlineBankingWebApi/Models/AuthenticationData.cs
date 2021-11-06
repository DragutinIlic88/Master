using OnlineBankingWebApi.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public sealed class AuthenticationData
	{
		[Required(ErrorMessage = nameof(EmailErrors.EmailRequiredError))]
		[EmailAddress(ErrorMessage = nameof(EmailErrors.EmailFormatNotCorrectError))]
		[MaxLength(256, ErrorMessage = nameof(EmailErrors.EmailTooLongError))]
		public string Email { get; set; }

		[Required(ErrorMessage = nameof(PasswordErrors.PasswordRequiredError))]
		[Password(minLength:8)]
		public string Password { get; set; }

	}
}
