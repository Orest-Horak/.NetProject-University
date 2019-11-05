//using HtmlAgilityPack;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static NetProject__UNIVERSITY_.Models.LinkBuilder;

//namespace NetProject__UNIVERSITY_.Models
//{
//    public class MaterialsCriteria
//    {
//        public List<string> ExtractFacultyLinks(string filename)
//        {
//            List<string> links = new List<string>();

//            using (FileStream fs = File.OpenRead(filename))
//            {
//                using (StreamReader streamReader = new StreamReader(fs, Encoding.UTF8))
//                {
//                    while (!streamReader.EndOfStream)
//                    {
//                        string line = streamReader.ReadLine();
//                        string link1 = line.Trim();
//                        if (link1[0] == '\ufeff')
//                        {
//                            link1 = link1.Substring(1);

//                        }
//                        links.Add(link1);
//                    }
//                }
//            }
//            return links;
//        }

//        public List<string> ConcatanateLinks(List<string> facultyLinks, string toAdd)
//        {
//            List<string> facultyNews = new List<string>(facultyLinks);

//            for (int i = 0; i < facultyNews.Count; i++)
//            {
//                facultyNews[i] = facultyNews[i] + toAdd;
//            }

//            return facultyNews;
//        }

//        public HtmlNodeCollection ExtractAllDepartments(HtmlNode articleRaw)
//        {
//            List<string> alldepartments = new List<string>();

//            // працює
//            //var departmentNodes = articleRaw.SelectNodes("//section//div[@class='text']//h2//a");

//            //помилка
//            var departmentNodes = articleRaw.SelectSingleNode("//section//div[@class='text']//h2//a");
//            // bioweb.lnu.edu.ua/about/departments
//            //HtmlNode dataNode = articleRaw.ChildNodes["body"].ChildNodes["section[@id='taxonomy - list - 3']"];

//            //alldepartments.Add(articleRaw.SelectSingleNode("ul//li//a").Attributes["href"].Value.Trim());

//            //помилка
//            return departmentNodes;
//        }

//        public void dump_department_mark(StreamWriter f, string departmentName, double departmentMark)
//        {

//            try
//            {
//                string info_line = string.Format("{0}\t{1}\n", departmentName, departmentMark);
//                f.WriteLine(info_line);
//            }
//            catch (Exception e)
//            {
//                f.WriteLine("-");
//            }
//        }

//        public static HtmlNodeCollection ExtractMaterials(HtmlNode materials, string materialsAttribute)
//        {
//            var materialsNodes = materials.SelectNodes(materialsAttribute);
//            return materialsNodes;
//        }

//        public double GetDepartmentMark(HtmlNodeCollection departmentmaterials)
//        {
//            int result = 0;

//            if (departmentmaterials.Count() > 0 && departmentmaterials.Count() < 11)
//            {
//                result = 1;
//            }
//            else if (departmentmaterials.Count() > 10 && departmentmaterials.Count() < 21)
//            {
//                result = 2;
//            }
//            else if (departmentmaterials.Count() > 20 && departmentmaterials.Count() < 31)
//            {
//                result = 3;
//            }
//            else if (departmentmaterials.Count() > 30 && departmentmaterials != null)
//            {
//                result = 4;
//            }

//            return result;
//        }

//        public void MainFunction()
//        {
//            string filenameRead = @"Data\materials_links.txt";
//            List<string> links = ExtractFacultyLinks(filenameRead);

//            List<string> departmentLinks = ConcatanateLinks(links, "/about/departments");
//            //List<string> materialsLinks = ConcatanateLinks(links, "/materials");

//            string filenameWrite = ("report_materials_" + DateTime.Now.ToString() + ".txt").Replace(':', '.');

//            using (StreamWriter f = new StreamWriter(filenameWrite, false))
//            {
//                f.WriteLine(string.Format("Матеріали \t Оцінка на {0}", DateTime.Today.ToString("dd.MM.yyyy")));

//                for (int i = 0; i < departmentLinks.Count; i++)
//                {
//                    HtmlWeb web = new HtmlWeb();
//                    string departmentLink = departmentLinks[i];
//                    var htmlDoc = web.Load(departmentLink);
//                    var departments = ExtractAllDepartments(htmlDoc.DocumentNode);
//                    double materialsMark = new double();

//                    string info_line = string.Format("Факультет {0}\n", ArticleCriteria.GetFaculty(departmentLink));
//                    f.WriteLine(info_line);

//                    foreach (var department in departments)
//                    {
//                        HtmlNodeCollection departmentmaterials;
//                        try
//                        {
//                            departmentmaterials = ExtractMaterials(web.Load(department.Attributes["href"].Value).DocumentNode, "//body//section[@class='materials']//div//ol//li");
//                            //departmentmaterials = ExtractMaterials(web.Load(department.Attributes["href"].Value).DocumentNode, "//body//li//ul//section[@class='materials']");
//                        }
//                        catch (Exception e)
//                        {
//                            departmentmaterials = null;
//                        }

//                        double departmentMark = GetDepartmentMark(departmentmaterials);
//                        dump_department_mark(f, department.InnerText, departmentMark);
//                        materialsMark += departmentMark;
//                    }
//                    materialsMark /= departments.Count;
//                    f.WriteLine(string.Format("ФАКУЛЬТЕТ \t Середня оцінка {0}", materialsMark));

//                }
//            }
//        }
//    }
//}
