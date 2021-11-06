using Akka.Actor;
namespace OnlineBankingActorSystem.Messagess.Payment
{
	public record PayArrivedEvent(ulong RequestId, Pay Pay, IActorRef Controller)
	{
		public override string ToString()
		{
			return $"{nameof(PayArrivedEvent)} message: request id: {RequestId}, pay message: {Pay}, sender: {Controller}";
		}
	}
}
