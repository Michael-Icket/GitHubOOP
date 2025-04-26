using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    public class Ambulancier : Werknemers
    {
        public bool Crijbewijs { get; set; }

        public bool Planner { get; set; }

        public int JarenErvaring { get; set; }

        public TypeAmbulancier TypeAmbu { get; set; }

        public Ambulancier(int werknemernummer, string naam, string voornaam, string telefoonnr, bool crijbewijs, bool planner, int jarenErvaring, TypeAmbulancier typeAmbu) 
            : base(werknemernummer, naam, voornaam, telefoonnr)

        {
            Crijbewijs = crijbewijs;
            JarenErvaring = jarenErvaring;
            TypeAmbu = typeAmbu;
            Planner = planner;
        }   
    }
}
