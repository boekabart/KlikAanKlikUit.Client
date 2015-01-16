namespace Glueware.KlikAanKlikUit.Client
{
    public class Device
    {
        public string Name { get; set; }
        public int RoomNo { get; set; }
        public int DeviceNo { get; set; }
        public byte[] Image { get; set; }
        public Room Room { get; set; }
        public bool Dimmable { get; set; }
        public string TpcUri { get; set; }
    }
}