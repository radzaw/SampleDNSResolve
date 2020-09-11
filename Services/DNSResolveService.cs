using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SampleDNSResolve.Repositories;
using SampleDNSResolve.Models;

namespace SampleDNSResolve.Services
{
	public class DNSResolveService: IDNSResolveService
	{
		IDomainNameRepository _domainRepo;

		public DNSResolveService(IDomainNameRepository domainRepo) => _domainRepo = domainRepo;

		public List<MxToIpResult> ResolveMxToIps(IEnumerable<string> domains, string dnsServer)
		{
			Queue<string> domainsQueue = new Queue<string>(domains);
			var mxRecords = new List<MxToIpResult>();

			int maxConcurrentQueries = 4;
			int runningQueries = 0;
			Task[] tasksList = new Task[maxConcurrentQueries];

			while (domainsQueue.Count > 0)
			{
				string domain = domainsQueue.Dequeue();
				Task task = Task.Run(async () =>
				{
					try
					{
						var results = await _domainRepo.GetMxRecords(domain, dnsServer);
						foreach (var result in results)
						{
							MxToIpResult mxToIp = new MxToIpResult()
							{
								Domain = domain,
								DnsServer = dnsServer,
								Preference = result.Preference,
							};
							mxToIp.MxIps = await _domainRepo.GetIpRecords(result.Domain, dnsServer);
							mxToIp.Status = ResolveMxStatus.OK;

							mxRecords.Add(mxToIp);
						}
					}
					catch (Exception e)
					{
						MxToIpResult mxToIp = new MxToIpResult()
						{
							Domain = domain,
							DnsServer = dnsServer,
							Status = ResolveMxStatus.Error
						};

						mxRecords.Add(mxToIp);
					}
				});

				for (int i = 0; i < tasksList.Length; i++)
				{
					if (tasksList[i] == null)
					{
						tasksList[i] = task;
						runningQueries++;
						break;
					}
				}

				if (runningQueries == maxConcurrentQueries)
				{
					int completedIdx = Task.WaitAny(tasksList);

					tasksList[completedIdx].Dispose();
					tasksList[completedIdx] = null;
					runningQueries--;
				}
			}

			if (runningQueries > 0)
			{
				var newTasksList = new Task[runningQueries];
				int j = 0;
				for (int i = 0; i < tasksList.Length; i++)
				{
					if (tasksList[i] != null)
					{
						newTasksList[j] = tasksList[i];
						j++;
					}
				}

				Task.WaitAll(newTasksList);
			}

			return mxRecords;
		}
	}
}
