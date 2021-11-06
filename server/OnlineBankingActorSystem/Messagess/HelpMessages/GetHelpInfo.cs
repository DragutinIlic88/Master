namespace OnlineBankingActorSystem.Messagess.HelpMessages
{
	public record GetHelpInfo(ulong RequestId)
	{
		public override string ToString()
		{
			return $"{nameof(GetHelpInfo)} message: request id: {RequestId}";
		}
	}
}
