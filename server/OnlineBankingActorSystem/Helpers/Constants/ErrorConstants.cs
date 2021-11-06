
namespace OnlineBankingActorSystem.Helpers.Constants
{
	public static class ErrorConstants
	{

		public static class UserValidationErrors
		{
			public static string UserWithInsertedBankIdNotExists => "UserWithInsertedBankIdNotExists";
			public static string UserInsertedMobileAndEmailNotCorrect => "UserInsertedMobileAndEmailNotCorrect";
			public static string UserAlreadyRegistered => "UserAlreadyRegistered";
			public static string EmailAndPasswordIsNotSpecified => "EmailAndPasswordIsNotSpecified";
			public static string PasswordIsNotSpecified => "PasswordIsNotSpecified";
			public static string EmailIsNotSpecified => "EmailIsNotSpecified";
			public static string UserWithSpecifiedEmailNotExist => "UserWithSpecifiedEmailNotExist";
			public static string UserWithSpecifiedEmailAndPasswordNotExist => "UserWithSpecifiedEmailAndPasswordNotExist";
			public static string UserTokenGenerationFailed => "UserTokenGenerationFailed";
			public static string UserCouldNotBetRetrievedFromDatabase => "UserCouldNotBetRetrievedFromDatabase";
			public static string UserCouldNotBeLoggedOut => "UserCouldNotBeLoggedOut";
			public static string UserIdCouldNotBeRetireved => "UserIdCouldNotBeRetireved";
			public static string MoreThanOneUserIdFindForSpecifiedToken => "MoreThanOneUserIdFindForSpecifiedToken";
			public static string UserTokenExpired => "UserTokenExpired";
			public static string StateInvalidWhileValidateingUser => "StateInvalidWhileValidateingUser";
		}

		public static class AccountsError
		{
			public static string AccountsReadFromDataBaseError => "AccountsReadFromDataBaseError";
			public static string AccountUpdateFromDataBaseError => "AccountUpdateFromDataBaseError";
			public static string InvalidNumberOfAccountsFetchedFromDataBaseError => "InvalidNumberOfAccountsFetchedFromDataBaseError";
			public static string FromAccountIsNotAccountOfCurrentUser => "FromAccountIsNotAccountOfCurrentUser";
			public static string AccountsSuccessfullyRolledBackOperationCancelledError => "AccountsSuccessfullyRolledBackOperationCancelledError";
			public static string AccountsManuallyRolledBackOperationCancelledError => "AccountsManuallyRolledBackOperationCancelledError";
		}

		public static class TransactionsError
		{
			public static string TransactionsReadFromDataBaseError => "TransactionsReadFromDataBaseError";
			public static string TransactionsWriteToDataBaseError => "TransactionsWriteToDataBaseError";
		}

		public static class CurrenciesError
		{
			public static string CurrenciesReadFromDataBaseError => "CurrenciesReadFromDataBaseError";
			public static string CouldNotGetExchangeRateForInsertedCurrencies => "CouldNotGetExchangeRateForInsertedCurrencies";
		}

		public static class ExchangeError
		{
			public static string NoSufficientFundsError => "NoSufficientFundsError";
			public static string InvalidCurrencyError => "InvalidCurrencyError";
			public static string ExchangeRateDoesntMatchError => "ExchangeRateDoesntMatchError";
			public static string GetExchangeRateError => "GetExchangeRateError";
		}

		public static class PaymentError
		{ 
			public static string NoSufficientFundsError => "NoSufficientFundsError";
		}

		public static class ProfileError
		{
			public static string UserNotExistInDatabase => "UserNotExistInDatabase";
			public static string DatabaseUpdateError => "DatabaseUpdateError";
		}

		public static class HelpError
		{ 
			public static string DatabaseGetError => "DatabaseGetError";
		}

		public static class NotificationError
		{
			public static string DatabaseDeleteError => "DatabaseDeleteError";
		}
	}
}
