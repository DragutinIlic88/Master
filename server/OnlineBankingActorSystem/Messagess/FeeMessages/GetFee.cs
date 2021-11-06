using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.FeeMessages
{

	public record GetFee(ulong RequestId, string Token, string FromCurrency, string ToCurrency)
	{
		public override string ToString()
		{
			return $"{nameof(GetFee)} message: requestId: {RequestId} , token: {Token}, from currency: {FromCurrency} to currency {ToCurrency}";
		}
	}
}
