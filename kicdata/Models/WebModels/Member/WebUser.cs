using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels.Member
{
	public class WebUser
	{

		public WebUser(RegisterViewModel rvm)
		{
			LegalName = rvm.LegalName;
			EmailAddress = rvm.EmailAddress;
			UserName = rvm.UserName;

			if (rvm.FetName is not null)
			{
				FetName = rvm.FetName;
			}
		}

		public string LegalName { get; set; }

		public string EmailAddress { get; set; }

		public string? FetName { get; set; }

		public string UserName { get; set; }

		public string? HashedPassword { get; set; }

		public bool HasFetName()
		{
			if (FetName is not null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}