using Akka.Actor;
using Akka.Persistence;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.Payment;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using OnlineBankingEntitiesLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Actors
{
	public class PaymentActor : BaseUntypedPersistentActor, ILogReceive
	{
		public override string PersistenceId => Self.Path.ToString();
		private readonly PaymentState state = new() { Controllers = new(), Messages = new() };
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props());
		private readonly IActorRef transactionStorageActor = Context.ActorOf(TransactionStorageActor.Props()); 
		private readonly IActorRef paymentPerformerActor = Context.ActorOf(PaymentPerformerActor.Props());


		private int _msgsSinceLastSnapshot = 0;

		public PaymentActor() :base(nameof(PaymentActor))
		{

		}

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case Pay pay:
					logger.Info($"{ActorName}, message received : {pay}");
					state.Controllers.TryAdd(pay.RequestId, Sender);
					state.Messages.TryAdd(pay.RequestId, pay);
					Persist(new PayArrivedEvent(pay.RequestId, pay, Sender),
						(payArrivedEvent) =>
						{
							if (++_msgsSinceLastSnapshot % 100 == 0)
							{
								SaveSnapshot(state);
								_msgsSinceLastSnapshot = 0;
							}
							logger.Info($"{ActorName}, message persisted: {payArrivedEvent}");
							userIdRetrieverActor.Tell(new RetrieveUserId(pay.RequestId, pay.UserToken), Self);
						});
					break;
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					state.Controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					state.Messages.TryRemove(retrievingUserIdFailed.RequestId, out _);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					var payMessage = state.Messages[userIdRetrieved.RequestId];
					accountStorageActor.Tell(new GetAccountsForPayment(payMessage.RequestId,userIdRetrieved.UserId, payMessage.AccountNumber, payMessage.BenericieryCustomerAccount));
					break;
				case CouldNotReadAccounts couldNotReadAccounts:
					logger.Info($"{ActorName} , message received : {couldNotReadAccounts}");
					state.Controllers.TryRemove(couldNotReadAccounts.RequestId, out controller);
					state.Messages.TryRemove(couldNotReadAccounts.RequestId, out _);
					Persist(new CouldNotReadAccountsArrivedEvent(couldNotReadAccounts.RequestId, couldNotReadAccounts.UserId, couldNotReadAccounts.ErrorMessage, controller), (couldNotReadAccountsArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotReadAccountsArrivedEvent}");
						controller.Tell(couldNotReadAccounts);
					});
					break;
				case RetrievedAccountsForPayment retrievedAccountsForPayment:
					logger.Info($"{nameof(ActorName)}, message recived {retrievedAccountsForPayment}");
					state.Messages.TryRemove(retrievedAccountsForPayment.RequestId, out payMessage);
					state.Controllers.TryRemove(retrievedAccountsForPayment.RequestId, out controller);
					Persist(new RetrievedAccountsForPaymentEventArrived(retrievedAccountsForPayment.RequestId), (retrievedAccountsForPaymentEventArrived) => {
						logger.Info($"{ActorName}, message persisted {retrievedAccountsForPaymentEventArrived}");
						//account where amount need to be payed doesn't belong to this bank
						if (retrievedAccountsForPayment.BeneficieryAccount == null)
						{
							paymentPerformerActor.Tell(new PayOnRemoteAccount(payMessage.RequestId, retrievedAccountsForPayment.UserAccount,
								payMessage.Amount, payMessage.BeneficieryCustomer, payMessage.BenericieryCustomerAccount, payMessage.Model, 
								payMessage.Reference,payMessage.PaymentCode, payMessage.PaymentPurpose, controller), Self);
						}
						//both accounts belongs to the bank
						else
						{
							paymentPerformerActor.Tell(new PayOnLocalAccount(payMessage.RequestId, retrievedAccountsForPayment.UserId, retrievedAccountsForPayment.UserAccount,
								retrievedAccountsForPayment.BeneficieryAccount, payMessage.Amount, payMessage.BeneficieryCustomer, payMessage.Model,
								payMessage.Reference, payMessage.PaymentCode, payMessage.PaymentPurpose, controller), Self);
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
				case PayArrivedEvent payArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {payArrivedEvent}");
					state.Controllers.TryAdd(payArrivedEvent.RequestId, payArrivedEvent.Controller);
					state.Messages.TryAdd(payArrivedEvent.RequestId, payArrivedEvent.Pay);
					break;
				case RetrievingUserIdFailedArrivedEvent retrievingUserIdFailedArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievingUserIdFailedArrivedEvent}");
					state.Controllers.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					state.Messages.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					break;
				case CouldNotReadAccountsArrivedEvent couldNotReadAccountsArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {couldNotReadAccountsArrivedEvent}");
					state.Controllers.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					state.Messages.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					break;
				case CouldNotUpdateAccountsEventArrived couldNotUpdateAccountsEventArrived:
					logger.Info($"{ActorName}, message recovered : {couldNotUpdateAccountsEventArrived}");
					state.Controllers.TryRemove(couldNotUpdateAccountsEventArrived.RequestId, out _);
					state.Messages.TryRemove(couldNotUpdateAccountsEventArrived.RequestId, out _);
					break;
				case RetrievedAccountsForPaymentEventArrived retrievedAccountsForPaymentEventArrived:
					logger.Info($"{ActorName}, message recovered : {retrievedAccountsForPaymentEventArrived}");
					state.Controllers.TryRemove(retrievedAccountsForPaymentEventArrived.RequestId, out _);
					state.Messages.TryRemove(retrievedAccountsForPaymentEventArrived.RequestId, out _);
					break;
				case SnapshotOffer offer:
					logger.Info($"{ActorName}, snapshot offer : {offer}");
					if (offer.Snapshot is PaymentState snapshotType)
					{
						state.Controllers = new ConcurrentDictionary<ulong, IActorRef>(state.Controllers.Concat(snapshotType.Controllers));
						state.Messages = new ConcurrentDictionary<ulong, Pay>(state.Messages.Concat(snapshotType.Messages));
					}
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new PaymentActor());

	}

	public class PaymentState { 
		public ConcurrentDictionary<ulong, IActorRef> Controllers { get; set; }
		public ConcurrentDictionary<ulong, Pay> Messages { get; set; }
	}
}
