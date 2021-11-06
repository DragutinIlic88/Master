using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingEntitiesLib
{
	public class UserRole
	{
		public string UserID { get; set; }
		public User User { get; set; }
		public string RoleID{get;set;}
		public Role Role { get; set; }
	}


	
}
