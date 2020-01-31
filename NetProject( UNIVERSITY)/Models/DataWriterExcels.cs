using NetProject__UNIVERSITY_;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject_UNIVERSITY_.Models
{
    public class DataWriterExcels
    {
        public static MemoryStream WriteExcelFacultyNews2(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
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
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyLink = faculty.FacultyLink,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            Page = subset?.Page ?? new string("0"),
                                            Link = subset?.Link ?? new string(""),
                                            PostingDate = subset?.PostingDate ?? new DateTime()
                                        };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyId);
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Загальна к-сть новин");
                excel.Workbook.Worksheets.Add("Новини факультету");

                var header_news_rows_up = new List<object[]>()
                   {
                       new object[] {"Факультет","Мова сторінки", "Кількість новин ", "Дата останньої публікації" },

                   };
                var header_news_rows_under = new List<object[]>()
                   {
                           new object[] { "Дата новини", "Факультет", "Мова сторінки", "Сторінка", "Посилання на новину" }

                   };


                var news_rows_isupper = new List<string[]>();
                var news_rows_isunder = new List<string[]>();
                foreach (var newsFacultyGroup in newsGroupByFaculties)
                {
                    var articleNumber = newsFacultyGroup.Count();
                    var datastring = "";
                    var datastring2 = "";
                    if (newsFacultyGroup.First().PostingDate.Equals(new DateTime()))
                    {
                        articleNumber = 0;
                    }

                    var lastPostingDate = newsFacultyGroup.Max(nfg => nfg.PostingDate);
                    datastring2 = lastPostingDate.ToString();
                    if (datastring2 == "01.01.0001 0:00:00")
                    {
                        datastring2 = "Немає дат";
                    }
                    news_rows_isupper.Add(new string[] { newsFacultyGroup.First().FacultyName.ToString(), newsFacultyGroup.First().FacultyLanguage, articleNumber.ToString(), datastring2 });

                    foreach (var facultyNews in newsFacultyGroup)
                    {
                        datastring = facultyNews.PostingDate.ToString("u").ToString();
                        if (datastring == "0001-01-01 00:00:00Z")
                        {
                            datastring = "Немає дати";
                        }
                        news_rows_isunder.Add(new string[] { datastring, facultyNews.FacultyName.ToString(), facultyNews.FacultyLanguage, (facultyNews.Page).ToString(), facultyNews.Link.ToString() });
                    }
                    if (newsFacultyGroup.First().FacultyLanguage.ToString() == "анг")
                    {
                        news_rows_isunder.Add(new string[] { "" });
                    }
                }


                // Determine the header range (e.g. A1:D1)
                string header_up = "A1";
                int i = 1;
                i = i + header_news_rows_under.Count();

                string header_isup = "A" + i.ToString();
                i = i + news_rows_isupper.Count();

                int j = 1;
                string header_under = "A" + j.ToString();
                j = j + header_news_rows_under.Count();

                string header_isunder = "A" + j.ToString();
                j = j + news_rows_isunder.Count();

                var worksheet1 = excel.Workbook.Worksheets["Загальна к-сть новин"];
                var worksheet2 = excel.Workbook.Worksheets["Новини факультету"];

                //Load the summary data into the sheet, starting from cell A1. Print the column names on row 1
                worksheet1.Cells[header_up].LoadFromArrays(header_news_rows_up);
                worksheet1.Cells[header_isup].LoadFromArrays(news_rows_isupper);
                worksheet2.Cells[header_under].LoadFromArrays(header_news_rows_under);
                worksheet2.Cells[header_isunder].LoadFromArrays(news_rows_isunder);
                worksheet1.Cells[worksheet1.Dimension.Address].AutoFitColumns();
                worksheet2.Cells[worksheet2.Dimension.Address].AutoFitColumns();
                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Новини факультетів (всі).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelDepartmentNews2(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<DepartmentNews> departmentNews;

            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                departmentNews = db.DepartmentNews.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var facultiesLeftJoinNews = from faculty in faculties
                                        join dateNews in departmentNews on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            DepartmentName = subset?.DepartmentName ?? new string(""),
                                            DepartmentNewsNumber = subset?.DepartmentNewsNumber ?? new int(),
                                            FiltersNewsNumber = subset?.FiltersNewsNumber ?? new int(),
                                        };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyId);

            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Новини кафедр");
                var header_news_rows_under = new List<object[]>()
                       {
                           new object[] {"Назва кафедри", "Факультет", "Мова сторінки", "Кількість новин на першій сторінці ", "Чи є у фільтрі" },

                       };

                var news_rows_isupper = new List<string[]>();
                var news_rows_isunder = new List<string[]>();
                string isfilter = "Немає";
                foreach (var newsFacultyGroup in newsGroupByFaculties)
                {
                    foreach (var department_News in newsFacultyGroup)
                    {
                        if (department_News.FiltersNewsNumber != 0)
                        {
                            isfilter = "Наявний";
                            news_rows_isunder.Add(new string[] { department_News.DepartmentName.ToString(), department_News.FacultyName.ToString(), department_News.FacultyLanguage, department_News.DepartmentNewsNumber.ToString(), isfilter.ToString() });
                            isfilter = "Немає";
                        }
                        else
                        {
                            news_rows_isunder.Add(new string[] { department_News.DepartmentName.ToString(), department_News.FacultyName.ToString(), department_News.FacultyLanguage, department_News.DepartmentNewsNumber.ToString(), isfilter.ToString() });
                        }

                    }
                    if (newsFacultyGroup.First().FacultyLanguage.ToString() == "анг")
                    {
                        news_rows_isunder.Add(new string[] { "" });
                    }

                }

                int j = 1;
                string header_under = "A" + j.ToString();
                j = j + header_news_rows_under.Count();

                string header_isunder = "A" + j.ToString();
                j = j + news_rows_isunder.Count();

                var worksheet2 = excel.Workbook.Worksheets["Новини кафедр"];

                worksheet2.Cells[header_under].LoadFromArrays(header_news_rows_under);
                worksheet2.Cells[header_isunder].LoadFromArrays(news_rows_isunder);
                worksheet2.Cells[worksheet2.Dimension.Address].AutoFitColumns();

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Новини кафедр (всі).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelFacultyNews1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
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
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyLink = faculty.FacultyLink,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            Page = subset?.Page ?? new string("0"),
                                            Link = subset?.Link ?? new string(""),
                                            PostingDate = subset?.PostingDate ?? new DateTime()
                                        };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyName);
            string used = "";
            var datastring = "";
            var news_rows_isunder = new List<string[]>();
            var header_news_rows_under = new List<object[]>()
            {
                new object[] { "Дата новини", "Факультет", "Мова сторінки", "Сторінка", "Посилання на новину" }

            };

            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach (var newsFacultyGroup in newsGroupByFaculties)
                {
                    used = newsFacultyGroup.Key;
                    if (used.Length > 25)

                    {
                        used = (newsFacultyGroup.Key).Substring(0, 25);
                    }
                    excel.Workbook.Worksheets.Add(used);

                    var worksheet = excel.Workbook.Worksheets["Новини факультету"];

                    worksheet = excel.Workbook.Worksheets[used];
                    used = "";

                    int j = 1;
                    string header_under = "A" + j.ToString();
                    j = j + header_news_rows_under.Count();

                    string header_isunder = "A" + j.ToString();
                    j = j + news_rows_isunder.Count();


                    worksheet.Cells[header_under].LoadFromArrays(header_news_rows_under);

                    foreach (var facultyNews in newsFacultyGroup)
                    {
                        datastring = facultyNews.PostingDate.ToString("u").ToString();
                        if (datastring == "0001-01-01 00:00:00Z")
                        {
                            datastring = "Немає дати";
                        }
                        news_rows_isunder.Add(new string[] { datastring, facultyNews.FacultyName.ToString(), facultyNews.FacultyLanguage, (facultyNews.Page).ToString(), facultyNews.Link.ToString() });
                    }

                    worksheet.Cells[header_isunder].LoadFromArrays(news_rows_isunder);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    news_rows_isunder.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Новини факультетів (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelDepartmentNews1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<DepartmentNews> departmentNews;

            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                departmentNews = db.DepartmentNews.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var facultiesLeftJoinNews = from faculty in faculties
                                        join dateNews in departmentNews on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            DepartmentName = subset?.DepartmentName ?? new string(""),
                                            DepartmentNewsNumber = subset?.DepartmentNewsNumber ?? new int(),
                                            FiltersNewsNumber = subset?.FiltersNewsNumber ?? new int(),
                                        };

            var newsGroupByFaculties = facultiesLeftJoinNews.GroupBy(dn => dn.FacultyName);

            string used = "";
            var news_rows_isunder = new List<string[]>();
            var header_news_rows_under = new List<object[]>()
            {
                 new object[] {"Назва кафедри", "Факультет", "Мова сторінки", "Кількість новин на першій сторінці ", "Чи є у фільтрі" },

            };

            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach (var newsFacultyGroup in newsGroupByFaculties)
                {
                    used = newsFacultyGroup.Key;
                    if (used.Length > 25)

                    {
                        used = (newsFacultyGroup.Key).Substring(0, 25);
                    }
                    excel.Workbook.Worksheets.Add(used);

                    var worksheet = excel.Workbook.Worksheets["Новини кафедри"];

                    worksheet = excel.Workbook.Worksheets[used];
                    used = "";

                    int j = 1;
                    string header_under = "A" + j.ToString();
                    j = j + header_news_rows_under.Count();

                    string header_isunder = "A" + j.ToString();
                    j = j + news_rows_isunder.Count();


                    worksheet.Cells[header_under].LoadFromArrays(header_news_rows_under);
                    string isfilter = "Немає";
                    foreach (var department_News in newsFacultyGroup)
                    {
                        if (department_News.FiltersNewsNumber != 0)
                        {
                            isfilter = "Наявний";
                            news_rows_isunder.Add(new string[] { department_News.DepartmentName.ToString(), department_News.FacultyName.ToString(), department_News.FacultyLanguage, department_News.DepartmentNewsNumber.ToString(), isfilter.ToString() });
                            isfilter = "Немає";
                        }
                        else
                        {
                            news_rows_isunder.Add(new string[] { department_News.DepartmentName.ToString(), department_News.FacultyName.ToString(), department_News.FacultyLanguage, department_News.DepartmentNewsNumber.ToString(), isfilter.ToString() });
                        }
                    }

                    worksheet.Cells[header_isunder].LoadFromArrays(news_rows_isunder);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    news_rows_isunder.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Новини кафедр (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        //public static MemoryStream WriteExcelLecturers2(MarkingDate date)
        //{
        //    var memoryStream = new MemoryStream();
        //    List<Faculties> faculties;
        //    List<Lecturers> lecturers;
        //    using (university_projectContext db = new university_projectContext())
        //    {
        //        faculties = db.Faculties.ToList();
        //        lecturers = db.Lecturers.Where(fn => fn.DateId == date.DateId).ToList();
        //    }

        //    var lecturersLeftJoinNews = from faculty in faculties
        //                                join dateNews in lecturers on faculty.FacultyId equals dateNews.FacultyId into facultyNews
        //                                from subset in facultyNews.DefaultIfEmpty()
        //                                select new
        //                                {
        //                                    FacultyId = faculty.FacultyId,
        //                                    FacultyLink = faculty.FacultyLink,
        //                                    FacultyName = faculty.FacultyName,
        //                                    FacultyLanguage = faculty.FacultyLanguage,
        //                                    Name = subset?.Name ?? new string(""),
        //                                    Position = subset?.Position ?? new string(""),
        //                                    Contact = subset?.Contact ?? new string(""),
        //                                    Biography = subset?.Biography ?? new string(""),
        //                                    ScientificInterests = subset?.ScientificInterests ?? new string(""),
        //                                    Link = subset?.Link ?? new string("")
        //                                };

        //    var lecturersGroupByFaculties = lecturersLeftJoinNews.GroupBy(dn => dn.FacultyId);
        //    using (ExcelPackage excel = new ExcelPackage())
        //    {
        //        excel.Workbook.Worksheets.Add("Викладачі");

        //        var header_lecturers_rows_up = new List<object[]>()
        //        {
        //             new object[] { "Факультет", "Мова сторінки", "Ім'я Прізвище По-батькові","Посада", "Контакти", "Біографія","Науковий інтерес","Посилання"}

        //        };


        //        var lecturers_rows_isupper = new List<string[]>();
        //        foreach (var lecturersFacultyGroup in lecturersGroupByFaculties)
        //        {
        //            string Name1 = "";
        //            string Position1 = "";
        //            string Contact1 = "";
        //            string Biography1 = "";
        //            string ScientificInterests1 = "";
        //            string Link1 = "";

        //            foreach (var lecturerss in lecturersFacultyGroup)
        //            {
        //                Name1 = lecturerss.Name.ToString();
        //                Position1 = lecturerss.Position.ToString();
        //                Contact1 = lecturerss.Contact.ToString();
        //                Biography1 = lecturerss.Biography.ToString();
        //                ScientificInterests1 = lecturerss.ScientificInterests.ToString();
        //                Link1 = lecturerss.Link.ToString();

        //                if (string.IsNullOrEmpty(Name1))
        //                {
        //                    Name1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Position1))
        //                {
        //                    Position1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Contact1))
        //                {
        //                    Contact1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Biography1))
        //                {
        //                    Biography1 = "";
        //                }
        //                if (string.IsNullOrEmpty(ScientificInterests1))
        //                {
        //                    ScientificInterests1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Link1))
        //                {
        //                    Link1 = "";
        //                }
        //                lecturers_rows_isupper.Add(new string[] { lecturerss.FacultyName.ToString(), lecturerss.FacultyLanguage.ToString(), Name1,
        //                    Position1, Contact1, Biography1, ScientificInterests1,Link1
        //                });
        //            }

        //            if (lecturersFacultyGroup.First().FacultyLanguage.ToString() == "анг")
        //            {
        //                lecturers_rows_isupper.Add(new string[] { "" });
        //            }
        //        }


        //        // Determine the header range (e.g. A1:D1)
        //        string header_up = "A1";
        //        int j = 1;
        //        j = j + header_lecturers_rows_up.Count();

        //        string header_under = "A" + j.ToString();
        //        j = j + lecturers_rows_isupper.Count();

        //        var worksheet1 = excel.Workbook.Worksheets["Викладачі"];

        //        //Load the summary data into the sheet, starting from cell A1. Print the column names on row 1
        //        worksheet1.Cells[header_up].LoadFromArrays(header_lecturers_rows_up);
        //        worksheet1.Cells[header_under].LoadFromArrays(lecturers_rows_isupper);
        //        worksheet1.Cells[worksheet1.Dimension.Address].AutoFitColumns();


        //        //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Викладачі (всі).xlsx");
        //        //excel.SaveAs(excelFile);
        //        memoryStream = new MemoryStream(excel.GetAsByteArray());
        //    }
        //    return memoryStream;
        //}

        //public static MemoryStream WriteExcelLecturers1(MarkingDate date)
        //{
        //    var memoryStream = new MemoryStream();
        //    List<Faculties> faculties;
        //    List<Lecturers> lecturers;
        //    List<Publications> lecturers_publications;

        //    using (university_projectContext db = new university_projectContext())
        //    {
        //        faculties = db.Faculties.ToList();
        //        lecturers = db.Lecturers.Where(fn => fn.DateId == date.DateId).ToList();
        //        lecturers_publications = (from lecturer in lecturers
        //                          join lecturerPublication in db.Publications on lecturer.PublicationId equals lecturerPublication.PublicationId
        //                                  select lecturerPublication).ToList();
        //    }

        //    //var lecturersLeftJoinNews = from faculty in faculties
        //    //                            join dateLecturer in lecturers on faculty.FacultyId equals dateLecturer.FacultyId
        //    //                            from publications in lecturers_publications
        //    //                            join dataPublication in lecturers on publications.PublicationId equals dataPublication.PublicationId
        //    //                            into facultyNews from subset in facultyNews.DefaultIfEmpty()
        //    //                            select new
        //    //                            {
        //    //                                FacultyId = faculty.FacultyId,
        //    //                                FacultyLink = faculty.FacultyLink,
        //    //                                FacultyName = faculty.FacultyName,
        //    //                                FacultyLanguage = faculty.FacultyLanguage,
        //    //                                Name = subset?.Name ?? new string(""),
        //    //                                Position = subset?.Position ?? new string(""),
        //    //                                Contact = subset?.Contact ?? new string(""),
        //    //                                Biography = subset?.Biography ?? new string(""),
        //    //                                ScientificInterests = subset?.ScientificInterests ?? new string(""),
        //    //                                AcademicStatus = subset?.AcademicStatus ?? new string(""),
        //    //                                Link = subset?.Link ?? new string(""),
        //    //                                Hyperlink = subset?.Hyperlink ?? new string("")
        //    //                            };

        //    var lecturersLeftJoinFaculty = from faculty in faculties
        //                                     join lecturer in lecturers on faculty.FacultyId equals lecturer.FacultyId into facultyNews
        //                                     from subset in facultyNews.DefaultIfEmpty()
        //                                     select new
        //                                     {
        //                                         FacultyLink = faculty.FacultyLink,
        //                                         FacultyName = faculty.FacultyName,
        //                                         FacultyLanguage = faculty.FacultyLanguage,
        //                                         Name = subset?.Name ?? new string(""),
        //                                         Position = subset?.Position ?? new string(""),
        //                                         Contact = subset?.Contact ?? new string(""),
        //                                         Biography = subset?.Biography ?? new string(""),
        //                                         ScientificInterests = subset?.ScientificInterests ?? new string(""),
        //                                         AcademicStatus = subset?.AcademicStatus ?? new string(""),
        //                                         Link = subset?.Link ?? new string(""),
        //                                         FacultyId = faculty.FacultyId,
        //                                         PublicationId = subset?.PublicationId ?? 0
        //                                     };

        //    var lecturersLeftJoinPublications = from facultyLecturer in lecturersLeftJoinFaculty
        //                                        join lecturerMark in lecturers_publications on facultyLecturer.PublicationId equals lecturerMark.PublicationId into facultyLecturers
        //                                         from subset in facultyLecturers.DefaultIfEmpty()
        //                                         select new {
        //                                             FacultyId = facultyLecturer.FacultyId,
        //                                             FacultyLink = facultyLecturer.FacultyLink,
        //                                             FacultyName = facultyLecturer.FacultyName,
        //                                             FacultyLanguage = facultyLecturer.FacultyLanguage,
        //                                             Name = facultyLecturer.Name ?? new string(""),
        //                                             Position = facultyLecturer.Position ?? new string(""),
        //                                             Contact = facultyLecturer.Contact ?? new string(""),
        //                                             Biography = facultyLecturer.Biography ?? new string(""),
        //                                             ScientificInterests = facultyLecturer.ScientificInterests ?? new string(""),
        //                                             AcademicStatus = facultyLecturer.AcademicStatus ?? new string(""),
        //                                             Link = facultyLecturer.Link ?? new string(""),
        //                                             HyperlinkNumber = subset?.HyperlinkNumber ?? new int(),
        //                                             Hyperlink = subset?.Hyperlink ?? new string("")
        //                                         };

        //    var lecturersGroupByFaculties = lecturersLeftJoinPublications.GroupBy(dn => dn.FacultyName);
        //    string used = "";

        //    var lecturers_rows_isupper = new List<string[]>();
        //    var header_lecturers_rows_up = new List<object[]>()
        //    {
        //          new object[] { "Факультет", "Мова сторінки", "Ім'я Прізвище По-батькові","Посада","Вчене звання", "Контакти", "Біографія","Науковий інтерес","Посилання","Кількість публікацій","Список публікацій"}

        //     };
        //    using (ExcelPackage excel = new ExcelPackage())
        //    {
        //        foreach (var lecturersFacultyGroup in lecturersGroupByFaculties)
        //        {
        //            used = lecturersFacultyGroup.Key;
        //            if (used.Length > 25)

        //            {
        //                used = (lecturersFacultyGroup.Key).Substring(0, 25);
        //            }
        //            excel.Workbook.Worksheets.Add(used);

        //            var worksheet = excel.Workbook.Worksheets["Викладачі"];

        //            worksheet = excel.Workbook.Worksheets[used];
        //            used = "";

        //            int j = 1;
        //            string header_under = "A" + j.ToString();
        //            j = j + header_lecturers_rows_up.Count();

        //            string header_isunder = "A" + j.ToString();
        //            j = j + lecturers_rows_isupper.Count();

        //            string Name1 = "";
        //            string Position1 = "";
        //            string Contact1 = "";
        //            string Biography1 = "";
        //            string ScientificInterests1 = "";
        //            string AcademicStatus1 = "";
        //            string Link1 = "";
        //            string Hyperlink1 = "";

        //            foreach (var lecturerss in lecturersFacultyGroup)
        //            {
        //                Name1 = lecturerss.Name.ToString();
        //                Position1 = lecturerss.Position.ToString();
        //                Contact1 = lecturerss.Contact.ToString();
        //                Biography1 = lecturerss.Biography.ToString();
        //                ScientificInterests1 = lecturerss.ScientificInterests.ToString();
        //                AcademicStatus1 = lecturerss.AcademicStatus.ToString();
        //                Link1 = lecturerss.Link.ToString();
        //                Hyperlink1 = lecturerss.Hyperlink.ToString();

        //                if (string.IsNullOrEmpty(Name1))
        //                {
        //                    Name1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Position1))
        //                {
        //                    Position1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Contact1))
        //                {
        //                    Contact1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Biography1))
        //                {
        //                    Biography1 = "";
        //                }
        //                if (string.IsNullOrEmpty(ScientificInterests1))
        //                {
        //                    ScientificInterests1 = "";
        //                }
        //                if (string.IsNullOrEmpty(AcademicStatus1))
        //                {
        //                    AcademicStatus1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Link1))
        //                {
        //                    Link1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Hyperlink1))
        //                {
        //                    Hyperlink1 = "";
        //                }


        //                lecturers_rows_isupper.Add(new string[] { lecturerss.FacultyName.ToString(), lecturerss.FacultyLanguage.ToString(), Name1,
        //                    Position1,AcademicStatus1, Contact1, Biography1, ScientificInterests1,Link1,lecturerss.HyperlinkNumber.ToString(),Hyperlink1
        //                });
        //            }
        //            worksheet.Cells[header_under].LoadFromArrays(header_lecturers_rows_up);
        //            worksheet.Cells[header_isunder].LoadFromArrays(lecturers_rows_isupper);
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //            lecturers_rows_isupper.Clear();
        //        }

        //        //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Викладачі (окремо).xlsx");
        //        //excel.SaveAs(excelFile);
        //        memoryStream = new MemoryStream(excel.GetAsByteArray());
        //    }
        //    return memoryStream;
        //}


        public static MemoryStream WriteExcelLecturers1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<Lecturers> lecturers;
            List<Publications> lecturers_publications;
            List<LecturersMark> lecturers_marks;

            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                lecturers = db.Lecturers.Where(fn => fn.DateId == date.DateId).ToList();
                lecturers_publications = (from lecturer in lecturers
                                          join lecturerPublication in db.Publications on lecturer.PublicationId equals lecturerPublication.PublicationId
                                          select lecturerPublication).ToList();
                lecturers_marks = (from lecturer in lecturers
                                   join lecturerMark in db.LecturersMark on lecturer.LecturerId equals lecturerMark.LecturerId
                                   select lecturerMark).ToList();
            }


            var lecturersLeftJoinFaculty = from faculty in faculties
                                           join lecturer in lecturers on faculty.FacultyId equals lecturer.FacultyId into facultyNews
                                           from subset in facultyNews.DefaultIfEmpty()
                                           select new
                                           {
                                               FacultyLink = faculty.FacultyLink,
                                               FacultyName = faculty.FacultyName,
                                               FacultyLanguage = faculty.FacultyLanguage,
                                               LecturerId = subset?.LecturerId ?? 0,
                                               Name = subset?.Name ?? new string(""),
                                               Position = subset?.Position ?? new string(""),
                                               Contact = subset?.Contact ?? new string(""),
                                               Biography = subset?.Biography ?? new string(""),
                                               ScientificInterests = subset?.ScientificInterests ?? new string(""),
                                               AcademicStatus = subset?.AcademicStatus ?? new string(""),
                                               Link = subset?.Link ?? new string(""),
                                               FacultyId = faculty.FacultyId,
                                               PublicationId = subset?.PublicationId ?? 0
                                           };

            var lecturersLeftJoinPublications = from facultyLecturer in lecturersLeftJoinFaculty
                                                join lecturerMark in lecturers_publications on facultyLecturer.PublicationId equals lecturerMark.PublicationId into facultyLecturers
                                                from subset in facultyLecturers.DefaultIfEmpty()
                                                select new
                                                {
                                                    FacultyId = facultyLecturer.FacultyId,
                                                    FacultyLink = facultyLecturer.FacultyLink,
                                                    FacultyName = facultyLecturer.FacultyName,
                                                    FacultyLanguage = facultyLecturer.FacultyLanguage,
                                                    LecturerId = facultyLecturer.LecturerId,
                                                    Name = facultyLecturer.Name ?? new string(""),
                                                    Position = facultyLecturer.Position ?? new string(""),
                                                    Contact = facultyLecturer.Contact ?? new string(""),
                                                    Biography = facultyLecturer.Biography ?? new string(""),
                                                    ScientificInterests = facultyLecturer.ScientificInterests ?? new string(""),
                                                    AcademicStatus = facultyLecturer.AcademicStatus ?? new string(""),
                                                    Link = facultyLecturer.Link ?? new string(""),
                                                    HyperlinkNumber = subset?.HyperlinkNumber ?? new int(),
                                                    Hyperlink = subset?.Hyperlink ?? new string("")
                                                };

            var lecturersLeftJoinMarks = from facultyLecturerWithMarks in lecturersLeftJoinPublications
                                         join lecturerMarks in lecturers_marks on facultyLecturerWithMarks.LecturerId equals lecturerMarks.LecturerId into facultyLecturers
                                                from subset in facultyLecturers.DefaultIfEmpty()
                                                select new
                                                {
                                                    FacultyId = facultyLecturerWithMarks.FacultyId,
                                                    FacultyLink = facultyLecturerWithMarks.FacultyLink,
                                                    FacultyName = facultyLecturerWithMarks.FacultyName,
                                                    FacultyLanguage = facultyLecturerWithMarks.FacultyLanguage,
                                                    LecturerId = facultyLecturerWithMarks.LecturerId,
                                                    Name = facultyLecturerWithMarks.Name ?? new string(""),
                                                    Position = facultyLecturerWithMarks.Position ?? new string(""),
                                                    Contact = facultyLecturerWithMarks.Contact ?? new string(""),
                                                    Biography = facultyLecturerWithMarks.Biography ?? new string(""),
                                                    ScientificInterests = facultyLecturerWithMarks.ScientificInterests ?? new string(""),
                                                    AcademicStatus = facultyLecturerWithMarks.AcademicStatus ?? new string(""),
                                                    Link = facultyLecturerWithMarks.Link ?? new string(""),
                                                    HyperlinkNumber = facultyLecturerWithMarks.HyperlinkNumber,
                                                    Hyperlink = facultyLecturerWithMarks.Hyperlink ?? new string(""),
                                                    NameMark = subset?.NameMark ?? new int(),
                                                    ContactMark = subset?.ContactMark ?? new int(),
                                                    BiographyMark = subset?.BiographyMark ?? new int(),
                                                    SchientificInterestsMark = subset?.SchientificInterestsMark ?? new int(),
                                                    PublicationMark = subset?.PublicationMark ?? new int(),
                                                    LecturerMark = subset?.LecturerMark ?? new int(),
                                                };



        var lecturersGroupByFaculties = lecturersLeftJoinMarks.GroupBy(dn => dn.FacultyName);
            string used = "";

            var lecturers_rows_isupper = new List<string[]>();
            var header_lecturers_rows_up = new List<object[]>()
            {
                  new object[] { "Факультет", "Мова сторінки", "Ім'я Прізвище По-батькові","Посада","Вчене звання", "Контакти", "Біографія","Науковий інтерес","Посилання","Кількість публікацій","Список публікацій"}

             };
            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach (var lecturersFacultyGroup in lecturersGroupByFaculties)
                {
                    used = lecturersFacultyGroup.Key;
                    if (used.Length > 25)

                    {
                        used = (lecturersFacultyGroup.Key).Substring(0, 25);
                    }
                    excel.Workbook.Worksheets.Add(used);

                    var worksheet = excel.Workbook.Worksheets["Викладачі"];

                    worksheet = excel.Workbook.Worksheets[used];
                    used = "";

                    int j = 1;
                    string header_under = "A" + j.ToString();
                    j = j + header_lecturers_rows_up.Count();

                    string header_isunder = "A" + j.ToString();
                    j = j + lecturers_rows_isupper.Count();

                    string Name1 = "";
                    string Position1 = "";
                    string Contact1 = "";
                    string Biography1 = "";
                    string ScientificInterests1 = "";
                    string AcademicStatus1 = "";
                    string Link1 = "";
                    string Hyperlink1 = "";

                    foreach (var lecturerss in lecturersFacultyGroup)
                    {
                        Name1 = lecturerss.Name.ToString();
                        Position1 = lecturerss.Position.ToString();
                        Contact1 = lecturerss.Contact.ToString();
                        Biography1 = lecturerss.Biography.ToString();
                        ScientificInterests1 = lecturerss.ScientificInterests.ToString();
                        AcademicStatus1 = lecturerss.AcademicStatus.ToString();
                        Link1 = lecturerss.Link.ToString();
                        Hyperlink1 = lecturerss.Hyperlink.ToString();
                        if (string.IsNullOrEmpty(Name1))
                        {
                            Name1 = "";
                        }
                        if (string.IsNullOrEmpty(Position1))
                        {
                            Position1 = "";
                        }
                        if (string.IsNullOrEmpty(Contact1))
                        {
                            Contact1 = "";
                        }
                        if (string.IsNullOrEmpty(Biography1))
                        {
                            Biography1 = "";
                        }
                        if (string.IsNullOrEmpty(ScientificInterests1))
                        {
                            ScientificInterests1 = "";
                        }
                        if (string.IsNullOrEmpty(AcademicStatus1))
                        {
                            AcademicStatus1 = "";
                        }
                        if (string.IsNullOrEmpty(Link1))
                        {
                            Link1 = "";
                        }
                        if (string.IsNullOrEmpty(Hyperlink1))
                        {
                            Hyperlink1 = "";
                        }

                        lecturers_rows_isupper.Add(new string[] { lecturerss.FacultyName.ToString(), lecturerss.FacultyLanguage.ToString(), Name1,
                            Position1,AcademicStatus1, Contact1, Biography1, ScientificInterests1,Link1,lecturerss.HyperlinkNumber.ToString(),Hyperlink1
                        });
                        lecturers_rows_isupper.Add(new string[] { "Оцінки за наповненість: ", " ", lecturerss.NameMark.ToString(), " ", " ",
                            lecturerss.ContactMark.ToString(), lecturerss.BiographyMark.ToString(), lecturerss.SchientificInterestsMark.ToString()," ", lecturerss.PublicationMark.ToString(), "Загальна оцінка: " + lecturerss.LecturerMark.ToString()
                        });
                    }
                    worksheet.Cells[header_under].LoadFromArrays(header_lecturers_rows_up);
                    worksheet.Cells[header_isunder].LoadFromArrays(lecturers_rows_isupper);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    lecturers_rows_isupper.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Викладачі (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelMaterials1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<Materials> materials;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                materials = db.Materials.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var facultiesLeftJoinMaterials = from faculty in faculties
                                             join dataMaterial in materials on faculty.FacultyId equals dataMaterial.FacultyId into facultyMaterial
                                             from subset in facultyMaterial.DefaultIfEmpty()
                                             select new
                                             {
                                                 FacultyId = faculty.FacultyId,
                                                 FacultyLink = faculty.FacultyLink,
                                                 FacultyName = faculty.FacultyName,
                                                 FacultyLanguage = faculty.FacultyLanguage,
                                                 DepartmentName = subset?.DepartmentName ?? new string(""),
                                                 MaterialsNumber = subset?.MaterialsNumber ?? new int(),
                                                 MaterialsLinks = subset?.MaterialsLinks ?? new string(""),
                                             };

            var materialsGroupByFaculties = facultiesLeftJoinMaterials.GroupBy(dn => dn.FacultyName);
            string used = "";
            var news_rows_isunder = new List<string[]>();
            var header_news_rows_under = new List<object[]>()
            {
                new object[] { "Назва кафедри", "Факультет", "Мова сторінки", "Кількість матеріалів", "Посилання на матеріали" }

            };

            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach (var materialsFacultyGroup in materialsGroupByFaculties)
                {
                    used = materialsFacultyGroup.Key;
                    if (used.Length > 22)

                    {
                        used = (materialsFacultyGroup.Key).Substring(0, 22);
                    }
                    excel.Workbook.Worksheets.Add(used);

                    var worksheet = excel.Workbook.Worksheets["Матеріали факультету"];

                    worksheet = excel.Workbook.Worksheets[used];
                    used = "";

                    int j = 1;
                    string header_under = "A" + j.ToString();
                    j = j + header_news_rows_under.Count();

                    string header_isunder = "A" + j.ToString();
                    j = j + news_rows_isunder.Count();


                    worksheet.Cells[header_under].LoadFromArrays(header_news_rows_under);

                    foreach (var facultyMaterials in materialsFacultyGroup)
                    {
                        news_rows_isunder.Add(new string[] { facultyMaterials.DepartmentName.ToString(), facultyMaterials.FacultyName.ToString(), facultyMaterials.FacultyLanguage, (facultyMaterials.MaterialsNumber).ToString(), facultyMaterials.MaterialsLinks.ToString() });
                    }

                    worksheet.Cells[header_isunder].LoadFromArrays(news_rows_isunder);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    news_rows_isunder.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Новини факультетів (окремо).xlsx");
                //excel.SaveAs(excelFile);

                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }


        //public static MemoryStream WriteExcelCourses1(MarkingDate date)
        //{
        //    var memoryStream = new MemoryStream();
        //    List<Faculties> faculties;
        //    List<Courses> date_courses;
        //    using (university_projectContext db = new university_projectContext())
        //    {
        //        faculties = db.Faculties.ToList();
        //        date_courses = db.Courses.Where(fn => fn.DateId == date.DateId).ToList();
        //    }


        //    var facultiesLeftJoinFaculty = from faculty in faculties
        //                                   join dateNews in date_courses on faculty.FacultyId equals dateNews.FacultyId into facultyNews
        //                                   from subset in facultyNews.DefaultIfEmpty()
        //                                   select new
        //                                   {
        //                                       FacultyId = faculty.FacultyId,
        //                                       FacultyLink = faculty.FacultyLink,
        //                                       FacultyName = faculty.FacultyName,
        //                                       FacultyLanguage = faculty.FacultyLanguage,
        //                                       CourseLink = subset?.CourseLink ?? new string(""),
        //                                       CourseDescription = subset?.CourseDescription ?? new string(""),
        //                                       Literature = subset?.Literature ?? new string(""),
        //                                       Materials = subset?.Materials ?? new string(""),
        //                                       Plan = subset?.Plan ?? new string("")
        //                                   };

        //    var coursesGroupByFaculties = facultiesLeftJoinFaculty.GroupBy(dn => dn.FacultyName);
        //    string used = "";
        //    var lecturers_rows_isupper = new List<string[]>();
        //    var header_lecturers_rows_up = new List<object[]>()
        //    {
        //          new object[] { "Факультет", "Мова сторінки", "Посилання на курс","Опис курсу","Рекомендована література", "Матеріали", "Навчальний план" }

        //     };
        //    using (ExcelPackage excel = new ExcelPackage())
        //    {
        //        foreach (var coursesFacultyGroup in coursesGroupByFaculties)
        //        {
        //            used = coursesFacultyGroup.Key;
        //            if (used.Length > 25)

        //            {
        //                used = (coursesFacultyGroup.Key).Substring(0, 25);
        //            }
        //            excel.Workbook.Worksheets.Add(used);

        //            var worksheet = excel.Workbook.Worksheets["Викладачі"];

        //            worksheet = excel.Workbook.Worksheets[used];
        //            used = "";

        //            int j = 1;
        //            string header_under = "A" + j.ToString();
        //            j = j + header_lecturers_rows_up.Count();

        //            string header_isunder = "A" + j.ToString();
        //            j = j + lecturers_rows_isupper.Count();

        //            string CourseLink1 = "";
        //            string CourseDescription1 = "";
        //            string Literature1 = "";
        //            string Materials1 = "";
        //            string Plan1 = "";

        //            foreach (var coursess in coursesFacultyGroup)
        //            {
        //                CourseLink1 = coursess.CourseLink.ToString();
        //                CourseDescription1 = coursess.CourseDescription.ToString();
        //                Literature1 = coursess.Literature.ToString();
        //                Materials1 = coursess.Materials.ToString();
        //                Plan1 = coursess.Plan.ToString();


        //                if (string.IsNullOrEmpty(CourseLink1))
        //                {
        //                    CourseLink1 = "";
        //                }
        //                if (string.IsNullOrEmpty(CourseDescription1))
        //                {
        //                    CourseDescription1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Literature1))
        //                {
        //                    Literature1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Materials1))
        //                {
        //                    Materials1 = "";
        //                }
        //                if (string.IsNullOrEmpty(Plan1))
        //                {
        //                    Plan1 = "";
        //                }


        //                lecturers_rows_isupper.Add(new string[] { coursess.FacultyName.ToString(), coursess.FacultyLanguage.ToString(), CourseLink1,
        //                    CourseDescription1,Literature1, Materials1, Plan1
        //                });
        //            }
        //            worksheet.Cells[header_under].LoadFromArrays(header_lecturers_rows_up);
        //            worksheet.Cells[header_isunder].LoadFromArrays(lecturers_rows_isupper);
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //            lecturers_rows_isupper.Clear();
        //        }

        //        //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Викладачі (окремо).xlsx");
        //        //excel.SaveAs(excelFile);
        //        memoryStream = new MemoryStream(excel.GetAsByteArray());
        //    }
        //    return memoryStream;
        //}

        public static MemoryStream WriteExcelCourses1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<Courses> date_courses;
            List<CoursesMark> dateCoursesWithMark;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                date_courses = db.Courses.Where(fn => fn.DateId == date.DateId).ToList();

                dateCoursesWithMark = (from course in date_courses
                                       join courseMark in db.CoursesMark on course.CourseId equals courseMark.CourseId
                                       select courseMark).ToList();
            }



            var facultiesLeftJoinFaculty = from faculty in faculties
                                           join dateNews in date_courses on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                           from subset in facultyNews.DefaultIfEmpty()
                                           select new
                                           {
                                               FacultyId = faculty.FacultyId,
                                               FacultyLink = faculty.FacultyLink,
                                               FacultyName = faculty.FacultyName,
                                               FacultyLanguage = faculty.FacultyLanguage,
                                               CourseId = subset?.CourseId ?? new int(),
                                               CourseLink = subset?.CourseLink ?? new string(""),
                                               CourseDescription = subset?.CourseDescription ?? new string(""),
                                               Materials = subset?.Materials ?? new string(""),
                                               Plan = subset?.Plan ?? new string("")
                                           };

            var CourseWithMarks = from courseswithMark in facultiesLeftJoinFaculty
                                  join dateCourse in dateCoursesWithMark on courseswithMark.CourseId equals dateCourse.CourseId into facultyCourse
                                  from subset in facultyCourse.DefaultIfEmpty()
                                  select new
                                  {
                                      FacultyId = courseswithMark.FacultyId,
                                      FacultyLink = courseswithMark.FacultyLink,
                                      FacultyName = courseswithMark.FacultyName,
                                      FacultyLanguage = courseswithMark.FacultyLanguage,
                                      CourseId = subset?.CourseId ?? new int(),
                                      CourseLink = courseswithMark.CourseLink ?? new string(""),
                                      CourseDescription = courseswithMark.CourseDescription ?? new string(""),
                                      Materials = courseswithMark.Materials ?? new string(""),
                                      Plan = courseswithMark.Plan ?? new string(""),
                                      PlanMark = subset?.PlanMark ?? new int(),
                                      MaterialsMark = subset?.MaterialsMark ?? new int(),
                                      DescriptionMark = subset?.DescriptionMark ?? new int(),
                                      CourseMark = subset?.CourseMark ?? new int(),
                                  };


            var coursesGroupByFaculties = CourseWithMarks.GroupBy(dn => dn.FacultyName);
            string used = "";
            var lecturers_rows_isupper = new List<string[]>();
            var header_lecturers_rows_up = new List<object[]>()
            {
                  new object[] { "Факультет", "Мова сторінки", "Посилання на курс","Опис курсу","Рекомендована література", "Навчальний план", "Загальна оцінка" }

             };
            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach (var coursesFacultyGroup in coursesGroupByFaculties)
                {
                    used = coursesFacultyGroup.Key;
                    if (used.Length > 25)

                    {
                        used = (coursesFacultyGroup.Key).Substring(0, 25);
                    }
                    excel.Workbook.Worksheets.Add(used);

                    var worksheet = excel.Workbook.Worksheets["Викладачі"];

                    worksheet = excel.Workbook.Worksheets[used];
                    used = "";

                    int j = 1;
                    string header_under = "A" + j.ToString();
                    j = j + header_lecturers_rows_up.Count();

                    string header_isunder = "A" + j.ToString();
                    j = j + lecturers_rows_isupper.Count();

                    string CourseLink1 = "";
                    string CourseDescription1 = "";
                    string Materials1 = "";
                    string Plan1 = "";

                    foreach (var coursess in coursesFacultyGroup)
                    {
                        CourseLink1 = coursess.CourseLink.ToString();
                        CourseDescription1 = coursess.CourseDescription.ToString();
                        Materials1 = coursess.Materials.ToString();
                        Plan1 = coursess.Plan.ToString();


                        if (string.IsNullOrEmpty(CourseLink1))
                        {
                            CourseLink1 = "";
                        }
                        if (string.IsNullOrEmpty(CourseDescription1))
                        {
                            CourseDescription1 = "";
                        }

                        if (string.IsNullOrEmpty(Materials1))
                        {
                            Materials1 = "";
                        }
                        if (string.IsNullOrEmpty(Plan1))
                        {
                            Plan1 = "";
                        }


                        lecturers_rows_isupper.Add(new string[] { coursess.FacultyName.ToString(), coursess.FacultyLanguage.ToString(), CourseLink1,
                            CourseDescription1, Materials1, Plan1
                        });

                        lecturers_rows_isupper.Add(new string[] { "Оцінка за наповненість" , " ", CourseLink1,

                            coursess.DescriptionMark.ToString(), coursess.MaterialsMark.ToString(), coursess.PlanMark.ToString(), "Загальна оцінка: " + coursess.CourseMark.ToString()
                        });
                    }
                    worksheet.Cells[header_under].LoadFromArrays(header_lecturers_rows_up);
                    worksheet.Cells[header_isunder].LoadFromArrays(lecturers_rows_isupper);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    lecturers_rows_isupper.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject__UNIVERSITY_\NetProject__UNIVERSITY_\excels\Викладачі (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }
    }

}
