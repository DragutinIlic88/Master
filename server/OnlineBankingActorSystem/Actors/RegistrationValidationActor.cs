
using Akka.Actor;
using Akka.Event;
using BankUsersDBContextLib;
using BankUsersDBEntitiesLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class RegistrationValidationActor : ReceiveActor, ILogReceive
	{
		private readonly string ActorName = nameof(RegistrationValidationActor);
		private readonly ILoggingAdapter logger = Logging.GetLogger(Context);

		protected override void PreStart()
		{
			logger.Info($"{ActorName} actor will start");
			base.PreStart();
		}
		public RegistrationValidationActor()
		{
			logger.Info($"{ActorName} actor constructor is called");

			ReceiveAsync<ValidateUser>(async message =>
			{
				logger.Info($"{ActorName} , {nameof(ValidateUser)} message received with data: {message}");
				await PerformValidationOfUser(message);
			});
		}

		private async Task PerformValidationOfUser(ValidateUser vu)
		{
			using (IServiceScope serviceScope = Context.CreateScope())
			{
				var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var usersWithSameEmailOrMobile = await onlineBankingContext.Users.Where(u => u.Email == vu.Email || u.MobileNumber == vu.Mobile).ToListAsync();

				if (usersWithSameEmailOrMobile == null || !usersWithSameEmailOrMobile.Any())
				{
					var bankUsersContext = serviceScope.ServiceProvider.GetService<BankUsersContext>();
					var userBankIds = await bankUsersContext.UserBankIds.Include(ub => ub.User).FirstOrDefaultAsync(ub => ub.BankId == vu.BankId);
					if (userBankIds != null)
					{
						logger.Info($"{ActorName}, {nameof(PerformValidationOfUser)}, User information retrieved: bankId - {userBankIds.BankId}, email - {userBankIds.User.Email}, mobile - {userBankIds.User.MobileNumber}");
						var valid = validateUser(userBankIds.User, vu, out bool sameEmail, out bool sameMobile, out string errorMessage);
						Sender.Tell(new UserValidated(vu.RequestId, valid, sameEmail, sameMobile, errorMessage), Self);

					}
					else
					{
						logger.Info($"{ActorName}, {nameof(PerformValidationOfUser)}, User with {vu.BankId} bankId not exists in BankUsers database");
						Sender.Tell(new UserValidated(vu.RequestId, IsValid: false, ErrorMessage: nameof(UserValidationErrors.UserWithInsertedBankIdNotExists)), Self);
					}
				}
				else
				{
					logger.Info($"{ActorName}, {nameof(PerformValidationOfUser)}, User with email {vu.Email} and mobile number {vu.Mobile} is already registered");
					Sender.Tell(new UserValidated(vu.RequestId, IsValid: false, ErrorMessage: nameof(UserValidationErrors.UserAlreadyRegistered)), Self);
				}
			}
		}

		private bool validateUser(User user, ValidateUser message, out bool sameEmail, out bool sameMobile, out string errorMessage)
		{
			if (user == null)
			{
				sameEmail = false;
				sameMobile = false;
				errorMessage = nameof(UserValidationErrors.UserWithInsertedBankIdNotExists);
				return false;
			}

			sameEmail = user.Email == message.Email;
			sameMobile = user.MobileNumber == message.Mobile;

			errorMessage = sameEmail || sameMobile ? "" : nameof(UserValidationErrors.UserInsertedMobileAndEmailNotCorrect);
			return sameEmail || sameMobile;
		}

		protected override void PreRestart(Exception reason, object message)
		{
			logger.Error($"{ActorName} will be resterted due the reason {reason.GetBaseException()}");
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
			return Akka.Actor.Props.Create(() => new RegistrationValidationActor());
		}
	}
}
