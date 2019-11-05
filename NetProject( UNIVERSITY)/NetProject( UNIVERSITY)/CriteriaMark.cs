using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class CriteriaMark
    {
        public decimal? Mark { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }
        public int? CriteriaId { get; set; }
        public int CriteriaMarkId { get; set; }

        public Criterias Criteria { get; set; }
        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
    }
}
