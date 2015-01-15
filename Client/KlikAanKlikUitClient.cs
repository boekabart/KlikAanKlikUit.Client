using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Glueware.KlikAanKlikUit.Client
{
    public class KlikAanKlikUitClient
    {
        private readonly string _requestUri;

        public KlikAanKlikUitClient(string uri)
        {
            _requestUri = uri;
        }

        public Task<int> GetRoomCount()
        {
            return CallForInt("getroomcount");
        }

        public Task<string> GetRoomName(int roomNo)
        {
            return CallForString("getroomname", "aRoom", roomNo);
        }

        public Task<int> GetRoomDeviceCount(int roomNo)
        {
            return CallForInt("getappcount", "aRoom", roomNo);
        }

        public Task<string> GetDeviceName(int roomNo, int devNo)
        {
            return CallForString("getappname", "aRoom", roomNo, "aApp", devNo);
        }

        public async Task<byte[]> GetDeviceImage(int roomNo, int devNo)
        {
            return
                Convert.FromBase64String(
                    await CallForString("getAppImage", "aRoom", roomNo, "aApp", devNo));
        }

        private static readonly XNamespace NsEnvelope = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace NsXsi = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly XNamespace NsKiKuIf = "urn:klakluIntf-Iklaklu";
        private static readonly XNamespace NsKiKu = "urn:klakluIntf";

        private async Task<int> CallForInt(string func, params object[] parms)
        {
            return int.Parse(await CallForString(func, parms));
        }

        private async Task<string> CallForString(string func, params object[] parms)
        {
            var callElem = new XElement(NsKiKuIf + func, ArgElements(parms));
            var body = new XElement(NsEnvelope + "Body", callElem);
            var envelope = new XElement(NsEnvelope + "Envelope", body);
            var doc = new XDocument(envelope);
            var bytes = doc.GetBytes(Encoding.UTF8);

            using (var wc = new HttpClient())
            {
                using (var content = new ByteArrayContent(bytes))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                    content.Headers.Add("SOAPAction", "urn:klakluIntf-Iklaklu#" + func);

                    var response = await wc.PostAsync(_requestUri, content);
                    response.EnsureSuccessStatusCode();
                    var responseDoc = XDocument.Load(await response.Content.ReadAsStreamAsync());
                    var responseElem = responseDoc.Descendants(NsKiKu + "TWebserverResponse").First();
                    var error = responseElem.Elements("error").Select(e => e.Value).FirstOrDefault();
                    if (!string.IsNullOrEmpty(error) &&
                        !error.Equals("noerror", StringComparison.OrdinalIgnoreCase))
                        throw new Exception(error);

                    var responseValue = responseElem.Elements("response").Select(e => e.Value).FirstOrDefault();
                    return responseValue;
                }
            }
        }

        private IEnumerable<XElement> ArgElements(IEnumerable<object> parms)
        {
            var tuples = parms.AsTuples<string, int>();
            return
                tuples.Select(
                    t =>
                        new XElement(NsKiKuIf + t.Item1, new XAttribute(NsXsi + "type", "int"),
                            new XText(t.Item2.ToString(CultureInfo.InvariantCulture))));
        }

        public Task TurnOn(int roomNo, int deviceNo)
        {
            return CallForString("apparaatAan", "aRoom", roomNo, "aApp", deviceNo);
        }

        public Task TurnOff(int roomNo, int deviceNo)
        {
            return CallForString("apparaatUit", "aRoom", roomNo, "aApp", deviceNo);
        }
    }
}