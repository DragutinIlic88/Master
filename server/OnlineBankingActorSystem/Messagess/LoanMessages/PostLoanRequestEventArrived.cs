using Akka.Actor;

namespace OnlineBankingActorSystem.Messagess.Loan
{
	public record PostLoanRequestEventArrived(ulong RequestId, PostLoanRequestMessage Message, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(PostLoanRequestEventArrived)} message: request id: {RequestId}, post loan message: {Message}, sender: {Sender}";
		}
	}
}
