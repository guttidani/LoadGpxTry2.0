using System;
using System.Collections.Generic;
using System.Text;
using Geolocation;

namespace LoadGpxTry2._0
{
    class RunnerDataProcessing
    {
        /// <summary>
        /// It count the full distance of the running from coordinate to coordinate
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double CountDistance(List<TrackDto> list)
        {
            double distance = 0.0;

            for (int i = 1; i < list.Count; i++)
            {
                distance += GeoCalculator.GetDistance(list[i - 1].Coordinate, list[i].Coordinate, distanceUnit: DistanceUnit.Meters); //mindig 0át ad vissza
            }

            return distance;
        }
        
        /// <summary>
        /// Add the elevation diffrence between A and B point if it is negative in a TrackDto list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double ElevationDown(List<TrackDto> list)
        {
            double eleDown =0;

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].Elevation- list[i-1].Elevation<0)
                {
                    eleDown += list[i].Elevation - list[i - 1].Elevation;
                }
            }

            return eleDown*-1;
        }

        /// <summary>
        /// Add the elevation diffrence between A and B point if it is positive in a TrackDto list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double ElevationUp(List<TrackDto> list)
        {
            double eleUp = 0;

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].Elevation - list[i - 1].Elevation > 0)
                {
                    eleUp += list[i].Elevation - list[i - 1].Elevation;
                }
            }

            return eleUp;
        }

        /// <summary>
        /// split the file name to get the runner name Example for a file name: Cserepes_Virág_2020-08-15_13-08-53.GPX
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
    }
}
