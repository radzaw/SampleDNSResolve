using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;

using SampleDNSResolve.Infrastructure;
using SampleDNSResolve.Models;
using SampleDNSResolve.Repositories;
using SampleDNSResolve.Services;

namespace SampleDNSResolve
{
	class Program
	{
		static IServiceProvider _serviceProvider;
		static ILogger _logger;

		static void RegisterServices()
		{
			var services = new ServiceCollection()
				.AddLogging(builder => builder.ClearProviders().AddConsole(options => {
					options.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Systemd;
				}));

			services.AddTransient<IDNSResolveService, DNSResolveService>();
			services.AddTransient<IDomainNameRepository, DomainNameRepository>();
			services.AddTransient<DomainNameClient>();
			services.AddTransient<InMemoryCache<List<MxRecord>>>();
			services.AddTransient<InMemoryCache<List<string>>>();

			_serviceProvider = services.BuildServiceProvider();

			_logger = _serviceProvider.GetService<ILoggerFactory>()
				.CreateLogger<Program>();
		}

		static void Main(string[] args)
		{
			RegisterServices();

			CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
				.WithParsed(Run);

			((IDisposable)_serviceProvider).Dispose();
		}

		static void Run(CommandLineOptions opts)
		{
			var dnsService = _serviceProvider.GetService<IDNSResolveService>();

			IEnumerable<string> domainsList = null;
			if (opts.InputFile == null)
			{
				domainsList = opts.Domains;
			}
			else
			{
				try
				{
					domainsList = File.ReadAllLines(opts.InputFile);
				}
				catch (Exception e)
				{
					_logger.LogError($"Could not read input file: {e.Message}");
				}
			}

			if (domainsList == null)
			{
				_logger.LogError("Domains list empty.");
				return;
			}

			if (opts.OutputFile != null)
			{
				if (File.Exists(opts.OutputFile))
				{
					File.Delete(opts.OutputFile);
				}
			}

			List<MxToIpResult> results = dnsService.ResolveMxToIps(domainsList, opts.DnsServer);
			string msg;
			foreach (var result in results)
			{
				if (result.Status != ResolveMxStatus.Error)
				{
					string ips = string.Join(',', result.MxIps);
					
					msg = $"{opts.DnsServer} {result.Domain} {ips} {result.Preference}";
				}
				else
				{
					msg = $"Failed to get info for {result.Domain}";
				}

				if (opts.OutputFile != null)
				{
					File.AppendAllText(opts.OutputFile, $"{msg}{Environment.NewLine}");
				}
				else
				{
					_logger.LogInformation(msg);
				}
			}
		}
	}
}
