using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public sealed class GetTransactionsModel
	{
		[Required(ErrorMessage =nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }
		public string AccountNumber { get; set; }
		public int? Beginning { get; set; }
		public int? TransactionsNumber { get; set; }
	}
}
