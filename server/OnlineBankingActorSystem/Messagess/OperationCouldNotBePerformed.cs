
namespace OnlineBankingActorSystem.Messagess
{
	public record OperationCouldNotBePerformed(ulong RequestId, string ErrorMessage ="")
	{
		public override string ToString()
		{
			return $"{nameof(OperationCouldNotBePerformed)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
