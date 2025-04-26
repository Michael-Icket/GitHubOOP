using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
	public abstract class Werknemers
	{
        public int WerknemerNummer { get; set; }
        public string Naam { get; set; }

        public string Voornaam { get; set; }

        public string Telefoonnr { get; set; }

        protected Werknemers(int werknemernummer, string naam, string voornaam, string telefoonnr)
        {
            WerknemerNummer = werknemernummer;
            Naam = naam;
            Voornaam = voornaam;
            Telefoonnr = telefoonnr;
        }
        public void ToonInfo()
        {
            
        }
    }
}
