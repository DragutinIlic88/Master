using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
	public interface IPasswordHasher
	{
		string Hash(string password);

		(bool Verified, bool NeedsUpgrade) Check(string hash, string password);
	}
}
