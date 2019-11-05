using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class Publications
    {
        public Publications()
        {
            Lecturers = new HashSet<Lecturers>();
        }

        public int? PublicationNumber { get; set; }
        public int? HyperlinkNumber { get; set; }
        public int PublicationId { get; set; }
        public string Publication { get; set; }
        public string Hyperlink { get; set; }

        public ICollection<Lecturers> Lecturers { get; set; }
    }
}
