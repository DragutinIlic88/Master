using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Persistence;
using OnlineBankingActorSystem.Helpers.Constants;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.ExchangeMessages;
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
	public class ExchangePerformerActor : BaseUntypedPersistentActor, ILogReceive
	{
		public override string PersistenceId => Self.Path.ToString();
		private ConcurrentDictionary<ulong, PerformExchanging> performExchangeMessageData = new();
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props(), "accountStorageActor");
		private readonly IActorRef transactionStorageActor = Context.ActorOf(TransactionStorageActor.Props(), "transactionStorageActor");
		private readonly IActorRef notificationActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<NotificationActor>(), "notificationActor");





		private int _msgsSinceLastSnapshot = 0;

		public ExchangePerformerActor() : base(nameof(ExchangePerformerActor))
		{ }

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case PerformExchanging performExchanging:
					logger.Info($"{ActorName}, message received : {performExchanging}");
					performExchangeMessageData.TryAdd(performExchanging.RequestId, performExchanging);
					Persist(new PerformExchangingEventArrived(performExchanging.RequestId, performExchanging), (performExchangingEventArrived) =>
					{
						if (++_msgsSinceLastSnapshot % 100 == 0)
						{
							SaveSnapshot(performExchangeMessageData);
							_msgsSinceLastSnapshot = 0;
						}
						logger.Info($"{ActorName}, message persisted {performExchangingEventArrived}");
						var fromAccount = performExchanging.FromAccount;
						var toAccount = performExchanging.ToAccount;
						var fromAmount = performExchanging.Amount;
						fromAccount.Amount -= fromAmount;
						var rate = performExchanging.ExchangeRate;
						var toAmount = fromAmount * rate;
						toAccount.Amount += performExchanging.Amount * performExchanging.ExchangeRate;
						accountStorageActor.Tell(new UpdateAccounts(performExchanging.RequestId, new List<Account>() { fromAccount, toAccount }), Self);
					});
					break;
				case AccountsUpdated accountsUpdated:
					logger.Info($"{ActorName}, message received : {accountsUpdated}");
					performExchangeMessageData.TryRemove(accountsUpdated.RequestId, out var performExchangingMessage);
					Persist(new AccountsUpdatedEventArrived(accountsUpdated.RequestId), (accountsUpdatedEventArrived) =>
					{
						logger.Info($"{ActorName}, message persisted {accountsUpdatedEventArrived}");
						notificationActor.Tell(new SendNotification(accountsUpdated.RequestId, performExchangingMessage.FromAccount.UserId,
							$"Exchange performed from account {performExchangingMessage.FromAccount.AccountNumber} to account {performExchangingMessage.ToAccount.AccountNumber}. " +
							$"Exchanged amount is {performExchangingMessage.Amount} {performExchangingMessage.FromAccount.Currency.ToUpper()} = { performExchangingMessage.Amount * performExchangingMessage.ExchangeRate} {performExchangingMessage.ToAccount.Currency.ToUpper()}",
							NotificationMessages.ExcangeSuccessTitle, "ExchangeNotification"), Self);
						transactionStorageActor.Tell(new WriteTransactions(performExchangingMessage.RequestId, new List<Transaction>()
						{
							new Transaction{
							AccountIban = performExchangingMessage.FromAccount.Iban,
							AccountNumber = performExchangingMessage.FromAccount.AccountNumber,
							BankIdentifierCode = performExchangingMessage.FromAccount.BankIdentifierCode,
							CreationTime = DateTime.Now,
							TransactionAmount = new Amount{Total =  performExchangingMessage.Amount, Currency = performExchangingMessage.FromAccount.Currency },
							TransactionDetails = $"Exchange transaction where amount {performExchangingMessage.Amount} is removed from account {performExchangingMessage.FromAccount.AccountNumber}",
							TransactionId = Guid.NewGuid(),
							TransactionName = "Exchange transaction",
							TransactionStatus = TransactionStatus.COMPLETED,
							TransactionType = "exchange",
							UserId = performExchangingMessage.FromAccount.UserId
							
							},
							new Transaction{
								AccountIban = performExchangingMessage.ToAccount.Iban,
								AccountNumber = performExchangingMessage.ToAccount.AccountNumber,
								BankIdentifierCode = performExchangingMessage.ToAccount.BankIdentifierCode,
								CreationTime = DateTime.Now,
								TransactionAmount = new Amount{ Total = performExchangingMessage.Amount * performExchangingMessage.ExchangeRate, Currency = performExchangingMessage.ToAccount.Currency},
								TransactionDetails = $"Exchange transaction where amount {performExchangingMessage.Amount * performExchangingMessage.ExchangeRate} is added to account {performExchangingMessage.ToAccount.AccountNumber}",
								TransactionId = Guid.NewGuid(),
								TransactionName = "Exchange transaction",
								TransactionStatus = TransactionStatus.COMPLETED,
								TransactionType = "exchange",
								UserId = performExchangingMessage.FromAccount.UserId
							}
						}), performExchangingMessage.Sender);
					});

					break;
				case CouldNotUpdateAccounts couldNotUpdateAccounts:
					logger.Info($"{ActorName}, message received : {couldNotUpdateAccounts}");
					performExchangeMessageData.TryRemove(couldNotUpdateAccounts.RequestId, out performExchangingMessage);
					Persist(new CouldNotUpdateAccountsEventArrived(couldNotUpdateAccounts.RequestId), (couldNotUpdateAccountsEventArrived) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotUpdateAccountsEventArrived}");
						performExchangingMessage.Sender.Tell(couldNotUpdateAccounts, Self);
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
				case PerformExchangingEventArrived performExchangingEventArrived:
					logger.Info($"{ActorName}, message recovered : {performExchangingEventArrived}");
					performExchangeMessageData.TryAdd(performExchangingEventArrived.RequestId, performExchangingEventArrived.PerformExchanging);
					break;
				case CouldNotUpdateAccountsEventArrived couldNotUpdateAccountsEventArrived:
					logger.Info($"{ActorName}, message recovered : {couldNotUpdateAccountsEventArrived}");
					performExchangeMessageData.TryRemove(couldNotUpdateAccountsEventArrived.RequestId, out _);
					break;
				case AccountsUpdatedEventArrived accountsUpdatedEventArrived:
					logger.Info($"{ActorName}, message recovered : {accountsUpdatedEventArrived}");
					performExchangeMessageData.TryRemove(accountsUpdatedEventArrived.RequestId, out _);
					break;
				case SnapshotOffer offer:
					logger.Info($"{ActorName}, snapshot offer : {offer}");
					if (offer.Snapshot is ConcurrentDictionary<ulong, PerformExchanging> snapshotAsDictionary)
						performExchangeMessageData = new ConcurrentDictionary<ulong, PerformExchanging>(performExchangeMessageData.Concat(snapshotAsDictionary));
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new ExchangePerformerActor());

	}
}
