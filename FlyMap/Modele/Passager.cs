using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyMap.Modele
{
    public class Passager
    {
		private int _reservationNum;
		private string _nom;
		private string _prenom;
		public CodeRegime _regime { get; set; }


		public Passager(int Reservation, string nom, string prenom, CodeRegime Regime)
		{
			_reservationNum = Reservation;
			_nom = nom;
			_prenom = prenom;
			_regime = Regime;
		}

		public string Nom
		{
			get { return _nom; }
			set { _nom = value; }
		}


		public string Prenom
		{
			get { return _prenom; }
			set {  _prenom = value; }
		}


		public int ReservationNum
		{
			get { return ReservationNum; }
			set { ReservationNum = value; }
		}

	}
}
