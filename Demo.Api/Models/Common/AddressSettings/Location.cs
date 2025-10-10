using NetTopologySuite.Geometries;
namespace Demo.Api.Models.Common.AddressSettings;

// for user interface request and response
public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public virtual Point ToPoint()
    {
        return new Point(Longitude, Latitude) { SRID = 4326 };
    } 

    public static Location? FromPoint(Point? point)
    {
        if (point is null || point.IsEmpty)
        {
            return null;
        }
        return new Location { Latitude = point.Y, Longitude = point.X };
    }

    public static implicit operator Point?(Location? location)
    {
        if (location is null)
        {
            return null;
        }
        return location.ToPoint();
    }

    public static implicit operator Location?(Point? point)
    {
        return FromPoint(point);
    }
}
