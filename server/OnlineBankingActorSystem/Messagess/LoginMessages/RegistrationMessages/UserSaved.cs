
namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record UserSaved (ulong RequestId, bool IsSuccess)
	{

		public override string ToString()
		{
			return $"{nameof(UserSaved)} message: requestId: {RequestId},  is valid: {IsSuccess}";
		}
	}
}