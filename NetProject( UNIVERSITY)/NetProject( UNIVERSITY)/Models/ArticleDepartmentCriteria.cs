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

        public List<string> ConcatanateLinks(List<string> facultyLinks, string toAdd)
        {
            List<string> facultyNews = new List<string>(facultyLinks);

            for (int i = 0; i < facultyNews.Count; i++)
            {
                facultyNews[i] = facultyNews[i] + toAdd;
            }

            return facultyNews;
        }
    


    }
}
