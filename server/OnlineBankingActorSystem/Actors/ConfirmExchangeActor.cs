using Akka.Actor;
using Akka.Persistence;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.ExchangeMessages;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;

using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class ConfirmExchangeActor : BaseUntypedPersistentActor, ILogReceive
	{
		public override string PersistenceId => Self.Path.ToString();
		private ConcurrentDictionary<ulong, IActorRef> controllers = new();
		private ConcurrentDictionary<ulong, ConfirmExchange> confirmExchangeMessageData = new();
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props());
		private readonly IActorRef exchangePerformerActor = Context.ActorOf(ExchangePerformerActor.Props());
		private readonly IActorRef feeStorageActor = Context.ActorOf(FeeStorageActor.Props());
		private int _msgsSinceLastSnapshot = 0;

		public ConfirmExchangeActor() : base(nameof(ConfirmExchangeActor))
		{ }

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case ConfirmExchange confirmExchange:
					logger.Info($"{ActorName}, message received : {confirmExchange}");
					controllers.TryAdd(confirmExchange.RequestId, Sender);
					confirmExchangeMessageData.TryAdd(confirmExchange.RequestId, confirmExchange);
					Persist(new ConfirmExchangeArrivedEvent(confirmExchange.RequestId, confirmExchange, Sender),
						(confirmExchangeArrivedEvent) =>
						{
							if (++_msgsSinceLastSnapshot % 100 == 0)
							{
								SaveSnapshot(new SnapshotType { Controllers =controllers, Messages = confirmExchangeMessageData });
								_msgsSinceLastSnapshot = 0;
							}
							logger.Info($"{ActorName}, message persisted: {confirmExchangeArrivedEvent}");
							userIdRetrieverActor.Tell(new RetrieveUserId(confirmExchangeArrivedEvent.RequestId, confirmExchange.UserToken), Self);
						});
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					confirmExchangeMessageData.TryRemove(retrievingUserIdFailed.RequestId, out _);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					var exchangeMessage = confirmExchangeMessageData[userIdRetrieved.RequestId];
					Persist(new RetrieveUserIdArrivedEvent(userIdRetrieved.RequestId, userIdRetrieved.Token), (retrieveUserIdArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrieveUserIdArrivedEvent}");
						feeStorageActor.Tell(new GetFeeRateFromDB(exchangeMessage.RequestId, userIdRetrieved.UserId, exchangeMessage.FromCurrency, exchangeMessage.ToCurrency, CurrenciesError.CouldNotGetExchangeRateForInsertedCurrencies), Self);
					});
					break;
				case GetFeeRateFromDB getFeeRateFromDB:
					logger.Info($"{ActorName} , message received : {getFeeRateFromDB}");
					controllers.TryRemove(getFeeRateFromDB.RequestId, out controller);
					Persist(new CouldNotGetFeeEventArived(getFeeRateFromDB.RequestId, getFeeRateFromDB.ErrorMessage), (couldNotReadAccountsArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotReadAccountsArrivedEvent}");
						controller.Tell(couldNotReadAccountsArrivedEvent);
					});
					break;
				case FeeRateRetrieved feeRateRetrieved:
					logger.Info($"{ActorName} , message received : {feeRateRetrieved}");
					exchangeMessage = confirmExchangeMessageData[feeRateRetrieved.RequestId];
					if (exchangeMessage.Rate == Decimal.Parse(feeRateRetrieved.ExchangeRate, NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands))
					{
						Persist(new ExchangeRateValidArrivedEvent(feeRateRetrieved.RequestId), (exchangeRateValidArrivedEvent) =>
						{
							logger.Info($"{ActorName}, message persisted: {exchangeRateValidArrivedEvent}");
							accountStorageActor.Tell(new GetFromAndToAccount(exchangeMessage.RequestId, feeRateRetrieved.UserId, exchangeMessage.FromAccount, exchangeMessage.ToAccount), Self);
						});
					}
					else {
						controllers.TryRemove(feeRateRetrieved.RequestId, out controller);
						confirmExchangeMessageData.TryRemove(feeRateRetrieved.RequestId, out _);
						Persist(new ExchangeRateInvalidArrivedEvent(feeRateRetrieved.RequestId,nameof(ExchangeError.ExchangeRateDoesntMatchError) ), (exchangeRateInvalidArrivedEvent) => {
							logger.Info($"{ActorName}, message persisted: {exchangeRateInvalidArrivedEvent}");
							controller.Tell(exchangeRateInvalidArrivedEvent);

						});
					}
					break;
				case CouldNotReadAccounts couldNotReadAccounts:
					logger.Info($"{ActorName} , message received : {couldNotReadAccounts}");
					controllers.TryRemove(couldNotReadAccounts.RequestId, out controller);
					confirmExchangeMessageData.TryRemove(couldNotReadAccounts.RequestId, out _);
					Persist(new CouldNotReadAccountsArrivedEvent(couldNotReadAccounts.RequestId, couldNotReadAccounts.UserId, couldNotReadAccounts.ErrorMessage, controller), (couldNotReadAccountsArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotReadAccountsArrivedEvent}");
						controller.Tell(couldNotReadAccounts);
					});
					break;
				case RetrievedToAndFromAccounts retrievedToAndFromAccounts:
					logger.Info($"{ActorName} , message received : {retrievedToAndFromAccounts}");
					controllers.TryRemove(retrievedToAndFromAccounts.RequestId, out controller);
					Persist(new RetrievedToAndFromAccountsArrivedEvent(retrievedToAndFromAccounts.RequestId, controller), (retrievedToAndFromAccountsArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {retrievedToAndFromAccountsArrivedEvent}");
						
						confirmExchangeMessageData.TryRemove(retrievedToAndFromAccounts.RequestId, out var confirmExchangeMessage);
						if (retrievedToAndFromAccounts.FromAccount.Amount < confirmExchangeMessage.Amount)
						{
							logger.Error($"{ActorName}, insufficient funds in from account where amount needs to be collected ");
							controller.Tell(new ConfirmExchangeFailed(confirmExchangeMessage.RequestId, ExchangeError.NoSufficientFundsError));
						}
						else if (retrievedToAndFromAccounts.ToAccount.Currency.ToUpper() != confirmExchangeMessage.ToCurrency.ToUpper())
						{
							logger.Error($"{ActorName}, chossen currency is not the same as currency of account where funds need to be transfered: account currency {retrievedToAndFromAccounts.ToAccount.Currency}, chosen currency {confirmExchangeMessage.ToCurrency} ");
							controller.Tell(new ConfirmExchangeFailed(confirmExchangeMessage.RequestId, ExchangeError.InvalidCurrencyError));
						}
						else
						{
							exchangePerformerActor.Tell(new PerformExchanging(retrievedToAndFromAccounts.RequestId, retrievedToAndFromAccounts.FromAccount, 
								retrievedToAndFromAccounts.ToAccount, confirmExchangeMessage.Rate, confirmExchangeMessage.Amount, controller), controller);
						}

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
				case ConfirmExchangeArrivedEvent confirmExchangeArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {confirmExchangeArrivedEvent}");
					controllers.TryAdd(confirmExchangeArrivedEvent.RequestId, confirmExchangeArrivedEvent.Sender);
					confirmExchangeMessageData.TryAdd(confirmExchangeArrivedEvent.RequestId, confirmExchangeArrivedEvent.ConfirmExchange);
					break;
				case RetrievingUserIdFailedArrivedEvent retrievingUserIdFailedArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievingUserIdFailedArrivedEvent}");
					controllers.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					confirmExchangeMessageData.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					break;
				case RetrieveUserIdArrivedEvent retrieveUserIdArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrieveUserIdArrivedEvent}");
					break;
				case ExchangeRateValidArrivedEvent exchangeRateValidArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {exchangeRateValidArrivedEvent}");
					break;
				case ExchangeRateInvalidArrivedEvent exchangeRateInvalidArrivedEvent:
					controllers.TryRemove(exchangeRateInvalidArrivedEvent.RequestId, out _);
					confirmExchangeMessageData.TryRemove(exchangeRateInvalidArrivedEvent.RequestId, out _);
					logger.Info($"{ActorName}, message recovered : {exchangeRateInvalidArrivedEvent}");
					break;
				case CouldNotGetFeeEventArived couldNotGetFeeEventArived:
					logger.Info($"{ActorName}, message recovered : {couldNotGetFeeEventArived}");
					controllers.TryRemove(couldNotGetFeeEventArived.RequestId, out _);
					confirmExchangeMessageData.TryRemove(couldNotGetFeeEventArived.RequestId, out _);
					break;
				case CouldNotReadAccountsArrivedEvent couldNotReadAccountsArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {couldNotReadAccountsArrivedEvent}");
					controllers.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					confirmExchangeMessageData.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					break;
				case RetrievedToAndFromAccountsArrivedEvent retrievedToAndFromAccountsArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievedToAndFromAccountsArrivedEvent}");
					controllers.TryRemove(retrievedToAndFromAccountsArrivedEvent.RequestId, out _);
					confirmExchangeMessageData.TryRemove(retrievedToAndFromAccountsArrivedEvent.RequestId, out _);
					break;
				case SnapshotOffer offer:
					logger.Info($"{ActorName}, snapshot offer : {offer}");
					if (offer.Snapshot is SnapshotType snapshotType)
					{
						controllers = new ConcurrentDictionary<ulong, IActorRef>(controllers.Concat(snapshotType.Controllers));
						confirmExchangeMessageData = new ConcurrentDictionary<ulong, ConfirmExchange>(confirmExchangeMessageData.Concat(snapshotType.Messages));
					}
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new ConfirmExchangeActor());

	}

	public class SnapshotType {
		public ConcurrentDictionary<ulong, IActorRef> Controllers { get; set; }
		public ConcurrentDictionary<ulong, ConfirmExchange> Messages { get; set; }
	}
}
