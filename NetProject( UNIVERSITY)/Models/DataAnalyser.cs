using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            List<FacultyNews> facultyNewsOnDate;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                facultyNewsOnDate = db.FacultyNews.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var defaultDateTime = new DateTime();
            var facultiesLeftJoinNews = from faculty in faculties
                                        join dateNews in facultyNewsOnDate on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            PostingDate = subset?.PostingDate ?? defaultDateTime
                                        };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyId);

            foreach (var newsFacultyGroup in newsGroupByFaculties)
            {
                var articleNumber = newsFacultyGroup.Count();
                if (newsFacultyGroup.First().PostingDate.Equals(defaultDateTime))
                {
                    articleNumber = 0;
                }

                var lastPostingDate = newsFacultyGroup.Max(nfg => nfg.PostingDate);
                double facultyNewsMark = GetFacultyNewsMark(articleNumber, lastPostingDate, date, newsFacultyGroup.First().FacultyLanguage);

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

        public static double GetFacultyNewsMark(int articleNumber, DateTime lastPostingDate,
                                                MarkingDate date, string facultyLanguange)
        {
            double expectedArticleNumber = 90.0;
            double facultyNewsMark = 0.0;
            double engMaxMark = 2.0;
            if(facultyLanguange == "анг") {
                facultyNewsMark = Math.Min(engMaxMark, (engMaxMark * (articleNumber / expectedArticleNumber)));
            }
            double ukrMaxMark = 10.0;
            if (facultyLanguange == "укр")
            {
                facultyNewsMark = Math.Min(ukrMaxMark, (ukrMaxMark * (articleNumber / expectedArticleNumber)));
            }
         
            if ((date.Date - lastPostingDate).GetValueOrDefault().Days > 14)
            {
                return facultyNewsMark / 2.0;
            }
            else
            {
                return facultyNewsMark;
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

        public static LecturersMark GetLecturerMark(Lecturers lecturer, Faculties faculty)
        {
            var lecturerMark = new LecturersMark();
            lecturerMark.LecturerId = lecturer.LecturerId;
            decimal? sumLecturerMark = 0.0M;
            //position and name
            if (string.IsNullOrEmpty(lecturer.Name) || string.IsNullOrEmpty(lecturer.Position))
            {
                lecturerMark.NameMark = 0.0M;
            }
            else
            {
                lecturerMark.NameMark = 1.0M;
            }
            sumLecturerMark += lecturerMark.NameMark;
            //contacts
            if (!string.IsNullOrEmpty(lecturer.Contact))
            {
                var containOtherContactsNumber = lecturer.Contact.Contains(".com") || (lecturer.Contact.Count(ch => ch == '@') > 1);
                var containLnuEduUa = lecturer.Contact.Contains("@lnu.edu.ua");
                if (containLnuEduUa == false)
                {
                    lecturerMark.ContactMark = 0.0M;
                }
                else if (containOtherContactsNumber)
                {
                    lecturerMark.ContactMark = 2.0M;
                }
                else
                {
                    lecturerMark.ContactMark = 1.0M;
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
                lecturerMark.BiographyMark = 0.0M;
            }
            else
            {
                lecturerMark.BiographyMark = 1.0M;
            }
            sumLecturerMark += lecturerMark.BiographyMark;
            //scientific interests
            if (string.IsNullOrEmpty(lecturer.ScientificInterests))
            {
                lecturerMark.SchientificInterestsMark = 0.0M;
            }
            else
            {
                lecturerMark.SchientificInterestsMark = 1.0M;
            }
            sumLecturerMark += lecturerMark.SchientificInterestsMark;
            //publication
            if (lecturer.Publication != null)
            {
                int lowerHyperlinklimit = GetLowerHyperlinkLimit(lecturer);
                //Faculties faculty = new Faculties();
                //faculty.FacultyLanguage = "";
                //using (university_projectContext db = new university_projectContext())
                //{
                //    faculty = db.Faculties.Where(f => f.FacultyId == lecturer.FacultyId).First();
                //}
                //int maxMark = lecturer.Faculty.FacultyLanguage.Equals("укр") ? 7 : 3;
                //double maxMark = faculty.FacultyLanguage.Equals("укр") ? 7.0 : 3.0;
                double maxMark = 5.0;
                if (lecturer.Publication.HyperlinkNumber == null)
                {
                    lecturer.Publication.HyperlinkNumber = 0;
                }
                if (lecturer.Publication.HyperlinkNumber >= lowerHyperlinklimit)
                {
                    lecturerMark.PublicationMark = (decimal)maxMark;
                }
                else
                {
                    lecturerMark.PublicationMark = (decimal)Math.Min(maxMark,
                                                            (double)(maxMark * lecturer.Publication.HyperlinkNumber / (double)lowerHyperlinklimit));
                }

            }
            else
            {
                if (lecturer.Publication.HyperlinkNumber == null)
                {
                    lecturer.Publication.HyperlinkNumber = 0;
                }
                lecturerMark.PublicationMark = 0.0M;
            }
            sumLecturerMark += lecturerMark.PublicationMark;

            lecturerMark.LecturerMark = sumLecturerMark;
            return lecturerMark;
        }

        private static int GetLowerHyperlinkLimit(Lecturers lecturer)
        {
            string[] wordToCheck;

            //= new string[] { "асистент", "assistant" };
            //for (int i = 0; i < wordToCheck.Length; i++)
            //{
            //    if (lecturer.Position != null && lecturer.Position.Contains(wordToCheck[i], StringComparison.OrdinalIgnoreCase))
            //    {
            //        return 10;
            //    }
            //    if (lecturer.AcademicStatus != null && lecturer.AcademicStatus.Contains(wordToCheck[i], StringComparison.OrdinalIgnoreCase))
            //    {
            //        return 10;
            //    }
            //}

            wordToCheck = new string[]
            {
                "docent", "доцент",
                "professor", "професор",
                "старший викладач", "senior lecturer",
                "chairperson", "завідувач"
            };

            for (int i = 0; i < wordToCheck.Length; i++)
            {
                if (lecturer.Position != null && lecturer.Position.Contains(wordToCheck[i], StringComparison.OrdinalIgnoreCase))
                {
                    return 10;
                }
                if (lecturer.AcademicStatus != null && lecturer.AcademicStatus.Contains(wordToCheck[i], StringComparison.OrdinalIgnoreCase))
                {
                    return 10;
                }
            }

            return 5;
        }

        //public static void MarkLecturers(MarkingDate date)
        //{
        //    List<Faculties> faculties;
        //    List<Lecturers> lecturers;
        //    using (university_projectContext db = new university_projectContext())
        //    {
        //        faculties = db.Faculties.ToList();
        //        lecturers = db.Lecturers.Where(l => l.DateId == date.DateId).ToList();
        //        var test = from l in db.Lecturers
        //                   join p in db.Publications on l.PublicationId equals p.PublicationId
        //                   where l.DateId == date.DateId
        //                   select new { Lecturer = l, Publication = p };
        //        //publications = db.Publications.Where(p => p.PublicationId).ToList();
        //    }


        //    var lecturersGroupByFaculties = lecturers.GroupBy(l => l.FacultyId);


        //    foreach (var lecturesFacultyGroup in lecturersGroupByFaculties)
        //    {
        //        var lecturersFacultyMarks = new List<LecturersMark>();

        //        foreach (var lecturer in lecturesFacultyGroup)
        //        {
        //            var lecturerMark = GetLecturerMark(lecturer);
        //            lecturersFacultyMarks.Add(lecturerMark);
        //        }

        //        using (university_projectContext db = new university_projectContext())
        //        {

        //            //foreach (var lecturerMark in lecturersFacultyMarks)
        //            //{
        //            //    db.LecturersMark.Add(lecturerMark);

        //            //    db.Entry(lecturerMark).State = EntityState.Detached;
        //            //}
        //            db.LecturersMark.AddRange(lecturersFacultyMarks);
        //            db.SaveChanges();
        //        }

        //    }

        //}

        public static void MarkLecturers(MarkingDate date)
        {
            List<Faculties> faculties;
            List<Lecturers> lecturersWithPublications;

            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                //lecturers = db.Lecturers.Where(l => l.DateId == date.DateId).ToList();
                //from faculty in faculties
                //join departmentNews in DepartmentNews on faculty.FacultyId equals departmentNews.FacultyId into facultyNews
                //from subset in facultyNews.DefaultIfEmpty()
                //select new { FacultyId = faculty.FacultyId, Mark = subset?.Mark ?? 0.0M };

                lecturersWithPublications = (from l in db.Lecturers
                                             join p in db.Publications on l.PublicationId equals p.PublicationId into lecturerPublication
                                             from subset in lecturerPublication.DefaultIfEmpty()
                                             where l.DateId == date.DateId
                                             select new Lecturers
                                             {
                                                 AcademicStatus = l.AcademicStatus,
                                                 Biography = l.Biography,
                                                 Contact = l.Contact,
                                                 DateId = l.DateId,
                                                 FacultyId = l.FacultyId,
                                                 LecturerId = l.LecturerId,
                                                 Link = l.Link,
                                                 Name = l.Name,
                                                 Position = l.Position,
                                                 PublicationId = l.PublicationId,
                                                 Publication = subset ?? new Publications(),
                                                 ScientificInterests = l.ScientificInterests
                                             }).ToList();
                //publications = db.Publications.Where(p => p.PublicationId).ToList();
            }


            var lecturersGroupByFaculties = lecturersWithPublications.GroupBy(l => l.FacultyId);


            foreach (var lecturesFacultyGroup in lecturersGroupByFaculties)
            {
                var lecturerFaculty = faculties.Where(f => f.FacultyId == lecturesFacultyGroup.Key).First();
                var lecturersFacultyMarks = new List<LecturersMark>();

                foreach (var lecturer in lecturesFacultyGroup)
                {
                    var lecturerMark = GetLecturerMark(lecturer, lecturerFaculty);
                    lecturersFacultyMarks.Add(lecturerMark);
                }

                using (university_projectContext db = new university_projectContext())
                {
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



        public static decimal GetMaterialsMark(Materials mymaterials)
        {
            decimal result = 0.0M;

            if (mymaterials.MaterialsNumber >= 1 && mymaterials.MaterialsNumber <= 3)
            {
                result = 1.0M;
            }
            else if (mymaterials.MaterialsNumber >= 4 && mymaterials.MaterialsNumber <= 6)
            {
                result = 2.0M;
            }
            else if (mymaterials.MaterialsNumber >= 7 && mymaterials.MaterialsNumber <= 8)
            {
                result = 3.0M;
            }
            else if (mymaterials.MaterialsNumber >= 9)
            {
                result = 4.0M;
            }

            return result;
        }

        public static void AnalyseMaterials(MarkingDate date)
        {
            List<Faculties> faculties;
            List<Materials> materials;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                materials = db.Materials.Where(dn => dn.DateId == date.DateId).ToList();

                //setting marks
                foreach (var material in materials)
                {
                    material.Mark = GetMaterialsMark(material);
                }

                db.SaveChanges();
            }


            var facultiesLeftJoinNews = from faculty in faculties
                                        join Material in materials on faculty.FacultyId equals Material.FacultyId into facultyMaterials
                                        from subset in facultyMaterials.DefaultIfEmpty()
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
                        CriteriaId = 5
                    });
                    db.SaveChanges();
                }

            }

        }

        public static CoursesMark GetCoursesMark(Courses course)
        {
            var courseMark = new CoursesMark();
            courseMark.CourseId = course.CourseId;
            //опис курсу 
            if (string.IsNullOrEmpty(course.CourseDescription))
            {
                courseMark.DescriptionMark = 0.00M;
            }
            else
            {
                courseMark.DescriptionMark = 2.00M;
            }
            //навчальна програма
            if (string.IsNullOrEmpty(course.Plan))
            {
                courseMark.PlanMark = 0.00M;
            }
            else
            {
                courseMark.PlanMark = 1.00M;
                //try
                //{
                //    if (course.Plan.EndsWith(".pdf") || course.Plan.EndsWith(".doc"))
                //    {
                //        courseMark.PlanMark = 1.00M;
                //    }
                //    else
                //    {
                //        courseMark.PlanMark = 0.00M;
                //    }
                //}
                //catch (Exception e)
                //{
                //    courseMark.PlanMark = 0.00M;
                //}
            }
            //матеріали
            if (string.IsNullOrEmpty(course.Materials))
            {
                courseMark.MaterialsMark = 0.00M;
            }
            else
            {
                courseMark.MaterialsMark = 1.00M;
                try
                {
                    var hyperlinks = Regex.Matches(course.Materials, "href=\\\"(.*?)\"").Select(hp => hp.Groups[1].Value).ToList();
                    //int hyperlinkNumber = hyperlinks.Where(hp => hp.EndsWith(".pdf") || hp.EndsWith(".doc")).Count();
                    int hyperlinkNumber = hyperlinks.Count();
                    if (hyperlinkNumber >= 1)
                    {
                        courseMark.MaterialsMark = 2.00M;
                    }
                }
                catch (Exception e)
                {

                }
            }
            //загальна оцінка
            courseMark.CourseMark = courseMark.DescriptionMark + courseMark.PlanMark + courseMark.MaterialsMark;

            return courseMark;
        }

        public static void MarkCourses(MarkingDate date)
        {
            List<Courses> courses;
            using (university_projectContext db = new university_projectContext())
            {
                courses = db.Courses.Where(c => c.DateId == date.DateId).ToList();
            }


            var coursesGroupByFaculties = courses.GroupBy(l => l.FacultyId);

            foreach (var courseFacultyGroup in coursesGroupByFaculties)
            {
                var courseFacultyMarks = new List<CoursesMark>();

                foreach (var course in courseFacultyGroup)
                {
                    var courseMark = GetCoursesMark(course);
                    courseFacultyMarks.Add(courseMark);
                }

                using (university_projectContext db = new university_projectContext())
                {
                    db.CoursesMark.AddRange(courseFacultyMarks);
                    db.SaveChanges();
                }

            }

        }

        public static void AnalyseCourses(MarkingDate date)
        {
            List<Faculties> faculties;
            List<Courses> courses;
            List<CoursesMark> coursesMarks;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                courses = db.Courses.Where(l => l.DateId == date.DateId).ToList();
                coursesMarks = (from course in courses
                                join courseMark in db.CoursesMark on course.CourseId equals courseMark.CourseId
                                select courseMark).ToList();
            }

            var facultiesLeftJoinCourses = from faculty in faculties
                                           join course in courses on faculty.FacultyId equals course.FacultyId into facultyNews
                                           from subset in facultyNews.DefaultIfEmpty()
                                           select new { FacultyId = faculty.FacultyId, CourseId = subset?.CourseId ?? 0 };

            var facultiesLeftJoinCoursesMark = from facultyCourse in facultiesLeftJoinCourses
                                               join courseMark in coursesMarks on facultyCourse.CourseId equals courseMark.CourseId into facultyCourses
                                               from subset in facultyCourses.DefaultIfEmpty()
                                               select new { FacultyId = facultyCourse.FacultyId, Mark = subset?.CourseMark ?? 0.0M };


            var courseGroupByFaculties = facultiesLeftJoinCoursesMark.GroupBy(dn => dn.FacultyId);

            foreach (var courseFacultyGroup in courseGroupByFaculties)
            {
                decimal coursesFacultyMark = courseFacultyGroup.Average(cfg => cfg.Mark);

                using (university_projectContext db = new university_projectContext())
                {
                    db.CriteriaMark.Add(new CriteriaMark
                    {
                        Mark = coursesFacultyMark,
                        DateId = date.DateId,
                        FacultyId = courseFacultyGroup.Key,
                        CriteriaId = 4
                    });
                    db.SaveChanges();
                }

            }

        }

        public static void MainAnalyse(MarkingDate date)
        {
            //AnalyseFacultyNews(date);
            //MarkCourses(date);
            //AnalyseCourses(date);
            MarkLecturers(date);
            AnalyseLecturers(date);
            //AnalyseMaterials(date);
        }
    }
}
