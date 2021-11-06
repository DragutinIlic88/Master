using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineBankingActorSystem.ServiceScopeExtension
{
	/**
	 * Class uses ISerivceScopeFactory to create scope around object which will be instatiated  in it (database context in this application).
	 * This is neccassery so we can control lifetime of database context , because actors live as long as system is alive, if not conifgured differentlly.
	 * Class use IExtension interface which is part of Akka.Extension and makes it possible to add new functionality to existing system.
	 */
	public class ServiceScopeExtension : IExtension
	{
		private IServiceScopeFactory _serviceScopeFactory;

		public void Initialize(IServiceScopeFactory serviceScopeFactory) {
			_serviceScopeFactory = serviceScopeFactory;
		}

		public IServiceScope CreateScope()
		{
			return _serviceScopeFactory.CreateScope();
		}
	}
}
