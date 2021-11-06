
namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record EditUserAddress(ulong RequestId, string UserToken, string Address)
	{
		public override string ToString()
		{
			return $"{nameof(EditUserAddress)} message: request id: {RequestId}, user token: {UserToken}, address: {Address}";
		}
	}
}
