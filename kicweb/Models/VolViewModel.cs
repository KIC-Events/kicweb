using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kicweb.Models
{
	public class VolViewModel
	{
		[Required]
		public string? LegalName { get; set; }

		[Required]
		public string? FetName { get; set; }

		[Required]
		public int? ClubID { get; set; }

		[Required]
		public string? EmailAddress { get; set; }

		public List<string>? Positions { get; set; }

		[Required]
		public string? Details { get; set; }
	}
}