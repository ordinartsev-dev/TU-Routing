using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend.Services {
    public class FetchAllPointers
    {
        private readonly AppDbContext _dbContext;

        public FetchAllPointers(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // DTO для повного об'єкта Place
        public class PlaceDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = default!;
            public string Category { get; set; } = default!;
            public string Description { get; set; } = default!;
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? ContourWKT { get; set; } // Можна додати ще контур як WKT (текстове представлення геометрії)
            public List<string?> Rooms { get; set; } = new List<string>();
        }

        public async Task<List<PlaceDto>> GetAllPointersAsync()
        {
            return await _dbContext.Places
                .Select(p => new PlaceDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Description = p.Description,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    ContourWKT = p.Contour != null ? p.Contour.AsText() : null,
                    Rooms = _dbContext.Rooms
                        .Where(r => r.PlaceId == p.Id)
                        .Select(r => r.RoomNumber)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}