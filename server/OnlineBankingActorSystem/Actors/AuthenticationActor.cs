using Akka.Actor;
using Akka.Event;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using System;
using System.Collections.Concurrent;

namespace OnlineBankingActorSystem.Actors
{
	class AuthenticationActor : BaseUntypedActor, ILogReceive
	{
		private readonly ConcurrentDictionary<ulong, IActorRef> loginManagerActors = new();
		private readonly IActorRef validationActor = Context.ActorOf(AuthenticationValidationActor.Props(), "authenticationValidationActor");
		private readonly IActorRef tokenGeneratorActor = Context.ActorOf(TokenGenerationActor.Props(), "tokenGenerationActor");
		private readonly IActorRef userRetrieverActor = Context.ActorOf(UserRetrieverActor.Props(), "userRetrieverActor");

		public AuthenticationActor():base(nameof(AuthenticationActor))
		{}

		protected override void OnReceive(object message)
		{
			switch (message) {
				case Authenticate auth:
					logger.Info($"{ActorName}, message received: {auth}");
					loginManagerActors.TryAdd(auth.RequestId, Sender);
					validationActor.Tell(new ValidateUserAuthentication(auth), Self);
					break;
				case AuthenticationAllowed authAllowed:
					logger.Info($"{ActorName}, message received: {authAllowed}");
					tokenGeneratorActor.Tell(new GenerateToken(authAllowed.RequestId, authAllowed.Email, authAllowed.PasswordHash), Self);
					break;
				case AuthenticationFailed authFailed:
					logger.Info($"{ActorName}, message received: {authFailed}");
					loginManagerActors[authFailed.RequestId].Tell(authFailed,Self);
					loginManagerActors.TryRemove(authFailed.RequestId, out _);
					break;
				case TokenGenerated tokenGen:
					logger.Info($"{ActorName}, message received: {tokenGen}");
					userRetrieverActor.Tell(new FetchUser(tokenGen.RequestId,tokenGen.Email,tokenGen.PasswordHash,tokenGen.Token,tokenGen.GenerationTime), loginManagerActors[tokenGen.RequestId]);
					loginManagerActors.TryRemove(tokenGen.RequestId, out _);
					break;
				case TokenGenerationFailed tokenGenFailed:
					logger.Error($"{ActorName}, message received: {tokenGenFailed}");
					loginManagerActors[tokenGenFailed.RequestId].Tell(tokenGenFailed, Self);
					loginManagerActors.TryRemove(tokenGenFailed.RequestId, out _);
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new AuthenticationActor());
		
	}
}
