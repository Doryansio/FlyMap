using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Modele
{
    public class Vol
    {
        public string NumVol {  get; set; }
        public Avion Appareil { get; set; }
        public List<Passager> Passager { get; set; }
        public double DepartLat { get; set; }
        public double DepartLng { get; set; }
        public double ArriveLat { get; set; }
        public double ArriveLng { get; set; }


        public Vol (string numero, Avion avion, double dLat, double dLng, double aLat, double aLng)
        {
            NumVol = numero;
            Appareil = avion;
            DepartLat = dLat;
            DepartLng = dLng;
            ArriveLat = aLat;
            ArriveLng = aLng;

        }
    }
}
