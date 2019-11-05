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

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Новини факультетів (всі).xlsx");
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

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Новини кафедр (всі).xlsx");
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
                    news_rows_isunder.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Новини факультетів (окремо).xlsx");
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
                    news_rows_isunder.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Новини кафедр (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelLecturers2(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<Lecturers> lecturers;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                lecturers = db.Lecturers.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var lecturersLeftJoinNews = from faculty in faculties
                                        join dateNews in lecturers on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyLink = faculty.FacultyLink,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            Name = subset?.Name ?? new string(""),
                                            Position = subset?.Position ?? new string(""),
                                            Contact = subset?.Contact ?? new string(""),
                                            Biography = subset?.Biography ?? new string(""),
                                            ScientificInterests = subset?.ScientificInterests ?? new string(""),
                                            Link = subset?.Link ?? new string("")
                                        };

            var lecturersGroupByFaculties = lecturersLeftJoinNews.GroupBy(dn => dn.FacultyId);
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Викладачі");

                var header_lecturers_rows_up = new List<object[]>()
                {
                     new object[] { "Факультет", "Мова сторінки", "Ім'я Прізвище По-батькові","Посада", "Контакти", "Біографія","Науковий інтерес","Посилання"}

                };


                var lecturers_rows_isupper = new List<string[]>();
                foreach (var lecturersFacultyGroup in lecturersGroupByFaculties)
                {
                    string Name1 = "";
                    string Position1 = "";
                    string Contact1 = "";
                    string Biography1 = "";
                    string ScientificInterests1 = "";
                    string Link1 = "";

                    foreach (var lecturerss in lecturersFacultyGroup)
                    {
                        Name1 = lecturerss.Name.ToString();
                        Position1 = lecturerss.Position.ToString();
                        Contact1 = lecturerss.Contact.ToString();
                        Biography1 = lecturerss.Biography.ToString();
                        ScientificInterests1 = lecturerss.ScientificInterests.ToString();
                        Link1 = lecturerss.Link.ToString();

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
                        if (string.IsNullOrEmpty(Link1))
                        {
                            Link1 = "";
                        }
                        lecturers_rows_isupper.Add(new string[] { lecturerss.FacultyName.ToString(), lecturerss.FacultyLanguage.ToString(), Name1,
                            Position1, Contact1, Biography1, ScientificInterests1,Link1
                        });
                    }

                    if (lecturersFacultyGroup.First().FacultyLanguage.ToString() == "анг")
                    {
                        lecturers_rows_isupper.Add(new string[] { "" });
                    }
                }


                // Determine the header range (e.g. A1:D1)
                string header_up = "A1";
                int j = 1;
                j = j + header_lecturers_rows_up.Count();

                string header_under = "A" + j.ToString();
                j = j + lecturers_rows_isupper.Count();

                var worksheet1 = excel.Workbook.Worksheets["Викладачі"];

                //Load the summary data into the sheet, starting from cell A1. Print the column names on row 1
                worksheet1.Cells[header_up].LoadFromArrays(header_lecturers_rows_up);
                worksheet1.Cells[header_under].LoadFromArrays(lecturers_rows_isupper);


                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Викладачі (всі).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream WriteExcelLecturers1(MarkingDate date)
        {
            var memoryStream = new MemoryStream();
            List<Faculties> faculties;
            List<Lecturers> lecturers;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                lecturers = db.Lecturers.Where(fn => fn.DateId == date.DateId).ToList();
            }

            var lecturersLeftJoinNews = from faculty in faculties
                                        join dateNews in lecturers on faculty.FacultyId equals dateNews.FacultyId into facultyNews
                                        from subset in facultyNews.DefaultIfEmpty()
                                        select new
                                        {
                                            FacultyId = faculty.FacultyId,
                                            FacultyLink = faculty.FacultyLink,
                                            FacultyName = faculty.FacultyName,
                                            FacultyLanguage = faculty.FacultyLanguage,
                                            Name = subset?.Name ?? new string(""),
                                            Position = subset?.Position ?? new string(""),
                                            Contact = subset?.Contact ?? new string(""),
                                            Biography = subset?.Biography ?? new string(""),
                                            ScientificInterests = subset?.ScientificInterests ?? new string(""),
                                            Link = subset?.Link ?? new string("")
                                        };

            var lecturersGroupByFaculties = lecturersLeftJoinNews.GroupBy(dn => dn.FacultyName);
            string used = "";

            var lecturers_rows_isupper = new List<string[]>();
            var header_lecturers_rows_up = new List<object[]>()
            {
                  new object[] { "Факультет", "Мова сторінки", "Ім'я Прізвище По-батькові","Посада", "Контакти", "Біографія","Науковий інтерес","Посилання"}

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
                    string Link1 = "";

                    foreach (var lecturerss in lecturersFacultyGroup)
                    {
                        Name1 = lecturerss.Name.ToString();
                        Position1 = lecturerss.Position.ToString();
                        Contact1 = lecturerss.Contact.ToString();
                        Biography1 = lecturerss.Biography.ToString();
                        ScientificInterests1 = lecturerss.ScientificInterests.ToString();
                        Link1 = lecturerss.Link.ToString();

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
                        if (string.IsNullOrEmpty(Link1))
                        {
                            Link1 = "";
                        }

                        lecturers_rows_isupper.Add(new string[] { lecturerss.FacultyName.ToString(), lecturerss.FacultyLanguage.ToString(), Name1,
                            Position1, Contact1, Biography1, ScientificInterests1,Link1
                        });
                    }
                    worksheet.Cells[header_under].LoadFromArrays(header_lecturers_rows_up);
                    worksheet.Cells[header_isunder].LoadFromArrays(lecturers_rows_isupper);
                    lecturers_rows_isupper.Clear();
                }

                //FileInfo excelFile = new FileInfo(@"D:\3 курс 2 сем\Dot NET\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\Викладачі (окремо).xlsx");
                //excel.SaveAs(excelFile);
                memoryStream = new MemoryStream(excel.GetAsByteArray());
            }
            return memoryStream;
        }
    }

}
