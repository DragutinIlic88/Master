using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Models
{
	public class LoanResponseModel
	{
		public Guid LoanId { get; set; }
		public string LoanName { get; set; }
		public decimal TotalAmount { get; set; }
		public string Currency { get; set; }
		public DateTime CreationDate { get; set; }
		public string EndingDate { get; set; }
		public string FromAccountNumber { get; set; }
		public string ToAccountNumber { get; set; }
		public string LoanType { get; set; }
		public string LoanStatus { get; set; }
		public decimal ParticipationAmount { get; set; }
		public string Collateral { get; set; }
		public string Purpose { get; set; }
		public decimal RemainingAmount { get; set; }
		public decimal DebtAmount { get; set; }
		public string PaymentDeadlineDate { get; set; }
	}
}
