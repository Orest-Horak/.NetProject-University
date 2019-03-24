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


        /*
        private void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "D:/LNU/3 course/2 сем/.net Project/Script/Python-скрипт (новини ф-тів)/scraper.py";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    */


    }
}

