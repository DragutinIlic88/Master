namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record GetUserData(ulong RequestId, string UserToken)
	{
		public override string ToString()
		{
			return $"{nameof(GetUserData)}, message: request id: {RequestId}, user token: {UserToken}";
		}
	}
}
