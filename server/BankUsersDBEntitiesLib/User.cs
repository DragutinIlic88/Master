using System.Collections.Generic;

namespace BankUsersDBEntitiesLib
{
	public class User
	{

		public string UniquePersonalIdentificationNumber { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string MobileNumber { get; set; }

		public string Email { get; set; }

		public List<UserBankIds> UserBanksIds { get; set; }
	}
}
