using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NetProject__UNIVERSITY_.Models
{
    public class LecturerPageCriteria
    {
        //inforow = span(label)+span(value)
        public static HtmlNodeCollection GetAllInfoRows(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//div[@class='info']//p");
            return result;
        }
        //sections
        public static HtmlNodeCollection GetAllSections(HtmlNode lecturerPage)
        {
            var result = lecturerPage.SelectNodes("//section[@class='section']");
            return result;
        }

    }
}
