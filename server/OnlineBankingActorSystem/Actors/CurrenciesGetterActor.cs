using Akka.Actor;
using Akka.Persistence;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.CurrencyMessages;
using System.Collections.Concurrent;
using System.Linq;


namespace OnlineBankingActorSystem.Actors
{
	class CurrenciesGetterActor : BaseUntypedPersistentActor
	{
		public override string PersistenceId =>Self.Path.ToString();
		private ConcurrentDictionary<ulong, IActorRef> controllers = new();
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef currenciesStorageActor = Context.ActorOf(CurrenciesStorageActor.Props());
		private int _msgsSinceLastSnapshot = 0;

		public CurrenciesGetterActor() : base(nameof(CurrenciesGetterActor))
		{

		}

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case GetCurrencies getCurrencies:
					logger.Info($"{ActorName}, message received : {getCurrencies}");
					controllers.TryAdd(getCurrencies.RequestId, Sender);
					Persist(new GetCurrenciesArrivedEvent(getCurrencies.RequestId, getCurrencies.Token, Sender),
						(getCurrenciesArrivedEvent)=> {
							if (++_msgsSinceLastSnapshot % 100 == 0)
							{
								SaveSnapshot(controllers);
								_msgsSinceLastSnapshot = 0;
							}
							logger.Info($"{ActorName}, message persisted: {getCurrenciesArrivedEvent}");
							userIdRetrieverActor.Tell(new RetrieveUserId(getCurrenciesArrivedEvent.RequestId, getCurrenciesArrivedEvent.Token), Self);
						});
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					controllers.TryRemove(userIdRetrieved.RequestId, out var removedController);
					Persist(new RetrieveUserIdArrivedEvent(userIdRetrieved.RequestId, userIdRetrieved.Token),(retrieveUserIdArrivedEvent) => { 
						currenciesStorageActor.Tell(new ReadCurrencies(userIdRetrieved.RequestId, userIdRetrieved.UserId), removedController);
					});
					break;
				case SaveSnapshotSuccess saveSnapshotSuccess:
					DeleteMessages(saveSnapshotSuccess.Metadata.SequenceNr);
					break;
				case SaveSnapshotFailure saveSnapshotFailure:
					logger.Error($"{ActorName}, failed to save snapshot");
					break;
			}
		}

		protected override void OnRecover(object message)
		{
			switch (message)
			{
				case GetCurrenciesArrivedEvent getCurrenciesArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {getCurrenciesArrivedEvent}");
					controllers.TryAdd(getCurrenciesArrivedEvent.RequestId, getCurrenciesArrivedEvent.Sender);
					break;
				case RetrievingUserIdFailedArrivedEvent retrievingUserIdFailedArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievingUserIdFailedArrivedEvent}");
					controllers.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					break;
				case RetrieveUserIdArrivedEvent retrieveUserIdArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrieveUserIdArrivedEvent}");
					controllers.TryRemove(retrieveUserIdArrivedEvent.RequestId, out _);
					break;
				case SnapshotOffer offer:
					logger.Info($"{ActorName}, snapshot offer : {offer}");
					if (offer.Snapshot is ConcurrentDictionary<ulong, IActorRef> snapshotAsDictionary)
					 controllers = new ConcurrentDictionary<ulong, IActorRef>(controllers.Concat(snapshotAsDictionary));
					break;
			}
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create(() => new CurrenciesGetterActor());
		}
	}
}
