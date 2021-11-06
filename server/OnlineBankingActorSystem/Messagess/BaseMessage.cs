using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBankingActorSystem.Messagess
{
	public class BaseMessage
	{
		public ulong RequestId { get; }

		public BaseMessage(ulong requestId)
		{
			RequestId = requestId;
		}
	}
}
