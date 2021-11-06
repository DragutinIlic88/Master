using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.ExchangeMessages;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{

	
	[Route("api/[controller]")]
	[ApiController]
	public class ExchangeController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _confirmExchangeActorProvider;
		private readonly IIncrement _exchangeIncrementor;

		public ExchangeController(ILoggerManager logger, ConfirmExchangeActorProvider confirmExchangeActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_confirmExchangeActorProvider = confirmExchangeActorProvider();
			_exchangeIncrementor = incrementor;
		}
		[HttpPost("confirmExchange")]
		public async Task<IActionResult> ConfirmExchange(ConfirmExchangeModel confirmExchangeModel) {
			_logger.LogInfo($"{nameof(ConfirmExchange)} , confirm exchange rate for user with token {confirmExchangeModel.UserToken}, from account {confirmExchangeModel.FromAccount}, to account {confirmExchangeModel.ToAccount}, rate {confirmExchangeModel.Rate}, amount {confirmExchangeModel.Amount} , to currency {confirmExchangeModel.ToCurrency} will be processed.");
			var result = await _confirmExchangeActorProvider.Ask(new ConfirmExchange(_exchangeIncrementor.Increment(nameof(ConfirmExchange)),
				confirmExchangeModel.UserToken, confirmExchangeModel.FromAccount, confirmExchangeModel.ToAccount, confirmExchangeModel.Amount, 
				confirmExchangeModel.ToCurrency,confirmExchangeModel.FromCurrency, confirmExchangeModel.Rate ));
			return Ok(result);
		}
	}
}
