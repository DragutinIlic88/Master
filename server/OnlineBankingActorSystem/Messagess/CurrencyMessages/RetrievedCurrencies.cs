using OnlineBankingEntitiesLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess.CurrencyMessages
{
	
	public record RetrievedCurrencies(ulong RequestId, string UserId, List<Currency> Currencies)
	{
		public override string ToString()
		{
			StringBuilder text = new();
			text.Append($"{nameof(RetrievedCurrencies)} message: requestId: {RequestId} , userId: {UserId} {Environment.NewLine}");
			text.Append($"Accounts: {Environment.NewLine}");
			foreach (var currency in Currencies)
			{
				text.Append($"currency id: {currency.CurrencyId}, currency code: {currency.Code}, currency name: {currency.Name} {Environment.NewLine}");
			}
			return text.ToString();
		}
	}
}
