using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.Loan;
using OnlineBankingActorSystem.Messagess.TransactionMessages;
using OnlineBankingEntitiesLib;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/loans")]
	[ApiController]
	public class LoanController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IActorRef _loanActor;
		private readonly IIncrement _loanIncrementor;

		public LoanController(ILoggerManager logger, LoanActorProvider loanActorProvider, IIncrement incrementor)
		{
			_logger = logger;
			_loanActor = loanActorProvider();
			_loanIncrementor = incrementor;
		}

		[HttpPost("postLoanRequest")]
		public async Task<IActionResult> PostLoanRequest(LoanModel loanModel)
		{
			_logger.LogInfo($"{nameof(PostLoanRequest)},loan request for user with token {loanModel.UserToken} is requested due to {loanModel.Purpose} and will be processed");
			var result = await _loanActor.Ask(new PostLoanRequestMessage(_loanIncrementor.Increment(nameof(PostLoanRequest)), loanModel.UserToken, loanModel.FromAccount,
				loanModel.ReceiveAccount, loanModel.TotalAmount, loanModel.Currency, loanModel.StartDate, loanModel.EndDate,
				loanModel.Purpose, loanModel.Participation, loanModel.Collateral));
			return Ok(result);
		}

		[HttpPost("getLoans")]
		public async Task<IActionResult> GetLoans(GetLoanModel loanModel)
		{
			_logger.LogInfo($"{nameof(GetLoans)}, user with token {loanModel.UserToken} requested his loans.");
			var result = await _loanActor.Ask(new GetLoansMessage(_loanIncrementor.Increment(nameof(PostLoanRequest)), loanModel.UserToken));
			if (result is RetrievedTransactions)
			{
				var formatedLoanResult = ParseTransactionsToLoans(result as RetrievedTransactions);
				return Ok(formatedLoanResult);
			}
			return Ok(result);
		}

		private List<LoanResponseModel> ParseTransactionsToLoans(RetrievedTransactions result)
		{
			var loans = result.Transactions.Select((transaction) => {
				var toAccountNumber = transaction.TransactionDetails.Substring(transaction.TransactionDetails.IndexOf("to account")+ 10,
					transaction.TransactionDetails.IndexOf("with total amount")- (transaction.TransactionDetails.IndexOf("to account") + 10)).Trim();
				var participation = transaction.TransactionDetails.Substring(transaction.TransactionDetails.IndexOf("participation:") + 14,
					transaction.TransactionDetails.IndexOf(", collaterall:") - (transaction.TransactionDetails.IndexOf("participation:") + 14)).Trim();
				var collateral = transaction.TransactionDetails.Substring(transaction.TransactionDetails.IndexOf("collaterall:") + 12,
					transaction.TransactionDetails.IndexOf("should start:") - (transaction.TransactionDetails.IndexOf("collaterall:") + 12)).Trim();
				var purpose = transaction.TransactionDetails.Substring(transaction.TransactionDetails.IndexOf("purpose:") + 8).Trim();
				var status = transaction.TransactionStatus switch
				{
					TransactionStatus.WAITING_FOR_AUTHORISATION => "Waiting for authorisation",
					TransactionStatus.AUTHORISED => "Authorised , should start soon",
					TransactionStatus.COMPLETED => "Started",
					TransactionStatus.AUTHORISATION_DECLINED => "Authorisation declined",
					TransactionStatus.AUTHORISED_CANCELED => "Cancelled by the user",
					TransactionStatus.CANCELLED => "Cancelled by the bank",
					TransactionStatus.INVALID => "Invalidated by the bank",
					TransactionStatus.FINISHED => "Finished with returning funds",
					_ => throw new ArgumentOutOfRangeException(nameof(transaction.TransactionStatus), $"Not expected status value: {transaction.TransactionStatus}"),
				};
				return new LoanResponseModel
				{
					CreationDate = transaction.CreationTime,
					Currency = transaction.TransactionAmount.Currency,
					EndingDate = transaction.EndTime.HasValue ? transaction.EndTime.ToString() : "no-ending-date",
					FromAccountNumber = transaction.AccountNumber,
					ToAccountNumber = toAccountNumber,
					LoanId = transaction.TransactionId,
					LoanStatus = status,
					LoanType = string.IsNullOrEmpty(collateral) ? "UNSECURED" : "SECURED",
					LoanName = transaction.TransactionName,
					TotalAmount = transaction.TransactionAmount.Total,
					ParticipationAmount =string.IsNullOrEmpty(participation)?0: Decimal.Parse(participation,NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol |NumberStyles.AllowThousands),
					Collateral = collateral,
					Purpose = purpose
				};
			}).ToList();
			return loans;
		}
	}
}
