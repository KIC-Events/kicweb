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
            if (Environment.GetEnvironmentVariable("AASPNETCORE_ENVIRONMENT") == "Production")
            {
                builder.AddJsonFile("appsettings.Production.json");
            }
            else
            {
                builder.AddJsonFile("appsettings.Development.json");
            }
            IConfigurationRoot configuration = builder.Build();
            IEmailService emailService = new EmailService(configuration, null, null);

            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Send emails from an excel sheet.");
            Console.WriteLine("2. Create comps from excel sheet.");
            Console.WriteLine("3. Invite to closed beta from excel sheet.");
            Console.WriteLine("4. Delete duplicates from DB.");
            Console.WriteLine("5. Send emails from DB");
            Console.WriteLine("100. Exit.");
            string response = Console.ReadLine();

            switch (response)
            {
                case "1":
                    SendEmailFromList(configuration, emailService);
                    break;
                case "2":
                    CreateCompsFromList(configuration);
                    break;
                case "3":
                    InviteToBeta(configuration, emailService);
                    break;
                case "4":
                    DeleteDuplicates(configuration);
                    break;
                case "5":
                    SendEmailsFromDB(configuration, emailService);
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

        private static void SendEmailsFromDB(IConfigurationRoot configuration, IEmailService emailService)
        {
            Console.WriteLine("Sending emails from DB");
            DbContextOptionsBuilder<KiCdbContext> builder = new DbContextOptionsBuilder<KiCdbContext>();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            KiCdbContext context = new KiCdbContext(builder.Options);
            EmailFromDB emailFromDB = new EmailFromDB(configuration, context, emailService);
            emailFromDB.SendEmailsFromDB();
        }

        private static void DeleteDuplicates(IConfigurationRoot configuration)
        {
            Console.WriteLine("Deleting dupes.");
            Console.WriteLine("Buckle up.");
            DbContextOptionsBuilder<KiCdbContext> builder = new DbContextOptionsBuilder<KiCdbContext>();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            KiCdbContext context = new KiCdbContext(builder.Options);
            DeleteDuplicatesFromDB deleteDuplicatesFromDB = new DeleteDuplicatesFromDB(context);
            deleteDuplicatesFromDB.RunDelete();
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
            EmailFromList emailFromList = new EmailFromList(fPath);
            Console.WriteLine("connecting to DB...");
            DbContextOptionsBuilder<KiCdbContext> builder = new DbContextOptionsBuilder<KiCdbContext>();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            KiCdbContext context = new KiCdbContext(builder.Options);
            emailFromList.GetCompCodes(context);
            emailFromList.BuildEmails();
            emailFromList.SendEmails(emailService);
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
            return;
        }

        private static void CreateCompsFromList(IConfigurationRoot configuration)
        {
            Console.WriteLine("You have chosen to send an email batch from an excel file. If this is incorrect, exit the program and start over.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please enter the full path to the file.");
            string fPath = Console.ReadLine();
            Console.WriteLine("Setting up...");
            if (!File.Exists(fPath))
            {
                Console.WriteLine("That didn't work. Try again.");
                return;
            }
            DbContextOptionsBuilder<KiCdbContext> builder = new DbContextOptionsBuilder<KiCdbContext>();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            //DbContextOptions<KiCdbContext> options = (DbContextOptions<KiCdbContext>)builder.Options;
            KiCdbContext context = new KiCdbContext(builder.Options);
            CompsFromList compsFromList = new CompsFromList(fPath, context);
            compsFromList.WriteToDB(context);
            Console.WriteLine("Complete");
            return;
        }

        private static void InviteToBeta(IConfigurationRoot configuration, IEmailService emailService)
        {
            Console.WriteLine("You have chosen to send an email beta invite from an excel file. If this is incorrect, exit the program and start over.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please enter the full path to the file.");
            string fPath = Console.ReadLine();
            Console.WriteLine("Setting up...");
            if (!File.Exists(fPath))
            {
                Console.WriteLine("That didn't work. Try again.");
                return;
            }
            DbContextOptionsBuilder<KiCdbContext> builder = new DbContextOptionsBuilder<KiCdbContext>();
            builder.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            //DbContextOptions<KiCdbContext> options = (DbContextOptions<KiCdbContext>)builder.Options;
            KiCdbContext context = new KiCdbContext(builder.Options);
            BetaInviteFromList betaInviteFromList = new BetaInviteFromList(fPath);
            betaInviteFromList.BuildEmails();
            betaInviteFromList.SendEmails(emailService);
        }
    }
}