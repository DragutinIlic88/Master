using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class PaymentModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }

		[Required(ErrorMessage =nameof(AccountErrors.AccountNumberRequiredError))]
		public string AccountNumber { get; set; }

		[Required(ErrorMessage = nameof(PaymentErrors.BeneficiaryCustomerRequiredError))]
		public string BeneficiaryCustomer { get; set; }

		[Required(ErrorMessage = nameof(PaymentErrors.BeneficiaryCustomerAccountRequiredError))]
		public string  BeneficiaryCustomerAccount { get; set; }

		[Required(ErrorMessage =nameof(AmountErrors.AmountRequiredError))]
		[Range(0.001, Double.MaxValue, ErrorMessage = nameof(AmountErrors.AmountLessThenZeroError))]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = nameof(CurrencyErrors.CurrencyRequiredError))]
		[MinLength(3,ErrorMessage =nameof(CurrencyErrors.InvalidCurrencyLength))]
		[MaxLength(3,ErrorMessage =nameof(CurrencyErrors.InvalidCurrencyLength))]
		public string Currency { get; set; }

		[Range(0,99,ErrorMessage =nameof(PaymentErrors.ModelOutOfRangeError))]
		public int? Model { get; set; }

		public string Reference { get; set; }

		public string  PaymentCode { get; set; }

		public string  PaymentPurpose { get; set; }

	}
}
