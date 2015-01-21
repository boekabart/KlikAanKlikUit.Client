using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class RoomExtensions
    {
        public static KlikAanKlikUitClient KlikAanKlikUitClient(this Room room)
        {
            return new KlikAanKlikUitClient(room.TpcUri);
        }

        public static Task TurnAllOn(this Room room)
        {
            return room.CachedDevices.TurnOn();
        }

        public static Task TurnAllOn(this IEnumerable<Room> rooms)
        {
            return Task.WhenAll(rooms.Select(TurnAllOn));
        }

        public static Task TurnAllOff(this Room room)
        {
            return room.CachedDevices.TurnOff();
        }

        public static Task TurnAllOff(this IEnumerable<Room> rooms)
        {
            return Task.WhenAll(rooms.Select(TurnAllOff));
        }

        public static async Task<Device[]> GetDevices(this Room dev)
        {
            return dev.CachedDevices = dev.CachedDevices ?? await dev.KlikAanKlikUitClient().GetDevices(dev);
        }
    }
}
