using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class UserRetrieverActor : UntypedActor,ILogReceive
	{
		private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
		private readonly string ActorName = nameof(AuthenticationActor);

		protected override void PreStart()
		{
			logger.Info($"{ActorName} actor will start");
			base.PreStart();
		}
		protected override void OnReceive(object message)
		{
			switch (message) {
				case FetchUser getUser:
					FetchUserFromDatabase(getUser);
					break;
			}
		}

		private void FetchUserFromDatabase(FetchUser message)
		{
			using IServiceScope serviceScope = Context.CreateScope();
			var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
			var user = onlineBankingContext.Users.Where(u => u.Email == message.Email && u.PasswordHash == message.PasswordHash).SingleOrDefault();
			if (user != null)
			{
				logger.Info("User successfully retrieved from database");
				user.LastLoginDate = DateTime.Now.ToString();
				onlineBankingContext.Update(user);
				var updated = onlineBankingContext.SaveChanges();
				if (updated != 0)
				{
					logger.Info($"{ActorName}: last login date of user {user.UserID} updated");
				}
				else {
					logger.Error($"{ActorName}: last login date of user {user.UserID} could not be updated");
				}
				Sender.Tell(new UserFetched(message.RequestId,user.UserID, user.UserName,  message.Email, message.PasswordHash, user.MobileNumber, user.HomePhoneNumber, user.HomeAddress,user.ProfileImagePath, message.Token, message.GenerationTime),Self);
			}
			else
			{
				logger.Error("User could not be fetched from database");
				Sender.Tell(new FetchUserFailed(message.RequestId, UserValidationErrors.UserCouldNotBetRetrievedFromDatabase), Self);
				
			}
		}

		protected override void PreRestart(Exception reason, object message)
		{
			logger.Error($"{ActorName} will be resterted due the reason {reason.Message}");
			foreach (IActorRef each in Context.GetChildren())
			{
				Context.Unwatch(each);
				Context.Stop(each);
			}
			PostStop();
		}

		protected override void PostStop()
		{
			logger.Info($"{ActorName} stopped");
			base.PostStop();
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create(() => new UserRetrieverActor());
		}
	}
}
