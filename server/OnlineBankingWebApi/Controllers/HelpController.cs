using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.HelpMessages;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/help")]
	[ApiController]
	public class HelpController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IActorRef _helpActor;
		private readonly IIncrement _helpIncrementor;

		public HelpController(ILoggerManager logger, HelpActorProvider helpActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_helpActor = helpActorProvider();
			_helpIncrementor = incrementor;
		}

		[HttpGet]
		public async Task<IActionResult> GetHelpInformation()
		{
			_logger.LogInfo($"{nameof(GetHelpInformation)}, geting help information for online banking");
			var result = await _helpActor.Ask(new GetHelpInfo(_helpIncrementor.Increment(nameof(GetHelpInformation))));
			return Ok(result);
		}
	}

	
}
