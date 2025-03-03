using System.Threading.Tasks;

namespace Jane
{
	class Program
	{
		public static Task Main(string[] args)
			=> Startup.RunAsync(args);
	}
}