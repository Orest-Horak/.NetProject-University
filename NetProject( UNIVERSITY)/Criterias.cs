using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class Criterias
    {
        public Criterias()
        {
            CriteriaMark = new HashSet<CriteriaMark>();
        }

        public string CriteriaName { get; set; }
        public string Description { get; set; }
        public int CriteriaId { get; set; }

        public ICollection<CriteriaMark> CriteriaMark { get; set; }
    }
}
