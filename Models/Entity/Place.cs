using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Backend.Models
{

    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Geometry Contour { get; set; } // Справжня геометрія

        public List<Entrance> Entrances { get; set; } = new(); // Колекція входів

        public Place()
        {
        }

        public Place(int id, string name, string category, string description, double latitude, double longitude,
            Geometry contour)
        {
            Id = id;
            Name = name;
            Category = category;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Contour = contour;
            Entrances = new List<Entrance>();
        }
    }
}