using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoCoordinate;
using Geolocation;
//using System.Device.Location;

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
        //public string LoadGPXTracks(string sFile)
        public List<TrackDto> LoadGPXTracks(string sFile)
        {
            XDocument gpxDoc = GetGpxDoc(sFile);
            XNamespace gpx = GetGpxNameSpace();
            var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                         select new
                         {
                             Name = track.Element(gpx + "name") != null ?
                            track.Element(gpx + "name").Value : null,
                             Segs = (
                                from trackpoint in track.Descendants(gpx + "trkpt")
                                select new
                                {
                                    Latitude = trackpoint.Attribute("lat").Value,
                                    Longitude = trackpoint.Attribute("lon").Value,
                                    Elevation = trackpoint.Element(gpx + "ele") != null ? trackpoint.Element(gpx + "ele").Value : null,
                                    Time = trackpoint.Element(gpx + "time") != null ? trackpoint.Element(gpx + "time").Value : null
                                }
                              )
                         };

            StringBuilder sb = new StringBuilder();
            List<TrackDto> tracksAsList = new List<TrackDto>();
            Coordinate temp = new Coordinate();

            foreach (var trk in tracks)
            {
                // Populate track data objects.
                foreach (var trkSeg in trk.Segs)
                {
                    // Populate detailed track segments
                    // in the object model here.

                    //temp = (double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture), double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture));
                    TrackDto trackDto = new TrackDto
                    {
                        Latitude = double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture),
                        Longitude = double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                        // nem  tudom hogy itt jól adom e át az értékeket a kordinatanak
                        Coordinate = new Coordinate(double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture), double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture)),
                        Elevation = double.Parse(trkSeg.Elevation, System.Globalization.CultureInfo.InvariantCulture),
                        Time = convertIsoToDateTime(trkSeg.Time)
                    };
                    //Coordinate A = new Coordinate(double.Parse(trkSeg.Latitude, System.Globalization.CultureInfo.InvariantCulture), double.Parse(trkSeg.Longitude, System.Globalization.CultureInfo.InvariantCulture));

                    tracksAsList.Add(trackDto);
                    sb.Append(
                      string.Format("Track:{0} - Latitude:{1} Longitude:{2} " + "Elevation:{3} Date:{4}\n",
                        trk.Name, trkSeg.Latitude, trkSeg.Longitude, trkSeg.Elevation, trkSeg.Time));
                }
            }
            double _eleDif = FindMaxEle(tracksAsList) - FindMinEle(tracksAsList);
            string _runnerName = getNameFromFileName(Path.GetFileName(sFile));
            TimeSpan _duration = tracksAsList.Last().Time - tracksAsList.First().Time;


            #region MakeNewRunner
            Runner runner = new Runner
            {
                Name = _runnerName,
                DateofRunning = tracksAsList.First().Time.Date,
                RunTime = Convert.ToDateTime(_duration.ToString()),
                Elevation = _eleDif,
                Distance = CountDistance(tracksAsList) / 1000
            };
            #endregion

            Coordinate aPoint = new Coordinate(47.45475187872554, 19.182808074989666);
            Coordinate bPoint = new Coordinate(47.450486370753225, 19.17913507644961);
            
            double _distance2 = GeoCalculator.GetDistance(aPoint, bPoint) / 0.62137; //miles to km

            Console.WriteLine("distance between A and B point in m: {0}", _distance2);
            Console.WriteLine("Max Ele: {0} - Min Ele: {1} - Dif ele: {2}", FindMaxEle(tracksAsList), FindMinEle(tracksAsList), _eleDif);
            Console.WriteLine("Name: {0}, Duration: {1}", _runnerName, _duration);
            Console.WriteLine("Megtett km: {0}", CountDistance(tracksAsList) / 1000);

            //return sb.ToString();
            return tracksAsList; // Return as List
        }

        /// <summary>
        /// It is a string to DateTime converter (yyyy-MM-dd'T'HH:mm:ss'Z')
        /// </summary>
        /// <param name="iso"></param>
        /// <returns>DateTime (yyyy-MM-dd'T'HH:mm:ss'Z') </returns>
        public DateTime convertIsoToDateTime(string iso)
        {
            return DateTime.ParseExact(iso, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// split the file name to get the runner name Example for a file name: Daniel_Smith_2020-08-15_13-08-53.GPX
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns>The name of the runner</returns>
        public string getNameFromFileName(string _fileName)
        {
            string[] _split = _fileName.Split('_');
            string runnerName = null;

            for (int i = 0; i < 2; i++)
            {
                runnerName += _split[i];
                if (i == 0)
                {
                    runnerName += " ";
                }
            }
            return runnerName;
        }
        #region CountElevationDif
        public double FindMaxEle(List<TrackDto> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Empty list");
            }
            double maxEle = double.MinValue;
            foreach (TrackDto type in list)
            {
                if (type.Elevation > maxEle)
                {
                    maxEle = type.Elevation;
                }
            }
            return maxEle;
        }

        public double FindMinEle(List<TrackDto> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Empty list");
            }
            double minEle = 500.0;
            foreach (TrackDto type in list)
            {
                if (type.Elevation < minEle)
                {
                    minEle = type.Elevation;
                }
            }
            return minEle;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string ToString(List<TrackDto> list)
        {

            return base.ToString();
        }

        public double CountDistance(List<TrackDto> list) 
        {
            Console.WriteLine("Távolság számolás");
            double _distance = 0.1;
            for (int i = 1; i < list.Count; i++)
            {
                _distance = _distance + GeoCalculator.GetDistance(list[i - 1].Coordinate, list[i].Coordinate); //mindig 0át ad vissza
                Console.WriteLine("Távolságok: " + GeoCalculator.GetDistance(list[i - 1].Coordinate, list[i].Coordinate));
            }

            return _distance;
        }
    }
}
