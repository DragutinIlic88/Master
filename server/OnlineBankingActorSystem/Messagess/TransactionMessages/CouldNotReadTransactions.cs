using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record CouldNotReadTransactions(ulong RequestId, string UserId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotReadTransactions)} message: requestId: {RequestId} , userId: {UserId}, error message {ErrorMessage}";
		}
	}
}
