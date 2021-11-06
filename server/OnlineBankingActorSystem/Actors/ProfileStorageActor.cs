using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.ProfileMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using OnlineBankingEntitiesLib;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	class ProfileStorageActor : BaseReceiveActor, ILogReceive
	{

		public ProfileStorageActor() :base(nameof(ProfileStorageActor))
		{
			ReceiveAsync<EditUserAddress>(async message=> { 
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateUserAddressAsync(message);
			});

			ReceiveAsync<EditUserEmail>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateUserEmailAsync(message);

			});

			ReceiveAsync<EditUserMobile>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateUserMobileAsync(message);
			});

			ReceiveAsync<EditUserName>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateUserNameAsync(message);
			});

			ReceiveAsync<EditUserLogo>(async message =>
			{
				logger.Info($"{ActorName} , message received with data: {message}");
				await UpdateUserLogoAsync(message);
			});

			ReceiveAsync<GetUserData>(async message => {
				logger.Info($"{ActorName} , message received with data: {message}");
				await GetUserDataAsync(message);
			});
		}

		private async Task GetUserDataAsync(GetUserData msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user logo can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}

				Sender.Tell(new UserDataRetrieved(msg.RequestId, msg.UserToken, userToken.User.UserName, userToken.User.Email,
					userToken.User.MobileNumber, userToken.User.HomeAddress, userToken.User.ProfileImagePath, userToken.User.LastLoginDate, userToken.User.RegistrationDate), Self);

			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user name can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		private async Task UpdateUserLogoAsync(EditUserLogo msg)
		{
			try {
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user logo can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}

				var profilePath = await SaveProfileImageAsync(msg.Logo, msg.RootFolderPath, msg.LogoName);
				if (!string.IsNullOrEmpty(userToken.User.ProfileImagePath))
				{
					var oldProfilePath = userToken.User.ProfileImagePath;
					if (File.Exists(oldProfilePath))
					{
						File.Delete(oldProfilePath);
						logger.Info($"{ActorName} - {nameof(UpdateUserLogoAsync)}: file with path {oldProfilePath} successfully deleted");
					}
					logger.Warning($"{ActorName} - {nameof(UpdateUserLogoAsync)}: file with path {oldProfilePath} does not exist");
				}

				userToken.User.ProfileImagePath = profilePath;
				userToken.User.LastModifiedDate = DateTime.Now.ToString();
				context.Update(userToken);
				var saved = await context.SaveChangesAsync();
				if (saved < 1)
				{
					logger.Error($"{ActorName}, error occurred while updating database , {saved} rows updated");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
					return;
				}
				logger.Info($"{ActorName} user successfully updated");
				Sender.Tell(new UserEdited(msg.RequestId), Self);

			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user name can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		private async Task<string> SaveProfileImageAsync(IFormFile file,string rootFolder, string imageName)
		{
			var uniqueImageName = new String(Path.GetFileNameWithoutExtension(imageName).Take(10).ToArray()).Replace(' ', '-');
			uniqueImageName = uniqueImageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageName);
			var profilePath = Path.Combine(rootFolder, "ProfileImages", uniqueImageName);
			using var fileStream = new FileStream(profilePath, FileMode.Create);
			await file.CopyToAsync(fileStream);
			return profilePath;
		}

		private async Task UpdateUserNameAsync(EditUserName msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user name can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}
				userToken.User.UserName = msg.UserName;
				userToken.User.LastModifiedDate = DateTime.Now.ToString();
				context.Update(userToken);
				var saved = await context.SaveChangesAsync();
				if (saved < 1)
				{
					logger.Error($"{ActorName}, error occurred while updating database , {saved} rows updated");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
					return;
				}
				logger.Info($"{ActorName} user successfully updated");
				Sender.Tell(new UserEdited(msg.RequestId), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user name can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		private async Task UpdateUserMobileAsync(EditUserMobile msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user mobile can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}
				userToken.User.MobileNumber = msg.Mobile;
				userToken.User.LastModifiedDate = DateTime.Now.ToString();
				context.Update(userToken);
				var saved = await context.SaveChangesAsync();
				if (saved < 1)
				{
					logger.Error($"{ActorName}, error occurred while updating database , {saved} rows updated");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
					return;
				}
				logger.Info($"{ActorName} user successfully updated");
				Sender.Tell(new UserEdited(msg.RequestId), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user mobile can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		private async Task UpdateUserAddressAsync(EditUserAddress msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user address can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}
				userToken.User.HomeAddress = msg.Address;
				userToken.User.LastModifiedDate = DateTime.Now.ToString();
				context.Update(userToken);
				var saved = await context.SaveChangesAsync();
				if (saved < 1)
				{
					logger.Error($"{ActorName}, error occurred while updating database , {saved} rows updated");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
					return;
				}
				logger.Info($"{ActorName} user successfully updated");
				Sender.Tell(new UserEdited(msg.RequestId), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user address can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		private async Task UpdateUserEmailAsync(EditUserEmail msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingContext>();
				var userToken = await context.UserTokens.Include(ut => ut.User).SingleOrDefaultAsync(ut => ut.TokenValue == msg.UserToken);
				if (userToken == null)
				{
					logger.Error($"{ActorName}, user email can not be updated , user does not exist in database");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.UserNotExistInDatabase), Self);
					return;
				}
				userToken.User.Email = msg.Email;
				userToken.User.LastModifiedDate = DateTime.Now.ToString();
				context.Update(userToken);
				var saved = await context.SaveChangesAsync();
				if (saved < 1)
				{
					logger.Error($"{ActorName}, error occurred while updating database , {saved} rows updated");
					Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
					return;
				}
				logger.Info($"{ActorName} user successfully updated");
				Sender.Tell(new UserEdited(msg.RequestId), Self);
			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user email can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotUpdateUser(msg.RequestId, ProfileError.DatabaseUpdateError), Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new ProfileStorageActor());

	}
}
