using Akka.Actor;

namespace OnlineBankingActorSystem.Messagess.AccountMessages
{
	public record CouldNotReadAccountsArrivedEvent(ulong RequestId, string UserId, string ErrorMessage, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(CouldNotReadAccountsArrivedEvent)} message: requestId: {RequestId} ,userId {UserId}, error message {ErrorMessage}, sender {Sender}";
		}
	}
}
