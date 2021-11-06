using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Models
{
	public class EditUserLogoModel
	{
		[Required(ErrorMessage = nameof(ProfileErrors.LogoRequiredError))]
		public string LogoName { get; set; }

		[Required(ErrorMessage = nameof(ProfileErrors.LogoRequiredError))]
		public IFormFile LogoFile { get; set; }

		[Required(ErrorMessage = nameof(UserTokenErrors.UserTokenRequiredError))]
		public string UserToken { get; set; }
	}
}
