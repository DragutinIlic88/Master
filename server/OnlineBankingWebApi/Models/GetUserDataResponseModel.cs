using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Models
{
	public class GetUserDataResponseModel
	{
		public string UserToken { get; set; }

		public string UserName { get; set; }

		public string MobileNumber { get; set; }

		public string Email { get; set; }

		public string HomeAddress { get; set; }

		public string Logo { get; set; }
		public string RegistrationDate { get; set; }
		public string LastLoginDate { get; set; }
	}
}
