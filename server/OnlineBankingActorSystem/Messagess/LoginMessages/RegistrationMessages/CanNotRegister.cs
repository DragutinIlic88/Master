
namespace OnlineBankingActorSystem.Messagess.LoginMessages.RegistrationMessages
{
	public record CanNotRegister (ulong RequestId, string ErrorMessage, string Details = "")
	{

		public override string ToString()
		{
			return $"{nameof(CanNotRegister)} message: request id: {RequestId}, error message: {ErrorMessage}, details: {Details}";
		}
	}
}
