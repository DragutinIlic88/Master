using Akka.Actor;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using System.Collections.Concurrent;


namespace OnlineBankingActorSystem.Actors
{
	public class TransactionsGetterActor : BaseUntypedActor, ILogReceive
	{

		private readonly ConcurrentDictionary<ulong, IActorRef> controllers = new();
		private readonly ConcurrentDictionary<ulong, GetTransactions> getTransactionsMessageData = new();
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef transactionStorageActor = Context.ActorOf(TransactionStorageActor.Props());

		public TransactionsGetterActor() : base(nameof(TransactionsGetterActor))
		{}

		protected override void OnReceive(object message)
		{
			//TOOD: think about using persistance actor for every actor which uses dictionaries
			switch (message)
			{
				case GetTransactions getTransactions:
					logger.Info($"{ActorName}, message received: {getTransactions}");
					controllers.TryAdd(getTransactions.RequestId, Sender);
					getTransactionsMessageData.TryAdd(getTransactions.RequestId, getTransactions);
					userIdRetrieverActor.Tell(new RetrieveUserId(getTransactions.RequestId, getTransactions.Token), Self);
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					controllers[retrievingUserIdFailed.RequestId].Tell(retrievingUserIdFailed, Self);
					controllers.TryRemove(retrievingUserIdFailed.RequestId, out _);
					getTransactionsMessageData.TryRemove(retrievingUserIdFailed.RequestId, out _);
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					var getTransactionsMessage = getTransactionsMessageData[userIdRetrieved.RequestId];
					transactionStorageActor.Tell(new ReadTransactions(userIdRetrieved.RequestId, getTransactionsMessage.Token, userIdRetrieved.UserId, getTransactionsMessage.AccountNumber, getTransactionsMessage.Beginning, getTransactionsMessage.TransactionsNumber), controllers[userIdRetrieved.RequestId]);
					controllers.TryRemove(userIdRetrieved.RequestId, out _);
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new TransactionsGetterActor());
		
	}
}
