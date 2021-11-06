using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OnlineBankingEntitiesLib
{
	public class ExchangeRate
	{
		public string FromCurrencyId { get; set; }
		public string ToCurrencyId { get; set; }
		public string  DateOfInsert { get; set; }
		public string Rate { get; set; }
		public virtual Currency FromCurrency { get; set; }
		public virtual Currency ToCurrency { get; set; }

	}
}
