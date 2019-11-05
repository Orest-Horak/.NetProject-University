using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class DepartmentNews
    {
        public string DepartmentName { get; set; }
        public int DepartmentNewsId { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }
        public decimal? Mark { get; set; }
        public int? DepartmentNewsNumber { get; set; }
        public int? FiltersNewsNumber { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
    }
}
