

namespace Backend.Contracts
{
    public class ListOfPoints
    {
        public List<CustomPoint> Points { get; set; }
    }


    public class CustomPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}