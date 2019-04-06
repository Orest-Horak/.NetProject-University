# Програма призначена для автоматизованого веб-скрапінгу
# сторінок викладачів ЛНУ (лише з новим дизайном) і отримання
# інформації, що необхідна для оцінювання сторінок.

import re
import os
import time


class Lecturer:

    def __init__(self, link, html_main=""):

        self.link = link
        self.name = None
        self.general_info = General_info()
        self.sections = {}
        self.interests = None
        self.courses = Courses()
        self.publications = Publications()  # if None -> AttributeError: 'NoneType' object has no attribute 'number'
        self.biography = None
        self.schedule = None

        try:

            # 1) поля, які точно будуть - за умови, якщо сторінка існує
            self.name = re.search(r"""<h1 class='page-title'>(.*?)</h1>""", html_main).groups()[0]
            self.general_info = General_info(
                re.search(r"""<div class='info'>(.*?)</div>""", html_main).groups()[0]
            )
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
                self.courses = Courses(
                    re.search(r"""<section class='section'><h2>(Курси|Навчальні дисципліни)</h2>(.*?)</section>""", html_main).groups()[1]
                )
            if {'Публікації', 'Вибрані публікації'} & self.sections: # & means intersection
                self.publications = Publications(
                    re.search(r"""<section class='section'><h2>(Вибрані )?[Пп]ублікації</h2>(.*?)</section>""", html_main).groups()[1]
                )
            if 'Біографія' in self.sections:
                self.biography = re.search(r"""<section class='section'><h2>Біографія</h2>(.*?)</section>""", html_main).groups()[0]
        except AttributeError or ValueError:  # e.g. file is empty or doesn't contain desired tags
            pass


class General_info():

# ['Веб-сторінка', 'Вчене звання', 'Електронна пошта', 'Науковий ступінь', 'Посада',
# 'Профіль у Academia.edu', 'Профіль у Facebook', 'Профіль у Google Scholar', 'Профіль у LinkedIn', '
# Телефон (мобільний)', 'Телефон (робочий)', 'Телефони (робочі)']

    def __init__(self, raw_info=""):
        self.raw = raw_info
        self.sections = {}
        self.position = None            # Посада
        self.scientific_degree = None   # Науковий ступінь
        self.academic_status = None     # Вчене звання
        self.phone = None               # Телефон(робочий, мобільний)
        self.email = None               # Електронна пошта
        self.google_scholar = None      # Профіль у Google Scholar
        self.academia_edu = None        # Профіль у Academia.edu
        self.facebook = None            # Профіль у Facebook
        self.linkedin = None            # Профіль у LinkedIn
        self.web_page = None            # Веб-сторінка

        try:
            self.sections = set(re.findall(r"""<span class='label'>(.*?)<span>""", raw_info))
            if 'Посада' in self.sections:
                self.position = re.search(
                    r"""Посада<span>:</span></span> <span class='value'>(.*?)<""",
                    raw_info).groups()[0]
            if 'Науковий ступінь' in self.sections:
                self.scientific_degree = re.search(
                    r"""Науковий ступінь<span>:</span></span> <span class='value'>(.*?)<""",
                    raw_info).groups()[0]
            if 'Вчене звання' in self.sections:
                self.academic_status = re.search(
                    r"""Вчене звання<span>:</span></span> <span class='value'>(.*?)<""",
                    raw_info).groups()[0]
            if {'Телефон (робочий)', 'Телефони (робочі)', 'Телефон (мобільний)'} & self.sections:
                self.phone = re.search(
                    r"""<a href='tel:(\+\d*)""",
                    raw_info).groups()[0]  # телефон (будь-який. Буде вибрано той, який вказано першим)
            if 'Електронна пошта' in self.sections:
                self.email = re.search(
                    r"""<a href='mailto:(.*?)'""",
                    raw_info).groups()[0]
            if 'Профіль у Google Scholar' in self.sections:
                self.google_scholar = re.search(
                    r"""<a href='(https://scholar.*?)'""",
                    raw_info).groups()[0]
            if 'Профіль у Academia.edu' in self.sections:
                self.academia_edu = re.search(
                    r"""<a href='(https://independent.academia.edu/.*?)'""",
                    raw_info).groups()[0]
            if 'Профіль у Facebook' in self.sections:
                self.facebook = re.search(
                    r"""<a href='(https://www.facebook.com/.*?)'""",
                    raw_info).groups()[0]
            if 'Профіль у LinkedIn' in self.sections:
                self.linkedin = re.search(
                    r"""<a href='(https://www.linkedin.com/.*?)'""",
                    raw_info).groups()[0]
            if 'Веб-сторінка' in self.sections:
                self.web_page = re.search(
                    r"""Веб-сторінка<span>:</span></span> <span class='value'><a href='(.*?)'""",
                    raw_info).groups()[0]
        except AttributeError or ValueError as e:
            print(e)


