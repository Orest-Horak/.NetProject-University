using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class Materials
    {
        public int? DateId { get; set; }
        public int? FacultyId { get; set; }
        public string DepartmentName { get; set; }
        public string MaterialsLinks { get; set; }
        public int? MaterialsNumber { get; set; }
        public decimal? Mark { get; set; }
        public int MaterialsId { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
    }
}
