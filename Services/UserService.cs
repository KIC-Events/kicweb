using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kicweb.Models;
using kicweb.Models.Member;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Intrinsics.Arm;

namespace kicweb.Services
{
	public class UserService : IUserService
	{
		public User CreateUser(RegisterViewModel rvm)
		{
			User user = new User(rvm);

			user.HashedPassword = EncryptPassword(user, rvm.Password);

			return user;
		}

		private string EncryptPassword(User user, string password)
		{
			string salt = user.EmailAddress.Split('@')[1];

			string saltedPassword = password + salt;

			SHA256 hashAlgorithm = SHA256.Create();

			byte[] hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

			string result = Convert.ToBase64String(hash);

			return result;
		}
	}
}