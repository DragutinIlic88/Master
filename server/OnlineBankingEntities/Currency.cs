using System.Collections.Generic;

namespace OnlineBankingEntitiesLib
{
	public class Currency
	{
		public string CurrencyId { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }

		public string Country { get; set; }

		public ICollection<UserCurrency> UserCurrencies { get; set; }
	}
}
