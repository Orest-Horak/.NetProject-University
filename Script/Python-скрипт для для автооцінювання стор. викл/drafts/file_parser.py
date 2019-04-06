# Програма призначена для автоматизованого веб-скрапінгу
# сторінок викладачів ЛНУ (лише з новим дизайном) і отримання
# інформації, що необхідна для оцінювання сторінок.

import requests
import re
import os
import html.parser as prs # https://docs.python.org/3/library/html.parser.html


class Lecturer:

    def __init__(self, link, html_main=None):

        self.link = link
        self.name = None
        self.general_info = None
        self.sections = {}
        self.interests = None
        self.courses = Courses()
        self.publications = Publications()  # if None -> AttributeError: 'NoneType' object has no attribute 'number'
        self.biography = None
        self.schedule = None

        try:

            # 1) поля, які точно будуть - за умови, якщо сторінка існує
            self.name = re.search(r"""<h1 class=\'page-title\'>(.*?)</h1>""", html_main).groups()[0]
            self.general_info = re.search(r"""<div class='info'>(.*?)</div>""", html_main).groups()[0] #  = General_info(re.search())

            # 2) Поля, які в принципі модуть бути відсутні
            self.sections = set(re.findall(r"""<section class='section'><h2>(.*?)</h2>""", html_main))
            # для перевірки того, що взагалі там може бути (які розділи)
            # має велике значення для розробки і, якщо буде потрібно, для подальшого вдосконалення програми
            # Дотепер траплялися такі назви розділів:
            # ['Біографія', 'Вибрані публікації', 'Курси', 'Методичні матеріали', 'Навчальні дисципліни', 'Нагороди',
            # 'Наукова біографія', 'Наукові інтереси', 'Проекти', 'Публікації', 'Розклад', 'Різне']

            if 'Наукові інтереси' in self.sections:
                self.interests = re.search(r"""<section class='section'><h2>Наукові інтереси</h2>(.*?)</section>""", html_main).groups()[0]
            if 'Курси' in self.sections:
                self.courses = re.search(r"""<section class='section'><h2>(Курси|Навчальні дисципліни)</h2>(.*?)</section>""", html_main).groups()[1]
            if {'Публікації', 'Вибрані публікації'} & self.sections: # & means intersection
                self.publications = Publications(
                    re.search(r"""<section class='section'><h2>(Вибрані )?[Пп]ублікації</h2>(.*?)</section>""", html_main).groups()[1]
                )
            if 'Біографія' in self.sections:
                self.biography = re.search(r"""<section class='section'><h2>Біографія</h2>(.*?)</section>""", html_main).groups()[0]
        except AttributeError or ValueError:  # e.g. file is empty or doesn't contain desired tags
            pass

# class General_info(info):
#
#     def __init__(self):
#         pass


class Publications:

    def __init__(self, publ_list_raw=None):

        self.raw = publ_list_raw
        # наступні атрибути містять інфу про кількість публікацій
        # визначення кількості поки що не ідеально точне, але покращується
        self.lst = None
        self.href_list = None
        self.number = None
        self.href_number = None

        try:
            lst_1 = re.findall(r"""<p[.:;]*?>(.*?)</p>""", publ_list_raw)
            lst_2 = re.findall(r"""<li[.:;]*?>(.*?)</li>""", publ_list_raw)
            lst_3 = re.findall(r"""<tr[.:;]*?>(.*?)</tr>""", publ_list_raw)
            lst_4 = re.findall(r"""/>(.*?)<br />""", publ_list_raw)
            lst = max([lst_1, lst_2, lst_3, lst_4])
                # they can be put in many different tags incl.<li></li> <p></p> or <tr></tr>
            href_list = re.findall(r""" href=["'](.*?)['"]>.*?</a>""", publ_list_raw)
            self.lst = parse_publ_list(lst)
            self.href_list = href_list
            self.number = len(lst)
            self.href_number = len(href_list)
        except AttributeError or ValueError:
            pass


class Courses:

    def __init__(self, course_list_plain=None):
        pass


def parse_publ_list(lst): # not everything within <a></a> are publications
    new_lst = []
    for element in lst:
        # element = re.sub(r"""<strong>.*?</strong>""", "", element) # category, not a publication
        # actually not the best way to cut out a category name
        element = re.sub(r"""<[^<>]*?>""", "", element) # delete remaining tags
        if len(element) > 40: # not made empty yet / not too short for publ. title
            # actually better way to cut out a publication name
            new_lst.append(element)
            # у деяких випадках цього недостатньо. Перевірка по прізвищу була б кращим варіантом, ніж по <p></p>
            # але тут потрібно знати,як транслітерується прізвище. А в польській воно може транслітеруватись по іншому.
            # До того ж, можуть використовуватись різні стандарти транслітерації.
            # Тому поки що алгоритм підрахунку публікацій залишається таким, як є.
    return new_lst


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename) as f:
        for line in f:
            name, link = line.strip().split("\t")
            links.append((name, link))
    return links


# def get_main(link):
#     """Extracts all the content inside <main></main> tag."""
#     if re.match(r"""http""", link):
#         res = requests.get(link)
#         # print(res.text)
#         try:
#             html_main = re.search(r"""<main class='content-area' role='main'>(.*)</main>""", res.text.replace("\n", "")).groups()[0]
#         except AttributeError: # sometimes the response doesn't have html
#             return
#         return html_main
#     return


def _main():
    sections_diversity = set()

    # filename = input("Введіть назву файлу з посиланнями на сторінки викладачів (з розширенням):\n")
    filename = "link_list_1.txt"
    # файл містить список - ім'я та лінк, розділені \t
    links = extract_links(filename)
    print(links)

    dirname = "dump_html_main"
    os.chdir('./' + dirname)
    # files = os.listdir()
    # print(files) ###

    for elem in links:
        name = elem[0]
        link = elem[1]
        with open(name + '.txt', 'rt', encoding='utf-8') as f:
            html_main = f.read()
        lecturer = Lecturer(link, html_main)

        print(lecturer.name, lecturer.link) ###
        print(lecturer.general_info) ###
        print(lecturer.sections)
        print('Наукові інтереси\t', lecturer.interests)
        print('Курси\t\t\t\t', lecturer.courses)
        print('Публікації\t\t\t', lecturer.publications.raw)
        print('Публікації\t\t\t', lecturer.publications.number)
        if lecturer.publications.number > 0:
            print("\t*\t", end="")
            print("\n\t*\t".join(lecturer.publications.lst))
        print('Публікації\t\t\t', lecturer.publications.href_number)
        if lecturer.publications.href_number > 0:
            print("\t*\t", end="")
            print("\n\t*\t".join(lecturer.publications.href_list))
        print('Біографія\t\t\t', lecturer.biography)

        sections_diversity.update(lecturer.sections)
    print(sorted(sections_diversity))


if __name__ == "__main__":
    _main()