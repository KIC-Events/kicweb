using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels.Member
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Please provide your full legal name.")]
		[Display(Name = "Legal Name")]
		public string? LegalName { get; set; }

		[Required(ErrorMessage = "Please provide a valid email address.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		[Display(Name = "Email Address")]
		public string? EmailAddress { get; set; }

		[Display(Name = "Fetlife Account Name")]
		public string? FetName { get; set; }

		[Required(ErrorMessage = "Please enter your desired username.")]
		[Display(Name = "User Name")]
		public string? UserName { get; set; }

		[Required(ErrorMessage = "Please enter your password.")]
		[Display(Name = "Password")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Password doesn't match.")]
		[Display(Name = "Re-Enter Password")]
		public string? Password2 { get; set; }

		public RegisterViewModel(string legalName, string emailAddress, string userName, string password, string password2, string? fetlife = null)
		{
			LegalName = legalName;
			EmailAddress = emailAddress;
			UserName = userName;
			Password = password;
			Password2 = password2;

			if (fetlife is not null)
			{
				FetName = fetlife;
			}
		}

		public RegisterViewModel()
		{

		}
	}
}