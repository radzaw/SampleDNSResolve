using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDNSResolve.Models
{
	public class MxToIpResult
	{
		public string Domain { get; set; }
		public List<string> MxIps { get; set; }
		public string DnsServer { get; set; }
		public int Preference { get; set; }

		public ResolveMxStatus Status { get; set; }
	}
}
