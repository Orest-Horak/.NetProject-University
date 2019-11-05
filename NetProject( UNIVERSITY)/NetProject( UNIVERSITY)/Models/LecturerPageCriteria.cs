using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text;
using static NetProject__UNIVERSITY_.Models.LinkBuilder;


namespace NetProject__UNIVERSITY_.Models
{
    public class Lecturer
    {
        string Name { get; set; }
        string Position { get; set; }
        string AcademicRank { get; set; }
        string Contacts { get; set; }

        string Biography { get; set; }
        string ScientificInterests { get; set; }

        string Link { get; set; }

        public bool IsNullOrEmpty(string data)
        {
            return String.IsNullOrEmpty(data);
        }

        public void SetInfoFromRows(HtmlNodeCollection infoRows)
        {
            var wordsToCheck = new string[0];
            //copy
            var infoRowsClone = infoRows.ToList();
            //position
            wordsToCheck = new string[] { "посада" };
            for (int i = 0; i < infoRowsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    this.Position = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
                    infoRowsClone.RemoveAt(i);
                    break;
                }
            }
            //academic rank
            wordsToCheck = new string[] { "звання" };
            for (int i = 0; i < infoRowsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    this.AcademicRank = LecturerPageCriteria.GetValueInnerTextFromRow(infoRowsClone[i]);
                    infoRowsClone.RemoveAt(i);
                    break;
                }
            }
            //contacts
            wordsToCheck = new string[] { "профіль", "пошта" };
            this.Contacts = "";
            for (int i = 0; i < infoRowsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromRow(infoRowsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    var contact = LecturerPageCriteria.GetValueLinkFromRow(infoRowsClone[i]);
                    this.Contacts = string.Concat(Contacts, " ", contact);
                    infoRowsClone.RemoveAt(i);
                }
            }

            //

            //

