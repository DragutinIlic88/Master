
namespace OnlineBankingActorSystem.Messagess.FeeMessages
{
	public record CouldNotGetFeeEventArived(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotGetFeeEventArived)} message : request id: {RequestId}, error message {ErrorMessage}";
		}
	}
}
