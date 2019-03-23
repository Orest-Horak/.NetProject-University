using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models
{
    public class ArticleCriteria
    {
        List<ArticleCriteria> article_list = new List<ArticleCriteria>();
        public string date_text { get; set; }
        public DateTime date { get; set; }
        public string link { get; set; }
        public string page { get; set; }
        public string faculty { get; set; }

        public void dump_news_info(StreamWriter  f, ArticleCriteria link)
        {

            string info_line = string.Format("\t{0}{1}{2}\n", link,(article_list.Count).ToString(), article_list[0].date_text);
            f.WriteLine(info_line);
            foreach (var article in article_list)
            {
                info_line = string.Format("\t{0}{1}{2}{3}\n", article.date_text, article.faculty,(article.page).ToString(), article.link);
                f.WriteLine(info_line);
            }
        }
    }
}
