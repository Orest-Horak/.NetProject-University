using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class Lecturers
    {
        public Lecturers()
        {
            LecturersMark = new HashSet<LecturersMark>();
        }

        public string Name { get; set; }
        public string Position { get; set; }
        public string Contact { get; set; }
        public string Biography { get; set; }
        public string ScientificInterests { get; set; }
        public int LecturerId { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }
        public int? PublicationId { get; set; }
        public string Link { get; set; }
        public string AcademicStatus { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
        public Publications Publication { get; set; }
        public ICollection<LecturersMark> LecturersMark { get; set; }
    }
}
