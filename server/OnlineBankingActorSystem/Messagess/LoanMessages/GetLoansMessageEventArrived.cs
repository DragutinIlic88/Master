using Akka.Actor;

namespace OnlineBankingActorSystem.Messagess.Loan
{
	public record GetLoansMessageEventArrived(ulong RequestId, GetLoansMessage Message, IActorRef Sender)
	{
		public override string ToString()
		{
			return $"{nameof(GetLoansMessageEventArrived)} message: request id: {RequestId}, get loans message: {Message}, sender: {Sender}";
		}
	}
}
