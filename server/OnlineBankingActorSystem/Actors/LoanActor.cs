using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Persistence;
using OnlineBankingActorSystem.Helpers.Constants;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.Loan;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
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
	class LoanActor : BaseUntypedPersistentActor, ILogReceive, IWithUnboundedStash
	{
		public new IStash Stash { get; set; }
		public override string PersistenceId => Self.Path.ToString();
		private readonly LoanState _state = new() { PostLoanMessages = new(), GetLoansMessages = new() , Controllers= new()};
		private int _msgsSinceLastSnapshot = 0;
		private readonly IActorRef userIdRetrieverActor = Context.ActorOf(UserIdRetrieverActor.Props());
		private readonly IActorRef transactionStorageActor = Context.ActorOf(TransactionStorageActor.Props());
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props());
		private readonly IActorRef notificationActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<NotificationActor>());



		public LoanActor() : base(nameof(LoanActor))
		{

		}

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case PostLoanRequestMessage postLoanRequestMessage:

					logger.Info($"{ActorName} - {postLoanRequestMessage} received.");
					_state.PostLoanMessages.TryAdd(postLoanRequestMessage.RequestId, postLoanRequestMessage);
					_state.Controllers.TryAdd(postLoanRequestMessage.RequestId, Sender);
					Persist(new PostLoanRequestEventArrived(postLoanRequestMessage.RequestId, postLoanRequestMessage, Sender), (eventArrived) => {
						if (++_msgsSinceLastSnapshot % 100 == 0)
						{
							SaveSnapshot(_state);
							_msgsSinceLastSnapshot = 0;
						}
						logger.Info($"{ActorName}, message persisted {eventArrived}");
						userIdRetrieverActor.Tell(new RetrieveUserId(postLoanRequestMessage.RequestId, postLoanRequestMessage.UserToken), Self);
						BecomeStacked(PostLoan);
					});
					break;
				case GetLoansMessage getLoansMessage:
					logger.Info($"{ActorName} - {getLoansMessage} received.");
					_state.GetLoansMessages.TryAdd(getLoansMessage.RequestId, getLoansMessage);
					_state.Controllers.TryAdd(getLoansMessage.RequestId, Sender);
					Persist(new GetLoansMessageEventArrived(getLoansMessage.RequestId, getLoansMessage, Sender), (eventArrived) =>
					{
						if (++_msgsSinceLastSnapshot % 100 == 0)
						{
							SaveSnapshot(_state);
							_msgsSinceLastSnapshot = 0;
						}
						logger.Info($"{ActorName}, message persisted {eventArrived}");
						userIdRetrieverActor.Tell(new RetrieveUserId(getLoansMessage.RequestId, getLoansMessage.UserToken), Self);
						BecomeStacked(GetLoans);
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

		private void PostLoan(object message)
		{
			switch (message)
			{
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					_state.PostLoanMessages.TryRemove(retrievingUserIdFailed.RequestId, out _);
					_state.Controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					var postMessage = _state.PostLoanMessages[userIdRetrieved.RequestId];
					accountStorageActor.Tell(new GetFromAndToAccount(postMessage.RequestId, userIdRetrieved.UserId, postMessage.FromAccount, postMessage.ReceiveAccount));
					break;
				case RetrievedToAndFromAccounts retrievedToAndFromAccounts:
					logger.Info($"{ActorName} , message received : {retrievedToAndFromAccounts}");
					_state.PostLoanMessages.TryRemove(retrievedToAndFromAccounts.RequestId, out postMessage);
					_state.Controllers.TryRemove(retrievedToAndFromAccounts.RequestId, out controller);
					Persist(new RetrievedToAndFromAccountsArrivedEvent(retrievedToAndFromAccounts.RequestId,controller), (userIdRetrievedEventArrived) => { 
						logger.Info($"{ActorName}, message persisted: {userIdRetrievedEventArrived}");
						notificationActor.Tell(new SendNotification(retrievedToAndFromAccounts.RequestId, retrievedToAndFromAccounts.UserId, 
							NotificationMessages.LoanWaitingForAuthorizationMessage, NotificationMessages.LoanWaitingForAuthroizationTitle, "LoanNotification"),Self);
						transactionStorageActor.Tell(new WriteTransactions(retrievedToAndFromAccounts.RequestId, new List<Transaction> { new Transaction { 
							UserId = retrievedToAndFromAccounts.UserId,
							TransactionType = "loan",
							AccountIban = retrievedToAndFromAccounts.FromAccount.Iban,
							AccountNumber = retrievedToAndFromAccounts.FromAccount.AccountNumber,
							BankIdentifierCode = retrievedToAndFromAccounts.FromAccount.BankIdentifierCode,
							CreationTime = postMessage.StartDate,
							EndTime = postMessage.EndDate,
							TransactionAmount = new Amount{Total =postMessage.Participation.HasValue ? postMessage.TotalAmount - postMessage.Participation.Value : postMessage.TotalAmount
							, Currency = postMessage.Currency },
							TransactionId = Guid.NewGuid(),
							TransactionStatus= TransactionStatus.WAITING_FOR_AUTHORISATION,
							TransactionName = $"Loan requested from account {retrievedToAndFromAccounts.FromAccount.AccountNumber} with amount {postMessage.TotalAmount} ",
							TransactionDetails = $"Loan transaction request from account {retrievedToAndFromAccounts.FromAccount.AccountNumber} to account {retrievedToAndFromAccounts.ToAccount.AccountNumber} " +
							$"with total amount {postMessage.TotalAmount} , participation: {postMessage.Participation} , collaterall: {postMessage.Collateral}  " +
							$"should start: {postMessage.StartDate} and end: {postMessage.EndDate} with the purpose: {postMessage.Purpose}."
						} }), controller);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case CouldNotReadAccounts couldNotReadAccounts:
					logger.Info($"{ActorName} , message received : {couldNotReadAccounts}");
					_state.PostLoanMessages.TryRemove(couldNotReadAccounts.RequestId, out _);
					_state.Controllers.TryRemove(couldNotReadAccounts.RequestId, out controller);
					Persist(new CouldNotReadAccountsArrivedEvent(couldNotReadAccounts.RequestId, couldNotReadAccounts.UserId,couldNotReadAccounts.ErrorMessage, controller), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(couldNotReadAccounts, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case SaveSnapshotSuccess saveSnapshotSuccess:
					DeleteMessages(saveSnapshotSuccess.Metadata.SequenceNr);
					break;
				case SaveSnapshotFailure saveSnapshotFailure:
					logger.Error($"{ActorName}, failed to save snapshot");
					break;
				default:
					Stash.Stash();
					break;
			}
		}

		private void GetLoans(object message) {
			switch (message)
			{
				case RetrievingUserIdFailed retrievingUserIdFailed:
					logger.Info($"{ActorName} , message received : {retrievingUserIdFailed}");
					_state.GetLoansMessages.TryRemove(retrievingUserIdFailed.RequestId, out _);
					_state.Controllers.TryRemove(retrievingUserIdFailed.RequestId, out var controller);
					Persist(new RetrievingUserIdFailedArrivedEvent(retrievingUserIdFailed.RequestId, retrievingUserIdFailed.ErrorMessage), (retrievingUserIdFailedArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted: {retrievingUserIdFailedArrivedEvent}");
						controller.Tell(retrievingUserIdFailed, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case UserIdRetrieved userIdRetrieved:
					logger.Info($"{ActorName} , message received : {userIdRetrieved}");
					_state.GetLoansMessages.TryRemove(userIdRetrieved.RequestId, out var getMessage);
					_state.Controllers.TryRemove(userIdRetrieved.RequestId, out controller);
					Persist(new RetrieveUserIdArrivedEvent(userIdRetrieved.RequestId, userIdRetrieved.Token), (userIdRetrievedEventArrived) => {
						logger.Info($"{ActorName}, message persisted: {userIdRetrievedEventArrived}");
						transactionStorageActor.Tell(new ReadLoanTransactions(userIdRetrieved.RequestId, userIdRetrieved.UserId), controller);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case SaveSnapshotSuccess saveSnapshotSuccess:
					DeleteMessages(saveSnapshotSuccess.Metadata.SequenceNr);
					break;
				case SaveSnapshotFailure saveSnapshotFailure:
					logger.Error($"{ActorName}, failed to save snapshot");
					break;
				default:
					Stash.Stash();
					break;
			}
		}

		protected override void OnRecover(object message)
		{
			switch (message)
			{
				case PostLoanRequestEventArrived postLoanRequestEventArrived:
					logger.Info($"{ActorName}, message recovered : {postLoanRequestEventArrived}");
					_state.PostLoanMessages.TryAdd(postLoanRequestEventArrived.RequestId, postLoanRequestEventArrived.Message);
					_state.Controllers.TryAdd(postLoanRequestEventArrived.RequestId, postLoanRequestEventArrived.Sender);
					break;
				case GetLoansMessageEventArrived getLoansMessageEventArrived:
					logger.Info($"{ActorName}, message recovered : {getLoansMessageEventArrived}");
					_state.GetLoansMessages.TryAdd(getLoansMessageEventArrived.RequestId, getLoansMessageEventArrived.Message);
					_state.Controllers.TryAdd(getLoansMessageEventArrived.RequestId, getLoansMessageEventArrived.Sender);
					break;
				case RetrievingUserIdFailedArrivedEvent retrievingUserIdFailedArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievingUserIdFailedArrivedEvent}");
					_state.GetLoansMessages.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					_state.PostLoanMessages.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					_state.Controllers.TryRemove(retrievingUserIdFailedArrivedEvent.RequestId, out _);
					break;
				case RetrieveUserIdArrivedEvent retrieveUserIdArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrieveUserIdArrivedEvent}");
					_state.GetLoansMessages.TryRemove(retrieveUserIdArrivedEvent.RequestId, out _);
					_state.PostLoanMessages.TryRemove(retrieveUserIdArrivedEvent.RequestId, out _);
					_state.Controllers.TryRemove(retrieveUserIdArrivedEvent.RequestId, out _);
					break;
				case CouldNotReadAccountsArrivedEvent couldNotReadAccountsArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {couldNotReadAccountsArrivedEvent}");
					_state.GetLoansMessages.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					_state.PostLoanMessages.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					_state.Controllers.TryRemove(couldNotReadAccountsArrivedEvent.RequestId, out _);
					break;
				case RetrievedToAndFromAccountsArrivedEvent retrievedToAndFromAccountsArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {retrievedToAndFromAccountsArrivedEvent}");
					_state.GetLoansMessages.TryRemove(retrievedToAndFromAccountsArrivedEvent.RequestId, out _);
					_state.PostLoanMessages.TryRemove(retrievedToAndFromAccountsArrivedEvent.RequestId, out _);
					_state.Controllers.TryRemove(retrievedToAndFromAccountsArrivedEvent.RequestId, out _);
					break;
			}
		}

	

		public static Props Props() => Akka.Actor.Props.Create(() => new LoanActor());
	}

	class LoanState
	{
		public ConcurrentDictionary<ulong, PostLoanRequestMessage> PostLoanMessages { get; set; }
		public ConcurrentDictionary<ulong, GetLoansMessage> GetLoansMessages { get; set; }
		public ConcurrentDictionary<ulong, IActorRef> Controllers { get; set; }
	}
}
