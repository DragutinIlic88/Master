namespace OnlineBankingActorSystem.Helpers.Constants
{
	public static class NotificationMessages
	{
		public static string LoanWaitingForAuthorizationMessage => "Request for loan is successfully sent. You will be informed about further steps in shortest possible time" +
			"which is needed so that reqeust can be processed.";

		public static string LoanWaitingForAuthroizationTitle => "Waiting for authorization of loan.";

		public static string PaymentSuccessMessage => "Payment was successfully processed. Funds from account are removed.";
		public static string PaymentSuccessTitle => "Payment processed successfully.";
		public static string ExcangeSuccessTitle => "Exchange performed successfully.";
	}

}
