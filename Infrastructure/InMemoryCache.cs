using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SampleDNSResolve.Infrastructure
{
	public class InMemoryCache<T>
	{
		ConcurrentDictionary<string, T> cacheStorage = new ConcurrentDictionary<string, T>();

		public bool Check(string id)
		{
			return cacheStorage.ContainsKey(id);
		}

		public T Get(string id)
		{
			T result;
			cacheStorage.TryGetValue(id, out result);
			return result;
		}

		public void Add(string id, T element)
		{
			cacheStorage.TryAdd(id, element);
		}
	}
}
