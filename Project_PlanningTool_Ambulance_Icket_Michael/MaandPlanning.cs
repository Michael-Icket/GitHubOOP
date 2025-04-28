using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    public class MaandPlanning
    {
        public string PlanningNr { get; set; }

        public List<int> PlanningIndexen { get; set; }

        public MaandPlanning( string planingNr, List< int > planningIndexen)
        {
            PlanningNr = planingNr;
            PlanningIndexen = planningIndexen;
        }
    }
}
