using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models
{
    public class ArticleDepartmentCriteria
    {
        public bool IsFilter { get; set; }


        public bool ISNULL(HtmlNode articleRaw)
        {
            if(articleRaw==null)
            {
                return false;
            }
        }


    }
}
