namespace OnlineBankingActorSystem.Messagess.AccountMessages
{

	
	public record CouldNotUpdateAccounts(ulong RequestId,  string ErrorMessage)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotUpdateAccounts)} message: requestId: {RequestId} , error message {ErrorMessage}";
		}
	}
}
