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

            for (int i = 1; i < list.Count - 1; i++)
            {
                distance += GeoCalculator.GetDistance(list[i - 1].Coordinate, list[i].Coordinate, distanceUnit: DistanceUnit.Meters); //mindig 0át ad vissza
            }

            return distance;
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
