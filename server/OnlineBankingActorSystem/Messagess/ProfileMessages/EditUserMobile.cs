
namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record EditUserMobile(ulong RequestId, string UserToken, string Mobile)
	{
		public override string ToString()
		{
			return $"{nameof(EditUserMobile)} message: request id: {RequestId}, user token: {UserToken}, mobile: {Mobile}";
		}
	}
}
