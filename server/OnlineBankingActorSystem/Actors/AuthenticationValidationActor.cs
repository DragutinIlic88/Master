using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using System;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;
using System.Collections.Concurrent;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;

namespace OnlineBankingActorSystem.Actors
{
	class AuthenticationValidationActor : BaseReceiveActor, ILogReceive
	{

		private readonly IActorRef passwordHasherActor = Context.ActorOf(PasswordHasherActor.Props(), "passwordHasher");
		private readonly ConcurrentDictionary<ulong, IActorRef> authenticationActors = new();

		public AuthenticationValidationActor() : base(nameof(AuthenticationValidationActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<ValidateUserAuthentication>(async message =>
			{
				logger.Info($"{ActorName} , {nameof(AuthenticationValidationActor)} message received with data: {message}");
				if (!String.IsNullOrEmpty(message.Password) && !String.IsNullOrEmpty(message.Email))
				{
					using IServiceScope serviceScope = Context.CreateScope();
					var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
					var userWithSameEmail = await onlineBankingContext.Users.Where(u => u.Email == message.Email).SingleOrDefaultAsync();
					if (userWithSameEmail == null)
					{
						logger.Info($"{ActorName}, User which has email address {message.Email} is not registered");
						Sender.Tell(new AuthenticationFailed(message.RequestId, UserValidationErrors.UserWithSpecifiedEmailAndPasswordNotExist),Self);
					}
					else
					{
						passwordHasherActor.Tell(new UnhashPasswordForAuthenticationValidation(message, userWithSameEmail.PasswordHash), Sender);

					}
				}
				else
				{
					if (String.IsNullOrEmpty(message.Password) && String.IsNullOrEmpty(message.Email))
					{
						Sender.Tell(new AuthenticationFailed(message.RequestId, nameof(UserValidationErrors.EmailAndPasswordIsNotSpecified)), Self);

					}
					else if (String.IsNullOrEmpty(message.Password))
					{
						Sender.Tell(new AuthenticationFailed(message.RequestId, nameof(UserValidationErrors.PasswordIsNotSpecified)), Self);
					}
					else
					{
						Sender.Tell(new AuthenticationFailed(message.RequestId, nameof(UserValidationErrors.EmailIsNotSpecified)), Self);
					}
				}
			});
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new AuthenticationValidationActor());
		
	}
}
