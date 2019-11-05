using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using NetProject__UNIVERSITY_;
using HtmlAgilityPack;
using static NetProject__UNIVERSITY_.Models.DataCollector;

namespace NetProject__UNIVERSITY_.Models
{
    public class MainClass
    {

        //public static void MainFacultyNewsTest()
        //{
        //    var data = new List<Tuple<Faculties, List<Article>>>();
        //    var facultyNewsNumber = new List<Tuple<Faculties, int>>();

        //    using (university_projectContext db = new university_projectContext())
        //    {
        //        var faculties = db.Faculties.ToList();

        //        var ite = 0;
        //        for (; ite < 41; ite += 2)
        //        {
        //            var faculties2 = faculties.GetRange(ite, 2);
        //            foreach (var faculty in faculties2)
        //            {
        //                var facultyNews = DataCollector.CollectFacultyNews(faculty,date, DateTime.Parse("01.05.2019"));
        //                data.Add(new Tuple<Faculties, List<Article>>(faculty, facultyNews));
        //                facultyNewsNumber.Add(new Tuple<Faculties, int>(faculty, facultyNews.Count));
        //            }

        //            data.Clear();
        //        }

        //    }

        //    string filenameWrite = ("FacultyResults" + DateTime.Today.ToString("u") + ".txt").Replace(':', '.');

        //    using (StreamWriter f = new StreamWriter(filenameWrite, false))
        //    {

        //        f.WriteLine(string.Format("ФАКУЛЬТЕТ \t КІЛЬКІСТЬ НОВИН"));
        //        foreach (var element in facultyNewsNumber)
        //        {
        //            f.WriteLine(element.Item1.FacultyName + "\t" + element.Item1.FacultyLink + "\t" + element.Item2.ToString());
        //        }
        //    }


        //    var end = "";

        //}

        public static void MainFacultyNews(MarkingDate date)
        {
            List<Faculties> faculties;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
            }

            foreach (var faculty in faculties)
            {
                var facultyNews = CollectFacultyNews(faculty, date, DateTime.Today.AddDays(-180));

                using (university_projectContext db = new university_projectContext())
                {
                    db.FacultyNews.AddRange(facultyNews);
                    db.SaveChanges();
                }

            }

        }

        public static void MainDepartmentNews(MarkingDate date)
        {
            List<Faculties> faculties;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
            }

            foreach (var faculty in faculties)
            {
                var departmentNews = CollectDepartmentsNews(faculty, date);

                using (university_projectContext db = new university_projectContext())
                {
                    db.DepartmentNews.AddRange(departmentNews);
                    db.SaveChanges();
                }

            }

        }

        public static void MainLecturers(MarkingDate date)
        {
            List<Faculties> faculties;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
            }

            foreach (var faculty in faculties)
            {
                var lecturers = CollectLecturers(faculty, date);

                using (university_projectContext db = new university_projectContext())
                {
                    db.Lecturers.AddRange(lecturers);
                    db.SaveChanges();
                }

            }

        }

        public static void MainCollecting(MarkingDate date)
        {
            MainFacultyNews(date);
            //MainDepartmentNews(date);
            //MainLecturers(date);
        }
    }
}
