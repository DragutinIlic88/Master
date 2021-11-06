using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using OnlineBankingEntitiesLib;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class FeeStorageActor : BaseReceiveActor, ILogReceive
	{

		private readonly IActorRef currencyExchangeRateActor = Context.ActorOf(CurrencyExchangeRateActor.Props(), "currencyExchangeRateActor");
		private readonly ConcurrentDictionary<ulong, IActorRef> controllers = new();

		public FeeStorageActor() : base(nameof(FeeStorageActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			Receive<ReadFees>(message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				controllers.TryAdd(message.RequestId, Sender);
				currencyExchangeRateActor.Tell(new PerformCurrencyExchangeRate(message.RequestId, message.UserId, message.FromCurrency, message.ToCurrency), Self);

			});
			ReceiveAsync<FeeRateRetrieved>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await StoreAndSendFee(message);

			});

			ReceiveAsync<GetFeeRateFromDB>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await GetFeeFromDBAndSend(message);

			});

			ReceiveAsync<GetPaymentFeeRateFromDB>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await GetFeeForPaymentFromDBAsync(message);
			});

			Receive<object>(message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");

			});
		}

		private async Task StoreAndSendFee(FeeRateRetrieved message)
		{

			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingCurrencyContext = serviceScope.ServiceProvider.GetService<OnlineBankingCurrencyContext>();
			var fromCurrency = await onlineBankingCurrencyContext.Currencies.SingleOrDefaultAsync(c => c.Code == message.FromCurrency);
			var toCurrency = await onlineBankingCurrencyContext.Currencies.SingleOrDefaultAsync(c => c.Code == message.ToCurrency);
			var existingChangeRate = await onlineBankingCurrencyContext.ExchangeRates
				.SingleOrDefaultAsync(er => er.FromCurrencyId == fromCurrency.CurrencyId && er.ToCurrencyId == toCurrency.CurrencyId);

			if (existingChangeRate == null)
			{
				var exchangeRate = new ExchangeRate
				{
					FromCurrencyId = fromCurrency.CurrencyId,
					ToCurrencyId = toCurrency.CurrencyId,
					DateOfInsert = message.DateOfInsert,
					Rate = message.ExchangeRate,
					FromCurrency = fromCurrency,
					ToCurrency = toCurrency
				};

				onlineBankingCurrencyContext.Add(exchangeRate);
				var savedExchangeRate = await onlineBankingCurrencyContext.SaveChangesAsync();
				if (savedExchangeRate != 0)
				{
					logger.Info($"{ActorName}, exchange rate is successfully added to the database");
				}
				else
				{
					logger.Error($"{ActorName}, exchange rate is not added to the database");
				}
			}
			else
			{
				existingChangeRate.DateOfInsert = message.DateOfInsert;
				existingChangeRate.Rate = message.ExchangeRate;
				onlineBankingCurrencyContext.Update(existingChangeRate);
				var savedExchangeRate = await onlineBankingCurrencyContext.SaveChangesAsync();
				if (savedExchangeRate != 0)
				{
					logger.Info($"{ActorName}, exchange rate is successfully updated to the database");
				}
				else
				{
					logger.Error($"{ActorName}, exchange rate is not updated to the database");
				}
			}

			controllers.TryRemove(message.RequestId, out var controller);
			controller.Tell(message, Self);
		}

		private async Task GetFeeFromDBAndSend(GetFeeRateFromDB message)
		{
			var existingChangeRate =await  GetExchangeRateAsync(message.FromCurrency, message.ToCurrency);
			controllers.TryRemove(message.RequestId, out var controller);
			if (existingChangeRate == null)
			{
				logger.Info($"{ActorName}, {nameof(GetFeeFromDBAndSend)} fee could not bee retrieved from third party nor from DB");
				if (controller != null)
				{
					controller.Tell(message, Self);
				}
				else {
					Sender.Tell(message, Self);
				}
			}
			else
			{
				logger.Info($"{ActorName}, {nameof(GetFeeFromDBAndSend)} fee is fetched from data base");
				if (controller != null)
				{
					controller.Tell(new FeeRateRetrieved(message.RequestId, message.UserId,
						existingChangeRate.FromCurrency.Code, existingChangeRate.ToCurrency.Code, existingChangeRate.DateOfInsert, existingChangeRate.Rate), Self);
				}
				else {
					Sender.Tell(new FeeRateRetrieved(message.RequestId, message.UserId,
						existingChangeRate.FromCurrency.Code, existingChangeRate.ToCurrency.Code, existingChangeRate.DateOfInsert, existingChangeRate.Rate), Self);
				}
			}
		}

		private async Task GetFeeForPaymentFromDBAsync(GetPaymentFeeRateFromDB message)
		{
			var exchangeRate = await GetExchangeRateAsync(message.FromCurrency, message.ToCurrency);
			if (exchangeRate == null)
			{
				logger.Info($"{ActorName}, {nameof(GetFeeForPaymentFromDBAsync)} exchange rate could not bee retrieved from data base");
				Sender.Tell(new CouldNotRetrieveExchangeRateFromDB(message.RequestId, ExchangeError.GetExchangeRateError));
			}
			else
			{
				logger.Info($"{ActorName}, {nameof(GetFeeForPaymentFromDBAsync)} efee is fetched from data base");
				Sender.Tell(new FeeRateRetrieved(message.RequestId, "", exchangeRate.FromCurrency.Code, exchangeRate.ToCurrency.Code, 
					exchangeRate.DateOfInsert, exchangeRate.Rate), Self);
			}
		}

		private async Task<ExchangeRate> GetExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingCurrencyContext = serviceScope.ServiceProvider.GetService<OnlineBankingCurrencyContext>();
			var fromCurrency = await onlineBankingCurrencyContext.Currencies.SingleOrDefaultAsync(c => c.Code == fromCurrencyCode);
			var toCurrency = await onlineBankingCurrencyContext.Currencies.SingleOrDefaultAsync(c => c.Code == toCurrencyCode);
			var existingChangeRate = await onlineBankingCurrencyContext.ExchangeRates.Include(er => er.FromCurrency).Include(er => er.ToCurrency)
				.SingleOrDefaultAsync(er => er.FromCurrencyId == fromCurrency.CurrencyId && er.ToCurrencyId == toCurrency.CurrencyId);

			return existingChangeRate;
		}


		public static Props Props() => Akka.Actor.Props.Create(() => new FeeStorageActor());

	}
}
