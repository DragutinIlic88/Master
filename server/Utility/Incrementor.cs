using Contracts;
using System;
using System.Collections.Concurrent;

namespace Utility
{
	public class Incrementor : IIncrement
	{ 
		private ConcurrentDictionary<string, ulong> _counterDictionary = new ConcurrentDictionary<string, ulong>();

		public ulong Increment(string typeName) {

			
			return _counterDictionary.AddOrUpdate(typeName, 1, (typeName, oldCounter) => UInt64.MaxValue == oldCounter ? 1 : oldCounter + 1);
			
		}
	}
}
