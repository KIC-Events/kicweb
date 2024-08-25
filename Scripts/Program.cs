using KiCData.Models;
using KiCData.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Scripts
{
    public class Scripts
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            IEmailService emailService = new EmailService(configuration, null, null);

            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Send emails from an excel sheet.");
            Console.WriteLine("100. Exit.");
            string response = Console.ReadLine();

            switch (response)
            {
                case "1":
                    SendEmailFromList(configuration, emailService);
                    break;
                case "100":
                    Console.WriteLine("Goodbye.");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("That is an invalid option. Try again.");
                    break;
            }

            Main(args);
        }

        private static void SendEmailFromList(IConfigurationRoot configuration, IEmailService emailService)
        {
            Console.WriteLine("You have chosen to send an email batch from an excel file. If this is incorrect, exit the program and start over.");
            Console.WriteLine("The excel file must have the following properties:");
            Console.WriteLine("The default worksheet has the email list.");
            Console.WriteLine("The first column contains first names.");
            Console.WriteLine("The second column contains last names.");
            Console.WriteLine("The third column contains email addresses.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please enter the full path to the file.");
            string fPath = Console.ReadLine();
            if (!File.Exists(fPath)) 
            { 
                Console.WriteLine("That didn't work. Try again.");
                return;
            }
            Console.WriteLine("Please enter the reason for the discount.");
            string reason = Console.ReadLine();
            Console.WriteLine("Please enter the discount amount (in the form of dd.cc):");
            string amt = Console.ReadLine();
            double compAmount = double.Parse(amt);
            EmailFromList emailFromList = new EmailFromList(fPath);
            Console.WriteLine("connecting to DB...");
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            DbContextOptions<KiCdbContext> options = (DbContextOptions<KiCdbContext>)builder.Options;
            KiCdbContext context = new KiCdbContext(options);
            emailFromList.GetCompCodes(context, reason, compAmount);
            emailFromList.BuildEmails(compAmount);
            emailFromList.SendEmails(emailService);
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
            return;
        }
    }
}