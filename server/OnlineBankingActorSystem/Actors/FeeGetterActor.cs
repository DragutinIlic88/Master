using Akka.Actor;
using Akka.Persistence;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using System.Collections.Concurrent;
using System.Linq;

namespace OnlineBankingActorSystem.Actors
{
	class FeeGetterActor : BaseUntypedPersistentActor, ILogReceive
	{
		public override string PersistenceId => Self.Path.ToString();
		private ConcurrentDictionary<ulong, IActorRef> controllers = new();
		private readonly ConcurrentDictionary<ulong, GetFee> getFeeMessageData = new();
		private readonly IActorRef feeStorageActor = Context.ActorOf(FeeStorageActor.Props());
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private int _msgsSinceLastSnapshot = 0;


		public FeeGetterActor() : base(nameof(AccountGetterActor))
		{ }


		public static Props Props() => Akka.Actor.Props.Create(() => new FeeGetterActor());

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case GetFee getFee:
					logger.Info($"{ActorName}, message received : {getFee}");
					controllers.TryAdd(getFee.RequestId, Sender);
					getFeeMessageData.TryAdd(getFee.RequestId, getFee);
					Persist(new GetFeeArrivedEvent(getFee.RequestId, getFee.Token,getFee.FromCurrency,getFee.ToCurrency, Sender),
						(getFeeArrivedEvent) => {
							if (++_msgsSinceLastSnapshot % 100 == 0)
							{
								SaveSnapshot(controllers);
								_msgsSinceLastSnapshot = 0;
							}
							logger.Info($"{ActorName}, message persisted: {getFeeArrivedEvent}");
							userIdRetrieverActor.Tell(new RetrieveUserId(getFeeArrivedEvent.RequestId, getFeeArrivedEvent.Token), Self);
						});
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					getFeeMessageData.TryRemove(retrievingUserIdFailed.RequestId, out _);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					controllers.TryRemove(userIdRetrieved.RequestId, out var removedController);
					getFeeMessageData.TryRemove(userIdRetrieved.RequestId, out var getFeeMessage);
					Persist(new RetrieveUserIdArrivedEvent(userIdRetrieved.RequestId, userIdRetrieved.Token), (retrieveUserIdArrivedEvent) => {
						logger.Info($"{ActorName}, message persisted: {retrieveUserIdArrivedEvent}");
						feeStorageActor.Tell(new ReadFees(getFeeMessage.RequestId, userIdRetrieved.UserId, getFeeMessage.FromCurrency, getFeeMessage.ToCurrency), removedController);
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
				case GetFeeArrivedEvent getFeeArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {getFeeArrivedEvent}");
					controllers.TryAdd(getFeeArrivedEvent.RequestId, getFeeArrivedEvent.Sender);
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
	}
}
