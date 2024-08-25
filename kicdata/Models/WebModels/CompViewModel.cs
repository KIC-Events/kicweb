using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    
    public class CompViewModel
    {
        private readonly KiCdbContext _context;

        [Required(ErrorMessage = "Please enter the number of comps being added")]
        [Display(Name = "Comp Quantity")]
        public int CompQuantity { get; set; }

        [Required(ErrorMessage = "Please enter the amount off of the ticket price to be comp")]
        [Display(Name = "Comp Amount")]
        public double? CompAmount { get; set; }

        [Display(Name = "Comp Reason")]
        public string? CompReason { get; set; }

        [Required(ErrorMessage = "Please enter the user authorizing the comps")]
        [Display(Name = "Authorizing User")]
        public string AuthorizingUser { get; set; }

        public string DiscountCode 
        { get
            {
                return SetDiscountCode(_context);
            } 
        }

        private static string SetDiscountCode(KiCdbContext kiCdbContext)
        {
            string result = string.Empty;
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int size = 10;
            Random res = new Random();
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                result += str[x];
            }
            if (kiCdbContext.TicketComp.Where(a=> a.DiscountCode == result).Any())
            {
                SetDiscountCode(kiCdbContext);
                return result;
            }
            else 
            { 
                return result; 
            }
        }
    }

}
