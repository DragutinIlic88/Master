using System;
using System.Collections.Generic;

namespace OnlineBankingEntitiesLib
{
	public class User
	{
		public string UserID { get; set; }

		public string UserName { get; set; }

		public string NormalizedUserName { get; set; }

		public string Email { get; set; }

		public string NormalizedEmail { get; set; }

		public bool EmailConfirmed { get; set; }

		public string PasswordHash { get; set; }

		public string MobileNumber { get; set; }

		public string HomePhoneNumber { get; set; }

		public string HomeAddress { get; set; }

		public string ProfileImagePath { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public string LastModifiedDate { get; set; }

		public string LastLoginDate { get; set; }

		public string RegistrationDate { get; set; }

		public ICollection<UserRole> UserRoles { get; set; }

		public UserToken Token { get; set; }

	}
}
