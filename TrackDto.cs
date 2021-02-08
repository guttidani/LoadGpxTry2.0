using System;
using Geolocation;
using GeoCoordinate;

namespace LoadGpxTry2._0
{
    public class TrackDto
    {
        public int Id { get; set; }
        public Coordinate Coordinate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; }
        public double Distance { get; set; }
    }
}
