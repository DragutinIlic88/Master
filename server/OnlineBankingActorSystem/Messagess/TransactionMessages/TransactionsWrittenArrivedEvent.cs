
namespace OnlineBankingActorSystem.Messagess.TransactionMessages
{
	public record TransactionsWrittenArrivedEvent (ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(TransactionsWrittenArrivedEvent)} message: requestId: {RequestId}";
		}
	}
}
