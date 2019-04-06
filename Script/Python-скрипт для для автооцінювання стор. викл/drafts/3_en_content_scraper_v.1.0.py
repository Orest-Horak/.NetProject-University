# Програма призначена для автоматизованого веб-скрапінгу
# англомовного контенту сторінок викладачів ЛНУ (лише з новим дизайном) і отримання
# інформації з його англомовної сторінки
# (вмісту тега <main></main>, у якому міститься вся інфа про викладача)


import requests
import re
import os
import time


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename, encoding='utf-8') as f:
        for line in f:
            data = line.strip().split("\t")
            name, link = data[0], data[1]
            links.append((name, link))
    ###print(links)
    return links


def get_en_link_and_main(ua_link):
    """Extracts link to the English page. Calls get_main()"""
    max_time = 60
    res = requests.get(ua_link, timeout=max_time)
    if res.status_code == 200:
        try:
            en_link = re.search(
                r"""<link rel="alternate" hreflang="en-US" href="(.*?)" />""",
                res.text.replace("\n", "")).groups()[0]
            return (res.status_code, en_link) + get_main(en_link)
        except AttributeError:  # no match -> None -> no groups
            en_link = None
            return (res.status_code, en_link, None, None)
    return (res.status_code, None, None, None)


def get_main(en_link):
    """Extracts all the content inside <main></main> tag."""
    max_time = 60
    res = requests.get(en_link, timeout=max_time)
    if res.status_code == 200:
        try:
            html_main = re.search(
                r"""<main class='content-area' role='main'>(.*)</main>""",
                res.text.replace("\n", "")).groups()[0]
        except AttributeError:  # no match -> None -> no groups
            html_main = None
        return (res.status_code, html_main)
    return (res.status_code, None)



def dump_html_main(name, html_main):
    """Creates new directory.
    Outputs a html fragment containing information about particular lecturer
    as a separate txt file in this directory

    :param name: text
    :param html_main: text
    :return: None

    """
    with open(name + ".txt", 'wt', encoding="utf-8") as f:
        try:
            f.write(html_main)
        except TypeError or UnicodeEncodeError as e:  # None encountered or encode error
            print(e)
            f.write("")


def _main():
    # filename = input("Введіть назву файлу з посиланнями на сторінки викладачів (з розширенням):\n")
    filename = "link_list_reserved (selected staff).txt"
    # файл містить список - ім'я та лінк, розділені \t
    links = extract_links(filename)

    dirname = "dump_html_main_en"
    if not os.path.exists('./' + dirname):
        os.makedirs('./' + dirname)
    os.chdir('./' + dirname)

    dump = []
    dump_headers = "\t".join([
        'ПІБ ВИКЛАДАЧА',
        'ПОСИЛАННЯ НА СТОРІНКУ (UA)',
        'чи вдалося зайти на сторінку (UA)'
        'ПОСИЛАННЯ НА СТОРІНКУ (EN)'
        'чи вдалося зайти на сторінку (EN)'
    ])
    dump.append(dump_headers)
    print(dump_headers)

    for elem in links:
        name = elem[0]
        link = elem[1]
        # print(name, link)

        response = get_en_link_and_main(link)
        html_main = response[3]
        # print("###", response)

        existed = os.path.exists(name + ".txt")
        if existed:
            was_empty = os.path.getsize(name + ".txt") == 0
        if not existed or was_empty:
            dump_html_main(name, ("", html_main)[bool(html_main)])
            # if not existed:
            #     print("New file created. ", end="")
            # elif was_empty:
            #     print("Overwritten. ", end="")
            # print("Size:", os.path.getsize(name + ".txt"), "b.")
        dump_line = "\t".join((name, link) + tuple(map(str, response[0:3])))
        dump.append(dump_line)
        print(dump_line)

        filename = "en_content.txt"
        with open(filename, "wt", encoding="utf-8") as f:
            f.write("\n".join(dump))


if __name__ == "__main__":
    _main()
