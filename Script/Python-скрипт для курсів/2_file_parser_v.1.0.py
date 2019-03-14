# Програма призначена для автоматизованого веб-скрапінгу
# сторінок курсів викладачів ЛНУ (лише з новим дизайном) і отримання
# інформації, що необхідна для оцінювання сторінок.


import re
import os
import time



class Course:

    def __init__(self, link, html_main=""):

        self.link = link
        self.name = None
        self.lecturers = {}
        self.sections = {}
        self.description = None             # Опис курсу
        self.literature = None              # Рекомендована література
        self.materials = None               # Матеріали
        self.syllabus = None                # Навчальна програма


        try:

            # 1) поля, які точно будуть - за умови, якщо сторінка існує
            self.name = re.search(r"""<h1 class='page-title'>(.*?)</h1>""", html_main).groups()[0]
            # ####### self.lecturers = set(re.findall(r"""""")) - {'Лектор'}

            # 2) Поля, які в принципі модуть бути відсутні
            self.sections = set(re.findall(r"""<section class='.*?'><h2>(.*?)</h2>""", html_main))
            # для перевірки того, що взагалі там може бути (які розділи)
            # має велике значення для розробки і, якщо буде потрібно, для подальшого вдосконалення програми
            # Дотепер траплялися такі назви розділів:
            # []

            if 'Опис курсу' in self.sections:
                self.description = re.search(
                    r"""<h2>Опис курсу</h2>(.*?)</section>""",
                    html_main).groups()[0]
            if 'Рекомендована література' in self.sections:
                self.literature = re.search(
                    r"""<h2>Рекомендована література</h2>(.*?)</section>""",
                    html_main).groups()[0]
            if 'Матеріали' in self.sections:
                self.materials = re.search(
                    r"""<h2>Матеріали</h2>(.*?)</section>""",
                    html_main).groups()[0]
            if 'Навчальна програма' in self.sections:
                self.syllabus = re.search(
                    r"""<h2>Навчальна програма</h2><a href='(.*?)'>""",
                    html_main).groups()[0]
        except AttributeError or ValueError:  # e.g. file is empty or doesn't contain desired tags
            pass


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename, encoding='utf-8') as f:
        for line in f:
            link = line.strip()
            if link[0] == '\ufeff':
                link = link[1:]
            links.append(link)
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


def print_info(course):
    print(course.name, course.link)  ###
    print("опис курсу:\t\t\t\t\t\t", course.description)  ###
    print("рекомендована література:\t\t", course.literature)  ###
    print("матеріали:\t\t\t\t\t\t", course.materials)  ###
    print("навчальна програма:\t\t\t\t", course.syllabus)  ###

    # print("-" * 152)


def _main():
    start_time = time.clock()

    sections_diversity = set()
    # gi_sections_diversity = set()


    courses = ["\t".join([
        "Назва курсу",
        "Посилання на сторінку курсу",
        "Чи є опис курсу",
        "Чи є рекомендована література",
        "Чи є матеріали",
        "Чи є навчальна програма",
        "\n",
    ])]  # captions. KEEP IN ACCORDANCE WITH SELECTED INFO!!!

    # filename = input("Введіть назву файлу з посиланнями на сторінки курсів (з розширенням):\n")
    filename = "268_courses.txt"
    links = extract_links(filename)

    dirname = "dump_html_main"
    os.chdir('./' + dirname)
    # files = os.listdir()
    # print(files) ###

    for link in links:
        faculty_acronym, course_name = re.search(
            r"""http://(.*?).lnu.edu.ua/course/(.*)""",
            link).groups()
        filename = faculty_acronym + ' ' + course_name + ".txt"
        print(link)
        with open(filename, 'rt', encoding="utf-8") as f:
            html_main = f.read()
            course = Course(link, html_main)
            print_info(course)

            selected_info = "\t".join([
                str(course.name),
                str(course.link),
                ("FALSE", "TRUE")[bool(course.description)],
                ("FALSE", "TRUE")[bool(course.literature)],
                ("FALSE", "TRUE")[bool(course.materials)],
                str(course.syllabus),
                "\n"
            ])

            courses.append(selected_info)

            sections_diversity.update(course.sections)
            # gi_sections_diversity.update(course.general_info.sections)
    print(sorted(sections_diversity))
    # print(sorted(gi_sections_diversity))

    filename = "final_table.txt"
    with open(filename, "wt", encoding="utf-8") as f:
        for line in courses:
            f.write(line)

    timedelta = time.clock() - start_time
    print("Parsing has been done in {:<2.4} seconds".format(str(timedelta)))


if __name__ == "__main__":
    _main()
