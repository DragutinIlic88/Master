using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingWebApi.Models;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/accounts")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IActorRef _accountGetterActor;
		private readonly IIncrement _accountIncrementor;

		public AccountController(ILoggerManager logger, AccountGetterActorProivder accountGetterActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_accountGetterActor = accountGetterActorProvider();
			_accountIncrementor = incrementor;
		}

		[HttpPost("getAccounts")]
		public async Task<IActionResult> GetAccounts(GetAccountsModel getAccountsModel) 
		{
			_logger.LogInfo($"{nameof(GetAccounts)} , account for user with token {getAccountsModel.UserToken} will be retireved");
			var result = await _accountGetterActor.Ask(new GetAccounts(_accountIncrementor.Increment(nameof(GetAccounts)), getAccountsModel.UserToken));
			return Ok(result);
		}
	}
}
