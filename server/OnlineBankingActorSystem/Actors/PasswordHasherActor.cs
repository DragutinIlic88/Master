using Akka.Actor;
using Contracts;
using OnlineBankingActorSystem.Helpers.Hash;
using OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages;
using OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages;
using System;
using System.Linq;
using System.Security.Cryptography;
using static OnlineBankingActorSystem.Helpers.Constants.ErrorConstants;

namespace OnlineBankingActorSystem.Actors
{
	public class PasswordHasherActor : BaseUntypedActor, IPasswordHasher, ILogReceive
	{

		private const int SaltSize = 16; //128 bit
		private const int KeySize = 32; // 256 bit
		private HashingOptions Options { get; }

		//for more information about Options pattern in ASP.NET Core please visit
		//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
		public PasswordHasherActor(HashingOptions options) : base(nameof(PasswordHasherActor))
		{
			logger.Info($"{ActorName} actor constructor is called");
			Options = options;
		}


		public (bool Verified, bool NeedsUpgrade) Check(string hash, string password)
		{
			var parts = hash.Split('.', 3);

			if (parts.Length != 3)
			{
				throw new FormatException("Unexpected hash format. " + "Should be formatted as `{iterations}.{salt}.{hash}`");
			}

			var iterations = Convert.ToInt32(parts[0]);
			var salt = Convert.FromBase64String(parts[1]);
			var key = Convert.FromBase64String(parts[2]);

			var needsUpgrade = iterations != Options.Iterations;

			using (var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
			{
				var keyToCheck = algorithm.GetBytes(KeySize);

				var verified = keyToCheck.SequenceEqual(key);

				return (verified, needsUpgrade);
			}
		}

		public string Hash(string password)
		{
			//for more information about used alghoritam take a look at 
			//https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.7.2
			using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Options.Iterations, HashAlgorithmName.SHA256))
			{
				var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
				var salt = Convert.ToBase64String(algorithm.Salt);

				return $"{Options.Iterations}.{salt}.{key}";
			}
		}

		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case HashPassword msg:
					try
					{
						logger.Info($"{ActorName}, {nameof(HashPassword)} message received with data: {msg}");
						var result = Hash(msg.Password);
						logger.Info($"{ActorName}, hash function produced {result} value");
						Sender.Tell(result, Self);
					}
					catch (Exception e)
					{
						logger.Error($"{ActorName} hashing of password could not be performed due to {e.Message}");
						Sender.Tell(new Failure { Exception = e }, Self);
					}
					break;
				case UnhashPasswordForAuthenticationValidation msg:
					try
					{
						logger.Info($"{ActorName}, {nameof(UnhashPasswordForAuthenticationValidation)} message received with data: {msg}");
						var (verified, needsUpgrade) = Check(msg.PasswordHash, msg.Password);
						logger.Info($"{ActorName}, check function produced {verified} value for hash and password comparassment");
						if (!verified)
						{
							Sender.Tell(new AuthenticationFailed(msg.RequestId, nameof(UserValidationErrors.UserWithSpecifiedEmailAndPasswordNotExist)),Self);
						}
						else { 
							Sender.Tell(new AuthenticationAllowed(msg.RequestId, msg.Email, msg.Password, msg.PasswordHash), Self);

						}
					}
					catch (Exception e)
					{
						logger.Error($"{ActorName} hashing of password could not be performed due to {e.Message}");
						Sender.Tell(new Failure { Exception = e }, Self);
					}
					break;
			}
		}
		public static Props Props()=> Akka.Actor.Props.Create(() => new PasswordHasherActor(new HashingOptions()));
		
	}
}
