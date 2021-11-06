using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record GetFeeRateFromDB(ulong RequestId, string UserId, string FromCurrency, string ToCurrency, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(ReadFees)} message, request id: {RequestId}, userId: {UserId},  from curerncy: {FromCurrency} to currency: {ToCurrency}, error: {ErrorMessage}";
		}
	}
}
