﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingActorSystem.Messagess.LoginMessages.AuthenticationMessages
{
	public record TokenGenerated(ulong RequestId, string Email, string PasswordHash, string Token, string GenerationTime)
	{
		public override string ToString()
		{
			return $"{nameof(TokenGenerated)} message: requestId: {RequestId}, email: {Email}, password {PasswordHash}, token: {Token}, generation time: {GenerationTime}";
		}
	}
}
