using FlyMap.Modele;
using FlyMap.Services;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private  List<AeroportApi> _tousLesAeroports;
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
            // On force GMap à ignorer le cache local buggé et à tout prendre sur le serveur
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
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
            if (Depart.SelectedItem == null || Arrive.SelectedItem == null) 
            {
                MessageBox.Show("Veuillez selectionner un Aeroport de depart et d'arrivé.");
                return;
            }
            if (Depart.SelectedItem == Arrive.SelectedItem)
            {
                MessageBox.Show("L'aeroport de depart et d'arrivé sont identique.");
                return;
            }

            AeroportApi aeroportDepart = (AeroportApi)Depart.SelectedItem;
            AeroportApi aeroportArrive = (AeroportApi)Arrive.SelectedItem;
            
            this.VolActuel = new Vol("1053", new Avion(01, "airbus test", "av-073-aj", 50, 300.00, 14500.0), aeroportDepart.Latitude, aeroportDepart.Longitude,
                aeroportArrive.Latitude, aeroportArrive.Longitude);

            double distance = CalculateurDistance.CalculDistance(VolActuel.DepartLng, VolActuel.ArriveLng, VolActuel.DepartLat, VolActuel.ArriveLat);

            this.DataContext = null;
            this.DataContext = this;

            if(distance > VolActuel.Appareil.DistanceMax)
            {
                MessageBox.Show("vous n'aurez pas assez d'autonomie pour ce vol !","erreur" );
                
            }
            else 
            {
                 TracerVol(VolActuel);
            }

            
            // Zoom automatique
            MainMap.ZoomAndCenterMarkers(null);

            Debug.WriteLine($"[TEST] la distance entre le depart et l'arrivé est de {distance} km");
            Debug.WriteLine($"[TEST l'autonomie de reel de l'avion est de {VolActuel.Appareil.DistanceMax} km");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Chargement.Visibility = Visibility.Visible;

            try
            {
                //var result = await FournisseurAeroport.GetAeroportsAsync(_httpClient);

                //_tousLesAeroports = result.AeroportApis
                //    .Where(aeroport => aeroport.Latitude > 0)
                //    .ToList();
                _tousLesAeroports = new List<AeroportApi>
                {
                    new AeroportApi { Name = "Paris (CDG)", Latitude = 49.0097, Longitude = 2.5479 },
                    new AeroportApi { Name = "New York (JFK)", Latitude = 40.6413, Longitude = -73.7781 },
                    new AeroportApi { Name = "Reykjavik (KEF)", Latitude = 63.9850, Longitude = -22.6056 }, // L'escale parfaite !
                    new AeroportApi { Name = "Dakar (DSS)", Latitude = 14.6711, Longitude = -17.0733 }, // Le détour horrible
                    new AeroportApi { Name = "Madrid (MAD)", Latitude = 40.4983, Longitude = -3.5676 }
                };
                Depart.ItemsSource = _tousLesAeroports;
                Depart.DisplayMemberPath = "Name";
                Arrive.ItemsSource = _tousLesAeroports;
                Arrive.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement : " + ex.Message);
            }
            finally 
            {
                Chargement.Visibility = Visibility.Collapsed;
            }

        }

        
        private void StartFlightAnimation()
        {
            // si un timer existe deja on l'arrete
            if(_flightTimer != null) _flightTimer.Stop();

            _flightTimer = new System.Windows.Threading.DispatcherTimer();
            _flightTimer.Interval = TimeSpan.FromMilliseconds(50); // un point toute les milisecondes
            _flightTimer.Tick += AnimationStep;
            _flightTimer.Start();
        }
        /// <summary>
        /// génére l'animation du deplacement de l'avion sur la courbe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationStep(object sender, EventArgs e)
        {
            if(_currentPointIndex < _pointsAnimation.Count -1)
            {
                //on deplace le marqueur au point suivant
                PointLatLng pointActuel = _pointsAnimation[_currentPointIndex ];
                Console.WriteLine(pointActuel.ToString());
                PointLatLng pointSuivant = _pointsAnimation[_currentPointIndex + 1];

                //Deplacement
                _avionMarker.Position = pointActuel;

                //calcul du cap avec la fonction de Gmap
                double angleCap = GMapProviders.EmptyProvider.Projection.GetBearing(pointActuel, pointSuivant);

                //recuperation de l'image (element graphique)
                System.Windows.UIElement monImgAvion = _avionMarker.Shape;

                monImgAvion.RenderTransform = new RotateTransform(angleCap); // permet de changer l'orientation de l'image PNG pour que le la direction de l'avion soit dynamique 

                _currentPointIndex++;
            }
            else
            {
                //le vol est terminé
                _avionMarker.Position = _pointsAnimation[_pointsAnimation.Count - 1];
                _flightTimer.Stop();
                MessageBox.Show("Le vol est terminé");
            }
        }

        private void TracerVol(Vol leVol)
        {
            MainMap.Markers.Clear(); // evite d'avoir plusieurs marker surposé
            _currentPointIndex = 0;
            _pointsAnimation = GenerationPoints.GenererPointsGrandCercle(VolActuel.DepartLat, VolActuel.DepartLng, VolActuel.ArriveLat, VolActuel.ArriveLng, 100);

            GMapRoute route = new GMapRoute(_pointsAnimation);
            route.Shape = new System.Windows.Shapes.Path() { Stroke = Brushes.DodgerBlue, StrokeThickness = 3 };
            MainMap.Markers.Add(route);

            //creation de l'avion au point de depart avec index a 0
            _avionMarker = new GMapMarker(_pointsAnimation[0]);
            //System.Windows.Controls.Image avionImg = new System.Windows.Controls.Image();
            System.Windows.Shapes.Path avionVectoriel = new System.Windows.Shapes.Path();
            avionVectoriel.Fill = System.Windows.Media.Brushes.Black;
            string svgData = "M21,16V14L13,9V3.5A1.5,1.5 0 0,0 11.5,2A1.5,1.5 0 0,0 10,3.5V9L2,14V16L10,13.5V19L8,20.5V22L11.5,21L15,22V20.5L13,19V13.5L21,16Z";
            avionVectoriel.Data = System.Windows.Media.Geometry.Parse(svgData);


            //avionImg.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("C:\\Users\\Doryan\\source\\repos\\FlyMap\\FlyMap\\Image\\avion.png"));
            //avionImg.Stretch = System.Windows.Media.Stretch.Fill;
            avionVectoriel.Width = 14;
            avionVectoriel.Height = 14;
            avionVectoriel.ToolTip = $"Vol {VolActuel.NumVol}";
            avionVectoriel.Stretch = Stretch.Uniform;
            avionVectoriel.RenderTransformOrigin = new Point(0.5, 0.5);
            
            _avionMarker.Shape = avionVectoriel;
            

            _avionMarker.Offset = new Point(-7, -7);
            MainMap.Markers.Add(_avionMarker);

            //lancement de l'animation 
            StartFlightAnimation();

        }

    }


}
