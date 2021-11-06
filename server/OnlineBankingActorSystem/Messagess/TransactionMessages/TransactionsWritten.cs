
namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{

	public record TransactionsWritten(ulong RequestId, int NumberOfTransactions)
	{
		public override string ToString()
		{
			return $"{nameof(TransactionsWritten)} message, request id: {RequestId}, number of transactions written: {NumberOfTransactions}";
		}
	}
}
