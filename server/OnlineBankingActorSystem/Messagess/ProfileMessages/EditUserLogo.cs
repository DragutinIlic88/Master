using Microsoft.AspNetCore.Http;

namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record EditUserLogo(ulong RequestId, string UserToken,string RootFolderPath,  string LogoName, IFormFile Logo)
	{
		public override string ToString()
		{
			return $"{nameof(EditUserAddress)}, message: request id: {RequestId}, logo name: {LogoName}, root folder: {RootFolderPath}"; 
		}
	}
}
