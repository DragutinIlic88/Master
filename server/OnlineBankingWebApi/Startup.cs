using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggerService;
using System.IO;
using Contracts;
using OnlineBankingDBContextLib;
using BankUsersDBContextLib;
using Microsoft.EntityFrameworkCore;
using OnlineBankingActorSystem;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using Akka.Actor;
using Utility;
using Microsoft.Extensions.FileProviders;
using OnlineBankingWebApi.Hubs;
using Contracts.Enums;

namespace OnlineBankingWebApi
{
	public class Startup
	{

		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		public Startup(IConfiguration configuration)
		{
			LoggerManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{


			//adding NLoger into dependency injection container
			services.AddSingleton<ILoggerManager, LoggerManager>();

			services.AddSingleton<IIncrement, Incrementor>();

			//adding OnlineBankingContext into dependency injection container
			services.AddDbContext<OnlineBankingContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi"))
				);

			//adding OnlineBankingAccountContext into depenedecy injeciton container
			services.AddDbContext<OnlineBankingAccountContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingAccountDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding OnlineBankingTransactionContext into depenedecy injeciton container
			services.AddDbContext<OnlineBankingTransactionContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingTransactionDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding OnlineBankingCurrencyContext into depenedecy injeciton container
			services.AddDbContext<OnlineBankingCurrencyContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingCurrencyDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding OnlineBankingHelpContext into depenedecy injeciton container
			services.AddDbContext<OnlineBankingHelpContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingHelpDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding OnlineBankingNotificationContext into depenedecy injeciton container
			services.AddDbContext<OnlineBankingNotificationContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("OnlineBankingNotificationContext"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding BankUsersContext into dependency injection container
			services.AddDbContext<BankUsersContext>(options =>
			options.EnableSensitiveDataLogging()
			.UseSqlServer(Configuration.GetConnectionString("BankUsersDatabase"), b => b.MigrationsAssembly("OnlineBankingWebApi")));

			//adding NotificationHubHelper to IoC container so it can be used inside 
			services.AddSingleton<INotificationHubHelper, NotificationHubHelper>();

			//adding OnlineBanking actor system inside IoC container
			//extension method defined inside OnlineBankingActorSystem namespace
			//this will be the system on the seed node inside of akka cluster
			services.AddActorSystem(AkkaSystemConfiguration.SeedNode);

			//adding OnlineBanking actor system inside IoC container
			//extension method defined inside OnlineBankingActorSystem namespace
			//this will be the system on the node who will join the cluster after seed node is established
			services.AddActorSystem(AkkaSystemConfiguration.Node);
			
			//adding OnlineBanking actors reference inside IoC container
			//extension method defined inside OnlineBankingActorSystem namespace
			services.AddActorProviders();

			//adding CORS where origin with url http://localhost:3000 or with porxy http://localhost:9000/api
			//can access this server
			services.AddCors(options =>
			{
				options.AddPolicy(name: MyAllowSpecificOrigins,
								  builder =>
								  {
									  builder.WithOrigins("http://localhost:3000", "http://localhost:9000/api")
									  .AllowAnyHeader()
									  .AllowAnyMethod()
									  .SetIsOriginAllowed((x) => true)
									  .AllowCredentials();
								  });
			});

			services.AddControllers();

			//adding signalR so notifications can be passed from server to proper user
			services.AddSignalR();

			
			
			services.PostConfigure<ApiBehaviorOptions>(options =>
			{
				var builtInFactory = options.InvalidModelStateResponseFactory;

				options.InvalidModelStateResponseFactory = context =>
				{
					var loggerManager = context.HttpContext.RequestServices.GetRequiredService<ILoggerManager>();
					loggerManager.LogInfo($"Request with URL {context.HttpContext.Request.Host}{context.HttpContext.Request.Path.Value}" +
						$" reached endpoint {context.ActionDescriptor.DisplayName}" +
						$" end produce error message: \"{context.ModelState.Values.First().Errors.First().ErrorMessage}\"");
					return builtInFactory(context);
				};
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			//new event is subscribed when application is started so it can start ActorSystem
			lifetime.ApplicationStarted.Register(() =>
			{
				app.ApplicationServices.GetService<ActorSystem>();
			});

			//new event is subscribed right before application is stoped so it can terminate ActorSystem
			lifetime.ApplicationStopping.Register(() =>
			{
				//terminate method is asynchronous so it needs to be chaind with wait method
				app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();

			});

			app.UseStaticFiles(new StaticFileOptions { 
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "ProfileImages")),
				RequestPath = "/ProfileImages"
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(MyAllowSpecificOrigins);

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				//using of attributed routing for REST APIs
				endpoints.MapControllers().RequireCors(MyAllowSpecificOrigins);
				endpoints.MapHub<NotificationHub>("/hubs/notificationHub");
			});
		}
	}
}
