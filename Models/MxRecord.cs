using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDNSResolve.Models
{
	public class MxRecord
	{
		public string Domain { get; set; }
		public int Preference { get; set; }
	}
}
