# Програма призначена для автоматизованого веб-скрапінгу
# сторінок викладачів ЛНУ (лише з новим дизайном) і отримання
# інформації, що необхідна для оцінювання сторінок
# (вмісту тега <main></main>, у якому міститься вся інфа про викладача)
# наступний етап - екстракція потрібної інформації із html-фрагмента -
# здійснюється іншою програмою


import requests
import re
import os
import time


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename) as f:
        for line in f:
            name, link = line.strip().split("\t")
            links.append((name, link))
    return links


def get_main(link):
    """Extracts all the content inside <main></main> tag."""
    max_time = 60
    if re.match(r"""http""", link):
        time.sleep(2) # to avoid overload
        res = requests.get(link, timeout=max_time)
        # print(res.text)
        try:
            # start_time = time.clock()

            html_main = re.search(r"""<main class='content-area' role='main'>(.*)</main>""", res.text.replace("\n", "")).groups()[0]
            html_main = re.search(r"""<main class='content-area' role='main'>(.*)</main>""", res.text.replace("\t", "    ")).groups()[0]
            # tabs are to be removed because they will serve as separators in the output

            # timedelta = time.clock() - start_time
            # if timedelta >= max_time:
            #     print("timeout exceeded")
            # else:
            #     print("request processed in {:<2.4} seconds".format(str(timedelta)))

        except AttributeError: # sometimes the response doesn't have html
            return
        return html_main
    return


def dump_html_main(name, html_main):
    """Creates new directory.
    Outputs a html fragment containing information about particular lecturer
    as a separate txt file in this directory

    :param html_main: text
    :return: None

    """
    with open(name + ".txt", 'wt', encoding="utf8") as f:
        try:
            f.write(html_main)
        except TypeError or UnicodeEncodeError: # None encountered or encode error
            f.write("")


def _main():
    filename = input("Введіть назву файлу з посиланнями на сторінки викладачів (з розширенням):\n")
    # filename = "test_link_set.txt"
    # файл містить список - ім'я та лінк, розділені \t
    links = extract_links(filename)

    dirname = "dump_html_main"
    if not os.path.exists('./' + dirname):
        os.makedirs('./' + dirname)
    os.chdir('./' + dirname)

    for elem in links:
        name = elem[0]
        link = elem[1]
        print(name, link)
        html_main = get_main(link)
        dump_html_main(name, html_main)


if __name__ == "__main__":
    _main()