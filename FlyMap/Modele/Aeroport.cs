using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Modele
{
    public class Aeroport
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        
             

        public Aeroport(int Id,string name, double longitude, double latitude) 
        {
            ID = Id;
            Name = name;
            Longitude = longitude;
            Latitude = latitude;

        }
        public Aeroport() { }

    }
}
