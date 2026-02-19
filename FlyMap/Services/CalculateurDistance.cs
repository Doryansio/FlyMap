using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Services
{
    public class CalculateurDistance
    {
        const int _rayonTerreKm = 6371;
        private const double _degToR = Math.PI / 180;


        public static double CalculDistance (double long1, double long2, double lat1, double lat2)
        {
            //conversion des latitudes en radians
            double latRad1 = lat1 * _degToR;
            double latRad2 = lat2 * _degToR;

            // calcul des deltas en radian
            double dLat = (lat1 - lat2) * _degToR;
            double dLong = (long1 - long2) * _degToR;

            //formule de haversine
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + (Math.Cos(latRad1) * Math.Cos(latRad2) *
                Math.Sin(dLong / 2) * Math.Sin(dLong / 2));

            // utilisation de la variation de la fonction arc tangente qui prend comme parametre √a et √1-a 
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = _rayonTerreKm * c;

            return distance;

            
        }
    }
}
