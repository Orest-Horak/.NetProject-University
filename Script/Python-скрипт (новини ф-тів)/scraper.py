import datetime
import re
import requests


class Article:

    def __init__(self, article_raw, page_number, faculty):

        self.date_text = "??.??.????"  # presentable date format
        # in case there is no date the article, AttributeError will be raised further,
        # but self.date_text will have sohehow meaningful value
        self.date = None  # comparable date format
        self.link = "no_link"
        # in case there is no link in the article, AttributeError will be raised further,
        # but self.link will have somehow meaningful value
        self.page = page_number
        # needed for checking
        # if one page number is missing than this page was not loaded
        self.faculty = faculty
        # useful for sorting

        try:
            self.date_text = re.search(
                r"""<div class='meta'>(.*?) \|""",
                article_raw).groups()[0].strip()
            day, month, year = (int(i) for i in self.date_text.split("."))
            self.date = datetime.date(year=year, month=month, day=day)
            self.link = re.search(
                r"""<a class='read-more' href='(.*?)'>""",
                article_raw).groups()[0]
        except AttributeError or ValueError:
            pass


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename, encoding='utf-8') as f:
        for line in f:
            link = line.strip()
            if link[0] == '\ufeff':
                link = link[1:]
            # if re.match(r"""http://.*?\.lnu\.edu\.ua/news"""):
            links.append(link)
    return links


def collect_news_info(link):
    faculty = re.search(
        r"""http://(.*?).lnu.edu.ua/news""",
        link).groups()[0]
    article_list = []  # list of Article objects
    keep_going = True
    page_number = 1
    while keep_going:
        if page_number == 1:
            page_link = link
            # usually there is no difference
            # but sometimes ~/news/page/1 causes request fail but ~/news doesn't
        else:
            page_link = link + "/page/" + str(page_number)
        res = requests.get(page_link, timeout=300)
        if res.status_code == 200:
            if res.text == "Notice! Your computer has been locked out.":
                page_number += 1  # maybe the next page will be loaded properly
            else:
                html_main = re.search(r"""<main class='content-area' role='main'>(.*?)</main>""",
                                      res.text.replace("\n", "")).groups()[0]
                articles_raw = re.findall(r"""<article>(.*?)</article>""", html_main)
                # list of raw html fragments <article>(.*?)</article>
                for article_raw in articles_raw:  # for each html fragment
                    article = Article(article_raw, page_number, faculty)
                    try:
                        if article.date >= from_date:
                            article_list.append(article)
                        else:
                            keep_going = False  # stop because we don't need older news
                            break
                    except TypeError:
                        # when None type object is compared with date type object
                        # TypeError can occur in case dates do not appear in news feed, e.g.
                        # http://electronics.lnu.edu.ua/news
                        # It's bad practice and should be corrected, but the program should
                        # keep running and collect the data
                        article_list.append(article)
                        # in this case data filtering is not possible
                        # but the date will be presented as ??.??.???? in the report
                        # so these news can be distinguished from others.
                page_number += 1
        else:
            keep_going = False  # stop because there are no more pages with news (or they are inaccessible)
    return article_list


def dump_news_info(f, link, article_list):
    info_line = "\t".join([
        link,
        str(len(article_list)),
        article_list[0].date_text,
        "\n"])
    f.write(info_line)
    print(info_line, end="")
    for article in article_list:
        info_line = "\t".join([
            article.date_text,
            article.faculty,
            str(article.page),
            article.link,
            "\n"])
        f.write(info_line)
        print(info_line, end="")

def _main():
    global from_date
    from_date = datetime.date(year=2019, month=1, day=1)  # DEFAULT

    filename_r = "link_list.txt"
    # файл містить список посилань на розділи "Новини" різних факультетів
    links = extract_links(filename_r)

    filename_w = "report " + str(datetime.datetime.today()).replace(":", ".") + ".txt"
    ### print(filename_w)
    with open(filename_w, "wt") as f:
        f.write("ФАКУЛЬТЕТ\tКІЛЬКІСТЬ НОВИН ВІД {}\tДАТА ОСТАННЬОЇ ПУБЛІКАЦІЇ\n".format(".".join([str(i) for i in (from_date.day, from_date.month, from_date.year)])))
        for link in links:
            article_list = collect_news_info(link)
            dump_news_info(f, link, article_list)


if __name__ == "__main__":
    _main()
