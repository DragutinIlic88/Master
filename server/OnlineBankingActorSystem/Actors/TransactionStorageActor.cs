using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.Loan;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class TransactionStorageActor : BaseReceiveActor, ILogReceive
	{

		public TransactionStorageActor():base(nameof(TransactionStorageActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<ReadTransactions>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadTransactionsFromDataBase(message);
			});

			ReceiveAsync<ReadLoanTransactions>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadTransactionsWithSpecificType(message, "loan");
			});

			ReceiveAsync<WriteTransactions>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await WriteTransactionsToDataBase(message);
			});
		}

		private async Task ReadTransactionsWithSpecificType(ReadLoanTransactions msg, string transactionType)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingTransactionContext = serviceScope.ServiceProvider.GetService<OnlineBankingTransactionContext>();
			try
			{
				var transactionsQuery = onlineBankingTransactionContext.Transactions.Where(t => t.UserId == msg.UserId && t.TransactionType == transactionType)
							.OrderByDescending(t => t.CreationTime).AsQueryable();
				if (!string.IsNullOrEmpty(msg.AccountNumber))
				{
					transactionsQuery = transactionsQuery.Where(t => t.AccountNumber == msg.AccountNumber);
				}
				var transactions = await transactionsQuery.ToListAsync();
				logger.Info($"{ActorName}, {nameof(ReadTransactionsFromDataBase)},loans  transactions for user {msg.UserId} are {transactions}");
				Sender.Tell(new RetrievedTransactions(msg.RequestId, msg.UserId, transactions), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadTransactionsFromDataBase)} , loans transactions could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadTransactions(msg.RequestId, msg.UserId, nameof(TransactionsError.TransactionsReadFromDataBaseError)), Self);
			}

		}

		private async Task ReadTransactionsFromDataBase(ReadTransactions msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingTransactionContext = serviceScope.ServiceProvider.GetService<OnlineBankingTransactionContext>();
			try
			{
				var transactionsQuery = onlineBankingTransactionContext.Transactions.Where(t => t.UserId == msg.UserId)
							.OrderByDescending(t=>t.CreationTime).AsQueryable();
				if (!string.IsNullOrEmpty(msg.AccountNumber))
				{
					transactionsQuery = transactionsQuery.Where(t => t.AccountNumber == msg.AccountNumber);
				}
				if (msg.Beginning > 0)
				{
					transactionsQuery = transactionsQuery.Skip(msg.Beginning.Value);
				}
				if (msg.TransactionsNumber > 0)
				{
					transactionsQuery = transactionsQuery.Take(msg.TransactionsNumber.Value);
				}
				var transactions  = await transactionsQuery.ToListAsync();
				logger.Info($"{ActorName}, {nameof(ReadTransactionsFromDataBase)}, transactions for user {msg.UserId} are {transactions}");
				Sender.Tell(new RetrievedTransactions(msg.RequestId, msg.UserId, transactions), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadTransactionsFromDataBase)} , transactions could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadTransactions(msg.RequestId, msg.UserId, nameof(TransactionsError.TransactionsReadFromDataBaseError)),Self);
			}
		}

		private async Task WriteTransactionsToDataBase(WriteTransactions msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingTransactionContext = serviceScope.ServiceProvider.GetService<OnlineBankingTransactionContext>();
			try
			{
				onlineBankingTransactionContext.Transactions.AddRange(msg.Transactions);
				var saved = await onlineBankingTransactionContext.SaveChangesAsync();
				if (saved == msg.Transactions.Count)
				{
					logger.Info($"{ActorName}, {nameof(WriteTransactionsToDataBase)}, accounts successfully written to database");
					Sender.Tell(new TransactionsWritten(msg.RequestId, saved), Self);
				}
				else
				{
					logger.Error($"{ActorName}, {nameof(WriteTransactionsToDataBase)}, transactions could not be written to database");
					Sender.Tell(new CouldNotWriteTransactions(msg.RequestId, nameof(TransactionsError.TransactionsWriteToDataBaseError)), Self);
				}
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadTransactionsFromDataBase)} , transactions could not be written to database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotWriteTransactions(msg.RequestId, nameof(TransactionsError.TransactionsWriteToDataBaseError)),Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new TransactionStorageActor());
		
	}
}
