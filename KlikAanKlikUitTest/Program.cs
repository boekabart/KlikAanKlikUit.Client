using System;
using System.IO;
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
            var client = new KlikAanKlikUitClient(host);

            var scenesT = client.GetScenes();
            var shit = await client.GetRooms();
            var scenes = await scenesT;
            await Task.WhenAll(shit.Select(Nice));
            foreach (var sc in scenes)
                sc.Nice();
            /*
                        Console.WriteLine("Go!");
                        await shit.TurnAllOn();
                        Console.WriteLine("All On");
            */
            await Task.Delay(5000);
            Console.WriteLine("Go!");
            await shit.TurnAllOff();
            Console.WriteLine("All Off");

/*
            await Task.Delay(5000);
            Console.WriteLine("Go!");
            var allDevices = shit.SelectMany(r => r.Devices);
            await allDevices.Where(dev => dev.Name.ToLowerInvariant().Contains("schemer")).TurnOn();
            Console.WriteLine("Schemers On");
*/
        }

        public static async Task Nice(this Room room)
        {
            Console.WriteLine("*" + room.Name);
            await Task.WhenAll((await room.GetDevices()).Select(Nice));
        }

        public static void Nice(this Scene sc)
        {
            Console.WriteLine("= " + sc.Name);
        }

        public static async Task Nice(this Device device)
        {
            Console.WriteLine("> " +device.Name);
            try
            {
                File.WriteAllBytes(string.Format(@"c:\d\temp\{0}_{1}.bmp", device.Room.Name, device.Name), await device.GetImage());
            }
            catch
            {
            }
        }
    }
}
