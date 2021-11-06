using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record CouldNotPerformPayment(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotPerformPayment)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
