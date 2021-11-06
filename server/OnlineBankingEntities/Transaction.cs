using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OnlineBankingEntitiesLib
{
	public class Transaction
	{
		public Guid TransactionId { get; set; }
		public string UserId { get; set; }
		public string AccountNumber { get; set; }
		public string AccountIban { get; set; }
		public string BankIdentifierCode { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime? EndTime { get; set; }
		public string TransactionDetails { get; set; }
		public string TransactionType { get; set; }
		public string TransactionName { get; set; }
		public TransactionStatus TransactionStatus { get; set; }
		public Amount TransactionAmount { get; set; }
		 
	}
	[ComplexType]
	public class Amount
	{ 
		public decimal Total { get; set; }
		public string Currency { get; set; }
	}

	public enum TransactionStatus{
		COMPLETED,
		AUTHORISED,
		INVALID,
		CANCELLED,
		AUTHORISATION_DECLINED,
		AUTHORISED_CANCELED,
		WAITING_FOR_AUTHORISATION,
		FINISHED
	}
}
