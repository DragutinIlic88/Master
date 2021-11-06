using System;
using System.Collections.Generic;
using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using Akka.Routing;
using OnlineBankingEntitiesLib;
using System.Threading.Tasks;
using Akka.Event;

namespace OnlineBankingActorSystem.Actors
{
	public class RegistrationStorageActor : BaseReceiveActor, ILogReceive
	{
		private readonly IActorRef roleAssignerActor = Context.ActorOf(RandomRoleAssignerActor.Props(), "roleAssigner");
		private readonly IActorRef passwordHasherActor = Context.ActorOf(PasswordHasherActor.Props(), "passwordHasher");
		public RegistrationStorageActor() : base(nameof(RegistrationStorageActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			ReceiveAsync<SaveUser>(async sUser => {
				logger.Info($"{ActorName} , {nameof(SaveUser)} message received with data: {sUser}");
				var originalSender = Sender;
				var tasks = new List<Task<object>>
				{
					roleAssignerActor.Ask(new GetRoleForUser(sUser.RequestId), TimeSpan.FromSeconds(1))
				};
				logger.Debug($"{ActorName}, message sent to {nameof(RandomRoleAssignerActor)} actor.");
				tasks.Add(passwordHasherActor.Ask(new HashPassword(sUser.RequestId, sUser.Password), TimeSpan.FromSeconds(3)));
				logger.Debug($"{ActorName}, message sent to {nameof(PasswordHasherActor)} actor.");

				await Task.WhenAll(tasks);
				var roleEnum = tasks[0].Result;
				var hashedPassword = tasks[1].Result;
				logger.Info($"{ActorName}, Role is retrieved ,and hash password is generated: role: {roleEnum}, hashed password: {hashedPassword}");
				using IServiceScope serviceScope = Context.CreateScope();
				var onlineBankingContext = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var newUser = new User
				{
					Email = sUser.Email,
					HomeAddress = sUser.Address,
					UserName = sUser.UserName,
					HomePhoneNumber = sUser.HomePhone,
					MobileNumber = sUser.Mobile,
					PasswordHash = hashedPassword.ToString(),
					UserID = Guid.NewGuid().ToString(),
					RegistrationDate = DateTime.Now.ToString(),
					LastModifiedDate = DateTime.Now.ToString()
				};

				var role = new OnlineBankingEntitiesLib.Role
				{
					RoleID = Guid.NewGuid().ToString(),
					Name = roleEnum.ToString(),
					CreationDate = DateTime.Now.ToString()
				};

				newUser.UserRoles = new List<UserRole> {
						new UserRole{
							Role = role,
							RoleID = role.RoleID,
							User = newUser,
							UserID = newUser.UserID
						}
					};

				onlineBankingContext.Add(newUser);
				var savedUsers = onlineBankingContext.SaveChanges();
				if (savedUsers != 0)
				{
					logger.Info($"{ActorName}, user is successfully saved to the database");
					originalSender.Tell(new UserSaved(sUser.RequestId, true), Self);
				}
				else
				{
					logger.Info($"{ActorName}, user is not saved to the database");
					originalSender.Tell(new UserSaved(sUser.RequestId, false), Self);
				}
			});
		}

		public static Props Props() =>Akka .Actor.Props.Create(() => new RegistrationStorageActor());
		
	}
}
