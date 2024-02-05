using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kicweb.Models
{
	public class VolViewModel
	{
		public string? LegalName { get; set; }

		public string? FetName { get; set; }

		public string? EmailAddress { get; set; }

		public List<string>? Positions { get; set; }

		public string? Details { get; set; }
	}
}