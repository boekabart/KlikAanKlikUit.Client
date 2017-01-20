using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Glueware.KlikAanKlikUit.Client;
using KlikAanKlikUitRest.Models;
using KlikAanKlikUitRest.Properties;
using Device = KlikAanKlikUitRest.Models.Device;
using Room = KlikAanKlikUitRest.Models.Room;
using Scene = KlikAanKlikUitRest.Models.Scene;

namespace KlikAanKlikUitRest.Controllers
{
    public class KaKuController : ApiController
    {
        private static KlikAanKlikUitClient Client
        {
            get { return new KlikAanKlikUitClient(Settings.Default.Tpc300Host); }
        }

        [Route("api/devices")]
        public async Task<IEnumerable<Device>> GetAllDevices()
        {
            var result = await Client
                .GetRooms();
            var devTasks = result
                .Select( r => r.GetDevices()).ToArray();
            await Task.WhenAll(devTasks);
            var allDevices = devTasks
                .SelectMany( devsTask => devsTask.Result)
                .Select(d => d.ToApiDevice()).ToArray();
            return allDevices;
        }

        [Route("api/scenes")]
        public async Task<IEnumerable<Scene>> GetAllScenes()
        {
            var result = await Client
                .GetScenes();
            var allScenes = result
                .Select(d => d.ToApiScene()).ToArray();
            return allScenes;
        }

        [Route("api/rooms/devices")]
        public async Task<IEnumerable<Room>> GetRoomsWithDevices()
        {
            var result = await Client
                .GetRooms();
            var apiRooms = result
                .Select(r => r.ToApiRoomWithDevices()).ToArray();
            await Task.WhenAll(apiRooms);
            return apiRooms.Select(t => t.Result);
        }

        [Route("api/rooms/{roomNo}/devices")]
        public async Task<IEnumerable<Device>> GetRoomDevices( int roomNo )
        {
            var result = await Client
                .GetDevices(roomNo);
            var apiDevs = result
                .Select(r => r.ToApiDevice());
            return apiDevs;
        }

        [Route("api/rooms")]
        public async Task<IEnumerable<Room>> GetRooms()
        {
            var result = await Client
                .GetRoomNames();
            var apiRooms = result
                .Select((name, no) => new Room {Name = name, RoomNo = no});
            return apiRooms;
        }

        [Route("api/devices/{id}/image")]
        [Route("api/devices/{id}/image{ignoreMe}.bmp")]
        [Route("api/devices/{id}/image{ignoreMe}.jpg")]
        public async Task<HttpResponseMessage> GetDeviceImage(int id, string ignoreMe)
        {
            var room = id.RoomNo();
            var device = id.DeviceNo();
            var bytes = await Client.GetDeviceImage(room, device);
            if (bytes == null || bytes.Length == 0)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            var httpResponseMessage = new HttpResponseMessage { Content = new ByteArrayContent(bytes)};
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/bmp");
            return httpResponseMessage;
        }

        [Route("api/scenes/{sceneNo}/activate")]
        [HttpPost]
        public async Task<IHttpActionResult> SceneActivate(int sceneNo)
        {
            await Client.ActivateScene(sceneNo);
            return Ok();
        }

        [Route("api/devices/{id}/on")]
        [HttpPost]
        public async Task<IHttpActionResult> DeviceOn(int id)
        {
            var room = id.RoomNo();
            var device = id.DeviceNo();
            await Client.TurnOn(room, device);
            return Ok();
        }

        [Route("api/devices/{id}/wakeupdim")]
        [HttpPost]
        public async Task<IHttpActionResult> DeviceWakeUpDim(int id)
        {
            var room = id.RoomNo();
            var device = id.DeviceNo();
            await Client.WakeUpDim(room, device);
            return Ok();
        }

        [Route("api/devices/{id}/off")]
        [HttpPost]
        public async Task<IHttpActionResult> DeviceOff(int id)
        {
            var room = id.RoomNo();
            var device = id.DeviceNo();
            await Client.TurnOff(room, device);
            return Ok();
        }

        [Route("api/devices/{id}/dim/{level}")]
        [HttpPost]
        public async Task<IHttpActionResult> DeviceDim(int id, int level)
        {
            if (level <= 0)
                return await DeviceOff(id);

            var room = id.RoomNo();
            var device = id.DeviceNo();
            await Client.Dim(room, device, level);
            return Ok();
        }

        [Route("api/rooms/{roomNo}/alloff")]
        public async Task<IHttpActionResult> AllOff(int roomNo)
        {
            var devices = await Client.GetDevices(roomNo);
            var actions = devices.Select(d => d.TurnOff());
            await Task.WhenAll(actions);
            return Ok();
        }

        [Route("api/rooms/{roomNo}/allon")]
        public async Task<IHttpActionResult> AllOn(int roomNo)
        {
            var devices = await Client.GetDevices(roomNo);
            var actions = devices.Select(d => d.TurnOn());
            await Task.WhenAll(actions);
            return Ok();
        }

        [Route("api/alloff")]
        [HttpPost]
        public async Task<IHttpActionResult> AllOff()
        {
            var devices = await GetAllDevices();
            var l = (from dev in devices select dev.Id into id let room = id.RoomNo() let device = id.DeviceNo() select Client.TurnOff(room, device)).ToList();
            await Task.WhenAll(l);
            return Ok();
        }
    }
}
