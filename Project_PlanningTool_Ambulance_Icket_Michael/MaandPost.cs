using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_PlanningTool_Ambulance_Icket_Michael
{
    public class MaandPost
    {
        public string PlanningNr { get; set; }

        public List<int> PlanningIndexen { get; set; }

        public MaandPost( string planingNr, List< int > planningIndexen)
        {
            PlanningNr = planingNr;
            PlanningIndexen = planningIndexen;
        }
    }
}
