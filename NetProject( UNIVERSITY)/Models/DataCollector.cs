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
using System.Text.RegularExpressions;

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

        public static FacultyNews GetFacultyNews(HtmlNode articleNode, int pageNumber, Faculties faculty, MarkingDate date)
        {
            var facultyNews = new FacultyNews
            {
                PostingDate = new DateTime(),
                Link = null,
                Page = pageNumber.ToString(),
                FacultyId = faculty.FacultyId,
                DateId = date.DateId
            };

            try
            {
                var dateNode = articleNode.ChildNodes["header"].ChildNodes["div"];
                //find a way to avoid a hardcode
                var dateString = dateNode.InnerText.Remove(11).Trim();
                //find a way to avoid a hardcode
                facultyNews.PostingDate = DateTime.Parse(dateString);
                facultyNews.Link = articleNode.SelectSingleNode(articleNode.XPath + "//div[@class='excerpt']//a[@class='read-more']").Attributes["href"].Value;
            }
            catch (Exception e)
            {
                return facultyNews;
            }

            return facultyNews;
        }

        //public static List<FacultyNews> CollectArticles(Faculties faculty, MarkingDate date, DateTime fromDate)
        //{
        //    var articleList = new List<FacultyNews>();
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
        //        var res = new HtmlDocument();
        //        try
        //        {
        //            res = web.Load(pageLink);
        //        }
        //        catch (Exception e)
        //        {
        //            return articleList;
        //        }

        //        if (res != null && pageLink.Equals(web.ResponseUri.AbsoluteUri))
        //        {
        //            // list of raw html fragments <article>(.*?)</article>
        //            var articlesRaw = res.DocumentNode.SelectNodes("//article");
        //            if (articlesRaw != null)
        //            {
        //                foreach (var articleRaw in articlesRaw)
        //                {
        //                    var article = GetFacultyNews(articleRaw, pageNumber, faculty, date);
        //                    if (article.PostingDate.Equals(new DateTime()))
        //                    {
        //                        keepGoing = false;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            if (article.PostingDate >= fromDate)
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

        public static List<FacultyNews> CollectFacultyNews(Faculties faculty, MarkingDate date, DateTime fromDate)
        {
            var facultyNews = new List<FacultyNews>();
            var newsLink = faculty.FacultyLink + "news";

            bool keepGoing = true;
            int pageNumber = 1;

            while (keepGoing)
            {
                string pageLink;
                if (pageNumber == 1)
                {
                    pageLink = newsLink;
                }
                else
                {
                    pageLink = newsLink + "/page/" + pageNumber.ToString();
                }

                HtmlWeb web = new HtmlWeb();
                HtmlDocument newsPage;
                try
                {
                    newsPage = web.Load(pageLink);
                }
                catch (Exception e)
                {
                    return facultyNews;
                }

                //if page is empty or not exist - end collecting
                if (newsPage != null && pageLink.Equals(web.ResponseUri.AbsoluteUri))
                {
                    // list of article html nodes <article>(.*?)</article>
                    var articlesNodes = newsPage.DocumentNode.SelectNodes("//article");
                    //if no articles - end collecting
                    if (articlesNodes != null)
                    {
                        var defaultDateTime = new DateTime();

                        foreach (var articleNode in articlesNodes)
                        {
                            var article = GetFacultyNews(articleNode, pageNumber, faculty, date);
                            if (article.PostingDate.Equals(defaultDateTime))
                            {
                                keepGoing = false;
                                break;
                            }
                            else
                            {
                                if (article.PostingDate >= fromDate)
                                {
                                    facultyNews.Add(article);
                                }
                                else
                                {
                                    //stop
                                    keepGoing = false;
                                    break;
                                }
                            }
                        }

                        pageNumber++;
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
            var result = lecturerPage.SelectNodes("//div[@class='info']/p");
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

        //public static void SetInfoFromRows(HtmlNodeCollection infoRows, Lecturers lecturer)
        //{
        //    string[] wordsToCheck;

        //    //copy
        //    var infoRowsClone = infoRows.ToList();

        //    //position
        //    if (infoRows != null)
        //    {
        //        wordsToCheck = new string[] { "посада", "position" };
        //        for (int i = 0; i < infoRowsClone.Count; i++)
        //        {
        //            var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

        //            if (Contains(label, wordsToCheck))
        //            {
        //                lecturer.Position = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
        //                infoRowsClone.RemoveAt(i);
        //                break;
        //            }
        //        }
        //    }
        //    //academic rank
        //    if (infoRows != null)
        //    {
        //        wordsToCheck = new string[] { "звання", "status" };
        //        for (int i = 0; i < infoRowsClone.Count; i++)
        //        {
        //            var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

        //            if (Contains(label, wordsToCheck))
        //            {
        //                lecturer.AcademicStatus = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
        //                infoRowsClone.RemoveAt(i);
        //                break;
        //            }
        //        }
        //    }
        //    //contacts
        //    if (infoRows != null)
        //    {
        //        wordsToCheck = new string[] { "профіль", "пошта", "profile", "email" };
        //        lecturer.Contact = "";
        //        for (int i = 0; i < infoRowsClone.Count; i++)
        //        {
        //            var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

        //            if (Contains(label, wordsToCheck))
        //            {
        //                var contact = LecturerPageCriteria.GetValueLinkFromRow(infoRowsClone[i]);
        //                lecturer.Contact = string.Concat(lecturer.Contact, " ", contact);
        //                infoRowsClone.RemoveAt(i);
        //            }
        //        }
        //    }
        //}

        public static void SetInfoFromRows(HtmlNodeCollection infoRows, Lecturers lecturer)
        {
            string[] wordsToCheck;

            if (infoRows != null)
            {
                //copy
                var infoRowsClone = infoRows.ToList();
                //position
                if (infoRowsClone.Any())
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
                if (infoRowsClone.Any())
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
                if (infoRowsClone.Any())
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

        }

        public static void SetInfoFromSections(HtmlNodeCollection sections, Lecturers lecturer)
        {
            string[] wordsToCheck;
            //copy
            if (sections != null)
            {
                var sectionsClone = sections.ToList();
                //biography
                if (sectionsClone.Any())
                {
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
                }
                //scientific interest
                if (sectionsClone.Any())
                {
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
                }
                //publications
                if (sectionsClone.Any())
                {
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
            }
        }

        private static int CountSubString(string str, string subString)
        {
            int count = 0;
            if (subString.Length > str.Length)
            {
                return 0;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == subString[0])
                {
                }
            }

            return count;
        }

        private static Publications GetPublication(HtmlNode publicationNode)
        {
            Publications publication = new Publications();
            if (publicationNode.InnerHtml != null)
            {
                //publication text
                publication.Publication = publicationNode.InnerHtml;
                //publications number
                publication.PublicationNumber = Regex.Matches(publication.Publication, "<li>").Count;
                //hyperlink
                //ВИПРАВИТИ
                var hrefMatches = Regex.Matches(publicationNode.InnerHtml, "href=\\\"(.*?)\"");
                if (hrefMatches != null && hrefMatches.Any())
                {
                    //var hyperlinks = hrefMatches.Select(hp => hp.Groups[1].Value).Where(hp => hp.EndsWith(".pdf") || hp.EndsWith(".doc"));
                    var hyperlinks = hrefMatches.Select(hp => hp.Groups[1].Value);
                    publication.HyperlinkNumber = hyperlinks.Count();
                    publication.Hyperlink = string.Join("\r\n", hyperlinks);
                }
            }
            return publication;
        }

        //public static List<Lecturers> CollectLecturer(Faculties faculty, MarkingDate date)
        //{
        //    var lecturers = new List<Lecturers>();
        //    // посилання на персонал факультетів
        //    string staffLink = faculty.FacultyLink + "/about/staff";

        //    HtmlWeb web = new HtmlWeb();
        //    var linksNodes = GetAllLecturerLinksNodes(web.Load(staffLink).DocumentNode);
        //    //отримання посилань на всіх працівників факультету
        //    var lecturerlinks = new List<string>();

        //    if (linksNodes != null)
        //    {
        //        lecturerlinks = GetAllLecturerLinks(linksNodes);
        //    }
        //    else
        //    {
        //        lecturerlinks = null;
        //    }

        //    if (lecturerlinks != null)
        //    {
        //        foreach (var lecturerLink in lecturerlinks)
        //        {
        //            var lecturer = new Lecturers();
        //            lecturer.Link = lecturerLink;
        //            lecturer.FacultyId = faculty.FacultyId;
        //            lecturer.DateId = date.DateId;
        //            try
        //            {
        //                var lecturerPage = web.Load(lecturerLink).DocumentNode;
        //                // Name, Position, Contact, AcademicStatus collecting
        //                if (lecturerPage != null)
        //                {
        //                    var infoRows = GetAllInfoRows(lecturerPage);
        //                    lecturer.Name = GetLecturerName(lecturerPage);
        //                    SetInfoFromRows(infoRows, lecturer);
        //                    // Biography, ScientificInterests, publications collecting
        //                    var sections = GetAllSections(lecturerPage);
        //                    SetInfoFromSections(sections, lecturer);
        //                }
        //            }
        //            catch
        //            {
        //            }

        //            lecturers.Add(lecturer);
        //        }

        //    }

        //    return lecturers;
        //}

        public static List<Lecturers> CollectLecturers(Faculties faculty, MarkingDate date)
        {
            var lecturers = new List<Lecturers>();
            // посилання на персонал факультетів
            string staffLink = faculty.FacultyLink + "about/staff";

            HtmlWeb web = new HtmlWeb();
            var staffNode = web.Load(staffLink).DocumentNode;
            var linksNodes = GetAllLecturerLinksNodes(staffNode);
            //отримання посилань на всіх працівників факультету
            List<string> lecturerlinks = null;

            if (linksNodes != null)
            {
                lecturerlinks = GetAllLecturerLinks(linksNodes);
            }

            if (lecturerlinks != null)
            {
                lecturerlinks = lecturerlinks.Distinct().ToList();

                foreach (var lecturerLink in lecturerlinks)
                {
                    var lecturer = new Lecturers
                    {
                        Link = lecturerLink,
                        FacultyId = faculty.FacultyId,
                        DateId = date.DateId
                    };
                    try
                    {
                        var lecturerPage = web.Load(lecturerLink).DocumentNode;
                        if (lecturerPage != null && lecturerLink.Equals(web.ResponseUri.AbsoluteUri))
                        {
                            //Name
                            lecturer.Name = GetLecturerName(lecturerPage);
                            //Position, Contact, AcademicStatus collecting
                            var infoRows = GetAllInfoRows(lecturerPage);
                            SetInfoFromRows(infoRows, lecturer);
                            // Biography, ScientificInterests, Publications collecting
                            var sections = GetAllSections(lecturerPage);
                            SetInfoFromSections(sections, lecturer);
                        }
                    }
                    catch
                    {
                    }

                    if (IsScientificStaff(lecturer))
                    {
                        lecturers.Add(lecturer);
                    }
                }
            }

            return lecturers;
        }

        private static bool IsScientificStaff(Lecturers lecturer)
        {
            if (string.IsNullOrEmpty(lecturer.Position))
            {
                return true;
            }
            string[] wordsToCheck = new string[] { "професор", "professor",
            "доцент", "docent",
            "cтарший викладач", "lecturer",
            "асистент", "завідувач кафедри",
            "assistant", "chairperson"};
          
            var label = lecturer.Position.ToLower();
            if (Contains(label, wordsToCheck))
            {
                return true;
            }

            return false;
        }

        //courses
        public static List<Courses> CollectCourses(Faculties faculty, MarkingDate date)
        {
            var courses = new List<Courses>();
            // посилання на спеціальності
            string bachelorSpecialityLink = faculty.FacultyLink + "academics/bachelor";
            string masterSpecialityLink = faculty.FacultyLink + "academics/master";
            //зчитування посилань на курси
            var web = new HtmlWeb();
            HtmlDocument bachelorSpecialityPage = null;
            HtmlDocument masterSpecialityPage = null;

            var linksToCoursesList = new List<string>();
            try
            {
                bachelorSpecialityPage = web.Load(bachelorSpecialityLink);
            }
            catch (Exception e)
            {
            }
            if (bachelorSpecialityPage != null && bachelorSpecialityLink.Equals(web.ResponseUri.AbsoluteUri))
            {
                try
                {
                    linksToCoursesList.AddRange(bachelorSpecialityPage.DocumentNode.SelectNodes("//section[@class='specialization']//h3[@class='title']/a")
                    .Select(p => p.Attributes["href"].Value).ToList());
                }
                catch (Exception e)
                {

                }
            }

            try
            {
                masterSpecialityPage = web.Load(masterSpecialityLink);
            }
            catch (Exception e)
            {
            }
            if (masterSpecialityPage != null && masterSpecialityLink.Equals(web.ResponseUri.AbsoluteUri))
            {
                try
                {
                    linksToCoursesList.AddRange(masterSpecialityPage.DocumentNode.SelectNodes("//section[@class='specialization']//h3[@class='title']/a")
                        .Select(p => p.Attributes["href"].Value).ToList());
                }
                catch (Exception e)
                {

                }
            }
            //сторінка всього навчального періоду
            var curriculumPage = new HtmlDocument();
            //specializations 
            foreach (var linkToCourses in linksToCoursesList)
            {
                string link = linkToCourses;
                if (!linkToCourses.Contains(faculty.FacultyLink))
                {
                    link = faculty.FacultyLink + linkToCourses.TrimStart('/');
                }
                //завантаження сторінки з навчальним планом для спеціальності
                try
                {
                    curriculumPage = web.Load(link);
                }
                catch (Exception e)
                {

                }
                //отримання всіх елементів з посиланнями на курси
                if (curriculumPage.DocumentNode != null && link.Equals(web.ResponseUri.AbsoluteUri))
                {
                    var courseNodes = curriculumPage.DocumentNode.SelectNodes("//table//tbody/tr/td[@class='title']");
                    if (courseNodes != null && courseNodes.Any())
                    {
                        foreach (var courseNode in courseNodes)
                        {
                            var course = new Courses();
                            course.DateId = date.DateId;
                            course.FacultyId = faculty.FacultyId;
                            //course.CourseLink = courseNode.OuterHtml;
                            HtmlDocument coursePage = null;
                            //var coursePageLink = "";
                            try
                            {
                                course.CourseLink = courseNode.ChildNodes["a"].Attributes["href"].Value;
                                //coursePageLink = courseNode.FirstChild.Attributes["href"].Value;
                                coursePage = web.Load(course.CourseLink);
                            }
                            catch (Exception e)
                            {

                            }

                            //обробка інформації на сторінці
                            if (coursePage != null && course.CourseLink != null && course.CourseLink.Equals(web.ResponseUri.AbsoluteUri))
                            {
                                //навчальна програма
                                try
                                {
                                    course.Plan = coursePage.DocumentNode.SelectNodes("//section[@class='attachments']//a")
                                         .Where(node => node.InnerHtml.Contains("Завантажити навчальну програму", StringComparison.OrdinalIgnoreCase) ||
                                         node.InnerHtml.Contains("Download curriculum", StringComparison.OrdinalIgnoreCase)).First().Attributes["href"].Value;
                                }
                                catch (Exception e)
                                {

                                }
                                //опис курсу
                                try
                                {
                                    course.CourseDescription = coursePage.DocumentNode.SelectSingleNode("//section[@class='description']").InnerText;
                                }
                                catch (Exception e)
                                {

                                }
                                //матеріали
                                try
                                {
                                    var materialsLines = coursePage.DocumentNode.SelectNodes("//section[@class='materials']//li").Select(node => node.InnerHtml);
                                    course.Materials = String.Join("\r\n", materialsLines);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            //добавлення у список курсів
                            courses.Add(course);
                        }
                    }
                }

            }

            return courses;
        }

        public static List<Materials> CollectMaterials(Faculties faculty, MarkingDate date)
        {
            var materials = new List<Materials>();
            List<string> materials_links = new List<string>();
            string departmentLink = faculty.FacultyLink + "/about/departments";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(departmentLink);
            var departments = ExtractAllDepartments(htmlDoc.DocumentNode);

            if (departments != null)
            {
                foreach (var department in departments)
                {
                    var materialsDBobj = new Materials();

                    materialsDBobj.DepartmentName = department.InnerText;
                    materialsDBobj.FacultyId = faculty.FacultyId;
                    materialsDBobj.DateId = date.DateId;

                    HtmlWeb departmentpage = new HtmlWeb();
                    string department_href = department.Attributes["href"].Value;
                    var department_page = departmentpage.Load(department_href);
                    HtmlNodeCollection nodecollection = department_page.DocumentNode.SelectNodes("//section[@class='materials']/div//li");


                    /*
                    if (nodecollection != null)
                    {
                        foreach (HtmlNode node in nodecollection)
                        {
                            HtmlNode firstChild = node.FirstChild;
                            if (firstChild != null)
                            {
                                var attributes = firstChild.Attributes;
                                if (attributes.Count() != 0)
                                {
                                    String hrefAttribute = attributes[0].Value;
                                    materials_links.Add(hrefAttribute);
                                }
                            }

                        }
                        materialsDBobj.MaterialsNumber = materials_links.Count();
                    }
                    */
                    if (nodecollection != null)
                    {
                        foreach (HtmlNode li in nodecollection)
                        {
                            var href_nodecollection = li.SelectNodes(li.XPath + "//a");
                            if (href_nodecollection != null)
                            {
                                foreach (HtmlNode elem in href_nodecollection)
                                {
                                    var attributes = elem.Attributes;
                                    if (attributes.Count() != 0)
                                    {
                                        String hrefAttribute = attributes[0].Value;
                                        materials_links.Add(hrefAttribute);
                                    }
                                }
                            }
                        }
                        materialsDBobj.MaterialsNumber = materials_links.Count();
                    }
                    String materialsLinks = "";
                    if (materials_links.Count() != 0)
                    {
                        for (int i = 0; i < materials_links.Count() - 1; i++)
                        {
                            materialsLinks += materials_links[i] + "\n";
                        }
                        materialsLinks += materials_links[materials_links.Count() - 1];
                    }
                    //materialsLinks += "\n" + "Посилання" + department_href;
                    materialsDBobj.MaterialsLinks = materialsLinks;
                    materials_links.Clear();

                    materials.Add(materialsDBobj);
                }
            }
            return materials;
        }

    }
}
