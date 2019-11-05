using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NetProject__UNIVERSITY_.Models
{
    public class DataAnalyser
    {

        //public double CalculateFacultyNewsMark(string link, List<ArticleCriteria> articleList)
        //{
        //    double result;

        //    result = Math.Min(4.0, (4.0 * (ArticleNumber / 180.0)));

        //    if ((DateTime.Now - LastArticleDate).Days > 14)
        //    {
        //        result = result / 2.0;
        //    }

        //    return result;
        //}

        public static void AnalyseFacultyNews(MarkingDate date)
        {
            List<Faculties> faculties;
            List<FacultyNews> DateNews;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                DateNews = db.FacultyNews.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var facultiesLeftJoinNews = from faculty in faculties
                                        join dateNews in DateNews on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new { FacultyId = faculty.FacultyId, PostingDate = subset?.PostingDate ?? new DateTime() };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyId);

            foreach (var newsFacultyGroup in newsGroupByFaculties)
            {

                var articleNumber = newsFacultyGroup.Count();
                if (newsFacultyGroup.First().PostingDate.Equals(new DateTime()))
                {
                    articleNumber = 0;
                }
                var lastPostingDate = newsFacultyGroup.Max(nfg => nfg.PostingDate);
                double facultyNewsMark = Math.Min(6.0, (6.0 * (articleNumber / 180.0))); ;

                if ((DateTime.Now - lastPostingDate).Days > 14)
                {
                    facultyNewsMark = facultyNewsMark / 2.0;
                }

                using (university_projectContext db = new university_projectContext())
                {
                    db.CriteriaMark.Add(new CriteriaMark
                    {
                        Mark = (decimal)(facultyNewsMark),
                        DateId = date.DateId,
                        FacultyId = newsFacultyGroup.Key,
                        CriteriaId = 1
                    });
                    db.SaveChanges();
                }

            }

        }

        public static decimal GetDepartmentMark(DepartmentNews departmentNews)
        {
            decimal result = 1.0M;

            if (departmentNews.FiltersNewsNumber >= 1 && departmentNews.DepartmentNewsNumber >= 1)
            {
                result = 2.0M;
            }
            else if (departmentNews.FiltersNewsNumber == 0 && departmentNews.DepartmentNewsNumber == 0)
            {
                result = 0.0M;
            }


            return result;
        }

        public static void AnalyseDepartmentNews(MarkingDate date)
        {
            List<Faculties> faculties;
            List<DepartmentNews> DepartmentNews;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                DepartmentNews = db.DepartmentNews.Where(dn => dn.DateId == date.DateId).ToList();

                //setting marks
                foreach (var departmentNews in DepartmentNews)
                {
                    departmentNews.Mark = GetDepartmentMark(departmentNews);
                }

                db.SaveChanges();
            }


            var facultiesLeftJoinNews = from faculty in faculties
                                        join departmentNews in DepartmentNews on faculty.FacultyId equals departmentNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new { FacultyId = faculty.FacultyId, Mark = subset?.Mark ?? 0.0M };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyId);

            foreach (var newsFacultyGroup in newsGroupByFaculties)
            {
                decimal departmentNewsMark = newsFacultyGroup.Average(nfg => nfg.Mark);

                using (university_projectContext db = new university_projectContext())
                {
                    db.CriteriaMark.Add(new CriteriaMark
                    {
                        Mark = departmentNewsMark,
                        DateId = date.DateId,
                        FacultyId = newsFacultyGroup.Key,
                        CriteriaId = 2
                    });
                    db.SaveChanges();
                }

            }

        }

        public static LecturersMark GetLecturerMark(Lecturers lecturer)
        {
            var lecturerMark = new LecturersMark();
            lecturerMark.LecturerId = lecturer.LecturerId;
            decimal? sumLecturerMark = 0.0M;
            //position and name
            if (string.IsNullOrEmpty(lecturer.Name) || string.IsNullOrEmpty(lecturer.Position))
            {
                lecturerMark.NameMark = 0;
            }
            else
            {
                lecturerMark.NameMark = 1;
            }
            sumLecturerMark += lecturerMark.NameMark;
            //contacts
            if (!string.IsNullOrEmpty(lecturer.Contact))
            {
                var containOtherContactsNumber = lecturer.Contact.Contains(".com");
                var containLnuEduUa = lecturer.Contact.Contains("@lnu.edu.ua");
                if (containLnuEduUa == false)
                {
                    lecturerMark.ContactMark = 0;
                }
                else if (containOtherContactsNumber)
                {
                    lecturerMark.ContactMark = 2;
                }
                else
                {
                    lecturerMark.ContactMark = 1;
                }
            }
            else
            {
                lecturerMark.ContactMark = 0.0M;
            }
            sumLecturerMark += lecturerMark.ContactMark;
            //biography
            if (string.IsNullOrEmpty(lecturer.Biography))
            {
                lecturerMark.BiographyMark = 0;
            }
            else
            {
                lecturerMark.BiographyMark = 1;
            }
            sumLecturerMark += lecturerMark.BiographyMark;
            //scientific interests
            if (string.IsNullOrEmpty(lecturer.ScientificInterests))
            {
                lecturerMark.SchientificInterestsMark = 0;
            }
            else
            {
                lecturerMark.SchientificInterestsMark = 1;
            }
            sumLecturerMark += lecturerMark.SchientificInterestsMark;

            lecturerMark.LecturerMark = sumLecturerMark;
            return lecturerMark;
        }

        public static void MarkLecturers(MarkingDate date)
        {
            List<Faculties> faculties;
            List<Lecturers> lecturers;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                lecturers = db.Lecturers.Where(l => l.DateId == date.DateId).ToList();
            }


            var lecturersGroupByFaculties = lecturers.GroupBy(l => l.FacultyId);


            foreach (var lecturesFacultyGroup in lecturersGroupByFaculties)
            {
                var lecturersFacultyMarks = new List<LecturersMark>();

                foreach (var lecturer in lecturesFacultyGroup)
                {
                    var lecturerMark = GetLecturerMark(lecturer);
                    lecturersFacultyMarks.Add(lecturerMark);
                }

                using (university_projectContext db = new university_projectContext())
                {

                    //foreach (var lecturerMark in lecturersFacultyMarks)
                    //{
                    //    db.LecturersMark.Add(lecturerMark);

                    //    db.Entry(lecturerMark).State = EntityState.Detached;
                    //}
                    db.LecturersMark.AddRange(lecturersFacultyMarks);
                    db.SaveChanges();
                }

            }

        }

        public static void AnalyseLecturers(MarkingDate date)
        {
            List<Faculties> faculties;
            List<Lecturers> lecturers;
            List<LecturersMark> lecturersMarks;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                lecturers = db.Lecturers.Where(l => l.DateId == date.DateId).ToList();
                lecturersMarks = (from lecturer in lecturers
                                  join lecturerMark in db.LecturersMark on lecturer.LecturerId equals lecturerMark.LecturerId
                                  select lecturerMark).ToList();
            }

            var facultiesLeftJoinLecturers = from faculty in faculties
                                             join lecturer in lecturers on faculty.FacultyId equals lecturer.FacultyId into facultyNews
                                             from subset in facultyNews.DefaultIfEmpty()
                                             select new { FacultyId = faculty.FacultyId, LecturerId = subset?.LecturerId ?? 0 };

            var facultiesLeftJoinLecturersMark = from facultyLecturer in facultiesLeftJoinLecturers
                                                 join lecturerMark in lecturersMarks on facultyLecturer.LecturerId equals lecturerMark.LecturerId into facultyLecturers
                                                 from subset in facultyLecturers.DefaultIfEmpty()
                                                 select new { FacultyId = facultyLecturer.FacultyId, Mark = subset?.LecturerMark ?? 0.0M };


            var lecturerGroupByFaculties = facultiesLeftJoinLecturersMark.GroupBy(dn => dn.FacultyId);

            foreach (var lecturerFacultyGroup in lecturerGroupByFaculties)
            {
                decimal lecturersFacultyMark = lecturerFacultyGroup.Average(nfg => nfg.Mark);

                using (university_projectContext db = new university_projectContext())
                {
                    db.CriteriaMark.Add(new CriteriaMark
                    {
                        Mark = lecturersFacultyMark,
                        DateId = date.DateId,
                        FacultyId = lecturerFacultyGroup.Key,
                        CriteriaId = 3
                    });
                    db.SaveChanges();
                }

            }

        }

        public static void MainAnalyse(MarkingDate date)
        {
            AnalyseFacultyNews(date);
            //AnalyseDepartmentNews(date);
            //MarkLecturers(date);
            //AnalyseLecturers(date);
        }
    }
}
