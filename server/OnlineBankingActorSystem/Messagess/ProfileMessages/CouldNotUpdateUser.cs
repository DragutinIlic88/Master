namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record CouldNotUpdateUser(ulong RequestId, string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotUpdateUser)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
