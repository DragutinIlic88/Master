using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record ReadTransactions(ulong RequestId, string Token, string UserId, string AccountNumber, int? Beginning, int? TransactionsNumber)
	{
		public override string ToString()
		{
			return $"{nameof(ReadTransactions)} message, request id: {RequestId}, userId: {UserId},  account number: {AccountNumber} beginning: {Beginning ?? 0}, transaction number {TransactionsNumber ?? 0}";
		}
	}
}
