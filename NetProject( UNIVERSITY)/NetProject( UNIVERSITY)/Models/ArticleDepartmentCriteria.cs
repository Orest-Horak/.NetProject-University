using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using static NetProject__UNIVERSITY_.Models.LinkBuilder;

namespace NetProject__UNIVERSITY_.Models
{

    class DepartmentEqualityComparer : IEqualityComparer<HtmlNode>
    {
        public bool Equals(HtmlNode n1, HtmlNode n2)
        {
            if (n2 == null && n1 == null)
                return true;
            else if (n1 == null || n2 == null)
                return false;
            else if (n1.Attributes["href"].Value == n2.Attributes["href"].Value)
                return true;
            else
                return false;
        }

        public int GetHashCode(HtmlNode nx)
        {
            return nx.Attributes["href"].Value.GetHashCode();
        }
    }

    public class ArticleDepartmentCriteria
    {
        //public List<string> ExtractFacultyLinks(string filename)
        //{
        //    List<string> links = new List<string>();

        //    using (FileStream fs = File.OpenRead(filename))
        //    {
        //        using (StreamReader streamReader = new StreamReader(fs, Encoding.UTF8))
        //        {
        //            while (!streamReader.EndOfStream)
        //            {
        //                string line = streamReader.ReadLine();
        //                string link1 = line.Trim();
        //                if (link1[0] == '\ufeff')
        //                {
        //                    link1 = link1.Substring(1);

        //                }
        //                links.Add(link1);
        //            }
        //        }
        //    }
        //    return links;
        //}

        //public List<string> ConcatanateLinks(List<string> facultyLinks, string toAdd)
        //{
        //    List<string> facultyNews = new List<string>(facultyLinks);

        //    for (int i = 0; i < facultyNews.Count; i++)
        //    {
        //        facultyNews[i] = facultyNews[i] + toAdd;
        //    }

        //    return facultyNews;
        //}

        public IEnumerable<HtmlNode> ExtractDepartmentsFromFilter(HtmlNode articleRaw)
        {
            List<string> departmentsFromFilter = new List<string>();
            // bioweb.lnu.edu.ua/news
            HtmlNode dataNode = articleRaw.SelectSingleNode("//aside//section[@class='widget widget-taxonomy-list']");
            //HtmlNode dataNode = articleRaw.ChildNodes["body"].ChildNodes["section[@id='taxonomy - list - 3']"];

            var departmentNodes = articleRaw.SelectNodes("//aside//section[@class='widget widget-taxonomy-list']//ul//li//a");
            var departmentNodes1 = departmentNodes.Distinct(new DepartmentEqualityComparer());
            //departmentsFromFilter.Add(articleRaw.SelectSingleNode("//ul//li//a").Attributes["href"].Value.Trim());

            return departmentNodes1;
        }

        public HtmlNodeCollection ExtractAllDepartments(HtmlNode articleRaw)
        {
            List<string> alldepartments = new List<string>();
            var departmentNodes = articleRaw.SelectNodes("//section//div[@class='text']//h2//a");
            // bioweb.lnu.edu.ua/about/departments
            //HtmlNode dataNode = articleRaw.ChildNodes["body"].ChildNodes["section[@id='taxonomy - list - 3']"];

            //alldepartments.Add(articleRaw.SelectSingleNode("ul//li//a").Attributes["href"].Value.Trim());

            return departmentNodes;
        }
        //побудова зв'язку кафедра-фільтр
        public Dictionary<HtmlNode, HtmlNode> LinkDeparmentsToFilters(HtmlNodeCollection departments, IEnumerable<HtmlNode> filters)
        {
            Dictionary<HtmlNode, HtmlNode> departmentFilter = new Dictionary<HtmlNode, HtmlNode>();

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

            return departmentFilter;
        }

        //example
        /*
             var html = @"http://bioweb.lnu.edu.ua/department/biophysics-and-bioinformatics";

           var test_res = ArticleDepartmentCriteria.ExtractArticles(htmlDoc.DocumentNode,"//body//div[@class='content news']");
           var test_res = ArticleDepartmentCriteria.ExtractArticles(htmlDoc.DocumentNode, "//body//div//div//section");
           */
        //function:  Get all news from the filter v1.2
        public static HtmlNodeCollection ExtractArticles(HtmlNode article, string articleAttribute)
        {
            string article_link = "//article";
            // bioweb.lnu.edu.ua/news/category/novyny-kafedry-zoolohiji          
            var articleNodes = article.SelectNodes(articleAttribute + article_link);
            return articleNodes;
        }

        public void dump_department_mark(StreamWriter f, string departmentName, double departmentMark)
        {

            try
            {
                string info_line = string.Format("{0}\t{1}\n", departmentName, departmentMark);
                f.WriteLine(info_line);
            }
            catch (Exception e)
            {
                f.WriteLine("-");
            }
        }

        public double GetDepartmentMark(HtmlNodeCollection departmentArticles, HtmlNodeCollection filterArticles, HtmlNodeCollection mainArticles)
        {
            int result = 1;

            if (filterArticles != null && departmentArticles.Count() > 0)
            {
                result = 2;
            }
            else if (filterArticles == null && departmentArticles == null)
            {
                result = 0;
            }


            return result;
        }

        public void MainFunction()
        {
            // посилання на факультети
            string filenameRead = @"Data\faculty_links.txt";

            List<string> links = ExtractFacultyLinks(filenameRead);
            // посилання на списки кафедр, новин
            List<string> departmentLinks = ConcatanateLinks(links, "/about/departments");
            List<string> newsLinks = ConcatanateLinks(links, "/news");

            string filenameWrite = ("report_department" + DateTime.Today.ToString("u") + ".txt").Replace(':', '.');
            using (StreamWriter f = new StreamWriter(filenameWrite, false))
            {

                f.WriteLine(string.Format("ФАКУЛЬТЕТ \t Кафедра \t Оцінка на {0}", DateTime.Today.ToString("dd.MM.yyyy")));
                for (int i = 0; i < departmentLinks.Count; i++)
                {
                    HtmlWeb web = new HtmlWeb();
                    string departmentLink = departmentLinks[i],
                           newsLink = newsLinks[i];
                    var mainNews = ExtractArticles(web.Load(newsLink).DocumentNode, "");
                    var htmlDoc = web.Load(departmentLink);
                    var departments = ExtractAllDepartments(htmlDoc.DocumentNode);
                    htmlDoc = web.Load(newsLink);
                    var filters = ExtractDepartmentsFromFilter(htmlDoc.DocumentNode);
                    var departmentFilter = LinkDeparmentsToFilters(departments, filters);
                    double facultyMark = new double();

                    string info_line = string.Format("{0}\n", ArticleCriteria.GetFaculty(departmentLink));
                    f.WriteLine(info_line);

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

                        double departmentMark = GetDepartmentMark(departmentArticles, filterArticles, mainNews);
                        dump_department_mark(f, department.InnerText, departmentMark);
                        facultyMark += departmentMark;
                    }

                    facultyMark /= departments.Count;
                    f.WriteLine(string.Format("ФАКУЛЬТЕТ \t Середня оцінка {0}", facultyMark));
                }

            }
        }

    }
}

