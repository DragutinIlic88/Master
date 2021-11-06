using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{
	public record UserDataRetrieved(ulong RequestId, string UserToken,string UserName,  string Email, string Mobile, string Address,string ProfileImagePath, string LastLoginDate, string RegistrationDate )
	{
		public override string ToString()
		{
			return $"{nameof(GetUserData)}, message: request id: {RequestId}, user token: {UserToken}, user name: {UserName}," +
				$" email: {Email}, mobile: {Mobile}, address: {Address}, image path: {ProfileImagePath},last login date: {LastLoginDate}, registration date: {RegistrationDate}";
		}
	}
}
