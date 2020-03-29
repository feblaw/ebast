using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace App.EmailSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // To prevent console from closing
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://localhost:1099")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
