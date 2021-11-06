using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OnlineBankingWebApi.Helpers.Attributes;
using OnlineBankingWebApi.Models;
using Contracts;
using Akka.Actor;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages;
using Microsoft.AspNetCore.Cors;

[assembly: ApiController]
namespace OnlineBankingWebApi.Controllers
{
	[ApiPrefixRoute]
	[EnableCors("AllowOrigin")]
	public class LoginController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _loginManagerActor;
		private readonly IIncrement _loginIncrementor;
		public LoginController(ILoggerManager logger, LoginManagerActorProvider loginManagerActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_loginManagerActor = loginManagerActorProvider();
			_loginIncrementor = incrementor;
		}

		[HttpPost("auth/")]
		public async Task<IActionResult> Authenticate([FromBody] AuthenticationData authData)
		{
			var result = await _loginManagerActor.Ask(new Authenticate(_loginIncrementor.Increment(nameof(Authenticate)), authData.Email, authData.Password));
			return Ok(result);
		}

		[HttpPost("signUp/")]
		[HttpPost("register/")]
		public async Task<IActionResult> Register([FromBody] RegistrationData registrationData)
		{
			var registerDataMessage = new Register(_loginIncrementor.Increment(nameof(Register)), registrationData.Email, registrationData.Password,
				registrationData.ConfirmPassword, registrationData.BankId, registrationData.Mobile,
				registrationData.UserName, registrationData.Address, registrationData.HomePhone);
			var result = await _loginManagerActor.Ask(registerDataMessage);
			return  Ok(result);
		}

		[HttpPost("verify/")]
		public async Task<IActionResult> Verify([FromBody] VerificationData verificationData) 
		{
			var data = verificationData;
			throw new Exception();
		}

		[HttpGet("logout/{token}")]
		[HttpGet("signOut/{token}")]
		public async Task<IActionResult> Logout(string token) {
			var result = await _loginManagerActor.Ask(new Logout(_loginIncrementor.Increment(nameof(Logout)), token));
			return Ok(result);
		}
	}
}
