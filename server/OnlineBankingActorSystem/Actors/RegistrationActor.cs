using System;
using System.Collections.Concurrent;
using Akka.Actor;
using Akka.Persistence;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using Akka.Event;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{

	public class RegistrationActor : UntypedPersistentActor, ILogReceive
	{
		private class RegistrationState
		{
			public string Email { get; set; }
			public string Password { get; set; }
			public string ConfimrPassword { get; set; }
			public string BankId { get; set; }
			public string Mobile { get; set; }
			public string UserName { get; set; }
			public string Address { get; set; }
			public string HomePhone { get; set; }

			public bool CanRegister { get; set; }

			public bool IsSaved { get; set; }
		}
		private readonly ConcurrentDictionary<ulong, RegistrationState> states = new();
		private readonly ConcurrentDictionary<ulong, IActorRef> loginManagerActors = new();
		private readonly IActorRef validationActor = Context.ActorOf(RegistrationValidationActor.Props(), "registrationValidationActor");
		private readonly IActorRef storageActor = Context.ActorOf(RegistrationStorageActor.Props(), "registrationStorageActor");
		private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
		private readonly string ActorName = nameof(RegistrationActor);

		public override string PersistenceId => Self.Path.Name;

		protected override void PreStart()
		{
			logger.Info($"{ActorName} actor will start");
			base.PreStart();
		}

		protected override void OnCommand(object message)
		{
			switch (message)
			{
				case Register register:
					logger.Info($"{ActorName}, Register message received with data: {register}");
					loginManagerActors.TryAdd(register.RequestId, Sender);
					var registrationArrivedEvent = new RegistrationArrived(register.RequestId, register.Email, register.Password,
						register.ConfirmPassword, register.BankId, register.Mobile, register.UserName, register.Address, register.HomePhone);
					Persist(registrationArrivedEvent, HandleEvent);
					break;
				case UserValidated uv:
					logger.Info($"{ActorName}, message received: {uv}");
					Persist(uv, HandleEvent);
					break;
				case UserSaved us:
					logger.Info($"{ActorName}, message received: {us}");
					Persist(us, HandleEvent);
					break;
				case SaveSnapshotSuccess snapshotSuccess:
					logger.Info($"{ActorName} snapshot is saved successfully");
					break;
				case SaveSnapshotFailure snapshotFailure:
					logger.Info($"{ActorName} shapshot could not be saved");
					break;

			}
		}

		protected override void PreRestart(Exception reason, object message)
		{
			logger.Error($"{ActorName} will be resterted due the reason {reason.Message}");
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

		protected override void OnRecover(object message)
		{
			HandleEvent(message);
		}

		private void HandleEvent(object @event)
		{
			switch (@event)
			{
				case RegistrationArrived regArrived:
					states.TryAdd(regArrived.RequestId, new RegistrationState
					{
						Email = regArrived.Email,
						Password = regArrived.Password,
						ConfimrPassword = regArrived.ConfirmPassword,
						BankId = regArrived.BankId,
						Mobile = regArrived.Mobile,
						UserName = regArrived.UserName,
						Address = regArrived.Address,
						HomePhone = regArrived.HomePhone
					});
					validationActor.Tell(new ValidateUser(regArrived.RequestId, regArrived.BankId, regArrived.Email, regArrived.Mobile), Self);
					break;
				case UserValidated uv:
					states.TryGetValue(uv.RequestId, out RegistrationState userValidatedState);
					if (userValidatedState != null)
					{
						userValidatedState.CanRegister = uv.IsValid;
					}
					else { 
						logger.Error($"{ActorName}, state with id {uv.RequestId} could not be retrieved from states");
						loginManagerActors[uv.RequestId].Tell(new CanNotRegister(uv.RequestId, UserValidationErrors.StateInvalidWhileValidateingUser), Self);
						loginManagerActors.TryRemove(uv.RequestId, out _);

					}
					if (userValidatedState.CanRegister)
					{
						storageActor.Tell(new SaveUser(uv.RequestId, userValidatedState.Email, userValidatedState.Password, userValidatedState.ConfimrPassword,
							userValidatedState.BankId, userValidatedState.Mobile, userValidatedState.UserName, userValidatedState.Address, userValidatedState.HomePhone), Self);
					}
					else
					{
						states.TryRemove(uv.RequestId,out _);
						SaveSnapshot(states);
						loginManagerActors[uv.RequestId].Tell(new CanNotRegister(uv.RequestId, uv.ErrorMessage), Self);
						loginManagerActors.TryRemove(uv.RequestId, out _);
					}
					break;
				case UserSaved us:
					states.TryGetValue(us.RequestId, out RegistrationState userSavedState);
					if (userSavedState != null)
					{
						userSavedState.IsSaved = us.IsSuccess;
						states.TryRemove(us.RequestId, out _);
						SaveSnapshot(states);
					}
					loginManagerActors[us.RequestId].Tell(us, Self);
					loginManagerActors.TryRemove(us.RequestId, out _);
					break;
				case SnapshotOffer snapshot when snapshot.Snapshot is ConcurrentDictionary<ulong, RegistrationState> registrationStates:
					if (registrationStates != null)
					{
						foreach (var state in registrationStates) {
							if (!states.TryAdd(state.Key, state.Value)) {
								logger.Info($"{ActorName}, state ({state.Key},{state.Value}) could not be added from snapshot to current state");
							}
						}
					}
					break;
			}
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create(() => new RegistrationActor());
		}
	}
}
