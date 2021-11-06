using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;


namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record RetrievedAccounts(ulong RequestId, string UserId, List<Account> Accounts)
	{
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(RetrievedAccounts)} message: requestId: {RequestId} , userId: {UserId} {Environment.NewLine}");
			text.Append($"Accounts: {Environment.NewLine}");
			foreach(var account in Accounts)
			{
				text.Append($"account number: {account.AccountNumber}, account iban: {account.Iban}, account type: {account.AccountType} {Environment.NewLine}");
			}
			return text.ToString();
		}
	}
}