class Publications:

    def __init__(self, publ_list_raw=""):

        self.raw = publ_list_raw
        # наступні атрибути містять інфу про кількість публікацій
        # визначення кількості поки що не ідеально точне, але покращується
        self.lst = None
        self.href_list = None
        self.number = None
        self.href_number = None

        try:
            lst_1 = re.findall(r"""<p.*?>(.*?)</p>""", publ_list_raw)
            lst_2 = re.findall(r"""<li.*?>(.*?)</li>""", publ_list_raw)
            lst_3 = re.findall(r"""<tr.*?>(.*?)</tr>""", publ_list_raw)
            lst_4 = re.findall(r""">(.*?)<br /""", publ_list_raw)
                 # they can be put in many different tags incl.<li></li> <p></p> or <tr></tr>

            lst = sorted([lst_1, lst_2, lst_3, lst_4], key=len)[3]  # choose the longest list
            # практика показала, що в ОДНОМУ з 1876 випадків цей підхід призвів до помилки
            # тому в наступній версії програми варто порівнювати ці списки вже після чистки, а не до неї
            # зазвичай це не має значення, але деколи може сильно вплинути на результат.

            href_list = re.findall(r""" href=["'](.*?)['"]>.*?</a>""", publ_list_raw)
            self.lst = parse_publ_list(lst)
            self.href_list = href_list
            self.number = len(self.lst)
            self.href_number = len(href_list)
        except AttributeError or ValueError or TypeError:
            pass


class Courses:

    def __init__(self, course_list_plain=""):

        self.raw = course_list_plain
        self.course_list = None
        self.href_list = None

        try:
            self.course_list = re.findall(r"""<a href='.*?'>(.*?)</a>""", course_list_plain)
            self.href_list = re.findall(r"""<a href='(.*?)'""", course_list_plain)
        except AttributeError or ValueError or TypeError:
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
    with open(filename, encoding='utf-8') as f:
        for line in f:
            data = line.strip().split("\t")
            name, link = data[0], data[1]
            if name[0] == '\ufeff':
                name = name[1:]
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


def print_info(lecturer):
    print(lecturer.name, lecturer.link)  ###
    # print(lecturer.general_info.raw)  ###
    # print(lecturer.general_info.sections) ###
    # print('\tПосада\t\t\t\t', lecturer.general_info.position) ###
    # print('\tНауковий ступінь\t', lecturer.general_info.scientific_degree)
    # print('\tВчене звання\t\t', lecturer.general_info.academic_status)
    print('\tТелефон\t\t\t\t', lecturer.general_info.phone)
    # print('\tЕлектронна пошта\t', lecturer.general_info.email)
    # print('\tПрофіль у Google Scholar\t', lecturer.general_info.google_scholar)
    # print('\tПрофіль у Academia.edu\t\t', lecturer.general_info.academia_edu)
    # print('\tПрофіль у Facebook\t\t\t', lecturer.general_info.facebook)
    # print('\tПрофіль у LinkedIn\t\t\t', lecturer.general_info.linkedin)
    # print('\tВеб-сторінка\t\t\t\t', lecturer.general_info.web_page)
    # print(lecturer.sections)
    # print('Наукові інтереси\t', lecturer.interests)
    # print('Курси\t\t\t\t', lecturer.courses.raw)
    # if lecturer.courses.course_list:
    #     print('\tСписок')
    #     print("\t*\t", end="")
    #     print("\n\t*\t".join(lecturer.courses.course_list))
    # if lecturer.courses.href_list:
    #     print('\tПосилання')
    #     print("\t*\t", end="")
    #     print("\n\t*\t".join(lecturer.courses.href_list))
    # if lecturer.publications.number < lecturer.publications.href_number:
    #     print("ACHTUNG")

    # print('Публікації\t\t\t', lecturer.publications.raw)
    # print('\tПублікації\t\t', lecturer.publications.number)
    # if lecturer.publications.number > 0:
    #     print("\t*\t", end="")
    #     print("\n\t*\t".join(lecturer.publications.lst))
    # print('\tГіперпосилання\t', lecturer.publications.href_number)
    # if lecturer.publications.href_number > 0:
    #     print("\t*\t", end="")
    #     print("\n\t*\t".join(lecturer.publications.href_list))

    # print('Біографія\t\t\t', lecturer.biography)
    # print("-" * 152)


