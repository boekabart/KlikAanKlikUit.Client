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

namespace KlikAanKlikUitRest.Controllers
{
    class ByteResult : IHttpActionResult
    {
        private readonly byte[] _bytes;
        private readonly string _contentType;

        public ByteResult(byte[] bytes, string contentType)
        {
            _bytes = bytes;
            _contentType = contentType;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(_bytes)
            };

            var contentType = _contentType;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return response;
        }
    }
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
            var selectMany = result
                .SelectMany(r => r.Devices);
            var allDevices = selectMany
                .Select(d => d.ToApiDevice()).ToArray();
            return allDevices;
        }

        [Route("api/rooms/devices")]
        public async Task<IEnumerable<Room>> GetDevicesPerRoom()
        {
            var result = await Client
                .GetRooms();
            var apiRooms = result
                .Select(r => r.ToApiRoom());
            return apiRooms;
        }

        [Route("api/devices/{id}/image")]
        public async Task<IHttpActionResult> GetDeviceImage(int id)
        {
            var room = id/100;
            var device = id%100;
            var bytes = await Client.GetDeviceImage(room, device);
            if (bytes == null || bytes.Length == 0)
                return NotFound();
            return new ByteResult(bytes, "image/bmp");
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
            if (level > 15)
                return await DeviceOn(id);

            var room = id.RoomNo();
            var device = id.DeviceNo();
            await Client.Dim(room, device, level);
            return Ok();
        }
    }
}
