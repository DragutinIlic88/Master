using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{
	public record ConfirmExchangeFailed(ulong RequestId,  string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(ConfirmExchangeFailed)} message: requestId {RequestId}, error message: {ErrorMessage}";
		}
	}
}
