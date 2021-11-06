﻿using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using System;

namespace OnlineBankingActorSystem.Actors
{
	public abstract class BaseUntypedPersistentActor : UntypedPersistentActor
	{
		protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
		protected readonly string ActorName;

		protected override void PreStart()
		{
			logger.Info($"{ActorName} actor will start");
			base.PreStart();
		}

		public BaseUntypedPersistentActor(string actorName)
		{
			ActorName = actorName;
		}

		protected override void PostStop()
		{
			logger.Info($"{ActorName} stopped");
			base.PostStop();
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
	}
}
