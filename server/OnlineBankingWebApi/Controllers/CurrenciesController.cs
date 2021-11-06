using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.CurrencyMessages;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CurrenciesController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IActorRef _currenciesGetterActorProvider;
		private readonly IIncrement _accountIncrementor;

		public CurrenciesController(ILoggerManager logger, CurrenciesGetterActorProvider currenciesGetterActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_currenciesGetterActorProvider = currenciesGetterActorProvider();
			_accountIncrementor = incrementor;
		}

		[HttpPost("getCurrencies")]
		public async Task<IActionResult> GetCurrencies(GetCurrenciesModel getCurrenciesModel)
		{
			_logger.LogInfo($"{nameof(GetCurrencies)} , account for user with token {getCurrenciesModel.UserToken} will be retireved");
			var result = await _currenciesGetterActorProvider.Ask(new GetCurrencies(_accountIncrementor.Increment(nameof(GetCurrencies)), getCurrenciesModel.UserToken));
			return Ok(result);
		}
	}
}
