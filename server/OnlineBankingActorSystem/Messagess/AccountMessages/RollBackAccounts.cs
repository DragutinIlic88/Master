using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;


namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record RollBackAccounts(ulong RequestId, List<Account> Accounts)
	{
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(RollBackAccounts)} message: requestId: {RequestId} {Environment.NewLine}");
			text.Append($"Accounts: {Environment.NewLine}");
			foreach (var account in Accounts)
			{
				text.Append($"account number: {account.AccountNumber}, account iban: {account.Iban}," +
					$" account type: {account.AccountType}, account amount: {account.Amount}{Environment.NewLine}");
			}
			return text.ToString();
		}
	}
}
