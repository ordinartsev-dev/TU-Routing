// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class HybridRouteService
    {
        private readonly GraphHopperService _graphHopperService;
        private readonly TransitRouteService _transitRouteService;
        private readonly FindTheNearestStationService _findTheNearestStationService;

        public HybridRouteService(GraphHopperService graphHopperService, TransitRouteService transitRouteService, FindTheNearestStationService findTheNearestStationService)
        {
            _graphHopperService = graphHopperService;
            _transitRouteService = transitRouteService;
            _findTheNearestStationService = findTheNearestStationService;
        }

        public async Task<PublicTransportStop> findTheNearestStation(double Lat, double Lon)
        {
            // Implement the logic to find the nearest station
            // This could involve calling other services or APIs
            // For example, you might call the FindTheNearestStationService here
            var nearestStation = await _findTheNearestStationService.FindTheNearestStationServiceAsync(Lat, Lon);
            return nearestStation;
        }
        public async Task<string> generateRouteToStation(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            var part1 = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return part1;
        }

        public async Task<string> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2)
        {
            // Implement the logic to generate a route to the station with transit
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            // Find the nearest station
            //var nearestStation1 = await _findTheNearestStationService.FindTheNearestStationServiceAsync(fromLat, fromLon);
            //var nearestStation2 = await _findTheNearestStationService.FindTheNearestStationServiceAsync(toLat, toLon);
            // Use the nearest stations to calculate the transit route
            // Assuming nearestStation1 and nearestStation2 have properties Lat and Lon
            var part2 = await _transitRouteService.CalculateTransitRouteAsync(stop1.Location.Latitude, stop1.Location.Longitude,
                                                                                 stop2.Location.Latitude, stop2.Location.Longitude);

            return part2;
        }

        public async Task<string> generateRouteToEndPoint(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            var part3 = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return part3;
        }

        public async Task<string> generateHybridRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a hybrid route
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            // Find the nearest station
            var nearestStation1 = await findTheNearestStation(fromLat, fromLon);
            Console.WriteLine(nearestStation1.Name + "Lat" + nearestStation1.Location.Latitude + "Long" + nearestStation1.Location.Longitude);
            var nearestStation2 = await findTheNearestStation(toLat, toLon);
            Console.WriteLine(nearestStation2.Name + "Lat" + nearestStation2.Location.Latitude + "Long" + nearestStation2.Location.Longitude);
            // Use the nearest stations to calculate the hybrid route
            // Assuming nearestStation1 and nearestStation2 have properties Lat and Lon
            // Generate the route to the station
            // Generate the transit route
            // Generate the route to the endpoint
            // Assuming nearestStation1 and nearestStation2 have properties Lat and Lon
            // Use the nearest stations to calculate the hybrid route
            // Assuming nearestStation1 and nearestStation2 have properties Lat and Lon
            // Use the nearest stations to calculate the hybrid route
            // Assuming nearestStation1 and nearestStation2 have properties Lat and Lon

            var part1 = await generateRouteToStation(fromLat, fromLon, nearestStation1.Location.Latitude, nearestStation1.Location.Longitude);
            var part2 = await generateTransitRoute(nearestStation1, nearestStation2);
            Console.WriteLine(part2);

            var part3 = await generateRouteToEndPoint(nearestStation2.Location.Latitude, nearestStation2.Location.Longitude, toLat, toLon);

            return $"{part1} + {part2} + {part3}";
        }

        

        
    }
    
}
