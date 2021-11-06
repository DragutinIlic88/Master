

namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record CouldNotWriteTransactionsEventArrived(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotWriteTransactions)} message: requestId: {RequestId}";
		}
	}
}
