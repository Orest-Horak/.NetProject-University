using System;
using System.Collections.Generic;

namespace NetProject__UNIVERSITY_
{
    public partial class CoursesMark
    {
        public int CourseMarkId { get; set; }
        public int CourseId { get; set; }
        public decimal? PlanMark { get; set; }
        public decimal? MaterialsMark { get; set; }
        public decimal? DescriptionMark { get; set; }
        public decimal? CourseMark { get; set; }

        public Courses Course { get; set; }
    }
}
