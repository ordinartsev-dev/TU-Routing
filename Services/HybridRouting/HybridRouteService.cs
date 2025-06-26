// Services/TransitRouteService.cs
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public async Task<PublicTransportStop[]> findTheNearestStation(double Lat, double Lon)
        {
            // Implement the logic to find the nearest station
            // This could involve calling other services or APIs
            // For example, you might call the FindTheNearestStationService here
            var nearestStation = await _findTheNearestStationService.FindTheNearestStationServiceAsync(Lat, Lon);
            return nearestStation;
        }
        //public async Task<WalkingRoute> generateRouteToStation(double fromLat, double fromLon, double toLat, double toLon)
        public async Task<(WalkingRoute obj, string jsonResponse)> generateWalkingRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a route to the station
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            Console.WriteLine($"Requesting route from {fromLat},{fromLon} to {toLat},{toLon}");
            var (obj, jsonResponse) = await _graphHopperService.GetRouteAsync(fromLat, fromLon, toLat, toLon);

            return (obj, jsonResponse);
        }

        public async Task<TransitRoute> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2, string depatureTime)
        //public async Task<string> generateTransitRoute(PublicTransportStop stop1, PublicTransportStop stop2)
        {
            
            var part2 = await _transitRouteService.CalculateTransitRouteAsync(stop1.Latitude, stop1.Longitude,
                                                                                 stop2.Latitude, stop2.Longitude, depatureTime);

            return part2;
        }
        

        public async Task<List<List<double>>> BuildFullCarRouteAsync(List<(double Lat, double Lon)> coordinates)
        {
            var fullRoutePoints = new List<List<double>>();

            if (coordinates == null || coordinates.Count < 2)
                return fullRoutePoints;

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                var from = coordinates[i];
                var to = coordinates[i + 1];

                var (carRoute, _) = await _graphHopperService.GetCarRouteAsync(from.Lat, from.Lon, to.Lat, to.Lon);

                if (carRoute != null && carRoute.Path != null && carRoute.Path.Length > 0 && !string.IsNullOrEmpty(carRoute.Path[0].points))
                {
                    var decodedPoints = PolylineDecoder.Decode(carRoute.Path[0].points);
                    foreach (var point in decodedPoints)
                    {
                        fullRoutePoints.Add(new List<double> { point.Latitude, point.Longitude });
                    }
                }
            }
            return fullRoutePoints;
        }


        public async Task<string> generateHybridRoute(double fromLat, double fromLon, double toLat, double toLon)
        {
            // Implement the logic to generate a hybrid route
            // This could involve calling other services or APIs
            // For example, you might call the GraphHopperService and TransitRouteService here
            // Find the nearest station
            var nearestStations1 = await findTheNearestStation(fromLat, fromLon);
            if (nearestStations1 == null || nearestStations1.Length == 0)
            {
                return "No nearest station found from the starting point.";
            }
            foreach (var station in nearestStations1)
            {
                Console.WriteLine(station.Name + "Lat" + station.Latitude + "Long" + station.Longitude);
            }
            //Console.WriteLine(nearestStations1.Name + "Lat" + nearestStations1.Location.Latitude + "Long" + nearestStations1.Location.Longitude);
            var nearestStations2 = await findTheNearestStation(toLat, toLon);
            if (nearestStations2 == null || nearestStations2.Length == 0)
            {
                return "No nearest station found from the endpoint.";
            }
            foreach (var station in nearestStations2)
            {
                Console.WriteLine(station.Name + "Lat" + station.Latitude + "Long" + station.Longitude);
            }
            //Console.WriteLine(nearestStations2.Name + "Lat" + nearestStations2.Location.Latitude + "Long" + nearestStations2.Location.Longitude);


            DateTime currentTime = DateTime.Now;

            List<TransitRouteResponse> transitRouteResponses = new List<TransitRouteResponse>();


            //TransitRouteResponse[] transitRouteResponses = new TransitRouteResponse[nearestStations1.Length * nearestStations2.Length];

            foreach (var station1 in nearestStations1)
            {
                foreach (var station2 in nearestStations2)
                {
                    // Add the combination to the array
                    var (walkingRoute1, jsonResponse) = await generateWalkingRoute(fromLat, fromLon, station1.Latitude, station1.Longitude);
                    if (walkingRoute1 == null || walkingRoute1.Path == null || walkingRoute1.Path.Length == 0 || string.IsNullOrEmpty(walkingRoute1.Path[0].points))
                    {
                        return "Failed to generate walking route to the first station.";
                    }

                    int walkingTime1 = walkingRoute1.Path[0].time;
                    DateTime departureTime = currentTime.AddMilliseconds(walkingTime1);

                    var trRoute = await generateTransitRoute(station1, station2, depatureTime: departureTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    if (trRoute == null || trRoute.routes == null || trRoute.routes.Length == 0)
                    {
                        Console.WriteLine($"No transit route found between {station1.Name} and {station2.Name}.");
                        continue; // Skip this combination if no transit route
                    }

                    var lastRoute = trRoute.routes[trRoute.routes.Length - 1];
                    var lastLeg = lastRoute.legs[lastRoute.legs.Length - 2];
                    var lastStopover = lastLeg.stopovers[lastLeg.stopovers.Length - 1];

                    // Check if the last stopover is the same as the second station
                    WalkingRoute walkingRoute2 = null;
                    string jsonResponse2 = null;
                    if (lastStopover.latitude == station2.Latitude && lastStopover.longitude == station2.Longitude)
                    {
                        Console.WriteLine($"The endstation from bvg response is the same as found!");
                        var result = await generateWalkingRoute(station2.Latitude, station2.Longitude, toLat, toLon);
                        walkingRoute2 = result.obj;
                        jsonResponse2 = result.jsonResponse;
                        if (walkingRoute2 == null || walkingRoute2.Path == null || walkingRoute2.Path.Length == 0 || string.IsNullOrEmpty(walkingRoute2.Path[0].points))
                        {
                            return "Failed to generate walking route from the second station to the endpoint.";
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The endstation from bvg response is not the same as found! Using the last stopover from bvg response.");
                        // Use the last stopover as the endpoint for the walking route
                        var result = await generateWalkingRoute(lastStopover.latitude, lastStopover.longitude, toLat, toLon);
                        walkingRoute2 = result.obj;
                        jsonResponse2 = result.jsonResponse;
                        if (walkingRoute2 == null || walkingRoute2.Path == null || walkingRoute2.Path.Length == 0 || string.IsNullOrEmpty(walkingRoute2.Path[0].points))
                        {
                            return "Failed to generate walking route from the second station to the endpoint.";
                        }
                    }

                    /*
                    var (walkingRoute2, jsonResponse2) = await generateWalkingRoute(station2.Latitude, station2.Longitude, toLat, toLon);
                    if (walkingRoute2 == null || walkingRoute2.Path == null || walkingRoute2.Path.Length == 0 || string.IsNullOrEmpty(walkingRoute2.Path[0].points))
                    {
                        return "Failed to generate walking route from the second station to the endpoint.";
                    }
                    */

                    var decodedPoints1 = PolylineDecoder.Decode(walkingRoute1.Path[0].points);
                    var decodedPoints2 = PolylineDecoder.Decode(walkingRoute2.Path[0].points);


                    var walkToDistance = walkingRoute1.Path[0].distance;
                    var walkFromDistance = walkingRoute2.Path[0].distance;


                    var trDuration = trRoute.routes.Sum(route => route.duration_minutes) * 60; // Convert minutes to seconds
                    var trDistance = trRoute.routes.Sum(route => route.walking_distance);

                    Console.WriteLine("Transit duration" + trDuration + " seconds");


                    var allCoordinatesOfGeometry = new List<List<double>>();

                    foreach (var route in trRoute.routes)
                    {
                        foreach (var leg in route.legs)
                        {
                            if (leg.polyline != null && leg.polyline.features != null)
                            {
                                foreach (var feature in leg.polyline.features)
                                {
                                    allCoordinatesOfGeometry.Add(new List<double>
                                    {
                                        feature.geometry.Coordinates[1], // latitude
                                        feature.geometry.Coordinates[0]  // longitude
                                    });
                                }
                            }
                        }
                    }

                    var segments = new List<HybridRouteSegment>
                    {
                        new HybridRouteSegment
                        {
                            Type = "walk",
                            Polyline = decodedPoints1.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList(),
                            DurationSeconds = walkingRoute1.Path[0].time / 1000,
                            DistanceMeters = (int)walkToDistance
                        },
                        new HybridRouteSegment
                        {
                            Type = "transit",
                            Polyline = trRoute.routes.SelectMany(route => route.legs.SelectMany(leg => leg.stopovers.Select(stopover => new List<double> { stopover.latitude, stopover.longitude }))).ToList(),
                            precisePolyline = allCoordinatesOfGeometry,
                            DurationSeconds = trDuration,
                            DistanceMeters = trDistance,
                            TransportType = trRoute.routes.SelectMany(route => route.legs).FirstOrDefault(leg => leg.type != "walking")?.type ?? "Unknown",
                            TransportLine = trRoute.routes.SelectMany(route => route.legs).Where(leg => leg.type != "walking" && !string.IsNullOrEmpty(leg.line)).Select(leg => leg.line).FirstOrDefault() ?? "Unknown",
                            FromStop = nearestStations1[0].Name,
                            ToStop = nearestStations2[0].Name
                        },
                        new HybridRouteSegment
                        {
                            Type = "walk",
                            Polyline = decodedPoints2.Select(point => new List<double> { point.Latitude, point.Longitude }).ToList(),
                            DurationSeconds = walkingRoute2.Path[0].time / 1000,
                            DistanceMeters = (int)walkFromDistance
                        }
                    };

                    int overalltime = (walkingTime1 + walkingRoute2.Path[0].time) / 1000 + trDuration; // in seconds

                    var temp = new TransitRouteResponse
                    {
                        Type = "Hybrid",
                        Start = new List<double> { decodedPoints1[0].Latitude, decodedPoints1[0].Longitude },
                        End = new List<double> { decodedPoints2[decodedPoints2.Count - 1].Latitude, decodedPoints2[decodedPoints2.Count - 1].Longitude },
                        Segments = segments,
                        DurationSeconds = overalltime,
                        DistanceMeters = segments.Sum(s => s.DistanceMeters)
                    };

                    transitRouteResponses.Add(temp);
                }
            }


            // Sort the list of TransitRouteResponses by DurationSeconds
            var sortedResponses = transitRouteResponses.OrderBy(r => r.DurationSeconds).ToList();

            if (sortedResponses.Count == 0)
            {
                return "No valid hybrid route found.";
            }

            var finalResponse = JsonSerializer.Serialize(sortedResponses[0]); // Serialize the first (shortest) response

            return finalResponse;
        }
    }
    
}
