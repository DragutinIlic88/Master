using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using OnlineBankingWebApi.Helpers.Attributes;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{
	[ApiPrefixRoute("transactions")]
	public class TransactionController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _transactionsGetterActor;
		private readonly IIncrement _transactionsIncrementor;

		public TransactionController(ILoggerManager logger, TransactionGetterActorProvider transactionsGetterActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_transactionsGetterActor = transactionsGetterActorProvider();
			_transactionsIncrementor = incrementor;
		}
		[HttpPost("getTransactions")]
		public async Task<IActionResult> GetTransactions(GetTransactionsModel requestData)
		{
			_logger.LogInfo($"{nameof(GetTransactions)} , request {requestData} arrived");
			var result = await _transactionsGetterActor.Ask(new GetTransactions(_transactionsIncrementor.Increment(nameof(GetTransactions)),requestData.UserToken, requestData.AccountNumber, requestData.Beginning, requestData.TransactionsNumber));
			return Ok(result);
		}
	}
}
