using Akka.Actor;
using Akka.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class UserIdRetrieverActor : BaseReceiveActor, ILogReceive
	{

		public UserIdRetrieverActor() : base(nameof(UserIdRetrieverActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<RetrieveUserId>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await RetrieveUserIdBasedOnToken(message);
			});
		}

		private async Task RetrieveUserIdBasedOnToken(RetrieveUserId msg)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
			try
			{
				var userToken = await onlineBankingContext.UserTokens.SingleOrDefaultAsync(ut => ut.TokenValue == msg.Token);
				if (userToken == null)
				{
					logger.Info($"{ActorName}, {nameof(RetrieveUserIdBasedOnToken)}, user with token {msg.Token} not exists.");
					Sender.Tell(new RetrievingUserIdFailed(msg.RequestId, nameof(UserValidationErrors.UserIdCouldNotBeRetireved)), Self);
				}
				else
				{
					if (DateTime.Compare(DateTime.Parse(userToken.TokenExpirationTime), DateTime.Now) < 0)
					{
						logger.Info($"{ActorName}, {nameof(RetrieveUserIdBasedOnToken)}, user token is expired");
						Sender.Tell(new RetrievingUserIdFailed(msg.RequestId, nameof(UserValidationErrors.UserTokenExpired)), Self);
					}
					else
					{
						logger.Info($"{ActorName}, {nameof(RetrieveUserIdBasedOnToken)}, user with token {msg.Token} has userId {userToken.UserID}.");
						Sender.Tell(new UserIdRetrieved(msg.RequestId, userToken.TokenValue, userToken.UserID), Self);
					}
				}
			}
			catch (Exception)
			{
				logger.Error($"{ActorName}, {nameof(RetrieveUserIdBasedOnToken)}, more than one user id exists for specified token.");
				Sender.Tell(new RetrievingUserIdFailed(msg.RequestId, nameof(UserValidationErrors.MoreThanOneUserIdFindForSpecifiedToken)), Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new UserIdRetrieverActor());

	}
}
