using System;
using System.Collections.Generic;


namespace Backend.Services
{
    public class PolylineDecoder
    {
        public static List<(double Latitude, double Longitude)> Decode(string encoded)
        {
            var poly = new List<(double Latitude, double Longitude)>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                }
                while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                }
                while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                poly.Add((lat / 1E5, lng / 1E5));
            }

            return poly;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Example usage
            var path = new { Path = new[] { new { points = "encodedPolylineString" } } };
            Console.WriteLine("Route points decoded:" + string.Join(", ", PolylineDecoder.Decode(path.Path[0].points)));
        }
    }
}