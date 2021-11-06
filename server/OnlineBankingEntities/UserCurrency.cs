using System.Collections.Generic;

namespace OnlineBankingEntitiesLib
{
	public class UserCurrency
	{
		public string UserId { get; set; }
		public string CurrencyId { get; set; }
		public Currency Currency { get; set; }
	}
}
