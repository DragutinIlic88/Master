using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Messagess.HelpMessages;
using OnlineBankingActorSystem.ServiceScopeExtension;
using OnlineBankingDBContextLib;
using System;
using System.Threading.Tasks;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class HelpActor : BaseReceiveActor, ILogReceive
	{

		public HelpActor() :base(nameof(HelpActor))
		{
			ReceiveAsync<GetHelpInfo>(async( message)=> {
				logger.Info($"{ActorName} , message received with data: {message}");
				await GetHelpInfoAsync(message);
			});
		}

		private async Task GetHelpInfoAsync(GetHelpInfo msg)
		{
			try
			{
				using IServiceScope serviceScope = Context.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<OnlineBankingHelpContext>();
				var helpInformation = await context.HelpInformations.FirstOrDefaultAsync();
				if (helpInformation == null)
				{
					logger.Error($"{ActorName}, help information does not exist in database");
					Sender.Tell(new CouldNotGetHelpInfo(msg.RequestId, HelpError.DatabaseGetError), Self);
					return;
				}

				Sender.Tell(new HelpInfoRetrieved(msg.RequestId, helpInformation.PhoneNumber, helpInformation.EmailAddress), Self);

			}
			catch (Exception e)
			{
				logger.Error($"{ActorName}, user name can not be updated , due to {e.GetBaseException()}");
				Sender.Tell(new CouldNotGetHelpInfo(msg.RequestId, HelpError.DatabaseGetError), Self);
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new HelpActor());
	}
}
