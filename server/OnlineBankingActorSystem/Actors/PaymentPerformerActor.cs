using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Persistence;
using OnlineBankingActorSystem.Helpers.Constants;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using OnlineBankingActorSystem.Messagess.Payment;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using OnlineBankingEntitiesLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class PaymentPerformerActor : BaseUntypedPersistentActor, ILogReceive, IWithUnboundedStash
	{

		public new IStash Stash { get; set; }
		public override string PersistenceId => Self.Path.ToString();
		private readonly PaymentPerformeState _state = new() { LocalAccountMessages = new(), RemoteAccountMessages = new() };
		private int _msgsSinceLastSnapshot = 0;
		private readonly IActorRef accountStorageActor = Context.ActorOf(AccountStorageActor.Props(), "accountStorage");
		private readonly IActorRef transactionStorageActor = Context.ActorOf(TransactionStorageActor.Props(), "transactionStorageActor");
		private readonly IActorRef currencyExchangeRateActor = Context.ActorOf(CurrencyExchangeRateActor.Props(), "currencyExchangeRateActor");
		private readonly IActorRef feeStorageActor = Context.ActorOf(FeeStorageActor.Props(), "feeStorageActor");
		private readonly IActorRef notificationActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<NotificationActor>(), "notificationActor");


		public PaymentPerformerActor() : base(nameof(PaymentPerformerActor))
		{

		}

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case PayOnLocalAccount payOnLocalAccount:
					
					logger.Info($"{ActorName} - {payOnLocalAccount} received.");
					_state.LocalAccountMessages.TryAdd(payOnLocalAccount.RequestId, payOnLocalAccount);
					Persist(new PayOnLocalAccountEventArrived(payOnLocalAccount.RequestId, payOnLocalAccount), (eventArrived)=> {
						if (++_msgsSinceLastSnapshot % 100 == 0)
						{
							SaveSnapshot(_state);
							_msgsSinceLastSnapshot = 0;
						}
						logger.Info($"{ActorName}, message persisted {eventArrived}");
						if (payOnLocalAccount.UserAccount.Amount >= payOnLocalAccount.Amount)
						{
							currencyExchangeRateActor.Tell(new PerformCurrencyExchangeRate(payOnLocalAccount.RequestId, payOnLocalAccount.UserId,
							payOnLocalAccount.UserAccount.Currency, payOnLocalAccount.BeneficieryAccount.Currency), Self);
							BecomeStacked(ProcessLocalPayment);
						}
						else 
						{
							logger.Warning($"{ActorName}, account does not have enough funds to proceed with payment");
							payOnLocalAccount.Sender.Tell(new CouldNotPerformPayment(payOnLocalAccount.RequestId, PaymentError.NoSufficientFundsError));
						}
					});
					break;
				case PayOnRemoteAccount payOnRemoteAccount:
					logger.Info($"{ActorName} - {payOnRemoteAccount} received.");
					_state.RemoteAccountMessages.TryAdd(payOnRemoteAccount.RequestId, payOnRemoteAccount);
					Persist(new PayOnRemoteAccountEventArrived(payOnRemoteAccount.RequestId, payOnRemoteAccount), (eventArrived) =>
					{
						if (++_msgsSinceLastSnapshot % 100 == 0)
						{
							SaveSnapshot(_state);
							_msgsSinceLastSnapshot = 0;
						}
						logger.Info($"{ActorName}, message persisted {eventArrived}");
						var userAccount = payOnRemoteAccount.UserAccount;
						if (userAccount.Amount >= payOnRemoteAccount.Amount)
						{
							userAccount.Amount -= payOnRemoteAccount.Amount;
							accountStorageActor.Tell(new UpdateAccounts(payOnRemoteAccount.RequestId, new List<Account>() { userAccount }), Self);
							BecomeStacked(ProcessRemotePayment);
						}
						else {
							logger.Warning($"{ActorName}, account does not have enough funds to proceed with payment");
							payOnRemoteAccount.Sender.Tell(new CouldNotPerformPayment(payOnRemoteAccount.RequestId, PaymentError.NoSufficientFundsError));
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

		private void ProcessLocalPayment(object message)
		{
			switch (message)
			{
				case FeeRateRetrieved feeRateRetrieved:
					logger.Info($"{ActorName} - {feeRateRetrieved} received");
					var payMessage = _state.LocalAccountMessages[feeRateRetrieved.RequestId];
					var userAccount = payMessage.UserAccount;
					userAccount.Amount -= payMessage.Amount;
					var beneficieryAccount = payMessage.BeneficieryAccount;
					beneficieryAccount.Amount += payMessage.Amount * Decimal.Parse(feeRateRetrieved.ExchangeRate, 
						NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands);
					accountStorageActor.Tell(new UpdateAccounts(feeRateRetrieved.RequestId, new List<Account>() { userAccount , beneficieryAccount}), Self);
					break;
				case GetFeeRateFromDB getFeeRateFromDB:
					logger.Info($"{ActorName} -- {getFeeRateFromDB} received");
					feeStorageActor.Tell(new GetPaymentFeeRateFromDB(getFeeRateFromDB.RequestId,  getFeeRateFromDB.FromCurrency, getFeeRateFromDB.ToCurrency), Self);
					break;
				case CouldNotRetrieveExchangeRateFromDB couldNotRetrieveExchangeRateFromDB:
					logger.Info($"{ActorName} -- {couldNotRetrieveExchangeRateFromDB} received");
					_state.LocalAccountMessages.TryRemove(couldNotRetrieveExchangeRateFromDB.RequestId, out payMessage);
					Persist(new CouldNotRetrieveExchangeRateFromDBEventArrived(couldNotRetrieveExchangeRateFromDB.RequestId), (couldNotUpdateAccountsEventArrived) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotUpdateAccountsEventArrived}");
						payMessage.Sender.Tell(couldNotRetrieveExchangeRateFromDB, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case CouldNotUpdateAccounts couldNotUpdateAccounts:
					logger.Info($"{ActorName} - {couldNotUpdateAccounts} received.");
					_state.LocalAccountMessages.TryRemove(couldNotUpdateAccounts.RequestId, out payMessage);
					Persist(new CouldNotUpdateAccountsEventArrived(couldNotUpdateAccounts.RequestId), (couldNotUpdateAccountsEventArrived) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotUpdateAccountsEventArrived}");
						payMessage.Sender.Tell(couldNotUpdateAccounts, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case AccountsUpdated accountsUpdated:
					logger.Info($"{ActorName} - {accountsUpdated} received.");
					payMessage = _state.LocalAccountMessages[accountsUpdated.RequestId];
					transactionStorageActor.Tell(new WriteTransactions(accountsUpdated.RequestId, new List<Transaction>{new Transaction {
							AccountIban = payMessage.UserAccount.Iban,
							AccountNumber = payMessage.UserAccount.AccountNumber,
							BankIdentifierCode = payMessage.UserAccount.BankIdentifierCode,
							CreationTime = DateTime.Now,
							TransactionAmount = new Amount{ Total = payMessage.Amount, Currency = payMessage.UserAccount.Currency},
							TransactionType = "payment",
							TransactionId = Guid.NewGuid(),
							TransactionStatus = TransactionStatus.COMPLETED,
							UserId = payMessage.UserAccount.UserId,
							TransactionDetails = $"Payment transaction where amount {payMessage.Amount} will be removed from the account {payMessage.UserAccount.AccountNumber} " +
							$"and  payed to the beneficiary {payMessage.BeneficiaryCustomer}" +
							$" on account {payMessage.BeneficieryAccount.AccountNumber} due to purpose: {payMessage.PaymentPurpose}, " +
							$"with refernce {payMessage.Reference ?? "no-refrence"}, and code {payMessage.PaymentCode ?? "no-code"}",
							TransactionName = $"Payment transaction from account {payMessage.UserAccount.AccountNumber} to account {payMessage.BeneficieryAccount.AccountNumber}"
						}, new Transaction{
							AccountIban = payMessage.BeneficieryAccount.Iban,
							AccountNumber = payMessage.BeneficieryAccount.AccountNumber,
							BankIdentifierCode = payMessage.BeneficieryAccount.BankIdentifierCode,
							CreationTime = DateTime.Now,
							TransactionAmount = new Amount{ Total = payMessage.Amount,Currency=payMessage.UserAccount.Currency},
							TransactionType = "payment",
							TransactionId = Guid.NewGuid(),
							TransactionStatus = TransactionStatus.COMPLETED,
							UserId = payMessage.BeneficieryAccount.UserId,
							TransactionDetails = $"Payment transaction where amount {payMessage.Amount} will be removed from the account {payMessage.UserAccount.AccountNumber} " +
							$"converted if necessary to the {payMessage.BeneficieryAccount.Currency} currency " +
							$"and  payed to the beneficiary {payMessage.BeneficiaryCustomer}" +
							$" on account {payMessage.BeneficieryAccount.AccountNumber} due to purpose: {payMessage.PaymentPurpose}, " +
							$"with refernce {payMessage.Reference ?? "no-refrence"}, and code {payMessage.PaymentCode ?? "no-code"}",
							TransactionName = $"Payment transaction from account {payMessage.UserAccount.AccountNumber} to account {payMessage.BeneficieryAccount.AccountNumber}"
						} 
					}));
					break;
				case TransactionsWritten transactionsWritten:
					logger.Info($"{ActorName} - {transactionsWritten} received.");
					_state.LocalAccountMessages.TryRemove(transactionsWritten.RequestId, out payMessage);
					Persist(new TransactionsWrittenArrivedEvent(transactionsWritten.RequestId), (transactionsWrittenArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {transactionsWrittenArrivedEvent}");
						notificationActor.Tell(new SendNotification(transactionsWritten.RequestId, payMessage.UserId,
							NotificationMessages.PaymentSuccessMessage, NotificationMessages.PaymentSuccessTitle, "PaymentNotification"), Self);
						payMessage.Sender.Tell(transactionsWrittenArrivedEvent, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case CouldNotWriteTransactions couldNotWriteTransactions:
					logger.Info($"{ActorName} - {couldNotWriteTransactions} received.");
					_state.LocalAccountMessages.TryRemove(couldNotWriteTransactions.RequestId, out payMessage);
					Persist(new CouldNotWriteTransactionsEventArrived(payMessage.RequestId), (couldNotWriteTransactionsEventArrived) => {
						logger.Info($"{ActorName}, message persisted {couldNotWriteTransactionsEventArrived}");
						payMessage.UserAccount.Amount += payMessage.Amount;
						accountStorageActor.Tell(new RollBackAccounts(payMessage.RequestId, new() { payMessage.UserAccount }), payMessage.Sender);
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

		private void ProcessRemotePayment(object message)
		{
			switch (message)
			{ 
				case AccountsUpdated accountsUpdated:
					logger.Info($"{ActorName} - {accountsUpdated} received.");
					var payMessage = _state.RemoteAccountMessages[accountsUpdated.RequestId];
					transactionStorageActor.Tell(new WriteTransactions(accountsUpdated.RequestId, new List<Transaction>{new Transaction {
							AccountIban = payMessage.UserAccount.Iban,
							AccountNumber = payMessage.UserAccount.AccountNumber,
							BankIdentifierCode = payMessage.UserAccount.BankIdentifierCode,
							CreationTime = DateTime.Now,
							TransactionAmount = new Amount{ Total = payMessage.Amount, Currency = payMessage.UserAccount.Currency},
							TransactionType = "payment",
							TransactionId = Guid.NewGuid(),
							TransactionStatus = TransactionStatus.AUTHORISED,
							UserId = payMessage.UserAccount.UserId,
							TransactionDetails = $"Payment transaction where amount {payMessage.Amount} will be payed to the beneficiary {payMessage.BeneficiaryCustomer}" +
							$" on account {payMessage.BeneficiaryCustomerAccount} due to purpose: {payMessage.PaymentPurpose}, " +
							$"with refernce {payMessage.Reference ?? "no-refrence"}, and code {payMessage.PaymentCode ?? "no-code"}",
							TransactionName = $"Payment transaction from account {payMessage.UserAccount.AccountNumber} to account {payMessage.BeneficiaryCustomerAccount}"
						}}));
					break;
				case CouldNotUpdateAccounts couldNotUpdateAccounts:
					logger.Info($"{ActorName} - {couldNotUpdateAccounts} received.");
					_state.RemoteAccountMessages.TryRemove(couldNotUpdateAccounts.RequestId, out payMessage);
					Persist(new CouldNotUpdateAccountsEventArrived(couldNotUpdateAccounts.RequestId), (couldNotUpdateAccountsEventArrived) =>
					{
						logger.Info($"{ActorName}, message persisted {couldNotUpdateAccountsEventArrived}");
						payMessage.Sender.Tell(couldNotUpdateAccounts, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case TransactionsWritten transactionsWritten:
					logger.Info($"{ActorName} - {transactionsWritten} received.");
					_state.RemoteAccountMessages.TryRemove(transactionsWritten.RequestId, out payMessage);
					Persist(new TransactionsWrittenArrivedEvent(transactionsWritten.RequestId), (transactionsWrittenArrivedEvent) =>
					{
						logger.Info($"{ActorName}, message persisted {transactionsWrittenArrivedEvent}");
						notificationActor.Tell(new SendNotification(transactionsWritten.RequestId, payMessage.UserAccount.UserId,
							NotificationMessages.PaymentSuccessMessage, NotificationMessages.PaymentSuccessTitle, "PaymentNotification"), Self);
						payMessage.Sender.Tell(transactionsWrittenArrivedEvent, Self);
						Stash.UnstashAll();
						UnbecomeStacked();
					});
					break;
				case CouldNotWriteTransactions couldNotWriteTransactions:
					logger.Info($"{ActorName} - {couldNotWriteTransactions} received.");
					_state.RemoteAccountMessages.TryRemove(couldNotWriteTransactions.RequestId, out payMessage);
					Persist(new CouldNotWriteTransactionsEventArrived(payMessage.RequestId), (couldNotWriteTransactionsEventArrived) => { 
						logger.Info($"{ActorName}, message persisted {couldNotWriteTransactionsEventArrived}");
						payMessage.UserAccount.Amount += payMessage.Amount;
						accountStorageActor.Tell(new RollBackAccounts(payMessage.RequestId, new() { payMessage.UserAccount }), payMessage.Sender);
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
				case PayOnRemoteAccountEventArrived payOnRemoteAccountEventArrived:
					logger.Info($"{ActorName}, message recovered : {payOnRemoteAccountEventArrived}");
					_state.RemoteAccountMessages.TryAdd(payOnRemoteAccountEventArrived.RequestId, payOnRemoteAccountEventArrived.Message);
					break;
				case PayOnLocalAccountEventArrived payOnLocalAccountEventArrived:
					logger.Info($"{ActorName}, message recovered : {payOnLocalAccountEventArrived}");
					_state.LocalAccountMessages.TryAdd(payOnLocalAccountEventArrived.RequestId, payOnLocalAccountEventArrived.Message);
					break;
				case CouldNotUpdateAccountsEventArrived couldNotUpdateAccountsEventArrived:
					logger.Info($"{ActorName}, message recovered : {couldNotUpdateAccountsEventArrived}");
					_state.RemoteAccountMessages.TryRemove(couldNotUpdateAccountsEventArrived.RequestId, out _);
					break;
				case TransactionsWrittenArrivedEvent transactionsWrittenArrivedEvent:
					logger.Info($"{ActorName}, message recovered : {transactionsWrittenArrivedEvent}");
					_state.RemoteAccountMessages.TryRemove(transactionsWrittenArrivedEvent.RequestId, out _);
					break;
				case CouldNotWriteTransactionsEventArrived couldNotWriteTransactions:
					logger.Info($"{ActorName}, message recovered : {couldNotWriteTransactions}");
					_state.RemoteAccountMessages.TryRemove(couldNotWriteTransactions.RequestId, out _);
					break;
				case SnapshotOffer offer:
					logger.Info($"{ActorName}, snapshot offer : {offer}");
					if (offer.Snapshot is PaymentPerformeState snapshotType)
					{
						_state.LocalAccountMessages = new ConcurrentDictionary<ulong, PayOnLocalAccount>(_state.LocalAccountMessages.Concat(snapshotType.LocalAccountMessages));
						_state.RemoteAccountMessages = new ConcurrentDictionary<ulong, PayOnRemoteAccount>(_state.RemoteAccountMessages.Concat(snapshotType.RemoteAccountMessages));
					}
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new PaymentPerformerActor());

	}

	class PaymentPerformeState
	{
		public ConcurrentDictionary<ulong, PayOnLocalAccount> LocalAccountMessages { get; set; }
		public ConcurrentDictionary<ulong, PayOnRemoteAccount> RemoteAccountMessages { get; set; }
	}
}
