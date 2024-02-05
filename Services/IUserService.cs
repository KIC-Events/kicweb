using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kicweb.Models.Member;

namespace kicweb.Services
{
	public interface IUserService
	{
		public User CreateUser(RegisterViewModel rvm);
	}
}