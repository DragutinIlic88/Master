using Akka.Actor;

namespace OnlineBankingActorSystem.Messagess.AccountMessages
{


	public record RetrievedToAndFromAccountsArrivedEvent(ulong RequestId, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(RetrievedToAndFromAccountsArrivedEvent)} message: requestId: {RequestId} , sender: {Sender}";
		}
	}
}
