using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Routing;
using Contracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using OnlineBankingActorSystem.Actors;
using OnlineBankingActorSystem.Constants;
using OnlineBankingActorSystem.ServiceScopeExtension;
using System;
using System.IO;

namespace OnlineBankingActorSystem
{
	public static class ServiceCollectionExtensions
	{
		public static void AddActorSystem(this IServiceCollection services, AkkaSystemConfiguration configurationOption)
		{
			var configPath =configurationOption == AkkaSystemConfiguration.SeedNode? 
				Path.Combine(ConfigurationConstants.ProjectDirectory, ConfigurationConstants.SeedNodeConfigurationFile) :
				Path.Combine(ConfigurationConstants.ProjectDirectory, ConfigurationConstants.NodeConfigurationFile);
			var configString = File.ReadAllText(configPath);
			var config = ConfigurationFactory.ParseString(configString);
			var bootstrap = BootstrapSetup.Create().WithConfig(config);
			
			services.AddSingleton(provider =>
			{
				//getting service scope from provider , and use extension method to add 
				//that service scope before regestering actor system
				var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();
				var di = DependencyResolverSetup.Create(provider);
				var actorSystemSetup = bootstrap.And(di);
				var actorSystem = ActorSystem.Create("OnlineBanking", actorSystemSetup);
				actorSystem.AddServiceScopeFactory(serviceScopeFactory);
				return actorSystem;
			}
			);
		}

		public static void AddActorProviders(this IServiceCollection services)
		{
			//delegate of type LoginManagerActorProvider is registered in IoC container
			//with function which returns corresponding actor reference
			//contariner will guarantee that the given actor will be created only once
			services.AddSingleton<LoginManagerActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var loginActor = actorSystem.ActorOf(LoginManagerActor.Props().WithRouter(FromConfig.Instance), "loginManager");
				return () => loginActor;
			});

			services.AddSingleton<AccountGetterActorProivder>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var accountGetterActor = actorSystem.ActorOf(AccountGetterActor.Props().WithRouter(FromConfig.Instance), "accountGetterActor");
				return () => accountGetterActor;
			});

			services.AddSingleton<TransactionGetterActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var transactionsGetterActor = actorSystem.ActorOf(TransactionsGetterActor.Props().WithRouter(FromConfig.Instance), "transactionGetterActor");
				return () => transactionsGetterActor;
			});

			services.AddSingleton<CurrenciesGetterActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var currenciesGetterActor = actorSystem.ActorOf(CurrenciesGetterActor.Props().WithRouter(FromConfig.Instance), "currenciesGetterActor");
				return () => currenciesGetterActor;
			});

			services.AddSingleton<FeeGetterActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var feeGetterActor = actorSystem.ActorOf(FeeGetterActor.Props().WithRouter(FromConfig.Instance), "feeGetterActor");
				return () => feeGetterActor;
			});


			services.AddSingleton<ConfirmExchangeActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var confirmExchangeActor = actorSystem.ActorOf(ConfirmExchangeActor.Props().WithRouter(FromConfig.Instance), "confirmExchangeActor");
				return () => confirmExchangeActor;
			});

			services.AddSingleton<PaymentActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var paymentActor = actorSystem.ActorOf(PaymentActor.Props().WithRouter(FromConfig.Instance), "paymentActor");
				return () => paymentActor;
			});

			services.AddSingleton<LoanActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var loanActor = actorSystem.ActorOf(LoanActor.Props().WithRouter(FromConfig.Instance), "loanActor");
				return () => loanActor;
			});

			services.AddSingleton<ProfileActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var profileActor = actorSystem.ActorOf(ProfileStorageActor.Props().WithRouter(FromConfig.Instance), "profileActor");
				return () => profileActor;
			});

			services.AddSingleton<HelpActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var helpActor = actorSystem.ActorOf(HelpActor.Props().WithRouter(FromConfig.Instance), "helpActor");
				return () => helpActor;
			});

			services.AddSingleton<NotificationActorProvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var notificationActor = actorSystem.ActorOf(DependencyResolver.For(actorSystem).Props<NotificationActor>(), "notificationActor");
				return () => notificationActor;
			});

			services.AddSingleton<NotificationStorageActorPorvider>(provider =>
			{
				var actorSystem = provider.GetService<ActorSystem>();
				var notificationStorageActor = actorSystem.ActorOf(NotificationStorageActor.Props().WithRouter(FromConfig.Instance), "notificationStorageActor");
				return () => notificationStorageActor;
			});
		}
	}
}
