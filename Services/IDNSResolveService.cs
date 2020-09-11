using System.Collections.Generic;

using SampleDNSResolve.Models;

namespace SampleDNSResolve.Services
{
	public interface IDNSResolveService
	{
		public List<MxToIpResult> ResolveMxToIps(IEnumerable<string> domains, string dnsServer);
	}
}
