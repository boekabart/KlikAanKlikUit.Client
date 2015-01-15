using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class ClientExtensions
    {
        public static async Task<IEnumerable<int>> GetRoomNumbers(this KlikAanKlikUitClient tpc)
        {
            return Enumerable.Range(0, await tpc.GetRoomCount());
        }

        public static async Task<string[]> GetRoomNames(this KlikAanKlikUitClient tpc)
        {
            var nameTasks = Enumerable.Range(0, await tpc.GetRoomCount()).Select(tpc.GetRoomName);
            return await Task.WhenAll(nameTasks);
        }

        public static async Task<IEnumerable<int>> GetDeviceNumbers(this KlikAanKlikUitClient tpc, int roomNo)
        {
            return Enumerable.Range(0, await tpc.GetRoomDeviceCount(roomNo));
        }

        public static async Task<Room[]> GetRooms(this KlikAanKlikUitClient tpc)
        {
            return await Task.WhenAll((await tpc.GetRoomNumbers()).Select(tpc.GetRoom));
        }

        public static async Task<Device[]> GetDevices(this KlikAanKlikUitClient tpc, int roomNo)
        {
            return await Task.WhenAll((await tpc.GetDeviceNumbers(roomNo)).Select(devNo => tpc.GetDevice(roomNo, devNo)));
        }

        public static async Task<Device> GetDevice(this KlikAanKlikUitClient tpc, int roomNo, int devNo)
        {
            var nameTask = tpc.GetDeviceName(roomNo, devNo);
            var imageTask = tpc.GetDeviceImage(roomNo, devNo);
            return new Device { DeviceNo = devNo, RoomNo = roomNo, Image = await imageTask, Name = await nameTask };
        }

        public static async Task<Room> GetRoom(this KlikAanKlikUitClient tpc, int roomNo)
        {
            var nameTask = tpc.GetRoomName(roomNo);
            var devicesTask = tpc.GetDevices(roomNo);
            return new Room
            {
                Devices = await devicesTask,
                RoomNo = roomNo,
                Name = await nameTask
            };
        }
    }
}