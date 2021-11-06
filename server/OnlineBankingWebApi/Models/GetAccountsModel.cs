
using System.ComponentModel.DataAnnotations;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public sealed class GetAccountsModel
	{
		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }
	}
}
