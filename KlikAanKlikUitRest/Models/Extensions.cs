using System.Linq;
using System.Threading.Tasks;
using Glueware.KlikAanKlikUit.Client;

namespace KlikAanKlikUitRest.Models
{
    static class Extensions
    {
        private const int Multiplier = 1000;
        public static int CalculateApiDeviceId(int roomNo, int deviceNo)
        {
            return roomNo * Multiplier + deviceNo;
        }

        public static int RoomNo(this int deviceId)
        {
            return deviceId / Multiplier;
        }

        public static int DeviceNo(this int deviceId)
        {
            return deviceId % Multiplier;
        }

        public static async Task<Room> ToApiRoomWithDevices(this Glueware.KlikAanKlikUit.Client.Room src)
        {
            return new Room
            {
                Name = src.Name,
                RoomNo = src.RoomNo,
                Devices = (await src.GetDevices()).Select(ToApiDevice).ToArray()
            };
        }

        public static Scene ToApiScene(this Glueware.KlikAanKlikUit.Client.Scene src)
        {
            return new Scene
            {
                Name = src.Name,
                SceneNo = src.SceneNo,
            };
        }

        public static Device ToApiDevice(this Glueware.KlikAanKlikUit.Client.Device src)
        {
            return new Device
            {
                Name = src.Name,
                RoomName = src.Room.Name,
                Dimmable = src.Dimmable,
                Id = CalculateApiDeviceId(src.RoomNo, src.DeviceNo)
            };
        }
    }
}