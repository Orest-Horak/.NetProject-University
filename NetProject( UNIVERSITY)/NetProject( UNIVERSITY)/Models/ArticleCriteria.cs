using System;
using System.Collections.Generic;
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
        List<ArticleCriteria> article_list = new List<ArticleCriteria>();
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
                this.DateFormat = articleRaw.SelectNodes("//div[@class='meta']").First().InnerText.Remove(11).Trim();
                //string[] stringDate = DateFormat.Split(new char[] { '.' });
                //int day = Int32.Parse(stringDate[0]), month = Int32.Parse(stringDate[1]), year = Int32.Parse(stringDate[2]);
                this.Date = DateTime.Parse(DateFormat);
                this.Link = articleRaw.SelectNodes("//div[@class='excerpt']//a[@class='read-more']").First().Attributes["href"].Value.Trim();
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
 
            using (FileStream fs = File.OpenRead("input.txt"))
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
                        links.Append(link1);
                    }
                }
            }
            return links;
        }
        
          private string GetFaculty(string facultyLink)
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
                var res = web.Load(link);

                if (res != null)
                {
                    // list of raw html fragments <article>(.*?)</article>
                    var articlesRaw = res.DocumentNode.SelectNodes("//article//div[@class='excerpt']//a[@class='read-more']");
                    foreach (var articleRaw in articlesRaw)
                    {
                        var article = new ArticleCriteria(articleRaw, pageNumber, faculty);
                        try
                        {
                            if (article.Date >= fromDate)
                            {
                                articleList.Append(article);
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
                            articleList.Append(article);
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

        public void dump_news_info(StreamWriter  f, ArticleCriteria link)
        {

            string info_line = string.Format("\t{0}{1}{2}\n", link,(article_list.Count).ToString(), article_list[0].DateFormat);
            f.WriteLine(info_line);
            foreach (var article in article_list)
            {
                info_line = string.Format("\t{0}{1}{2}{3}\n", article.DateFormat, article.Faculty, (article.PageNumber).ToString(), article.Link);
                f.WriteLine(info_line);
            }
        }
    }
}
