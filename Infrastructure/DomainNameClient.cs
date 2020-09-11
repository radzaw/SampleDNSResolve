using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DnsClient;

using SampleDNSResolve.Models;

namespace SampleDNSResolve.Infrastructure
{
	public class DomainNameClient
	{
		InMemoryCache<List<MxRecord>> _mxCache;
		InMemoryCache<List<string>> _ipCache;

		public DomainNameClient(InMemoryCache<List<MxRecord>> mxCache, InMemoryCache<List<string>> ipCache)
		{
			_mxCache = mxCache;
			_ipCache = ipCache;
		}

		async Task<IDnsQueryResponse> Resolve(string domain, string dnsServer, QueryType type)
		{
			var ipEndPoint = IPEndPoint.Parse(dnsServer);
			ipEndPoint.Port = 53;

			var lookup = new LookupClient(ipEndPoint);
			return await lookup.QueryAsync(domain, type);
		}

		public async Task<List<string>> ResolveIp(string domain, string dnsServer)
		{
			var cacheId = $"{domain}A";
			var fromCache = _ipCache.Get(cacheId);
			if (fromCache != null)
			{
				return fromCache;
			}

			var results = await Resolve(domain, dnsServer, QueryType.A);
			var filtered = results.Answers.ARecords().Select(a => a.Address.ToString()).ToList();

			_ipCache.Add(cacheId, filtered);

			return filtered;
		}

		public async Task<List<MxRecord>> ResolveMx(string domain, string dnsServer)
		{
			var cacheId = $"{domain}MX";
			var fromCache = _mxCache.Get(cacheId);
			if (fromCache != null)
			{
				return fromCache;
			}

			var results = await Resolve(domain, dnsServer, QueryType.MX);
			var filtered = results.Answers.MxRecords()
				.Select(r => new MxRecord() { Domain = r.DomainName.ToString(), Preference = r.Preference }).ToList();

			_mxCache.Add(cacheId, filtered);

			return filtered;
		}
	}
}
