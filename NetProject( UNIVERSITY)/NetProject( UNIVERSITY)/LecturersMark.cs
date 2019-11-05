using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class LecturersMark
    {
        public int LecturersMarkId { get; set; }
        public int LecturerId { get; set; }
        public decimal? NameMark { get; set; }
        public decimal? ContactMark { get; set; }
        public decimal? BiographyMark { get; set; }
        public decimal? SchientificInterestsMark { get; set; }
        public decimal? PublicationMark { get; set; }
        public decimal? HyperlinkMark { get; set; }
        public decimal? LecturerMark { get; set; }

        public Lecturers Lecturer { get; set; }
    }
}
