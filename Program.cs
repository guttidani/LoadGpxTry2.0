﻿using System;

namespace LoadGpxTry2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            GPXLoader gpxLoader = new GPXLoader();
            string waypoints = gpxLoader.LoadGPXWaypoints(@"D:\Tanulós\Futóverseny Project\LoadGpxTry2.0\trackek\Éva_Kolcsrn+Srecz_2020-08-15_13-08-53.GPX");
            string tracks = gpxLoader.LoadGPXTracks(@"D:\Tanulós\Futóverseny Project\LoadGpxTry2.0\trackek\Éva_Kolcsrn+Srecz_2020-08-15_13-08-53.GPX").ToString();

            //Console.WriteLine(waypoints);
            Console.WriteLine("-----");
            Console.WriteLine(tracks);
            Console.ReadKey();
        }
    }
}
