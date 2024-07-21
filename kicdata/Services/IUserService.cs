using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiCData.Models.WebModels.Member;

namespace KiCData.Services
{
	public interface IUserService
	{
		public WebUser CreateUser(RegisterViewModel rvm);
	}
}