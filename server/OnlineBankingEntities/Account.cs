using System;
namespace OnlineBankingEntitiesLib
{
	public class Account
	{
		public string Iban { get; set; }
		public string AccountNumber { get; set; }
		public string UserId { get; set; }
		public string BankIdentifierCode { get; set; }
		public DateTime CreationDate { get; set; }
		public string InstitutionName { get; set; }
		public string CountryCode { get; set; }
		public string AccountDetails { get; set; }
		public AccountType AccountType { get; set; }
		public string Currency { get; set; }
		public decimal Amount { get; set; }
	}

	public enum AccountType { 
		CHECKING,
		SAVINGS,
		MONEY_MARKET,
		CERTIFICATE_OF_DEPOSIT,
		INDIVIDUAL_RETIREMENT_ARRANGEMENT,
		BROKERAGE
	}
}
