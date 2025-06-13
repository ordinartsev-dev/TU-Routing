

namespace Backend.Contracts
{
    public class ListOfPoints
    {
        public List<Point> Points { get; set; }
    }


    public class Point
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}