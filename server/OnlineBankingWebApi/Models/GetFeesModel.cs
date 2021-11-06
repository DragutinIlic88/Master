using System.ComponentModel.DataAnnotations;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class GetFeesModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }
		[Required(ErrorMessage = nameof(FeeErrors.FromCurrencyRequiredError))]
		[MaxLength(3, ErrorMessage =nameof(FeeErrors.CurrencyLengthError))]
		[MinLength(3,ErrorMessage =nameof(FeeErrors.CurrencyLengthError))]
		public string FromCurrency { get; set; }
		[Required(ErrorMessage = nameof(FeeErrors.ToCurrencyRequiredError))]
		[MaxLength(3, ErrorMessage = nameof(FeeErrors.CurrencyLengthError))]
		[MinLength(3, ErrorMessage = nameof(FeeErrors.CurrencyLengthError))]
		public string ToCurrency { get; set; }
	}
}
