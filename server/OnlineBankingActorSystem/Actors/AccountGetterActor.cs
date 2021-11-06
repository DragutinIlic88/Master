using Akka.Actor;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using System.Collections.Concurrent;


namespace OnlineBankingActorSystem.Actors
{
	public class AccountGetterActor : BaseUntypedActor, ILogReceive
	{

		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props());
		private readonly ConcurrentDictionary<ulong, IActorRef> controllers = new();

		public AccountGetterActor() :base(nameof(AccountGetterActor))
		{}

		protected override void OnReceive(object message)
		{
			switch (message) {
				case GetAccounts getAccounts:
					logger.Info($"{ActorName} , message received : {getAccounts}");
					controllers.TryAdd(getAccounts.RequestId, Sender);
					userIdRetrieverActor.Tell(new RetrieveUserId(getAccounts.RequestId, getAccounts.Token), Self);
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					controllers[retrievingUserIdFailed.RequestId].Tell(retrievingUserIdFailed, Self);
					controllers.TryRemove(retrievingUserIdFailed.RequestId, out _);
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					accountStorageActor.Tell(new ReadAccounts(userIdRetrieved.RequestId, userIdRetrieved.UserId),controllers[userIdRetrieved.RequestId]);
					controllers.TryRemove(userIdRetrieved.RequestId, out _);
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new AccountGetterActor());
		
	}
}
