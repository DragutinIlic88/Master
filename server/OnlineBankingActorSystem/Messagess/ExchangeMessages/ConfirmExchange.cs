namespace OnlineBankingActorSystem.Messagess.ExchangeMessages
{

	public record ConfirmExchange(ulong RequestId, string UserToken, string FromAccount, string ToAccount , decimal Amount, string ToCurrency,string FromCurrency, decimal Rate)
	{
		public override string ToString()
		{
			return $"{nameof(ConfirmExchange)} message: requestId {RequestId}, user token: {UserToken} , from account: {FromAccount}, to account: {ToAccount}, amount: {Amount}, to currency: {ToCurrency}, from currency: {FromCurrency}, exchange rate {Rate}";
		}
	}
}
