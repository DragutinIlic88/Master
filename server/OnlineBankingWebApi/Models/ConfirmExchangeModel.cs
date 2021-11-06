using System;
using System.ComponentModel.DataAnnotations;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class ConfirmExchangeModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }

		[Required(ErrorMessage =nameof(AccountErrors.FromAccountNumberRequiredError))]
		public string FromAccount { get; set; }

		[Required(ErrorMessage =nameof(AccountErrors.ToAccountNumberRequredError))]
		public string  ToAccount { get; set; }

		[Required(ErrorMessage =nameof(AmountErrors.AmountRequiredError))]
		[Range(0.001, Double.MaxValue, ErrorMessage =nameof(AmountErrors.AmountLessThenZeroError))]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = nameof(ExchangeRateErrors.ExchangeRateRequiredError))]
		[Range(0.001, Double.MaxValue, ErrorMessage = nameof(ExchangeRateErrors.ExchangeRateLessThenZeroError))]
		public decimal Rate { get; set; }

		[Required(ErrorMessage =nameof(CurrencyErrors.ToCurrencyRequiredError))]
		[MaxLength(3,ErrorMessage =nameof(CurrencyErrors.InvalidCurrencyLength))]
		[MinLength(3, ErrorMessage = nameof(CurrencyErrors.InvalidCurrencyLength))]
		public string ToCurrency { get; set; }

		[Required(ErrorMessage = nameof(CurrencyErrors.ToCurrencyRequiredError))]
		[MaxLength(3, ErrorMessage = nameof(CurrencyErrors.InvalidCurrencyLength))]
		[MinLength(3, ErrorMessage = nameof(CurrencyErrors.InvalidCurrencyLength))]
		public string FromCurrency { get; set; }

	}
}