using Akka.Actor;
using OnlineBankingActorSystem.Helpers.Enums;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using System;


namespace OnlineBankingActorSystem.Actors
{

	public class RandomRoleAssignerActor : BaseUntypedActor
	{
		public RandomRoleAssignerActor() : base(nameof(RandomRoleAssignerActor))
		{ 
		}
		protected override void OnReceive(object message)
		{
			switch (message) {
				case GetRoleForUser msg:
					try
					{
						logger.Info($"{ActorName} , {nameof(GetRoleForUser)} message received with data: {msg}");
						var values = Enum.GetValues(typeof(Role));
						var random = new Random();
						Role randomRole = (Role)values.GetValue(random.Next(values.Length));
						Sender.Tell( randomRole, Self);
					}
					catch (Exception e) {
						Sender.Tell(new Failure { Exception = e }, Self);
					}
					break;
			}
		}

		public static Props Props() => Akka.Actor.Props.Create(() => new RandomRoleAssignerActor());
		
	}
}
