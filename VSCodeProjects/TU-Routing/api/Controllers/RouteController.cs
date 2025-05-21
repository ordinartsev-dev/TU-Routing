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

        public RouteController(GraphHopperService graphHopperService,
         TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService,
         HybridRouteService hybridRouteService)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
            _hybridRouteService = hybridRouteService;
        }

        [HttpGet("walking")]
        public async Task<IActionResult> GetRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon)
        {
            var result = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);
            return Ok(result);
        }

        [HttpGet("transit")]
        public async Task<IActionResult> GetTransitRoute(
            [FromQuery] double fromLat,
            [FromQuery] double fromLon,
            [FromQuery] double toLat,
            [FromQuery] double toLon)
        {
            var result = await _transitRouteService.CalculateTransitRouteAsync(fromLat, fromLon, toLat, toLon);
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
            // Вызываем все 3 части маршрута
            var result = await _hybridRouteService.generateHybridRoute(fromLat, fromLon, toLat, toLon);
            
            return Ok(result);
        }   

    }
}