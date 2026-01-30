using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlyMap.Modele;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace FlyMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer _flightTimer;
        private int _currentPointIndex = 0;
        private List<PointLatLng> _pointsAnimation; // les point du trajet sont stocké ici
        public List<Aeroport> Aeroports {  get; set; }
        public Aeroport SelectedAirport { get; set;}
        
        
        private GMapMarker _avionMarker;
        // propriete publique pour le volActuel

        public Vol VolActuel {  get; set; }
        public MainWindow()
        {
           
            InitializeComponent();
            InitializeMap();
            Aeroports = new List<Aeroport>
            {
                new Aeroport {ID = 1, Name = "CDG", Longitude = 2.55000, Latitude = 49.01280 },
                new Aeroport {ID = 2, Name = "New-York", Longitude = -73.780968, Latitude = 40.641766 },
                new Aeroport {ID = 3, Name = "Marseille", Longitude = 5.21500, Latitude = 43.436944}
            };
            SelectedAirport = Aeroports[0];
            

            this.DataContext = this;
            
        }
        /// <summary>
        /// Sert a afficher la carte de la bibliotheque Gmap
        /// </summary>
        private void InitializeMap() 
        {
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = new PointLatLng(48.8566, 2.3522);
            MainMap.MinZoom = 2;
            MainMap.MaxZoom = 17;
            MainMap.Zoom = 5;
            MainMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            MainMap.CanDragMap = true;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        } 

        private void BtnTracer_Click(object sender, RoutedEventArgs e)
        {
            MainMap.Markers.Clear(); // evite d'avoir plusieurs marker surposé
            _currentPointIndex = 0;
            //Avion avion = new Avion(01, "airbus test", "av-073-aj", 50, 300.00, 14500.0);
            this.VolActuel = new Vol("10a530", new Avion(01, "airbus test", "av-073-aj", 50, 300.00, 14500.0), 49.01280, 2.55000, 40.641766, -73.780968);
            _pointsAnimation = GenererPointsGrandCercle(VolActuel.DepartLat, VolActuel.DepartLng, VolActuel.ArriveLat, VolActuel.ArriveLng, 100);

            GMapRoute route = new GMapRoute(_pointsAnimation);
            route.Shape = new System.Windows.Shapes.Path() { Stroke = Brushes.DodgerBlue, StrokeThickness = 3 };
            MainMap.Markers.Add(route);

            //creation de l'avion au point de depart avec index a 0
            _avionMarker = new GMapMarker(_pointsAnimation[0]);
            System.Windows.Controls.Image avionImg = new System.Windows.Controls.Image();

            avionImg.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("C:\\Users\\Doryan\\source\\repos\\FlyMap\\FlyMap\\Image\\avion.png"));
            avionImg.Width = 14;
            avionImg.Height = 14;
            avionImg.ToolTip = $"Vol {VolActuel.NumVol}";
            
            avionImg.RenderTransformOrigin = new Point(0.5, 0.5);

            _avionMarker.Shape = avionImg;
            
            _avionMarker.Offset = new Point(-7, -7);
            MainMap.Markers.Add(_avionMarker);

            //lancement de l'animation 
            StartFlightAnimation();

            this.DataContext = null;
            this.DataContext = this;

            //// 3. Calcul des points de la courbe
            //List<PointLatLng> pointsDeLaCourbe = GenererPointsGrandCercle(
            //    VolActuel.DepartLat, VolActuel.DepartLng,
            //    VolActuel.ArriveLat, VolActuel.ArriveLng,
            //    50);

            //// 4. CRÉATION DE LA ROUTE (Version WPF)
            //// En WPF, GMapRoute hérite de GMapMarker !
            //GMapRoute routeVisuelle = new GMapRoute(pointsDeLaCourbe);

            //// On définit le style de la ligne
            //routeVisuelle.Shape = new System.Windows.Shapes.Path()
            //{
            //    Stroke = System.Windows.Media.Brushes.DodgerBlue,
            //    StrokeThickness = 3,
            //    ToolTip = "Vol Paris - New York"
            //};

            //// 5. AJOUT DIRECT À LA CARTE
            //MainMap.Markers.Add(routeVisuelle);

            

            // Zoom automatique
            MainMap.ZoomAndCenterMarkers(null);
        }

        // Cette méthode calcule les points intermédiaires pour faire une courbe
        private List<PointLatLng> GenererPointsGrandCercle(double lat1, double lon1, double lat2, double lon2, int nbPoints)
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
        private void StartFlightAnimation()
        {
            // si u ntimer existe deja on l'arrete
            if(_flightTimer != null) _flightTimer.Stop();

            _flightTimer = new System.Windows.Threading.DispatcherTimer();
            _flightTimer.Interval = TimeSpan.FromMilliseconds(50); // un point toute les milisecondes
            _flightTimer.Tick += AnimationStep;
            _flightTimer.Start();
        }
        private void AnimationStep(object sender, EventArgs e)
        {
            if(_currentPointIndex < _pointsAnimation.Count -1)
            {
                //on deplace le marqueur au point suivant
                PointLatLng pointActuel = _pointsAnimation[_currentPointIndex];
                PointLatLng pointSuivant = _pointsAnimation[_currentPointIndex +1];

                //Deplacement
                _avionMarker.Position = pointActuel;

                //calcul du cap avec la fonction de Gmap
                double angleCap = GMapProviders.EmptyProvider.Projection.GetBearing(pointActuel, pointSuivant);

                //recuperation de l'image (element graphique)
                System.Windows.UIElement monImgAvion = _avionMarker.Shape;

                monImgAvion.RenderTransform = new RotateTransform(angleCap);

                _currentPointIndex++;
            }
            else
            {
                //le vol est terminé
                _flightTimer.Stop();
                MessageBox.Show("Le vol est terminé");
            }
        }
    }


}