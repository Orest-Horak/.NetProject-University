using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models
{
    public class FacultyMark
    {
        public decimal? Ukr { get; set; }
        public decimal? Eng { get; set; }
        public decimal? Sum { get; set; }
    }

    public class ResultElement
    {
        public string FacultyName { get; set; }
        public List<Tuple<string, FacultyMark>> CriteriaMarks { get; set; }
    }
}
