using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.CurrencyMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class CurrenciesStorageActor : BaseReceiveActor, ILogReceive
	{
		public CurrenciesStorageActor() : base(nameof(CurrenciesStorageActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<ReadCurrencies>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await ReadCurrenciesFromDataBase(message);
			});
		}

		private async Task ReadCurrenciesFromDataBase(ReadCurrencies msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingCurrencyContext = serviceScope.ServiceProvider.GetService<OnlineBankingCurrencyContext>();
			try
			{
				var currencies = await onlineBankingCurrencyContext.UserCurrencies.Include(uc => uc.Currency)
					.Where(uc => uc.UserId == msg.UserId)
					.Select(uc => uc.Currency)
					.ToListAsync();
				var retriievedCurrenciesMessage = new RetrievedCurrencies(msg.RequestId, msg.UserId, currencies);
				logger.Info($"{ActorName}, {nameof(ReadCurrenciesFromDataBase)}, currencies are retrieved successfully and will be sent as part of {retriievedCurrenciesMessage}");
				Sender.Tell(retriievedCurrenciesMessage, Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, {nameof(ReadCurrenciesFromDataBase)} , currencies could not be retrieved from database due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotReadCurrenices(msg.RequestId, msg.UserId, nameof(CurrenciesError.CurrenciesReadFromDataBaseError)));
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new CurrenciesStorageActor());

	}
}
