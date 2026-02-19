using FlyMap.Services;

namespace FlyMap.Tests
{
    [TestClass]
    public sealed class CalculateurDistanceTests
    {
        [TestMethod]
        public void CalculerDistanceExacte()
        {
            double longitudeParis = 2.55000;
            double latitudeParis = 49.01280;
            double longitudeNY = -73.780968;
            double latitudeNY = 40.641766;

            double distanceAttenduParisNY = 0;
            int margeErreur = 10;

            double distanceCalculee = CalculateurDistance.CalculDistance(longitudeParis, longitudeNY, latitudeParis, latitudeNY);

            Assert.AreEqual(distanceAttenduParisNY, distanceCalculee, margeErreur);
        }
    }
}
