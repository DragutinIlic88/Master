using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	
	public record FeeRateRetrieved(ulong RequestId, string UserId, string FromCurrency, string ToCurrency, string DateOfInsert, string ExchangeRate)
	{
		public override string ToString()
		{
			return $"{nameof(FeeRateRetrieved)} message, request id: {RequestId}, userId: {UserId},  from curerncy: {FromCurrency} to currency: {ToCurrency}, date of insert: {DateOfInsert}, exchange rate: {ExchangeRate}";
		}
	}
}
