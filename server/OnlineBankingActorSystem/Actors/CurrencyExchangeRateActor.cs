using Akka.Actor;
using Akka.Pattern;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class CurrencyExchangeRateActor : BaseUntypedActor, ILogReceive
	{

		private static readonly string ApiHost = @"https://www.alphavantage.co/";
		private static readonly string ExchangeRateThirdPartyUri = @$"https://{ApiHost}/";
		private static readonly string ApiKey = "LSPCI3TZ628WW6E8";
		public CircuitBreaker Breaker { get; }

		public CurrencyExchangeRateActor() : base(nameof(CurrencyExchangeRateActor))
		{
			Breaker = new CircuitBreaker(
			Context.System.Scheduler,
			maxFailures: 5,
			callTimeout: TimeSpan.FromSeconds(10),
			resetTimeout: TimeSpan.FromMinutes(1)).OnOpen(NotifyMeOnOpen);
		}
		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case PerformCurrencyExchangeRate performCurrencyExchangeRate:
					logger.Info($"{ActorName}, received {performCurrencyExchangeRate}");
					ReadFeeFromAPI(performCurrencyExchangeRate);
					break;
			}
		}

		private void ReadFeeFromAPI(PerformCurrencyExchangeRate msg)
		{
			var uriString = @$"{ApiHost}query?function=CURRENCY_EXCHANGE_RATE&from_currency={msg.FromCurrency}&to_currency={msg.ToCurrency}&apikey={ApiKey}";
			Uri queryUri = new(uriString);
			using WebClient client = new();
			try
			{
				logger.Info($"{ActorName}, {nameof(ReadFeeFromAPI)} entering circuit breaker");
				//CircuitBreaker is used as a guardian when third party is not responsive for some reason
				//it will switch to open state and immediately return error. Open state will last one minute.
				//After that it will switch to half open state and try to send request to third party.
				//If it succeed state will become closed , otherwise it will again become open.
				var cbTask = Breaker.WithCircuitBreaker(() =>
			   {
					   var json = client.DownloadString(queryUri);
					   logger.Info($"{ActorName}, {nameof(ReadFeeFromAPI)} json retrieved from https://www.alphavantage.co/ is : {json}");
					   var options = new JsonSerializerOptions
					   {
						   AllowTrailingCommas = true
					   };
					   var feeRate = JsonSerializer.Deserialize<FeeRate>(json, options);
					   return Task.FromResult(new FeeRateRetrieved(msg.RequestId, msg.UserId, msg.FromCurrency, msg.ToCurrency, feeRate.CurrencyExchangeRate.LastRefreshed, feeRate.CurrencyExchangeRate.ExchangeRate));
			   });
				if (cbTask.IsCompletedSuccessfully)
				{
					cbTask.PipeTo(Sender);
				}
				else if (cbTask.IsFaulted)
				{
					logger.Error($"{ActorName}, {nameof(ReadFeeFromAPI)} could not retrieve exchange rate from https://www.alphavantage.co/ ");
					Sender.Tell(new GetFeeRateFromDB(msg.RequestId, msg.UserId, msg.FromCurrency, msg.ToCurrency, CurrenciesError.CouldNotGetExchangeRateForInsertedCurrencies), Self);
				}

			}
			catch (Exception ex)
			{
				logger.Error($"{ActorName}, {nameof(ReadFeeFromAPI)} could not retrieve exchange rate from https://www.alphavantage.co/ due to {ex.GetBaseException()}");
				Sender.Tell(new GetFeeRateFromDB(msg.RequestId, msg.UserId, msg.FromCurrency, msg.ToCurrency, CurrenciesError.CouldNotGetExchangeRateForInsertedCurrencies), Self);
			}

		}

		public static Props Props() => Akka.Actor.Props.Create(() => new CurrencyExchangeRateActor());


		private void NotifyMeOnOpen()
		{
			logger.Warning("My CircuitBreaker is now open, and will not close for one minute");
		}

	}
}
