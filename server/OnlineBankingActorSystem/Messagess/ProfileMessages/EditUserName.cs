using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.ProfileMessages
{

	public record EditUserName(ulong RequestId, string UserToken, string UserName)
	{
		public override string ToString()
		{
			return $"{nameof(EditUserName)} message: request id: {RequestId}, user token: {UserToken}, user name: {UserName}";
		}
	}
}
