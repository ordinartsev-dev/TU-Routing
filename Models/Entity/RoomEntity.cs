using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Rooms")]
    public class RoomEntity
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; } // назва кімнати, якщо є

        [Required]
        public string RoomNumber { get; set; } = string.Empty; // номер кімнати

        // Foreign key
        [ForeignKey("Place")]
        public int PlaceId { get; set; }

        public Place Place { get; set; } = null!;
    }
}