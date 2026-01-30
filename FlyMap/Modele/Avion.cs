using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Modele
{
    public class Avion : INotifyPropertyChanged  // classe qui fait office de contrat de notification
                                                 // les changements dans le code auront un impact sur l'interface
    {
        private int _id;
        private string _modele;
        private string _immatriculation;
        private int _capacitePassagers;
        private double _vitesseCroissiere;
        private double _distanceMax;


        public Avion(int Id, string Modele, string Immatriculation, int CapacitePassagers,
            double VitesseCroissiere, double DistanceMax)
        {
            this._id = Id;
            this._modele = Modele;
            this._immatriculation = Immatriculation;
            this._capacitePassagers = CapacitePassagers;
            this._vitesseCroissiere = VitesseCroissiere;
            this._distanceMax = DistanceMax;

        }

        public int Id { get => _id; set => _id = value; }
        public string Modele
        {
            get => _modele;
            set {  _modele = value; OnPropertyChanged(); }
        }
        public string Immatriculation { get => _immatriculation; set => _immatriculation = value; }
        public int CapacitePassagers { get => _capacitePassagers; set => _capacitePassagers = value; }
        public double VitesseCroissiere { get => _vitesseCroissiere; set => _vitesseCroissiere = value; }
        public double DistanceMax { get => _distanceMax; set => _distanceMax = value; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
