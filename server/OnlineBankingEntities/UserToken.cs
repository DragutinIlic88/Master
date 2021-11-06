using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingEntitiesLib
{
	public class UserToken
	{
		public string UserID { get; set; }
		public string TokenValue { get; set; }
		public string TokenType { get; set; }
		public string TokenGenerationTime { get; set; }
		public string TokenExpirationTime { get; set; }
		public User User { get; set; }
	}
}
