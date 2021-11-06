using System.ComponentModel.DataAnnotations;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class EditUserModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }

		[MaxLength(50, ErrorMessage = nameof(UserNameErrors.UserNameTooLongError))]
		public string  UserName { get; set; }

		[Phone(ErrorMessage = nameof(MobileErrors.MobileFormatNotCorectError))]
		public string  Mobile { get; set; }

		[EmailAddress(ErrorMessage = nameof(EmailErrors.EmailFormatNotCorrectError))]
		[MaxLength(256, ErrorMessage = nameof(EmailErrors.EmailTooLongError))]
		public string Email { get; set; }

		[MaxLength(256, ErrorMessage = nameof(AddressErrors.AddressTooLongError))]
		public string Address { get; set; }
	}
}
