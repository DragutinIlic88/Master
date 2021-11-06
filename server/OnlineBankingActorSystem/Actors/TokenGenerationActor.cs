using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using OnlineBankingEntitiesLib;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class TokenGenerationActor : BaseReceiveActor, ILogReceive
	{
		public TokenGenerationActor() :base(nameof(TokenGenerationActor))
		{
			logger.Info($"{ActorName} actor constructor is called");

			ReceiveAsync<GenerateToken>(async message =>
			{
				logger.Info($"{ActorName} , {nameof(GenerateToken)} message received with data: {message}");
				await Generate(message);

			});
		}


		//This is naive approach for generating token and allowing user to be registered only on one device at a time.
		//Project is not about login mechanisams, so it will be left as it is.
		//In the future if time allows implementation of refresh/access token , and allowing auth on multiple devices with sms confirmation
		//should be preferable.
		private async Task Generate(GenerateToken message)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
			var user = await onlineBankingContext.Users.Where(u => u.Email == message.Email && u.PasswordHash == message.PasswordHash).SingleOrDefaultAsync();
			if (user == null)
			{
				Sender.Tell(new TokenGenerationFailed(message.RequestId, UserValidationErrors.UserTokenGenerationFailed), Self);
				return;
			}
			var userToken = await onlineBankingContext.UserTokens.Where(ut => ut.UserID == user.UserID).SingleOrDefaultAsync();
			if (userToken != null)
			{
				userToken.TokenValue = Guid.NewGuid().ToString();
				userToken.TokenGenerationTime = DateTime.Now.ToShortDateString();
				userToken.TokenExpirationTime = DateTime.Now.AddDays(14).ToShortDateString();
			}
			else
			{
				userToken = new UserToken
				{
					UserID = user.UserID,
					TokenValue = Guid.NewGuid().ToString(),
					TokenType = "OAUTH",
					TokenGenerationTime = DateTime.Now.ToShortDateString(),
					TokenExpirationTime = DateTime.Now.AddDays(14).ToShortDateString()
				};
				onlineBankingContext.Add(userToken);
			}
			
			var saved = await onlineBankingContext.SaveChangesAsync();
			if (saved <= 0)
			{
				logger.Error($"{ActorName}, token could not be generated and saved in database");
				Sender.Tell(new TokenGenerationFailed(message.RequestId, UserValidationErrors.UserTokenGenerationFailed), Self);
			}
			else
			{
				logger.Info($"{ActorName} , token succssfully generated and saved");
				Sender.Tell(new TokenGenerated(message.RequestId, message.Email, message.PasswordHash, userToken.TokenValue, userToken.TokenGenerationTime), Self);

			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new TokenGenerationActor());
		
	}
}
