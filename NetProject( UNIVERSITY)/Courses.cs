using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class Courses
    {
        public Courses()
        {
            CoursesMark = new HashSet<CoursesMark>();
        }

        public string CourseLink { get; set; }
        public string CourseDescription { get; set; }
        public string Literature { get; set; }
        public string Materials { get; set; }
        public string Plan { get; set; }
        public int CourseId { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
        public ICollection<CoursesMark> CoursesMark { get; set; }
    }
}
