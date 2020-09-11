using System.Collections.Generic;
using System.Threading.Tasks;

using SampleDNSResolve.Models;

namespace SampleDNSResolve.Repositories
{
	public interface IDomainNameRepository
	{
		public Task<List<string>> GetIpRecords(string domain, string dnsServer);

		public Task<List<MxRecord>> GetMxRecords(string domain, string dnsServer);
	}
}
