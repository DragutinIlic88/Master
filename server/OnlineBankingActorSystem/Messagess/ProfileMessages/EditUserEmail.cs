namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record EditUserEmail(ulong RequestId, string UserToken, string Email)
	{
		public override string ToString()
		{
			return $"{nameof(EditUserEmail)} message: request id: {RequestId}, user token: {UserToken}, email: {Email}";
		}
	}
}
