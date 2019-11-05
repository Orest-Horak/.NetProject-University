using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetProject__UNIVERSITY_
{
    public partial class Faculties
    {
        public Faculties()
        {
            Courses = new HashSet<Courses>();
            CriteriaMark = new HashSet<CriteriaMark>();
            DepartmentNews = new HashSet<DepartmentNews>();
            FacultyNews = new HashSet<FacultyNews>();
            Lecturers = new HashSet<Lecturers>();
            SocialNews = new HashSet<SocialNews>();
        }

        public string FacultyName { get; set; }
        public int FacultyId { get; set; }
        public string FacultyLanguage { get; set; }
        public string FacultyLink { get; set; }

        public ICollection<Courses> Courses { get; set; }
        public ICollection<CriteriaMark> CriteriaMark { get; set; }
        public ICollection<DepartmentNews> DepartmentNews { get; set; }
        public ICollection<FacultyNews> FacultyNews { get; set; }
        public ICollection<Lecturers> Lecturers { get; set; }
        public ICollection<SocialNews> SocialNews { get; set; }
    }
}
