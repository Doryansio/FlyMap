using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Services
{
    public static class GenerationPoints
    {
        // Cette méthode calcule les points intermédiaires pour faire une courbe
        public static List<PointLatLng> GenererPointsGrandCercle(double lat1, double lon1, double lat2, double lon2, int nbPoints)
        {
            List<PointLatLng> chemin = new List<PointLatLng>();

            // 1. Conversion des degrés en Radians (les maths aiment les radians)
            double d2r = Math.PI / 180.0;
            double r2d = 180.0 / Math.PI;

            double phi1 = lat1 * d2r;
            double lam1 = lon1 * d2r;
            double phi2 = lat2 * d2r;
            double lam2 = lon2 * d2r;

            // 2. Calcul de la distance angulaire entre les deux points (Formule de Haversine)
            double delta = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((phi1 - phi2) / 2), 2) +
                           Math.Cos(phi1) * Math.Cos(phi2) * Math.Pow(Math.Sin((lam1 - lam2) / 2), 2)));

            // 3. Boucle pour créer chaque petit point de la courbe
            for (int i = 0; i <= nbPoints; i++)
            {
                double f = (double)i / nbPoints; // f va de 0 (départ) à 1 (arrivée)

                // Algorithme d'interpolation sphérique (Slerp)
                double A = Math.Sin((1 - f) * delta) / Math.Sin(delta);
                double B = Math.Sin(f * delta) / Math.Sin(delta);

                double x = A * Math.Cos(phi1) * Math.Cos(lam1) + B * Math.Cos(phi2) * Math.Cos(lam2);
                double y = A * Math.Cos(phi1) * Math.Sin(lam1) + B * Math.Cos(phi2) * Math.Sin(lam2);
                double z = A * Math.Sin(phi1) + B * Math.Sin(phi2);

                double newLat = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                double newLon = Math.Atan2(y, x);

                // 4. On rajoute le point converti en degrés dans notre liste
                chemin.Add(new PointLatLng(newLat * r2d, newLon * r2d));
            }

            return chemin;
        }
    }
}
