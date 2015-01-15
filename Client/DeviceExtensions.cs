using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class DeviceExtensions
    {
        public static Task TurnOn(this Device dev, KlikAanKlikUitClient tpc)
        {
            return tpc.TurnOn(dev.RoomNo, dev.DeviceNo);
        }

        public static Task TurnOff(this Device dev, KlikAanKlikUitClient tpc)
        {
            return tpc.TurnOff(dev.RoomNo, dev.DeviceNo);
        }

        public static Task TurnOn(this IEnumerable<Device> devs, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(devs.Select(d => TurnOn((Device) d, tpc)));
        }

        public static Task TurnOff(this IEnumerable<Device> devs, KlikAanKlikUitClient tpc)
        {
            return Task.WhenAll(devs.Select(d => d.TurnOff(tpc)));
        }
    }
}