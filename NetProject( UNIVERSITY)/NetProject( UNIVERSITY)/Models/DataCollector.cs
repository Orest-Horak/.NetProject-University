using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using NetProject__UNIVERSITY_.Models;

namespace NetProject__UNIVERSITY_.Models
{


    public class DataCollector
    {
        //public static List<Article> CollectArticles(Faculties faculty, DateTime fromDate)
        //{
        //    List<Article> articleList = new List<Article>();
        //    var link = faculty.FacultyLink + "news";

        //    bool keepGoing = true;
        //    int pageNumber = 1;
        //    while (keepGoing)
        //    {
        //        string pageLink;
        //        if (pageNumber == 1)
        //        {
        //            pageLink = link;
        //        }
        //        else
        //        {
        //            pageLink = link + "/page/" + pageNumber.ToString();
        //        }

        //        HtmlWeb web = new HtmlWeb();
        //        var res = web.Load(pageLink);

        //        if (res != null)
        //        {
        //            // list of raw html fragments <article>(.*?)</article>
        //            var articlesRaw = res.DocumentNode.SelectNodes("//article");
        //            if (articlesRaw != null)
        //            {
        //                foreach (var articleRaw in articlesRaw)
        //                {
        //                    var article = new Article(articleRaw, pageNumber, faculty);
        //                    if (article.Date.Equals(new DateTime()))
        //                    {
        //                        keepGoing = false;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            if (article.Date >= fromDate)
        //                            {
        //                                articleList.Add(article);
        //                            }
        //                            else
        //                            {
        //                                //stop
        //                                keepGoing = false;
        //                                break;
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            articleList.Add(article);
        //                        }
        //                    }

        //                }
        //                pageNumber += 1;
        //            }
        //            else
        //            {
        //                keepGoing = false;
        //            }
        //        }
        //        else
        //        {
        //            keepGoing = false;
        //        }

        //    }
        //    return articleList;
        //}

        //public static List<Article> CollectFacultyNews(Faculties faculty, DateTime fromDate)
        //{
        //    List<Article> facultyNews = CollectArticles(faculty, fromDate);
        //    return facultyNews;
        //}

        public static FacultyNews FacultyNewsGet(HtmlNode articleRaw, int pageNumber, Faculties faculty, MarkingDate date)
        {
            var facultyNews = new FacultyNews();
            facultyNews.PostingDate = new DateTime();
            facultyNews.Link = "no link";
            facultyNews.Page = pageNumber.ToString();
            //facultyNews.Faculty = faculty;
            facultyNews.FacultyId = faculty.FacultyId;
            //facultyNews.Date = date;
            facultyNews.DateId = date.DateId;
            try
            {
                HtmlNode dataNode = articleRaw.ChildNodes["header"].ChildNodes["div"];
                facultyNews.Link = articleRaw.SelectSingleNode("div[@class='excerpt']//a[@class='read-more']").Attributes["href"].Value.Trim();
                var DateFormat = dataNode.InnerText.Remove(11).Trim();
                facultyNews.PostingDate = DateTime.Parse(DateFormat);
            }
            catch (Exception e)
            {
                return facultyNews;
            }

            return facultyNews;
        }

