using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingEntitiesLib
{
	public class Role
	{
		public string RoleID { get; set; }
		public string Name { get; set; }
		public string  NormalizedName { get; set; }

		public string CreationDate { get; set; }

		public ICollection<UserRole> UserRoles { get; set; }
	}
}
