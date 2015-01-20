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

        public static async Task<IEnumerable<int>> GetSceneNumbers(this KlikAanKlikUitClient tpc)
        {
            return Enumerable.Range(0, await tpc.GetSceneCount());
        }

        public static async Task<string[]> GetSceneNames(this KlikAanKlikUitClient tpc)
        {
            var nameTasks = Enumerable.Range(0, await tpc.GetSceneCount()).Select(tpc.GetRoomName);
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

        public static async Task<Scene[]> GetScenes(this KlikAanKlikUitClient tpc)
        {
            return await Task.WhenAll((await tpc.GetSceneNumbers()).Select(tpc.GetScene));
        }

        public static async Task<Device[]> GetDevices(this KlikAanKlikUitClient tpc, int roomNo)
        {
            return await Task.WhenAll((await tpc.GetDeviceNumbers(roomNo)).Select(devNo => tpc.GetDevice(roomNo, devNo)));
        }

        public static async Task<Device[]> GetDevices(this KlikAanKlikUitClient tpc, Room room)
        {
            return await Task.WhenAll((await tpc.GetDeviceNumbers(room.RoomNo)).Select(devNo => tpc.GetDevice(room, devNo)));
        }

        public static async Task<Device> GetDevice(this KlikAanKlikUitClient tpc, int roomNo, int devNo)
        {
            var nameTask = tpc.GetDeviceName(roomNo, devNo);
            var dimmableTask = tpc.CanDeviceDim(roomNo, devNo);
            return new Device { DeviceNo = devNo, RoomNo = roomNo, Name = await nameTask, Dimmable = await dimmableTask, TpcUri = tpc.Uri.ToString() };
        }

        public static async Task<Device> GetDevice(this KlikAanKlikUitClient tpc, Room room, int devNo)
        {
            var nameTask = tpc.GetDeviceName(room.RoomNo, devNo);
            var dimmableTask = tpc.CanDeviceDim(room.RoomNo, devNo);
            return new Device { DeviceNo = devNo, RoomNo = room.RoomNo, Room = room, Name = await nameTask, Dimmable = await dimmableTask, TpcUri = tpc.Uri.ToString()};
        }

        public static async Task<Room> GetRoom(this KlikAanKlikUitClient tpc, int roomNo)
        {
            var retVal = new Room
            {
                RoomNo = roomNo,
                TpcUri = tpc.Uri.ToString()
            };

            var nameTask = tpc.GetRoomName(roomNo);
            var devicesTask = tpc.GetDevices(retVal);
            retVal.Name = await nameTask;
            retVal.Devices = await devicesTask;

            foreach (var dev in retVal.Devices)
                dev.Room = retVal;

            return retVal;
        }

        public static async Task<Scene> GetScene(this KlikAanKlikUitClient tpc, int sceneNo)
        {
            var nameTask = tpc.GetSceneName(sceneNo);
            return new Scene
            {
                SceneNo = sceneNo,
                Name = await nameTask,
                TpcUri = tpc.Uri.ToString()
            };
        }
    }
}