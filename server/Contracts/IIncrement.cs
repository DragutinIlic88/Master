using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
	public interface IIncrement
	{
		public ulong Increment(string typeName);
	}
}
