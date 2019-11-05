using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetProject__UNIVERSITY_.Models;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NetProject_UNIVERSITY_.Models;


namespace NetProject__UNIVERSITY_.Controllers
{
    public class HomeController : Controller
    {
        public void dump_news_info_excel(string link, List<ArticleCriteria> articleList)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");
                excel.Workbook.Worksheets.Add("Worksheet2");
                excel.Workbook.Worksheets.Add("Worksheet3");

                var headerRow = new List<string[]>() { new string[] { "ID", "First Name", "Last Name", "DOB" } };

                // Determine the header range (e.g. A1:D1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets["Worksheet1"];

                // Popular header row data
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                FileInfo excelFile = new FileInfo(@"C:\Users\Admin\Desktop\ADO.NET\NetProject-University-master\.NetProject-University-master\NetProject( UNIVERSITY)\NetProject( UNIVERSITY)\excels\test.xlsx");
                excel.SaveAs(excelFile);
            }
        }

        public IActionResult Index()
        {
            //var html = @"http://ami.lnu.edu.ua/news";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            //var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
            //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
            //var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//article//div[@class='excerpt']//a[@class='read-more']").First();

            //var html = @"http://electronics.lnu.edu.ua//news";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            //var node = htmlDoc.DocumentNode.SelectSingleNode("//aside//section[@class='widget widget-taxonomy-list']");


            //var department = new ArticleDepartmentCriteria();
            //var filters = department.ExtractDepartmentsFromFilter(htmlDoc.DocumentNode);
            //htmlDoc = web.Load(@"http://electronics.lnu.edu.ua/about/departments");
            //var departments = department.ExtractAllDepartments(htmlDoc.DocumentNode);
            //var departmentFilter = department.LinkDeparmentsToFilters(departments, filters);
            //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
            //var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//article//div[@class='excerpt']//a[@class='read-more']").First();


            //ArticleCriteria test = new ArticleCriteria(null, 0, "");
            //test.MainFunction(DateTime.Parse(DateTime.Today.AddDays(-180).ToString(), CultureInfo.CreateSpecificCulture("fr-FR")));

            //ArticleDepartmentCriteria test = new ArticleDepartmentCriteria();
            //test.MainFunction();

            //var html = @"http://financial.lnu.edu.ua/employee/stasyshyn-a-v";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            ////var result = LecturerPageCriteria.GetAllInfoRows(htmlDoc.DocumentNode);
            ////var result1 = LecturerPageCriteria.GetAllSections(htmlDoc.DocumentNode);
            //var nmae = LecturerPageCriteria.GetLecturerName(htmlDoc.DocumentNode);
            //var test = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='label']").InnerText.ToLower();


            //var html = @"http://ami.lnu.edu.ua/employee/dyyak";

            // HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);

            ////var nmae = LecturerPageCriteria.GetLecturerName(htmlDoc.DocumentNode);
            //var test = htmlDoc.DocumentNode.SelectNodes("//span[@class='value']");
            //var test = new LecturerPageCriteria();
            //test.MainFunction();

            //var html = @"http://physics.lnu.edu.ua/employee/yakibchuk";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            //var test = htmlDoc.DocumentNode.SelectNodes("//section[@class='section']");


            //var dsdsd = @"http://ami.lnu.edu.ua/employee/dyyak";
            //HtmlWeb ttt = new HtmlWeb();
            //var aa = ttt.Load(dsdsd);
            //var dsada = aa.DocumentNode.SelectNodes("//section[@class='section']");

            //var dsdsd = @"http://journ.lnu.edu.ua/employee/krupskyj-i-v";
            //HtmlWeb ttt = new HtmlWeb();
            //var aa = ttt.Load(dsdsd);
            //var dsada = aa.DocumentNode.SelectNodes("//section[@class='section']");


            //var Main = new MainClass();
            //var Date = new MarkingDate();
            //using (university_projectContext db = new university_projectContext())
            //{
            //    Date = db.MarkingDate.Where(md => md.DateId == 1).First();
            //}
            ////Main.MainLecturers(Date);
            //var DataAnalyser = new DataAnalyser();
            //DataAnalyser.AnalyseLecturers(Date);

            //DataWriterExcels.WriteExcelFacultyNews1(Date);

            //var dsdsd = @"http://ami.lnu.edu.ua/department/discrete-analysis-intelligent-system";
            //HtmlWeb ttt = new HtmlWeb();
            //var aa = ttt.Load(dsdsd);
            //var divElement = aa.DocumentNode.SelectSingleNode("//section[@class='materials']/div");
            //var divChildren = divElement.ChildNodes;

            //var el = new MaterialsCriteria();
            //el.MainFunction();

            //var html = @"http://geography.lnu.edu.ua/en/news/page/2";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            //if (html.Equals(web.ResponseUri))
            //{
            //    var result1 = LecturerPageCriteria.GetAllInfoRows(htmlDoc.DocumentNode);
            //}
            //var result = LecturerPageCriteria.GetAllInfoRows(htmlDoc.DocumentNode);
            ////var result1 = LecturerPageCriteria.GetAllSections(htmlDoc.DocumentNode);


            var html = @"http://ami.lnu.edu.ua/employee/dyyak";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var publicationNode = htmlDoc.DocumentNode.SelectNodes("//section[@class='section']").
                Where(n => DataCollector.Contains(LecturerPageCriteria.GetLowerLabelFromSection(n), new string[] { "публікації", "publication" })).First();
            var xpath = publicationNode.XPath;
            var publications = publicationNode.SelectNodes(publicationNode.XPath +"//li");
            var hyperlinks = publicationNode.ChildNodes;
                var test = 1;

            return View();
        }

        //  [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Details()
        {
            var markingDates = new List<MarkingDate>();
            using (university_projectContext db = new university_projectContext())
            {
                markingDates = db.MarkingDate.ToList();
            }
            if (markingDates != null)
            {
                ViewBag.data = markingDates;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Details(MarkingDate markingDate)
        {
            //marking dates
            var markingDates = new List<MarkingDate>();
            using (university_projectContext db = new university_projectContext())
            {
                markingDates = db.MarkingDate.ToList();
            }
            if (markingDates != null)
            {
                ViewBag.data = markingDates;
            }

            return View(markingDate);
        }

        public FileStreamResult GetFacultyNews(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelFacultyNews1(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetFacultyNewsbyFaculties(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelFacultyNews1(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetFacultyNewsall(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelFacultyNews2(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetDepartmentNewsbyFaculties(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelDepartmentNews1(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetDepartmentNewsall(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelDepartmentNews2(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetLecturersbyFaculties(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelLecturers1(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileStreamResult GetLecturersall(MarkingDate markingDate)
        {
            var memoryStream = DataWriterExcels.WriteExcelLecturers2(markingDate);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }



        [HttpGet]
        public IActionResult Result()
        {
            var markingDates = new List<MarkingDate>();
            using (university_projectContext db = new university_projectContext())
            {
                markingDates = db.MarkingDate.ToList();
            }
            if (markingDates != null)
            {
                ViewBag.data = markingDates;
            }

            return View("Results");
        }
        [HttpPost]
        public IActionResult Result(MarkingDate markingDate)
        {
            //marking dates
            var markingDates = new List<MarkingDate>();
            using (university_projectContext db = new university_projectContext())
            {
                markingDates = db.MarkingDate.ToList();
            }
            if (markingDates != null)
            {
                ViewBag.data = markingDates;
            }

            List<Faculties> faculties;
            List<CriteriaMark> criteriaMarks;
            List<Criterias> criterias;
            using (university_projectContext db = new university_projectContext())
            {
                faculties = db.Faculties.ToList();
                criteriaMarks = db.CriteriaMark.Where(fn => fn.DateId == markingDate.DateId).ToList();
                criterias = db.Criterias.ToList();
            }

            var facultiesMarks = from faculty in faculties
                                 join criteriaMark in criteriaMarks on faculty.FacultyId equals criteriaMark.FacultyId
                                 join criteria in criterias on criteriaMark.CriteriaId equals criteria.CriteriaId
                                 select new
                                 {
                                     faculty,
                                     criteriaMark,
                                     criteria
                                 };

            var marksGroupByFaculties = facultiesMarks.GroupBy(fm => fm.faculty.FacultyName);

            List<ResultElement> results = new List<ResultElement>();
            foreach (var facultyGroup in marksGroupByFaculties)
            {
                results.Add(new ResultElement
                {
                    FacultyName = facultyGroup.Key,
                    CriteriaMarks = new List<Tuple<string, FacultyMark>>()
                });
                var criteriaGroupBy = facultyGroup.GroupBy(fg => fg.criteria.CriteriaName);

                decimal? ukrSum = 0, engSum = 0;
                foreach (var criteriaGroup in criteriaGroupBy)
                {
                    var facultyMark = new FacultyMark();

                    foreach (var facultyElement in criteriaGroup)
                    {
                        if (facultyElement.faculty.FacultyLanguage.Equals("укр"))
                        {
                            facultyMark.Ukr = facultyElement.criteriaMark.Mark;
                            ukrSum += facultyElement.criteriaMark.Mark;
                        }
                        else
                        {
                            facultyMark.Eng = facultyElement.criteriaMark.Mark;
                            engSum += facultyElement.criteriaMark.Mark;
                        }
                    }

                    facultyMark.Sum = facultyMark.Ukr + facultyMark.Eng;
                    results[results.Count - 1].CriteriaMarks.Add(Tuple.Create(criteriaGroup.Key, facultyMark));
                }

                results[results.Count - 1].CriteriaMarks.Add(Tuple.Create("Сумарно",
                                                       new FacultyMark { Ukr = ukrSum, Eng = engSum, Sum = ukrSum + engSum }));
            }

            if (results == null)
            {
                results = new List<ResultElement>();
            }

            return View("Results", results.OrderByDescending(r => r.CriteriaMarks.Last().Item2.Sum));
        }

        [HttpGet]
        public IActionResult Rating()
        {
            ViewBag.data = DateTime.Now;
            return View("Rating");
        }
        [HttpPost]
        public IActionResult Rating(MarkingDate markingDate)
        {

            using (university_projectContext db = new university_projectContext())
            {
                db.MarkingDate.Add(markingDate);
                db.SaveChanges();
                markingDate.DateId = db.MarkingDate.Where(md => md.Date == markingDate.Date).FirstOrDefault().DateId;
            }

            MainClass.MainCollecting(markingDate);
            DataAnalyser.MainAnalyse(markingDate);

            return View("Rating", markingDate);
        }


        //  [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpGet]
        public IActionResult Log_in()
        {
            return View("Login_page");
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View("Registration");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Test1()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

        public IActionResult BuildResults()
        {
            var markingDates = new List<MarkingDate>();
            using (university_projectContext db = new university_projectContext())
            {
                markingDates = db.MarkingDate.ToList();
            }
            if (markingDates != null)
            {
                ViewBag.data = markingDates;
            }
            return RedirectToAction("Login_page", markingDates);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
