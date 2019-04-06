using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetProject__UNIVERSITY_.Models;
using System.Globalization;


namespace NetProject__UNIVERSITY_.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var html = @"http://ami.lnu.edu.ua/news";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

            Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//article//div[@class='excerpt']//a[@class='read-more']").First();

             /*
              var html = @"http://bioweb.lnu.edu.ua/department/biophysics-and-bioinformatics";
              
            var test_res = ArticleDepartmentCriteria.ExtractArticles(htmlDoc.DocumentNode,"//body//div[@class='content news']");
            var test_res = ArticleDepartmentCriteria.ExtractArticles(htmlDoc.DocumentNode, "//body//div//div//section");
            */


            ArticleCriteria test = new ArticleCriteria(null, 0, "");
            test.MainFunction(DateTime.Parse("01.01.2019", CultureInfo.CreateSpecificCulture("fr-FR")));

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
