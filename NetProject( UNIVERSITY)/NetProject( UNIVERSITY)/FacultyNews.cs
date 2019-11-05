using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetProject__UNIVERSITY_
{
    public partial class FacultyNews
    {
        public DateTime? PostingDate { get; set; }
        public string Page { get; set; }
        public string Link { get; set; }
        public int FacultyNewsId { get; set; }
        public int? FacultyId { get; set; }
        public int? DateId { get; set; }

        public MarkingDate Date { get; set; }
        public Faculties Faculty { get; set; }
    }
}
