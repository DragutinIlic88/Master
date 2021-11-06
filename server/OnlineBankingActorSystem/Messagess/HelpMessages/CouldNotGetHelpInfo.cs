namespace OnlineBankingActorSystem.Messagess.HelpMessages
{

	public record CouldNotGetHelpInfo(ulong RequestId, string ErrorMessage ="")
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotGetHelpInfo)} message: request id: {RequestId}, error message: {ErrorMessage}";
		}
	}
}
