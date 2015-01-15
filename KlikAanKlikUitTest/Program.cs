using System;
using System.Linq;
using System.Threading.Tasks;
using Glueware.KlikAanKlikUit.Client;

namespace KlikAanKlikUitTest
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
                Console.WriteLine("Usage: KlikAanKlikUitTest <host_optional_port>. I'll crash now");
            MainAsync(args.First()).Wait();
        }

        static async Task MainAsync( string host)
        {
            var kanaal = new KlikAanKlikUitClient(host);

            var shit = await kanaal.GetRooms();
            foreach (var room in shit)
                room.Nice();
/*
            Console.WriteLine("Go!");
            await shit.TurnAllOn(kanaal);
            Console.WriteLine("All On");
*/
            await Task.Delay(5000);
            Console.WriteLine("Go!");
            await shit.TurnAllOff(kanaal);
            Console.WriteLine("All Off");

/*
            await Task.Delay(5000);
            Console.WriteLine("Go!");
            var allDevices = shit.SelectMany(r => r.Devices);
            await allDevices.Where(dev => dev.Name.ToLowerInvariant().Contains("schemer")).TurnOn(kanaal);
            Console.WriteLine("Schemers On");
*/
        }

        public static void Nice(this Room room)
        {
            Console.WriteLine("*" + room.Name);
            foreach (var dev in room.Devices)
                dev.Nice();
        }
        public static void Nice(this Device room)
        {
            Console.WriteLine("> " +room.Name);

        }

    }
}
