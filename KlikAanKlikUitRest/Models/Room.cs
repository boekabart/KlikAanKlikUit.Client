using System.Linq;
using System.Threading.Tasks;
using Glueware.KlikAanKlikUit.Client;

namespace KlikAanKlikUitRest.Models
{
    public class Room
    {
        public string Name { get; set; }
        public int RoomNo { get; set; }
        public Device[] Devices { get; set; }
    }

    public class Scene
    {
        public string Name { get; set; }
        public int SceneNo { get; set; }
    }

    public class Device
    {
        public string Name { get; set; }
        public string RoomName { get; set; }
        public int Id { get; set; }
        public bool Dimmable { get; set; }
    }

    static class Extensions
    {
        private const int Multiplier = 1000;
        static public int CalculateApiDeviceId(int roomNo, int deviceNo)
        {
            return roomNo * Multiplier + deviceNo;
        }

        static public int RoomNo(this int deviceId)
        {
            return deviceId / Multiplier;
        }

        static public int DeviceNo(this int deviceId)
        {
            return deviceId % Multiplier;
        }

        static public async Task<Room> ToApiRoomWithDevices(this Glueware.KlikAanKlikUit.Client.Room src)
        {
            return new Room
            {
                Name = src.Name,
                RoomNo = src.RoomNo,
                Devices = (await src.GetDevices()).Select(ToApiDevice).ToArray()
            };
        }

        static public Scene ToApiScene(this Glueware.KlikAanKlikUit.Client.Scene src)
        {
            return new Scene
            {
                Name = src.Name,
                SceneNo = src.SceneNo,
            };
        }

        static public Device ToApiDevice(this Glueware.KlikAanKlikUit.Client.Device src)
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