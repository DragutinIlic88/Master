using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{

	public record CouldNotWriteTransactions(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotWriteTransactions)} message: requestId: {RequestId} , error message {ErrorMessage}";
		}
	}
}

