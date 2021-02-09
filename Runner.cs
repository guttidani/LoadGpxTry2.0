using System;
using System.Collections.Generic;
using System.Text;

namespace LoadGpxTry2._0
{
    class Runner
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public double Distance { get; set; }
        public double Elevation { get; set; }
        public DateTime RunTime { get; set; }
        public DateTime DateofRunning { get; set; }

        private static int _id = 1;

        public Runner()
        {
            this.Id = _id;
            _id++;
        }
    }
}
