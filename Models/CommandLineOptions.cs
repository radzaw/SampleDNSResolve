using System.Collections.Generic;

using CommandLine;

namespace SampleDNSResolve.Models
{
	class CommandLineOptions
	{
		[Option('i', "inputfile", Required = false, HelpText = "Domains to be checked in a file, each domain in separate line", Group = "domainsinput")]
		public string InputFile { get; set; }

		[Option('d', "domains", Group = "domainsinput", HelpText = "Space separated list of domains to check")]
		public IEnumerable<string> Domains { get; set; }

		[Option('s', "dns", Required = true, HelpText = "DNS server to use for checks")]
		public string DnsServer { get; set; }

		[Option('o', "outputfile", Required = false, HelpText = "Output file, if not specified output on console")]
		public string OutputFile { get; set; }

	}
}
