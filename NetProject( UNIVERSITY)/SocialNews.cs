using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class SocialNews
    {
        public int? PostNumbers { get; set; }
        public DateTime? LastPostDate { get; set; }
        public int SocialNewsId { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
    }
}
