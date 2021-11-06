using Akka.Actor;


namespace OnlineBankingActorSystem
{
	public delegate IActorRef LoginManagerActorProvider();
	public delegate IActorRef AccountGetterActorProivder();
	public delegate IActorRef TransactionGetterActorProvider();
	public delegate IActorRef CurrenciesGetterActorProvider();
	public delegate IActorRef FeeGetterActorProvider();
	public delegate IActorRef ConfirmExchangeActorProvider();
	public delegate IActorRef PaymentActorProvider();
	public delegate IActorRef LoanActorProvider();
	public delegate IActorRef ProfileActorProvider();
	public delegate IActorRef HelpActorProvider();
	public delegate IActorRef NotificationActorProvider();
	public delegate IActorRef NotificationStorageActorPorvider();
}
