using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;

namespace NetProject__UNIVERSITY_.Models
{
    public class ArticleCriteria
    {
        public string DateFormat { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
        public int PageNumber { get; set; }
        public string Faculty { get; set; }

        public ArticleCriteria(HtmlNode articleRaw, int pageNumber, string faculty)
        {
            this.DateFormat = "??.??.????";
            this.Date = new DateTime();
            this.Link = "no link";
            this.PageNumber = pageNumber;
            this.Faculty = faculty;
            try
            {
                //semiworking         
                //this.DateFormat = articleRaw.SelectSingleNode("//div[@class='meta']").InnerText.Remove(11).Trim();
                //this.Date = DateTime.Parse(DateFormat);
                //this.Link = articleRaw.Attributes["href"].Value.Trim();


                //string[] stringDate = DateFormat.Split(new char[] { '.' });
                //int day = Int32.Parse(stringDate[0]), month = Int32.Parse(stringDate[1]), year = Int32.Parse(stringDate[2]);
                HtmlNode dataNode = articleRaw.ChildNodes["header"].ChildNodes["div"];
                this.DateFormat = dataNode.InnerText.Remove(11).Trim();
                this.Date = DateTime.Parse(DateFormat);
                this.Link = articleRaw.SelectSingleNode("div[@class='excerpt']//a[@class='read-more']").Attributes["href"].Value.Trim();

                //div[@class='excerpt']//a[@class='read-more']


            }
            catch (Exception e)
            {
                { };
            }
        }

        //Extracts the links from the file.
        public List<string> ExtractLinks(string filename)
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

        public void dump_news_info(StreamWriter f, string link, List<ArticleCriteria> articleList)
        {
            try
            {
                string info_line = string.Format("{0}\t{1}\t{2}\n", link, articleList.Count.ToString(), articleList.Max(a => a.Date));
                f.WriteLine(info_line);
                foreach (var article in articleList)
                {
                    info_line = string.Format("{0}\t{1}\t{2}\t{3}\n", article.Date.ToString("u"), article.Faculty, (article.PageNumber).ToString(), article.Link);
                    f.WriteLine(info_line);
                }
            }
            catch (Exception e)
            {
                f.WriteLine("-");
            }
        }

        public void dump_news_info(StreamWriter f, string link, List<ArticleCriteria> articleList, double mark)
        {
            try
            {
                string info_line = string.Format("{0}\t{1}\t{2}\t{3}\n", link, articleList.Count.ToString(), articleList.Max(a => a.Date), mark);
                f.WriteLine(info_line);
                foreach (var article in articleList)
                {
                    info_line = string.Format("{0}\t{1}\t{2}\t{3}\n", article.Date.ToString("u"), article.Faculty, (article.PageNumber).ToString(), article.Link);
                    f.WriteLine(info_line);
                }
            }
            catch (Exception e)
            {
                f.WriteLine("-");
            }
        }

        public static string GetFaculty(string facultyLink)
        {
            return facultyLink.Split('.')[0].Split('/').Last();
        }

        public List<ArticleCriteria> CollectNewsInfo(string link, DateTime fromDate)
        {
            string faculty = GetFaculty(link);
            List<ArticleCriteria> articleList = new List<ArticleCriteria>();

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
                var res = web.Load(pageLink);

                if (res != null)
                {
                    // list of raw html fragments <article>(.*?)</article>
                    var articlesRaw = res.DocumentNode.SelectNodes("//article");
                    foreach (var articleRaw in articlesRaw)
                    {
                        var article = new ArticleCriteria(articleRaw, pageNumber, faculty);
                        try
                        {
                            if (article.Date >= fromDate)
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
                    pageNumber += 1;
                }
                else
                {
                    keepGoing = false;
                }

            }
            return articleList;
        }

        public void MainFunction(DateTime fromDate)
        {

            // посиланння на розділи "Новини" різних факультетів
            string filenameRead = @"Data\link_list.txt";

            List<string> links = ExtractLinks(filenameRead);

            string filenameWrite = ("report" + DateTime.Today.ToString("u") + ".txt").Replace(':', '.');

            using (StreamWriter f = new StreamWriter(filenameWrite, false))
            {

                f.WriteLine(string.Format("ФАКУЛЬТЕТ \t КІЛЬКІСТЬ НОВИН ВІД {0} \t ДАТА ОСТАННЬОЇ ПУБЛІКАЦІЇ", fromDate.Date.ToString("dd.MM.yyyy")));
                foreach (string link in links)
                {
                    List<ArticleCriteria> articleList = CollectNewsInfo(link, fromDate);
                    ArticleMark articleMark = new ArticleMark(link, articleList);
                    double mark = articleMark.CalculateFacultyMark(link, articleList);
                    dump_news_info(f, link, articleList, mark);
                }
            }


        }
    }
}
