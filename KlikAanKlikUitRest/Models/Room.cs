namespace KlikAanKlikUitRest.Models
{
    public class Room
    {
        public string Name { get; set; }
        public int RoomNo { get; set; }
        public Device[] Devices { get; set; }
    }
}