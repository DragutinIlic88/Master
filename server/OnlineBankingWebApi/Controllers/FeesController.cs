using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.FeeMessages;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FeesController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _feeGetterActorProvider;
		private readonly IIncrement _feesIncrementor;

		public FeesController(ILoggerManager logger, FeeGetterActorProvider feeGetterActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_feeGetterActorProvider = feeGetterActorProvider();
			_feesIncrementor = incrementor;
		}
		[HttpPost("getFeeInfo")]
		public async Task<IActionResult> GetFeeInfo(GetFeesModel getFeesModel)
		{
			
			_logger.LogInfo($"{nameof(GetFeeInfo)} , fee exchange rate for user with token {getFeesModel.UserToken} from currency {getFeesModel.FromCurrency} to currency {getFeesModel.ToCurrency} will be retireved");
			var result = await _feeGetterActorProvider.Ask(new GetFee(_feesIncrementor.Increment(nameof(GetFeeInfo)), getFeesModel.UserToken, getFeesModel.FromCurrency, getFeesModel.ToCurrency));
			return Ok(result);
		}

	}
}
