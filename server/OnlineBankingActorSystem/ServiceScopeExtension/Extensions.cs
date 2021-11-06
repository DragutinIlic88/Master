using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.ServiceScopeExtension;

namespace OnlineBankingActorSystem.ServiceScopeExtension
{
	/**
	 *Akka.Extensions are not particulary user friendly, that's why it is considered a good practice to use
	 *C# extension methods to give it a better look how to use ServiceScopeExtensionIdProvider and ServiceScopeExtension classes
	 *We need to methods: the first one to initialize extension and the second to create IServiceScope
	 */
	public static class Extensions
	{
		public static void AddServiceScopeFactory(this ActorSystem system, IServiceScopeFactory serviceScopeFactory) {
			system.RegisterExtension(ServiceScopeExtensionIdProvider.Instance);
			ServiceScopeExtensionIdProvider.Instance.Get(system).Initialize(serviceScopeFactory);
		}

		public static IServiceScope CreateScope(this IActorContext context) {
			return ServiceScopeExtensionIdProvider.Instance.Get(context.System).CreateScope();
		}

	}
}
