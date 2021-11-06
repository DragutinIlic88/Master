using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class LoanModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }
		[Required(ErrorMessage = nameof(AccountErrors.FromAccountNumberRequiredError))]
		public string FromAccount { get; set; }
		[Required(ErrorMessage = nameof(AccountErrors.ToAccountNumberRequredError))]
		public string ReceiveAccount { get; set; }
		[Required(ErrorMessage = nameof(AmountErrors.AmountRequiredError))]
		[Range(0.001, Double.MaxValue, ErrorMessage = nameof(AmountErrors.AmountLessThenZeroError))]
		public decimal TotalAmount { get; set; }
		public decimal? Participation { get; set; }
		[Required(ErrorMessage = nameof(CurrencyErrors.CurrencyRequiredError))]
		[MinLength(3, ErrorMessage = nameof(CurrencyErrors.InvalidCurrencyLength))]
		[MaxLength(3, ErrorMessage = nameof(CurrencyErrors.InvalidCurrencyLength))]
		public string Currency { get; set; }
		[Required(ErrorMessage = nameof(LoanErrors.StartDateRequiredError))]
		public DateTime StartDate { get; set; }
		[Required(ErrorMessage = nameof(LoanErrors.EndDateRequiredError))]
		public DateTime EndDate { get; set; }
		[Required(ErrorMessage = nameof(LoanErrors.PurposeRequiredError))]
		public string Purpose { get; set; }
		public string Collateral { get; set; }
	}
}