def _main():
    start_time = time.clock()

    sections_diversity = set()
    gi_sections_diversity = set()

    lecturers = ["\t".join([
            "ПIБ",
            "Посада",
            "Науковий ступінь",
            "Вчене звання",
            "Телефон(робочий, мобільний)",
            "Електронна пошта",
            "Профіль у Google Scholar",
            "Профіль у Academia.edu",
            "Профіль у Facebook",
            "Профіль у LinkedIn",
            "Веб-сторінка",
            "Чи є список курсів?",
            "Чи вказані наукові інтереси?",
            "Чи наведена біографія?",
            "Кількість наведених публікацій",
            "Кількість гіперпосилань у списку публікацій"
    + "\n"])]  # captions. KEEP IN ACCORDANCE WITH SELECTED INFO!!!

    filename = input("Введіть назву файлу з посиланнями на сторінки викладачів (з розширенням):\n")
    # filename = "link_list_1.txt"
    # файл містить список - ім'я, лінк та деякі інші дані, розділені \t
    links = extract_links(filename)

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
            print_info(lecturer)

            selected_info = "\t".join([
                str(lecturer.name),                              # ПIБ
                str(lecturer.general_info.position),             # Посада
                str(lecturer.general_info.scientific_degree),    # Науковий ступінь
                str(lecturer.general_info.academic_status),      # Вчене звання
                str(lecturer.general_info.phone),                # Телефон(робочий, мобільний)
                str(lecturer.general_info.email),                # Електронна пошта
                str(lecturer.general_info.google_scholar),       # Профіль у Google Scholar
                str(lecturer.general_info.academia_edu),         # Профіль у Academia.edu
                str(lecturer.general_info.facebook),             # Профіль у Facebook
                str(lecturer.general_info.linkedin),             # Профіль у LinkedIn
                str(lecturer.general_info.web_page),             # Веб-сторінка
                ("FALSE", "TRUE")[bool(lecturer.courses.raw)],    # Чи є список курсів?
                                       # Додати детальнішу інфу про курси!!!
                ("FALSE", "TRUE")[bool(lecturer.interests)],      # Чи вказані наукові інтереси?
                ("FALSE", "TRUE")[bool(lecturer.biography)],      # Чи вказана біографія?
                str(lecturer.publications.number),          # Кількість наведених публікацій
                str(lecturer.publications.href_number),     # Кількість гіперпосилань у списку публікацій
                "\n"
            ])

            lecturers.append(selected_info)

            sections_diversity.update(lecturer.sections)
            gi_sections_diversity.update(lecturer.general_info.sections)
    print(sorted(sections_diversity))
    print(sorted(gi_sections_diversity))

    filename = "final_table.txt"
    with open(filename, "wt", encoding="utf-8") as f:
        for line in lecturers:
            f.write(line)

    timedelta = time.clock() - start_time
    print("request processed in {:<2.4} seconds".format(str(timedelta)))


if __name__ == "__main__":
    _main()