            //
        }

        public void SetInfoFromSections(HtmlNodeCollection sections)
        {
            var wordsToCheck = new string[0];
            //copy
            var sectionsClone = sections.ToList();
            //position
            wordsToCheck = new string[] { "біографія" };
            for (int i = 0; i < sectionsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromSection(sectionsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    Biography = LecturerPageCriteria.GetInnerTextFromDivInSection(sectionsClone[i]);
                    sectionsClone.RemoveAt(i);
                    break;
                }
            }
            //academic rank
            wordsToCheck = new string[] { "наукові інтереси" };
            for (int i = 0; i < sectionsClone.Count; i++)
            {
                var label = LecturerPageCriteria.GetLowerLabelFromSection(sectionsClone[i]);

                if (Contains(label, wordsToCheck))
                {
                    ScientificInterests = LecturerPageCriteria.GetInnerTextFromDivInSection(sectionsClone[i]);
                    sectionsClone.RemoveAt(i);
                    break;
                }
            }
            //

        }

        public void SetName(HtmlNode lecturerPage)
        {
            Name = LecturerPageCriteria.GetLecturerName(lecturerPage);
        }

        public void SetLink(string link)
        {
            Link = link;
        }

        public bool Contains(string label, string[] wordsToCheck)
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

        public override string ToString()
        {
            string lecturerString = "";
            lecturerString += "ПІБ: " + Name + "\r\n";
            lecturerString += "Посада: " + Position + "\r\n";
            lecturerString += "Вчене звання: " + AcademicRank + "\r\n";
            lecturerString += "Контакти: " + Contacts + "\r\n";
            lecturerString += "Біографія: " + Biography + "\r\n";
            lecturerString += "Наукові інтереси: " + ScientificInterests + "\r\n";
            lecturerString += "Посилання: " + Link;
            return lecturerString;
        }

    }


    public class LecturerPageCriteria
    {
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
        //GettinInfo
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
        //inforow = span(label)+span(value)
        public static HtmlNodeCollection GetAllInfoRows(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//div[@class='info']//p");
            return result;
        }
        //public static string GetLabelFromRow(HtmlNode infoRow)
        //{
        //    string label = "";
        //    try
        //    {
        //        label = infoRow.SelectSingleNode("//span[@class='label']").InnerText;
        //    }
        //    catch
        //    {

        //    }
        //    return label;
        //}

        public static string GetLowerLabelFromRow(HtmlNode infoRow)
        {
            string label = "";
            try
            {
                label = infoRow.FirstChild.InnerText;
                //label = infoRow.SelectSingleNode("//span[@class='label']").InnerText;
            }
            catch
            {

            }
            return label.ToLower();
        }
        public static string GetValueInnerTextFromRow(HtmlNode infoRow)
        {
            string innerText = "";
            try
            {

                innerText = infoRow.ChildNodes.Last().InnerText;
                //innerText = infoRow.SelectSingleNode("//span[@class='value']").InnerText;
            }
            catch
            {

            }
            return innerText;
        }
        public static string GetValueLinkFromRow(HtmlNode infoRow)
        {
            string innerLink = "";
            try
            {
                innerLink = infoRow.ChildNodes.Last().FirstChild.Attributes["href"].Value;
                //innerLink = infoRow.SelectSingleNode("//span[@class='value']//a").Attributes["href"].Value;
            }
            catch
            {

            }
            return innerLink;
        }

        //sections
        public static HtmlNodeCollection GetAllSections(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//section[@class='section']");
            return result;
        }
        public static string GetLowerLabelFromSection(HtmlNode section)
        {
            string label = "";
            try
            {
                label = section.ChildNodes[1].InnerText;
                //label = infoRow.SelectSingleNode("//span[@class='label']").InnerText;
            }
            catch
            {

            }
            return label.ToLower();
        }
        public static string GetValueInnerTextFromSection(HtmlNode section)
        {
            string innerText = "";
            try
            {

                innerText = section.InnerText;
                //innerText = infoRow.SelectSingleNode("//span[@class='value']").InnerText;
            }
            catch
            {

            }
            return innerText;
        }
        public static string GetInnerTextFromDivInSection(HtmlNode section)
        {
            string innerText = "";
            try
            {

                innerText = section.ChildNodes["div"].InnerText;
                //innerText = infoRow.SelectSingleNode("//span[@class='value']").InnerText;
            }
            catch
            {

            }
            return innerText;
        }

        public List<string> ExtractFacultyLinks(string filename)
        {
            List<string> links = new List<string>();

            using (FileStream fs = File.OpenRead(filename))
            {
                using (StreamReader streamReader = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        string link1 = line.Trim();
                        if (link1[0] == '\ufeff')
                        {
                            link1 = link1.Substring(1);

                        }
                        links.Add(link1);
                    }
                }
            }
            return links;
        }
        public static string GetFaculty(string facultyLink)
        {
            return facultyLink.Split('.')[0].Split('/').Last();
        }


        //public List<string> ConcatanateLinks(List<string> facultyLinks, string toAdd)
        //{
        //    List<string> facultyNews = new List<string>(facultyLinks);

        //    for (int i = 0; i < facultyNews.Count; i++)
        //    {
        //        facultyNews[i] = facultyNews[i] + toAdd;
        //    }

        //    return facultyNews;
        //}


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

        //public void MainFunction()
        //{
        //    // посилання на персонал факультетів
        //    string filenameRead = @"Data\staff_links.txt";
        //    List<string> staffLinks = ExtractFacultyLinks(filenameRead);

        //    string filenameWrite = ("report_staff_" + DateTime.Now.ToString() + ".txt").Replace(':', '.');
        //    using (StreamWriter f = new StreamWriter(filenameWrite, false))
        //    {
        //        f.WriteLine(string.Format("Викладачі \t Оцінка на {0}", DateTime.Today.ToString("dd.MM.yyyy")));
        //        for (int i = 0; i < staffLinks.Count; i++)
        //        {
        //            HtmlWeb web = new HtmlWeb();
        //            string staffLink = staffLinks[i];
        //            var linksNodes = GetAllLecturerLinksNodes(web.Load(staffLink).DocumentNode);
        //            //отримання посилань на всіх працівників факультету
        //            var lecturerlinks = GetAllLecturerLinks(linksNodes);

        //            double facultyStaffMark = new double();
        //            string info_line = string.Format("Факультет {0}\n", ArticleCriteria.GetFaculty(staffLink));
        //            f.WriteLine(info_line);

        //            var lecturers = new List<Lecturer>();
        //            foreach (var lecturerLink in lecturerlinks)
        //            {
        //                var lecturer = new Lecturer();
        //                lecturer.SetLink(lecturerLink);
        //                try
        //                {
        //                    var lecturerPage = web.Load(lecturerLink).DocumentNode;
        //                    var infoRows = GetAllInfoRows(lecturerPage);
        //                    lecturer.SetName(lecturerPage);
        //                    lecturer.SetInfoFromRows(infoRows);
        //                }
        //                catch
        //                {
        //                }

        //                f.WriteLine(lecturer);
        //                //double departmentMark = GetDepartmentMark(departmentArticles, filterArticles, mainNews);
        //                //dump_department_mark(f, department.InnerText, departmentMark);
        //                //facultyMark += departmentMark;
        //            }

        //            //facultyMark /= departments.Count;
        //            //f.WriteLine(string.Format("ФАКУЛЬТЕТ \t Середня оцінка {0}", facultyMark));
        //        }
        //        f.WriteLine(string.Format("Час завершення {0}", DateTime.Now.ToString()));
        //    }
        //}

        public void MainFunction()
        {
            // посилання на персонал факультетів
            string filenameRead = @"Data\staff_links.txt";
            List<string> staffLinks = ExtractFacultyLinks(filenameRead);

            string filenameWrite = ("report_staff_" + DateTime.Now.ToString() + ".txt").Replace(':', '.');
            using (StreamWriter f = new StreamWriter(filenameWrite, false))
            {
                f.WriteLine(string.Format("Викладачі \t Оцінка на {0}", DateTime.Today.ToString("dd.MM.yyyy")));
            }
            for (int i = 0; i < staffLinks.Count; i++)
            {
                HtmlWeb web = new HtmlWeb();
                string staffLink = staffLinks[i];
                var linksNodes = GetAllLecturerLinksNodes(web.Load(staffLink).DocumentNode);
                //отримання посилань на всіх працівників факультету
                var lecturerlinks = GetAllLecturerLinks(linksNodes);

                double facultyStaffMark = new double();
                using (StreamWriter f = new StreamWriter(filenameWrite, true))
                {
                    string info_line = string.Format("Факультет {0}\n", ArticleCriteria.GetFaculty(staffLink));
                    f.WriteLine(info_line);
                }

                var lecturers = new List<Lecturer>();
                foreach (var lecturerLink in lecturerlinks)
                {
                    var lecturer = new Lecturer();
                    lecturer.SetLink(lecturerLink);
                    try
                    {
                        var lecturerPage = web.Load(lecturerLink).DocumentNode;
                        var infoRows = GetAllInfoRows(lecturerPage);
                        lecturer.SetName(lecturerPage);
                        lecturer.SetInfoFromRows(infoRows);
                        var sections = GetAllSections(lecturerPage);
                        lecturer.SetInfoFromSections(sections);
                    }
                    catch
                    {
                    }
                    using (StreamWriter f = new StreamWriter(filenameWrite, true))
                    {
                        f.WriteLine(lecturer);
                    }

                    //double departmentMark = GetDepartmentMark(departmentArticles, filterArticles, mainNews);
                    //dump_department_mark(f, department.InnerText, departmentMark);
                    //facultyMark += departmentMark;
                }

                //facultyMark /= departments.Count;
                //f.WriteLine(string.Format("ФАКУЛЬТЕТ \t Середня оцінка {0}", facultyMark));
            }
            using (StreamWriter f = new StreamWriter(filenameWrite, true))
            {
                f.WriteLine(string.Format("Час завершення {0}", DateTime.Now.ToString()));
            }
        }

    }
}
