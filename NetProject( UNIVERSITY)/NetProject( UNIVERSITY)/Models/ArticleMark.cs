using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models
{
    public class ArticleMark
    {
        public DateTime LastArticleDate { get; set; }
        public int ArticleNumber { get; set; }
        public string FacultyName { get; set; }
        public ArticleMark(DateTime LastArticleDate, int ArticleNumber, string FacultyName)
        {
            this.FacultyName = ArticleCriteria.GetFaculty(FacultyName);
            this.ArticleNumber = (ArticleNumber);
            this.LastArticleDate = (LastArticleDate);
        }
        public ArticleMark(string link, List<ArticleCriteria> articleList)
        {
            this.FacultyName = ArticleCriteria.GetFaculty(link);

            this.ArticleNumber = (articleList.Count);

            this.LastArticleDate = articleList.Max(a => a.Date);
        }

        public double CalculateFacultyMark(string link, List<ArticleCriteria> articleList)
        {
            double result;

            result = Math.Min(4.0, (4.0 * (ArticleNumber / 180.0)));

            if ((DateTime.Now - LastArticleDate).Days > 14)
            {
                result = result / 2.0;
            }

            return result;
        }
    }
}

