using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Store_chain
{
    public class Program
    {
        public static char PointChar = double.TryParse("1,2", out _) ? ',' : '.';

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
