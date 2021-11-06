using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.LoginMessages.LogoutMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System.Linq;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class LogoutActor : BaseReceiveActor, ILogReceive
	{

		public LogoutActor() : base(nameof(LogoutActor))
		{
			ReceiveAsync<Logout>( async logout => { 
				logger.Info($"{ActorName} , {logout} received");
				using IServiceScope serviceScope = Context.CreateScope();
				var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await onlineBankingContext.UserTokens.Where(ut => ut.TokenValue == logout.Token).SingleOrDefaultAsync();
				if (userToken == null) {
					Sender.Tell(new LogoutFailed(logout.RequestId, nameof(UserValidationErrors.UserCouldNotBeLoggedOut)), Self);
					return;
				}
				onlineBankingContext.Remove(userToken);
				var deleted = await onlineBankingContext.SaveChangesAsync();
				if (deleted > 0)
				{
					logger.Info($"{ActorName} user token {logout.Token} successfully removed from database.");
					Sender.Tell(new LogoutAllowed(logout.RequestId), Self);
				}
				else {
					logger.Error($"{ActorName} user token {userToken.TokenValue} for user {userToken.UserID} could not be removed from database.");
					Sender.Tell(new LogoutFailed(logout.RequestId, nameof(UserValidationErrors.UserCouldNotBeLoggedOut)), Self);
				}

			});
		}

		public static Props Props() => Akka.Actor.Props.Create(()=>new LogoutActor());

	}
}
