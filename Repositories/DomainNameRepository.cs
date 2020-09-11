using System.Collections.Generic;
using System.Threading.Tasks;

using SampleDNSResolve.Infrastructure;
using SampleDNSResolve.Models;

namespace SampleDNSResolve.Repositories
{
	public class DomainNameRepository: IDomainNameRepository
	{
		DomainNameClient _client;

		public DomainNameRepository(DomainNameClient client) => _client = client;

		public async Task<List<string>> GetIpRecords(string domain, string dnsServer)
		{
			var results = await _client.ResolveIp(domain, dnsServer);
			return results ?? new List<string>();
		}

		public async Task<List<MxRecord>> GetMxRecords(string domain, string dnsServer)
		{
			var results = await _client.ResolveMx(domain, dnsServer);
			return results ?? new List<MxRecord>();
		}
	}
}
