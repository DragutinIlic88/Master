

namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{

	public record UserEdited(ulong RequestId, string Message = null)
	{
		public override string ToString()
		{
			return $"{nameof(UserEdited)} message: request id: {RequestId}, message: {Message}";
		}
	}
}
