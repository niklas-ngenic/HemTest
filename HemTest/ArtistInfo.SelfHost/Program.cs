using ArtistInfoApi;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArtistInfo.SelfHost
{
    class Program
    {
        static void Main()
        {
            string baseAddress = "http://127.0.0.1:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine(string.Format("Serving Api on address: {0}", baseAddress));
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