        public static List<FacultyNews> CollectArticles(Faculties faculty, MarkingDate date, DateTime fromDate)
        {
            var articleList = new List<FacultyNews>();
            var link = faculty.FacultyLink + "news";

            bool keepGoing = true;
            int pageNumber = 1;
            while (keepGoing)
            {
                string pageLink;
                if (pageNumber == 1)
                {
                    pageLink = link;
                }
                else
                {
                    pageLink = link + "/page/" + pageNumber.ToString();
                }

                HtmlWeb web = new HtmlWeb();
                var res = new HtmlDocument();
                try
                {
                    res = web.Load(pageLink);
                }
                catch(Exception e)
                {
                    return articleList;
                }

                if (res != null && pageLink.Equals(web.ResponseUri.AbsoluteUri))
                {
                    // list of raw html fragments <article>(.*?)</article>
                    var articlesRaw = res.DocumentNode.SelectNodes("//article");
                    if (articlesRaw != null)
                    {
                        foreach (var articleRaw in articlesRaw)
                        {
                            var article = FacultyNewsGet(articleRaw, pageNumber, faculty, date);
                            if (article.PostingDate.Equals(new DateTime()))
                            {
                                keepGoing = false;
                                break;
                            }
                            else
                            {
                                try
                                {
                                    if (article.PostingDate >= fromDate)
                                    {
                                        articleList.Add(article);
                                    }
                                    else
                                    {
                                        //stop
                                        keepGoing = false;
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    articleList.Add(article);
                                }
                            }

                        }
                        pageNumber += 1;
                    }
                    else
                    {
                        keepGoing = false;
                    }
                }
                else
                {
                    keepGoing = false;
                }

            }
            return articleList;
        }

        public static List<FacultyNews> CollectFacultyNews(Faculties faculty, MarkingDate date, DateTime fromDate)
        {
            var facultyNews = CollectArticles(faculty, date, fromDate);
            return facultyNews;
        }


        public static IEnumerable<HtmlNode> GetFilters(HtmlNode articleRaw)
        {
            var filters = new List<HtmlNode>();
            try
            {
                filters = articleRaw.SelectNodes("//aside//section[@class='widget widget-taxonomy-list']//ul//li//a")
                                       .Distinct(new DepartmentEqualityComparer()).ToList();
            }
            catch { }
            return filters;
        }

        public static HtmlNodeCollection ExtractArticles(HtmlNode article, string articleAttribute)
        {
            string article_link = "//article";
            var articleNodes = article.SelectNodes(articleAttribute + article_link);
            return articleNodes;
        }

        public static HtmlNodeCollection ExtractAllDepartments(HtmlNode articleRaw)
        {
            var departmentNodes = articleRaw.SelectNodes("//section//div[@class='text']//h2//a");
            return departmentNodes;
        }

        public static Dictionary<HtmlNode, HtmlNode> LinkDeparmentsToFilters(HtmlNodeCollection departments, IEnumerable<HtmlNode> filters)
        {
            Dictionary<HtmlNode, HtmlNode> departmentFilter = new Dictionary<HtmlNode, HtmlNode>();
            if (departments != null)
            {
                foreach (var department in departments)
                {
                    try
                    {
                        var value = filters.First(f => f.InnerText.ToLower().Contains(department.InnerText.ToLower()));
                        departmentFilter[department] = value;
                    }
                    catch (Exception e)
                    {
                        departmentFilter[department] = null;
                    }
                }
            }
            return departmentFilter;
        }

        public static List<DepartmentNews> CollectDepartment(Faculties faculty, MarkingDate date)
        {
            var departmentNews = new List<DepartmentNews>();
            // посилання на списки кафедр, новин
            string departmentLink = faculty.FacultyLink + "/about/departments";
            string newsLink = faculty.FacultyLink + "/news";

            HtmlWeb web = new HtmlWeb();

            var mainNews = web.Load(newsLink).DocumentNode.SelectNodes("//article");

            var htmlDoc = web.Load(departmentLink);
            var departments = ExtractAllDepartments(htmlDoc.DocumentNode);

            htmlDoc = web.Load(newsLink);
            var filters = GetFilters(htmlDoc.DocumentNode);
            var departmentFilter = LinkDeparmentsToFilters(departments, filters);


            foreach (var department in departmentFilter.Keys)
            {
                var departmentArticles = ExtractArticles(web.Load(department.
                    Attributes["href"].Value).DocumentNode, "//body//div//div//section");
                HtmlNodeCollection filterArticles;
                try
                {
                    filterArticles = ExtractArticles(web.Load(departmentFilter[department].
                        Attributes["href"].Value).DocumentNode, "//body//div[@class='content news']");
                }
                catch (Exception e)
                {
                    filterArticles = null;
                }

                var departmentNew = new DepartmentNews();

                departmentNew.DepartmentName = department.InnerText;
                departmentNew.FacultyId = faculty.FacultyId;
                departmentNew.DateId = date.DateId;
                if (departmentArticles == null)
                {
                    departmentNew.DepartmentNewsNumber = 0;
                }
                else
                {
                    departmentNew.DepartmentNewsNumber = departmentArticles.Count;
                }

                if (filterArticles == null)
                {
                    departmentNew.FiltersNewsNumber = 0;
                }
                else
                {
                    departmentNew.FiltersNewsNumber = filterArticles.Count;
                }

                departmentNews.Add(departmentNew);

            }


            return departmentNews;

        }

        public static List<DepartmentNews> CollectDepartmentsNews(Faculties faculty, MarkingDate date)
        {
            var departmentNews = CollectDepartment(faculty, date);
            return departmentNews;
        }

        //Lecturers
        public static HtmlNodeCollection GetAllLecturerLinksNodes(HtmlNode staffpage)
        {
            var lecturersLinks = staffpage.SelectNodes("//td[@class='name']//a");
            return lecturersLinks;
        }

        public static List<string> GetAllLecturerLinks(HtmlNodeCollection linksNodes)
        {
            List<string> lecturersLinks = new List<string>();
            foreach (var aElement in linksNodes)
            {
                lecturersLinks.Add(aElement.Attributes["href"].Value);
            }

            return lecturersLinks;
        }

        public static HtmlNodeCollection GetAllInfoRows(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//div[@class='info']//p");
            return result;
        }

        public static HtmlNodeCollection GetAllSections(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//section[@class='section']");
            return result;
        }

        public static string GetLecturerName(HtmlNode lecturerPage)
        {
            string name = "";
            try
            {
                name = lecturerPage.SelectSingleNode("//h1[@class='page-title']").InnerText.Trim();
            }
            catch
            {
            }
            return name;
        }

        public static bool Contains(string label, string[] wordsToCheck)
        {
            for (int i = 0; i < wordsToCheck.Length; i++)
            {
                if (label.Contains(wordsToCheck[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetInfoFromRows(HtmlNodeCollection infoRows, Lecturers lecturer)
        {
            var wordsToCheck = new string[0];
            //copy
            var infoRowsClone = infoRows.ToList();
            //position
            if (infoRows != null)
            {
                wordsToCheck = new string[] { "посада", "position" };
                for (int i = 0; i < infoRowsClone.Count; i++)
                {
                    var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                    if (Contains(label, wordsToCheck))
                    {
                        lecturer.Position = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
                        infoRowsClone.RemoveAt(i);
                        break;
                    }
                }
            }
            //academic rank
            if (infoRows != null)
            {
                wordsToCheck = new string[] { "звання", "status" };
                for (int i = 0; i < infoRowsClone.Count; i++)
                {
                    var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                    if (Contains(label, wordsToCheck))
                    {
                        lecturer.AcademicStatus = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
                        infoRowsClone.RemoveAt(i);
                        break;
                    }
                }
            }
            //contacts
            if (infoRows != null)
            {
                wordsToCheck = new string[] { "профіль", "пошта", "profile", "email" };
                lecturer.Contact = "";
                for (int i = 0; i < infoRowsClone.Count; i++)
                {
                    var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                    if (Contains(label, wordsToCheck))
                    {
                        var contact = LecturerPageCriteria.GetValueLinkFromRow(infoRowsClone[i]);
                        lecturer.Contact = string.Concat(lecturer.Contact, " ", contact);
                        infoRowsClone.RemoveAt(i);
                    }
                }
            }
        }

        public static void SetInfoFromSections(HtmlNodeCollection sections, Lecturers lecturer)
        {
            var wordsToCheck = new string[0];
            //copy
            var sectionsClone = sections.ToList();
            //biography
            wordsToCheck = new string[] { "біографія", "biography" };
            for (int i = 0; i < sectionsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromSection(sectionsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    lecturer.Biography = LecturerPageCriteria.GetInnerTextFromDivInSection(sectionsClone[i]);
                    sectionsClone.RemoveAt(i);
                    break;
                }
            }
            //scientific interest
            wordsToCheck = new string[] { "наукові інтереси", "research interests" };
            for (int i = 0; i < sectionsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromSection(sectionsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    lecturer.ScientificInterests = LecturerPageCriteria.GetInnerTextFromDivInSection(sectionsClone[i]);
                    sectionsClone.RemoveAt(i);
                    break;
                }
            }
            //publications
            wordsToCheck = new string[] { "публікації", "publication" };
            for (int i = 0; i < sectionsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromSection(sectionsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    //publication creation
                    lecturer.Publication = GetPublication(sectionsClone[i]);
                    //publication
                    sectionsClone.RemoveAt(i);
                    break;
                }
            }

        }

        private static Publications GetPublication(HtmlNode publicationNode)
        {
            Publications publication = new Publications();
            //publication text
            publication.Publication = publicationNode.InnerHtml;
            //publications
            
            return publication;
        }

        public static List<Lecturers> CollectLecturer(Faculties faculty, MarkingDate date)
        {
            var lecturers = new List<Lecturers>();
            // посилання на персонал факультетів
            string staffLink = faculty.FacultyLink + "/about/staff";

            HtmlWeb web = new HtmlWeb();
            var linksNodes = GetAllLecturerLinksNodes(web.Load(staffLink).DocumentNode);
            //отримання посилань на всіх працівників факультету
            var lecturerlinks = new List<string>();

            if (linksNodes != null)
            {
                lecturerlinks = GetAllLecturerLinks(linksNodes);
            }
            else
            {
                lecturerlinks = null;
            }

            if (lecturerlinks != null)
            {
                foreach (var lecturerLink in lecturerlinks)
                {
                    var lecturer = new Lecturers();
                    lecturer.Link = lecturerLink;
                    lecturer.FacultyId = faculty.FacultyId;
                    lecturer.DateId = date.DateId;
                    try
                    {
                        var lecturerPage = web.Load(lecturerLink).DocumentNode;
                        // Name, Position, Contact, AcademicStatus collecting
                        if (lecturerPage != null)
                        {
                            var infoRows = GetAllInfoRows(lecturerPage);
                            lecturer.Name = GetLecturerName(lecturerPage);
                            SetInfoFromRows(infoRows, lecturer);
                            // Biography, ScientificInterests collecting
                            var sections = GetAllSections(lecturerPage);
                            SetInfoFromSections(sections, lecturer);
                        }
                    }
                    catch
                    {
                    }

                    lecturers.Add(lecturer);
                }

            }

            return lecturers;
        }

        public static List<Lecturers> CollectLecturers(Faculties faculty, MarkingDate date)
        {
            var lecturers = CollectLecturer(faculty, date);
            return lecturers;
        }

    }
}
