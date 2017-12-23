using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Shay.Framework;

namespace Shay.Web.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            DBootstrap.Instance.Dispose();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //.UseUrls("http://*:5000")
            .UseStartup<Startup>()
            .Build();
    }
}
