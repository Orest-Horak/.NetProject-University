using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

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

    }
}
