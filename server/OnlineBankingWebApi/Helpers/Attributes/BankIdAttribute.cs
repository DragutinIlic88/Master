using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Helpers.Attributes
{
	public class BankIdAttribute : ValidationAttribute
	{
		public int MinLength { get; set; }

		public int MaxLength { get; set; }

		public int AccountLength { get; set; }

		public int CreditCardLength { get; set; }

		public BankIdAttribute(int minLength = 0, int maxLength = 0, int accountLength = 0, int creditCardLength = 0)
		{
			MinLength = minLength;
			MaxLength = maxLength;
			AccountLength = accountLength;
			CreditCardLength = creditCardLength;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string bankId = value.ToString();

			if (!bankId.All(char.IsDigit)) {
				return new ValidationResult(BankIdErrors.BankIdFormatNotCorrectErrror);
			}

			if (MinLength > 0 && bankId.Length < MinLength)
			{
				return new ValidationResult(BankIdErrors.BankIdFormatNotCorrectErrror);
			}

			if (MaxLength > 0 && bankId.Length > MaxLength)
			{ 
				return new ValidationResult(BankIdErrors.BankIdFormatNotCorrectErrror);
			}

			
			var invalidAccountLength = AccountLength > 0 && bankId.Length != AccountLength;
			var invalidCreditCardLength = CreditCardLength > 0 && bankId.Length != CreditCardLength;
			var invalidBankIdLength = (invalidAccountLength && invalidCreditCardLength) || 
				(AccountLength == 0 && invalidCreditCardLength) || 
				(CreditCardLength == 0 && invalidAccountLength);

			if (invalidBankIdLength)
			{
				return new ValidationResult(BankIdErrors.BankIdFormatNotCorrectErrror);
			}
			

			return ValidationResult.Success;
		}

	}
}
