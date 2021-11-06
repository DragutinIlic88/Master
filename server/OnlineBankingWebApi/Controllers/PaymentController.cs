using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.AccountMessages;
using OnlineBankingActorSystem.Messagess.Payment;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _paymentActor;
		private readonly IIncrement _paymentIncrementor;

		public PaymentController(ILoggerManager logger, PaymentActorProvider paymentActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_paymentActor = paymentActorProvider();
			_paymentIncrementor = incrementor;
		}

		[HttpPost("performPayment")]
		public async Task<IActionResult> PaymentInfo(PaymentModel paymentModel) {
			_logger.LogInfo($"{nameof(PaymentInfo)}, payment info for user with token {paymentModel.UserToken} will be processed from {paymentModel.AccountNumber} and payed to accunt {paymentModel.BeneficiaryCustomerAccount}");
			var result = await _paymentActor.Ask(new Pay(_paymentIncrementor.Increment(nameof(PaymentInfo)),paymentModel.UserToken,paymentModel.AccountNumber, 
			paymentModel.BeneficiaryCustomer,paymentModel.BeneficiaryCustomerAccount,paymentModel.Amount,paymentModel.Currency,paymentModel.Model,
				paymentModel.Reference, paymentModel.PaymentCode,paymentModel.PaymentPurpose));
			return Ok(result);
		}
	}
}
