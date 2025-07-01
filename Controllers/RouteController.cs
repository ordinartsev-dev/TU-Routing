using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Contracts;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly GraphHopperService _graphHopperService;
        private readonly TransitRouteService _transitRouteService;

        private readonly FindTheNearestStationService _findTheNearestStationService;

        private readonly HybridRouteService _hybridRouteService;
        private readonly FindScooterService _findScooterService;
        private readonly ScooterRouteService _scooterRouteService;

        private readonly WalkingRouteService _walkingRouteService;
        private readonly HybridRouteServiceSeveralPoints _hybridRouteServiceSeveralPoints;

        private readonly FetchAllPointers _fetchAllPointers;


        public RouteController(GraphHopperService graphHopperService,
         TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService,
         HybridRouteService hybridRouteService, FindScooterService findScooterService,
          ScooterRouteService scooterRouteService, WalkingRouteService walkingRouteService, FetchAllPointers fetchAllPointers,
          HybridRouteServiceSeveralPoints hybridRouteServiceSeveralPoints)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
            _hybridRouteService = hybridRouteService;
            _findScooterService = findScooterService;
            _scooterRouteService = scooterRouteService;
            _walkingRouteService = walkingRouteService;
            _hybridRouteServiceSeveralPoints = hybridRouteServiceSeveralPoints;
            _fetchAllPointers = fetchAllPointers;
        }

        [HttpPost("walking")]
        public async Task<IActionResult> GetRoute(
            [FromBody] ListOfPoints request)
        {
            if (request == null || request.Points == null || request.Points.Count < 2)
            {
                return BadRequest("Invalid points provided. At least two points are required.");
            }

            // Проверка корректности координат
            for (int i = 0; i < request.Points.Count; i++)
            {
                var point = request.Points[i];
                
                // Проверка широты (должна быть от -90 до 90)
                if (point.Lat < -90 || point.Lat > 90)
                {
                    return BadRequest($"Invalid latitude value at point {i + 1}. Latitude must be between -90 and 90 degrees.");
                }
                
                // Проверка долготы (должна быть от -180 до 180)
                if (point.Lon < -180 || point.Lon > 180)
                {
                    return BadRequest($"Invalid longitude value at point {i + 1}. Longitude must be between -180 and 180 degrees.");
                }
            }

            try
            {
                var points = request.Points.Select(p => new List<double> { p.Lat, p.Lon }).ToList();
                var response = await _walkingRouteService.WalkingRouteAsync(points);
                
                if (response == null)
                {
                    return NotFound("No walking route found for the provided points.");
                }
                
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Error while calculating route: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("transit")]
        public async Task<IActionResult> GetTransitRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon,
            [FromQuery] string deptime)
        {
            var result = await _transitRouteService.CalculateTransitRouteAsync(fromLat, fromLon, toLat, toLon, deptime);
            return Ok(result);
        }

        [HttpGet("scooter")]
        public async Task<IActionResult> GetScooter(
            [FromQuery] double Lat,
            [FromQuery] double Lon)
        {
            var result = await _findScooterService.FindScooterAsync(Lat, Lon);
            return Ok(result);
        }

        [HttpPost("scooter-route")]
        public async Task<IActionResult> GetScooterRoute(
            [FromBody] ListOfPoints request)
        {
            if (request == null || request.Points == null || request.Points.Count < 2)
            {
                return BadRequest("Invalid points provided. At least two points are required.");
            }

            // Проверка корректности координат
            for (int i = 0; i < request.Points.Count; i++)
            {
                var point = request.Points[i];

                // Проверка широты (должна быть от -90 до 90)
                if (point.Lat < -90 || point.Lat > 90)
                {
                    return BadRequest($"Invalid latitude value at point {i + 1}. Latitude must be between -90 and 90 degrees.");
                }

                // Проверка долготы (должна быть от -180 до 180)
                if (point.Lon < -180 || point.Lon > 180)
                {
                    return BadRequest($"Invalid longitude value at point {i + 1}. Longitude must be between -180 and 180 degrees.");
                }
            }

            try
            {
                var points = request.Points.Select(p => new List<double> { p.Lat, p.Lon }).ToList();
                var result = await _scooterRouteService.ScooterRouteAsync(points);

                if (result == null)
                {
                    return NotFound("No scooter route found for the provided points.");
                }
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Error while calculating route: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("nearest-station")]
        public async Task<IActionResult> GetNearestStation(
            [FromQuery] double lat,
            [FromQuery] double lon)
        {
            var result = await _findTheNearestStationService.FindTheNearestStationServiceAsync(lat, lon);
            return Ok(result);
        }


        [HttpGet("hybrid")]
        public async Task<IActionResult> GetHybridRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon)
        {
            // Вызываем все 3 части маршрута
            var result = await _hybridRouteService.generateHybridRoute(fromLat, fromLon, toLat, toLon);

            return Ok(result);
        }

        [HttpPost("hybrid-several-points")]
        public async Task<IActionResult> GetHybridRouteSeveralPoints(
            [FromBody] ListOfPoints request)
        {
            if (request == null || request.Points == null || request.Points.Count < 2)
            {
                return BadRequest("Invalid points provided. At least two points are required.");
            }

            // Проверка корректности координат
            for (int i = 0; i < request.Points.Count; i++)
            {
                var point = request.Points[i];

                // Проверка широты (должна быть от -90 до 90)
                if (point.Lat < -90 || point.Lat > 90)
                {
                    return BadRequest($"Invalid latitude value at point {i + 1}. Latitude must be between -90 and 90 degrees.");
                }

                // Проверка долготы (должна быть от -180 до 180)
                if (point.Lon < -180 || point.Lon > 180)
                {
                    return BadRequest($"Invalid longitude value at point {i + 1}. Longitude must be between -180 and 180 degrees.");
                }
            }

            try
            {
                var points = request.Points.Select(p => new List<double> { p.Lat, p.Lon }).ToList();
                var result = await _hybridRouteServiceSeveralPoints.HybridRouteSeveralPointsAsync(points);

                if (result == null)
                {
                    return NotFound("No hybrid route found for the provided points.");
                }

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Error while calculating route: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }
        

        [HttpGet("all-pointers")]
        public async Task<IActionResult> GetAllPointers()
        {
            var pointers = await _fetchAllPointers.GetAllPointersAsync();
            return Ok(pointers);
        }
    }
}