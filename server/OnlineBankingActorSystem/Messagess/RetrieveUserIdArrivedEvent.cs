namespace OnlineBankingActorSystem.Messagess
{

	public record RetrieveUserIdArrivedEvent(ulong RequestId, string Token)
	{
		public override string ToString()
		{
			return $"{nameof(RetrieveUserIdArrivedEvent)} message: requestId: {RequestId}, error message: {Token}";
		}

	}
}
