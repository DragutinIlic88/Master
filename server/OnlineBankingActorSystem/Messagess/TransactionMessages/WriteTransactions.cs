
using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{

	public record WriteTransactions(ulong RequestId, List<Transaction> Transactions)
	{
	
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(RetrievedTransactions)} message: requestId: {RequestId} , {Environment.NewLine}");
			text.Append($"Transactions: {Environment.NewLine}");
			foreach (var transaction in Transactions)
			{
				text.Append($"account number: {transaction.AccountNumber}, transaction bank identifier code: {transaction.BankIdentifierCode}, transaction name: {transaction.TransactionName}, transaction type: {transaction.TransactionType} {Environment.NewLine}");
			}
			return text.ToString();
		}
	
	}
}
