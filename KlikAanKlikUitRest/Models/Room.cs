using System.Linq;

namespace KlikAanKlikUitRest.Models
{
    public class Room
    {
        public string Name { get; set; }
        public int RoomNo { get; set; }
        public Device[] Devices { get; set; }
    }

    public class Device
    {
        public string Name { get; set; }
        public string Room { get; set; }
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

        static public Room ToApiRoom(this Glueware.KlikAanKlikUit.Client.Room src)
        {
            return new Room
            {
                Name = src.Name,
                Devices = src.Devices.Select(ToApiDevice).ToArray()
            };
        }

        static public Device ToApiDevice(this Glueware.KlikAanKlikUit.Client.Device src)
        {
            return new Device
            {
                Name = src.Name,
                Room = src.Room.Name,
                Dimmable = src.Dimmable,
                Id = CalculateApiDeviceId(src.RoomNo, src.DeviceNo)
            };
        }
    }
}