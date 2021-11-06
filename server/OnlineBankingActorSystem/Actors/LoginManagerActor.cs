using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using OnlineBankingActorSystem.Messagess.NotificationMessages;
using System.Collections.Concurrent;



namespace OnlineBankingActorSystem.Actors
{
	public class LoginManagerActor : BaseUntypedActor, ILogReceive
	{
		private readonly IActorRef registrationActor = Context.System.ActorOf(RegistrationActor.Props());
		private readonly IActorRef authenticationActor = Context.System.ActorOf(AuthenticationActor.Props().WithRouter(new SmallestMailboxPool(5)));
		private readonly IActorRef logoutActor = Context.System.ActorOf(LogoutActor.Props().WithRouter(new SmallestMailboxPool(5)));
		private readonly IActorRef notificationActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<NotificationActor>());
		private readonly ConcurrentDictionary<string, IActorRef> controllers = new();

		public LoginManagerActor() : base(nameof(LoginManagerActor))
		{
		}

		protected override void OnReceive(object message)
		{
			switch (message)
			{

				case Register reg:
					logger.Info($"{ActorName},message received: {reg}");
					controllers.TryAdd("Register" + reg.RequestId.ToString(), Sender);
					registrationActor.Tell(reg, Self);
					break;
				case UserSaved us:
					logger.Info($"{ActorName},message received: {us}");
					var registerControllerRef = controllers["Register" + us.RequestId.ToString()];
					registerControllerRef.Tell(us, Self);
					controllers.TryRemove("Register" + us.RequestId.ToString(), out _);
					break;
				case CanNotRegister cantReg:
					logger.Info($"{ActorName},message received: {cantReg}");
					registerControllerRef = controllers["Register" + cantReg.RequestId.ToString()];
					registerControllerRef.Tell(cantReg, Self);
					controllers.TryRemove("Register" + cantReg.RequestId.ToString(), out _);
					break;
				case Authenticate auth:
					logger.Info($"{ActorName},message received: {auth}");
					controllers.TryAdd("Authenticate" + auth.RequestId.ToString(), Sender);
					authenticationActor.Tell(auth, Self);
					break;
				case UserFetched userFetched:
					logger.Info($"{ActorName},message received: {userFetched}");
					registerControllerRef = controllers[$"Authenticate{userFetched.RequestId}"];
					registerControllerRef.Tell(new Authenticated(userFetched.RequestId, userFetched.Token), Self);
					controllers.TryRemove($"Authenticate{userFetched.RequestId}", out _);
					notificationActor.Tell(new SendSavedNotifications(userFetched.RequestId, userFetched.UserId), Self);
					break;
				case TokenGenerationFailed genFailed:
					logger.Info($"{ActorName},message received: {genFailed}");
					registerControllerRef = controllers[$"Authenticate{genFailed.RequestId}"];
					registerControllerRef.Tell(genFailed, Self);
					controllers.TryRemove($"Authenticate{genFailed.RequestId}", out _);
					break;
				case FetchUserFailed fetchUserFailed:
					logger.Info($"{ActorName},message received: {fetchUserFailed}");
					registerControllerRef = controllers[$"Authenticate{fetchUserFailed.RequestId}"];
					registerControllerRef.Tell(fetchUserFailed, Self);
					controllers.TryRemove($"Authenticate{fetchUserFailed.RequestId}", out _);
					break;
				case AuthenticationFailed authFailed:
					logger.Info($"{ActorName},message received: {authFailed}");
					registerControllerRef = controllers[$"Authenticate{authFailed.RequestId}"];
					registerControllerRef.Tell(authFailed, Self);
					controllers.TryRemove($"Authenticate{authFailed.RequestId}", out _);
					break;
				case Logout logout:
					logger.Info($"{ActorName}, message received: {logout}");
					logoutActor.Tell(logout, Sender);
					break;
			}

		}

		public static Props Props() => Akka.Actor.Props.Create(() => new LoginManagerActor());
		
	}
}
