using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Geolocation;


namespace LoadGpxTry2._0
{
    public class GPXLoader
    {
        /// <summary>
        /// Load the Xml document for parsing
        /// </summary>
        /// <param name="sFile">Fully qualified file name (local)</param>
        /// <returns>XDocument</returns>
        private XDocument GetGpxDoc(string sFile)
        {
            XDocument gpxDoc = XDocument.Load(sFile);
            return gpxDoc;
        }

        /// <summary>
        /// Load the namespace for a standard GPX document
        /// </summary>
        /// <returns></returns>
        private XNamespace GetGpxNameSpace()
        {
            XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
            return gpx;
        }

        /// <summary>
        /// When passed a file, open it and parse all tracks
        /// and track segments from it.
        /// </summary>
        /// <param name="sFile">Fully qualified file name (local)</param>
        /// <returns>string containing line delimited waypoints from the
        /// file (for test)</returns>
        public List<TrackDto> LoadGPXTracks(string sFile)
        {
            XDocument gpxDoc = GetGpxDoc(sFile);
            XNamespace gpx = GetGpxNameSpace();
            RunnerDataProcessing rdp = new RunnerDataProcessing();
            var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                         select new
                         {
                             Name = track.Element(gpx + "name") != null ?
                            track.Element(gpx + "name").Value : null,
                             Segs = (from trackpoint in track.Descendants(gpx + "trkpt")
                                select new
                                {
                                    Latitude = trackpoint.Attribute("lat").Value,
                                    Longitude = trackpoint.Attribute("lon").Value,
                                    Elevation = trackpoint.Element(gpx + "ele") != null ? trackpoint.Element(gpx + "ele").Value : null,
                                    Time = trackpoint.Element(gpx + "time") != null ? trackpoint.Element(gpx + "time").Value : null
                                }   )
                         };

            List<TrackDto> tracksAsList = new List<TrackDto>();

            foreach (var trk in tracks)
            {
                // Populate track data objects.
                foreach (var trkSeg in trk.Segs)
                {
                    TrackDto trackDto = new TrackDto
                    {
                        Latitude = double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                        Longitude = double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                        Coordinate = new Coordinate(double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture), double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture)),
                        Elevation = double.Parse(trkSeg.Elevation, System.Globalization.CultureInfo.InvariantCulture),
                        Time = convertIsoToDateTime(trkSeg.Time)
                    };
                    tracksAsList.Add(trackDto);
                }
            }

            #region MakeNewRunner
            Runner runner = new Runner
            {
                Name = rdp.getNameFromFileName(Path.GetFileName(sFile)),
                DateofRunning = tracksAsList.First().Time.Date,
                RunTime = Convert.ToDateTime((tracksAsList.Last().Time - tracksAsList.First().Time).ToString()),
                ElevationUp = rdp.ElevationUp(tracksAsList),
                ElevationDown = rdp.ElevationDown(tracksAsList),
                Distance = rdp.CountDistance(tracksAsList) / 1000
            };
            #endregion

            return tracksAsList; // Return as List
        }

        /// <summary>
        /// It is a string to DateTime converter (yyyy-MM-dd'T'HH:mm:ss'Z')
        /// </summary>
        /// <param name="iso">Time in string (yyyy-MM-dd'T'HH:mm:ss'Z')</param>
        /// <returns>DateTime (yyyy-MM-dd'T'HH:mm:ss'Z') </returns>
        public DateTime convertIsoToDateTime(string iso)
        {
            return DateTime.ParseExact(iso, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        }
    }
}
