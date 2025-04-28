using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    public class MaandPlanAmbu: MaandPlanning
    {
        public int MaxDag { get; set; }

        public int MaxNacht { get; set; }

        public int AantalShifts { get; set; }

        public MaandPlanAmbu(int maxdag, int maxnacht, int aantalshifts, string planingNr, List<int> planningIndexen):base(planingNr, planningIndexen) 
        {
            MaxDag = maxdag;
            MaxNacht = maxnacht;
            AantalShifts = aantalshifts;
        }
    }
}
