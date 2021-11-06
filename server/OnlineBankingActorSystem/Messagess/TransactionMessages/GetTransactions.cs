using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record GetTransactions(ulong RequestId, string Token, string AccountNumber, int? Beginning, int? TransactionsNumber)
	{
		public override string ToString()
		{
			return $"{nameof(GetTransactions)} message, request id: {RequestId}, account number: {AccountNumber} beginning: {Beginning ?? 0}, transaction number {TransactionsNumber ?? 0}";
		}
	}
}
