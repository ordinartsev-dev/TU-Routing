using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Models
{
    public class TransitRouteResponse
    {
    public string RouteId { get; set; }

    public List<double> Start { get; set; }     // начальная точка маршрута
    public List<double> End { get; set; }       // конечная точка маршрута

    public int DistanceMeters { get; set; }
    public int DurationSeconds { get; set; }

    // Пешком до остановки ОТ
    public List<List<double>> WalkToTransportPolyline { get; set; }

    // Сегмент общественного транспорта
    public List<List<double>> TransportPolyline { get; set; }

    // Пешком от остановки ОТ до цели
    public List<List<double>> WalkFromTransportPolyline { get; set; }

    // Доп. информация (по желанию)
    public string TransportType { get; set; }  // например, "bus", "metro"
    public string TransportLine { get; set; }  // например, "M7"
    public string FromStop { get; set; }       // остановка посадки
    public string ToStop { get; set; }         // остановка выхода
    }
}