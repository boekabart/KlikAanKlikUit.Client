using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class DeviceExtensions
    {
        public static KlikAanKlikUitClient KlikAanKlikUitClient(this Device dev)
        {
            return new KlikAanKlikUitClient(dev.TpcUri);
        }

        public static Task TurnOn(this Device dev)
        {
            return dev.KlikAanKlikUitClient().TurnOn(dev.RoomNo, dev.DeviceNo);
        }

        public static Task TurnOff(this Device dev)
        {
            return dev.KlikAanKlikUitClient().TurnOff(dev.RoomNo, dev.DeviceNo);
        }

        public static Task Dim(this Device dev, int stand)
        {
            return dev.KlikAanKlikUitClient().Dim(dev.RoomNo, dev.DeviceNo, stand);
        }

        public static Task WakeUpDim(this Device dev)
        {
            return dev.KlikAanKlikUitClient().WakeUpDim(dev.RoomNo, dev.DeviceNo);
        }

        public static Task TurnOn(this IEnumerable<Device> devs)
        {
            return Task.WhenAll(devs.Select(TurnOn));
        }

        public static Task TurnOff(this IEnumerable<Device> devs)
        {
            return Task.WhenAll(devs.Select(TurnOff));
        }
    }
}