namespace Glueware.KlikAanKlikUit.Client
{
    public class Room
    {
        public string Name { get; set; }
        public int RoomNo { get; set; }
        internal Device[] CachedDevices { get; set; }
        public byte[] Image { get; set; }
        public string TpcUri { get; set; }
    }
}