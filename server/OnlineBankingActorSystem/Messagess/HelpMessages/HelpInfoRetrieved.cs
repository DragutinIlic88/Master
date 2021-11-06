using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.HelpMessages
{
	public record HelpInfoRetrieved(ulong RequestId, string HelpPhoneNumber, string HelpEmailAddress)
	{
		public override string ToString()
		{
			return $"{nameof(HelpInfoRetrieved)} message: request id: {RequestId}, phone number: {HelpPhoneNumber}, email address: {HelpEmailAddress}";
		}
	}
}
