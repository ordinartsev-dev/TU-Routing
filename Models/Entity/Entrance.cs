namespace Backend.Models
{

    public class Entrance
    {
        public int Id { get; set; }
        public int PlaceId { get; set; } // Зовнішній ключ на Place
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Place Place { get; set; } // Навігаційна властивість (можна для зручності)
    }
}