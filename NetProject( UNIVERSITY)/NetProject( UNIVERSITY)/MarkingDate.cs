using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class MarkingDate
    {
        public MarkingDate()
        {
            Courses = new HashSet<Courses>();
            CriteriaMark = new HashSet<CriteriaMark>();
            DepartmentNews = new HashSet<DepartmentNews>();
            FacultyNews = new HashSet<FacultyNews>();
            Lecturers = new HashSet<Lecturers>();
            SocialNews = new HashSet<SocialNews>();
        }

        public DateTime? Date { get; set; }
        public int DateId { get; set; }

        public ICollection<Courses> Courses { get; set; }
        public ICollection<CriteriaMark> CriteriaMark { get; set; }
        public ICollection<DepartmentNews> DepartmentNews { get; set; }
        public ICollection<FacultyNews> FacultyNews { get; set; }
        public ICollection<Lecturers> Lecturers { get; set; }
        public ICollection<SocialNews> SocialNews { get; set; }
    }
}
