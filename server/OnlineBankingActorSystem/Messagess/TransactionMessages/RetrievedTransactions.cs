using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record RetrievedTransactions(ulong RequestId,string UserId, List<Transaction> Transactions)
	{
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(RetrievedTransactions)} message: requestId: {RequestId} , userId: {UserId} {Environment.NewLine}");
			text.Append($"Transactions: {Environment.NewLine}");
			foreach (var transaction in Transactions)
			{
				text.Append($"account number: {transaction.AccountNumber}, transaction bank identifier code: {transaction.BankIdentifierCode}, transaction name: {transaction.TransactionName}, transaction type: {transaction.TransactionType} {Environment.NewLine}");
			}
			return text.ToString();
		}
	}
}
