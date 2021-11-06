using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Helpers.Constants
{
	public static class ErrorConstants
	{
		public static class BankIdErrors {
			public static string BankIdFormatNotCorrectErrror => "BankIdFormatNotCorrectErrror";
		}

		public static class PasswordErrors {
			public static string PasswordNotInValidFormat => "PasswordNotInValidFormat";
			public static string PasswordRequiredError => "PasswordRequiredError";
			public static string PasswordTooSmallError => "PasswordTooSmallError";

			public static string PasswordFormatNotCorrectError => "PasswordFormatNotCorrectError";

			public static string PasswordDoesNotMatchError => "PasswordDoesNotMatchError";
			public static string PasswordTooLongError => "PasswordTooLongError";
		}

		public static class EmailErrors {
			public static string EmailRequiredError => "EmailRequiredError";
			public static string EmailFormatNotCorrectError => "EmailFormatNotCorrectError";
			public static string EmailTooLongError => "EmailTooLongError";
		}

		public static class MobileErrors {
			public static string MobileRequiredError => "MobileRequiredError";

			public static string MobileFormatNotCorectError => "MobileFormatNotCorectError";
		}

		public static class PhoneErrrors {
			public static string PhoneFromatNotCorrectError => "PhoneFromatNotCorrectError";
		}

		public static class UserNameErrors {
			public static string UserNameRequiredError => "UserNameRequiredError";

			public static string UserNameTooLongError => "UserNameTooLongError";
		}

		public static class AddressErrors {
			public static string AddressTooLongError => "AddressTooLongError";
		}

		public static class VerificationType {
			public static string VerificationTypeRequiredError => "VerificationTypeRequiredError";
			public static string VerificationTypeValueError => "VerificationTypeValueError";
		}

		public static class UserTokenErrors {
			public static string UserTokenRequiredError => "UserTokenRequiredError";
		}

		public static class AccountErrors {
			public static string AccountNumberRequiredError => "AccountNumberRequiredError";
			public static string FromAccountNumberRequiredError => "FromAccountNumberRequiredError";
			public static string ToAccountNumberRequredError => "ToAccountNumberRequredError";

		}

		public static class FeeErrors {
			public static string FromCurrencyRequiredError => "FromCurrencyRequiredError";
			public static string ToCurrencyRequiredError => "ToCurrencyRequiredError";
			public static string CurrencyLengthError => "CurrencyLengthError";
		}

		public static class AmountErrors {
			public static string AmountRequiredError => "AmountRequiredError";
			public static string AmountLessThenZeroError => "AmountLessThenZeroError";
		}

		public static class ExchangeRateErrors {
			public static string ExchangeRateRequiredError => "ExchangeRateRequiredError";
			public static string ExchangeRateLessThenZeroError => "ExchangeRateLessThenZeroError";
		}

		public static class CurrencyErrors {
			public static string CurrencyRequiredError => "CurrencyRequiredError";
			public static string ToCurrencyRequiredError => "ToCurrencyRequiredError";
			public static string FromCurrencyRequiredError => "FromCurrencyRequiredError";
			public static string InvalidCurrencyLength => "InvalidCurrencyLength";
		}

		public static class PaymentErrors {
			public static string BeneficiaryCustomerRequiredError => "BeneficiaryCustomerRequiredError";
			public static string BeneficiaryCustomerAccountRequiredError => "BeneficiaryCustomerAccountRequiredError";
			public static string ModelOutOfRangeError => "ModelOutOfRangeError";

		}

		public static class LoanErrors {
			public static string StartDateRequiredError => "StartDateRequiredError";
			public static string EndDateRequiredError => "EndDateRequiredError";
			public static string PurposeRequiredError => "PurposeRequiredError";
		}

		public static class ProfileErrors {
			public static string LogoRequiredError => "LogoRequiredError";
			public static string LogoNameRequiredError => "LogoNameRequiredError";
		}
	}
}
