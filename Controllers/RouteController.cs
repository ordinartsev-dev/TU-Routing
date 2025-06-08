using Microsoft.AspNetCore.Mvc;
using Backend.Services;

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
        private readonly FetchAllPointers _fetchAllPointers;


        public RouteController(GraphHopperService graphHopperService,
            TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService,
            HybridRouteService hybridRouteService, FindScooterService findScooterService,
            ScooterRouteService scooterRouteService,
            FetchAllPointers fetchAllPointers)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
            _hybridRouteService = hybridRouteService;
            _findScooterService = findScooterService;
            _scooterRouteService = scooterRouteService;
            _fetchAllPointers = fetchAllPointers;
        }

        [HttpGet("walking")]
        public async Task<IActionResult> GetRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon)
        {
            var (obj, response) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);
            return Ok(response);
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

        [HttpGet("scooter-route")]
        public async Task<IActionResult> GetScooterRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon)
        {
            var result = await _scooterRouteService.ScooterRouteAsync(fromLat, fromLon, toLat, toLon);
            return Ok(result);
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
            
            var result = await _hybridRouteService.generateHybridRoute(fromLat, fromLon, toLat, toLon);

            return Ok(result);
        }

        [HttpGet("all-pointers")]
        public async Task<IActionResult> GetAllPointers()
        {
            var pointers = await _fetchAllPointers.GetAllPointersAsync();
            return Ok(pointers);
        }
    }
}