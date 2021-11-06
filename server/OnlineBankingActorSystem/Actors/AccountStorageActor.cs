using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.Payment;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class AccountStorageActor : BaseReceiveActor, ILogReceive
	{

		public AccountStorageActor() : base(nameof(AccountStorageActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<ReadAccounts>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadAccountsFromDataBaseAsync(message);
			});

			ReceiveAsync<GetFromAndToAccount>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadFromAndToAccountFromDBAsync(message);
			});

			ReceiveAsync<GetAccountsForPayment>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadPaymentAccountsAsync(message);
			});

			ReceiveAsync<UpdateAccounts>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateAccountsAsync(message);

			});

			ReceiveAsync<RollBackAccounts>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await RollBackAccountAsync(message);
			});
		}

		private async Task ReadAccountsFromDataBaseAsync(ReadAccounts msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingAccountContext = serviceScope.ServiceProvider.GetService<OnlineBankingAccountContext>();
			try
			{
				var accounts = await onlineBankingAccountContext.Accounts.Where(a => a.UserId == msg.UserId).ToListAsync();
				logger.Info($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)}, accounts for user {msg.UserId} are {accounts}");
				Sender.Tell(new RetrievedAccounts(msg.RequestId, msg.UserId, accounts), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)} , accounts could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.AccountsReadFromDataBaseError)),Self);
			}
		}

		private async Task ReadFromAndToAccountFromDBAsync(GetFromAndToAccount msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingAccountContext = serviceScope.ServiceProvider.GetService<OnlineBankingAccountContext>();
			try
			{
				var accounts = await onlineBankingAccountContext.Accounts.Where(a => a.AccountNumber  == msg.FromAccount || a.AccountNumber == msg.ToAccount).ToListAsync();
				logger.Info($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)}, accounts fetched from database are {accounts}");
				if (accounts.Count < 1 || accounts.Count > 2)
				{
					Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.InvalidNumberOfAccountsFetchedFromDataBaseError)), Self);
				}
				else
				{
					var fromAccount = accounts[0].AccountNumber == msg.FromAccount ? accounts[0] : accounts[1];
					var toAccount = accounts[0].AccountNumber == msg.ToAccount ? accounts[0] : accounts[1];
					if (fromAccount.UserId != msg.UserId)
					{
						logger.Warning($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)}, from account is not account of the right user: {fromAccount.UserId} different than {msg.UserId}");
						Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.FromAccountIsNotAccountOfCurrentUser)), Self);
					}
					else {
						Sender.Tell(new RetrievedToAndFromAccounts(msg.RequestId, msg.UserId, fromAccount, toAccount), Self);
					}
				}
				
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)} , accounts could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.AccountsReadFromDataBaseError)),Self);
			}
		}

		private async Task ReadPaymentAccountsAsync(GetAccountsForPayment msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingAccountContext = serviceScope.ServiceProvider.GetService<OnlineBankingAccountContext>();
			try
			{
				var accounts = await onlineBankingAccountContext.Accounts.Where(a => a.AccountNumber == msg.AccountNumber || a.AccountNumber == msg.BeneficiaryAccountNumber).ToListAsync();
				logger.Info($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)}, accounts fetched from database are {accounts}");
				if (accounts.Count <= 0 || accounts.Count >= 3)
				{
					Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.InvalidNumberOfAccountsFetchedFromDataBaseError)), Self);
				}
				else
				{
					var userAccount = accounts[0].AccountNumber == msg.AccountNumber ? accounts[0] : accounts[1];
					if (userAccount.UserId != msg.UserId)
					{
						logger.Warning($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)}, from account is not account of the right user: {userAccount.UserId} different than {msg.UserId}");
						Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.FromAccountIsNotAccountOfCurrentUser)), Self);
					}
					else
					{
						var beneficiaryAccount = accounts.Count == 2 ? accounts[0].AccountNumber == msg.BeneficiaryAccountNumber ? accounts[0] : accounts[1] : null;
						Sender.Tell(new RetrievedAccountsForPayment(msg.RequestId, msg.UserId, userAccount, beneficiaryAccount), Self);
					}
				}

			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadAccountsFromDataBaseAsync)} , accounts could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadAccounts(msg.RequestId, msg.UserId, nameof(AccountsError.AccountsReadFromDataBaseError)), Self);
			}
		}

		private async Task UpdateAccountsAsync(UpdateAccounts msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingAccountContext = serviceScope.ServiceProvider.GetService<OnlineBankingAccountContext>();
			try 
			{
				onlineBankingAccountContext.Accounts.UpdateRange(msg.Accounts);
				var saved = await onlineBankingAccountContext.SaveChangesAsync();
				if (saved == msg.Accounts.Count)
				{
					logger.Info($"{ActorName}, {nameof(UpdateAccountsAsync)}, accounts successfully updated to database");
					Sender.Tell(new AccountsUpdated(msg.RequestId, saved), Self);
				}
				else
				{
					logger.Error($"{ActorName}, {nameof(UpdateAccountsAsync)}, accounts could not be updated to database");
					Sender.Tell(new CouldNotUpdateAccounts(msg.RequestId, nameof(AccountsError.AccountUpdateFromDataBaseError)), Self);
				}
			} catch (Exception e) { 
				logger.Error($"{ActorName}, {nameof(UpdateAccountsAsync)}, accounts could not be updated to database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateAccounts(msg.RequestId, nameof(AccountsError.AccountUpdateFromDataBaseError)), Self);
			}
		}

		private async Task RollBackAccountAsync(RollBackAccounts msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingAccountContext = serviceScope.ServiceProvider.GetService<OnlineBankingAccountContext>();
			try
			{
				onlineBankingAccountContext.Accounts.UpdateRange(msg.Accounts);
				var saved = await onlineBankingAccountContext.SaveChangesAsync();
				logger.Info($"{ActorName}, {nameof(RollBackAccountAsync)}, accounts successfully rolled back in database");
				Sender.Tell(new OperationCouldNotBePerformed(msg.RequestId, nameof(AccountsError.AccountsSuccessfullyRolledBackOperationCancelledError)), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(RollBackAccountAsync)}, accounts could not be rolled back to database due to {e.GetBaseException()}");
				Sender.Tell(new OperationCouldNotBePerformed(msg.RequestId, nameof(AccountsError.AccountsManuallyRolledBackOperationCancelledError)), Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new AccountStorageActor());
		
	}
}
