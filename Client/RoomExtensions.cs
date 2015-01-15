using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class RoomExtensions
    {
        public static Task TurnAllOn(this Room room, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(room.Devices.TurnOn(tpc));
        }

        public static Task TurnAllOn(this IEnumerable<Room> rooms, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(rooms.Select(r => TurnAllOn((Room) r, tpc)));
        }

        public static Task TurnAllOff(this Room room, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(room.Devices.TurnOff(tpc));
        }

        public static Task TurnAllOff(this IEnumerable<Room> rooms, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(rooms.Select(r => r.TurnAllOff(tpc)));
        }
    }
}