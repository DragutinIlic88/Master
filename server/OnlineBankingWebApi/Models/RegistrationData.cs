using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using OnlineBankingWebApi.Helpers.Attributes;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class RegistrationData
	{
		[Required(ErrorMessage =nameof(EmailErrors.EmailRequiredError))]
		[EmailAddress(ErrorMessage =nameof(EmailErrors.EmailFormatNotCorrectError))]
		[MaxLength(256, ErrorMessage =nameof(EmailErrors.EmailTooLongError))]
		public string Email { get; set; }

		[Required(ErrorMessage =nameof(PasswordErrors.PasswordRequiredError))]
		[Password]
		[MaxLength(50, ErrorMessage =nameof(PasswordErrors.PasswordTooLongError))]
		public string Password { get; set; }

		[Required(ErrorMessage =nameof(PasswordErrors.PasswordRequiredError))]
		[Compare(nameof(Password), ErrorMessage =nameof(PasswordErrors.PasswordDoesNotMatchError))]
		public string ConfirmPassword { get; set; }

		[BankId(accountLength:18, creditCardLength:12)]
		public string BankId { get; set; }

		[Required(ErrorMessage =nameof(MobileErrors.MobileRequiredError))]
		[Phone(ErrorMessage =nameof(MobileErrors.MobileFormatNotCorectError))]
		public string Mobile { get; set; }

		[Required(ErrorMessage =nameof(UserNameErrors.UserNameRequiredError))]
		[MaxLength(50,ErrorMessage =nameof(UserNameErrors.UserNameTooLongError))]
		public string UserName { get; set; }

		[MaxLength(256, ErrorMessage = nameof(AddressErrors.AddressTooLongError))]
		public string Address { get; set; }

		public string HomePhone { get; set; }

	}
}
