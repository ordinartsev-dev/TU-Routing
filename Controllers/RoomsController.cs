using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomEntity>>> GetRooms()
        {
            return await _context.Rooms
                .Include(r => r.Place)
                .ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomEntity>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Place)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            return room;
        }

        // GET: api/Rooms/by-place/3
        [HttpGet("by-place/{placeId}")]
        public async Task<ActionResult<IEnumerable<RoomEntity>>> GetRoomsByPlace(int placeId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.PlaceId == placeId)
                .ToListAsync();

            return rooms;
        }
    }
}