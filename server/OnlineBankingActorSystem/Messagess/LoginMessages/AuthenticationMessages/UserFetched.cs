using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record UserFetched(ulong RequestId,string UserId,  string UserName, string Email, string PasswordHash, string MobileNumber, string HomePhoneNumber, string HomeAddress, string ProfileImagePath, string Token, string GenerationTime)
	{
		public override string ToString()
		{
			return $"{nameof(FetchUser)} message: requestId: {RequestId},user id: {UserId}, user name: {UserName}, email: {Email}, password {PasswordHash}, mobile: {MobileNumber}, home phone: {HomePhoneNumber}, address: {HomeAddress}, image path: {ProfileImagePath},  token: {Token}, generation time: {GenerationTime}";
		}
	}
}
