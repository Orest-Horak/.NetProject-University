

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



def dump_html_main(name, link, html_main):
    """Creates new directory.
    Outputs a html fragment containing information about particular lecturer
    as a separate txt file in this directory

    :param name: text
    :param html_main: text
    :return: None

    """
    faculty_acronym = re.search(r"""http://(.*?).lnu.edu.ua""", link).groups()[0]
    filename = faculty_acronym + ' ' + name + ".txt"
    with open(filename, 'wt', encoding="utf-8") as f:
        try:
            f.write(html_main)
        except TypeError or UnicodeEncodeError as e:  # None encountered or encode error
            print(e)
            f.write("")


def _main():
    # filename = input("Введіть назву файлу з посиланнями на сторінки викладачів (з розширенням):\n")
    filename = "link_list (newly selected staff).txt"
    # файл містить список - ім'я та лінк, розділені \t
    links = extract_links(filename)

    dirname = "dump_html_main_en"
    if not os.path.exists('./' + dirname):
        os.makedirs('./' + dirname)
    os.chdir('./' + dirname)

    for elem in links:
        name = elem[0]
        link = elem[1]
        # print(name, link)

        response = get_en_link_and_main(link)
        html_main = response[3]
        # print("###", response)

        for elem in links:
            name = elem[0]
            link = elem[1]
            print(name, link)
            old_filename = name + ".txt"
            faculty_acronym = re.search(r"""http://(.*?).lnu.edu.ua""", link).groups()[0]
            new_filename = faculty_acronym + ' ' + name + ".txt"
            existed = (os.path.exists(old_filename) or os.path.exists(new_filename))
            if os.path.exists(old_filename):
                os.rename(old_filename, new_filename)
                print(new_filename)
            if os.path.exists(new_filename):
                pass
            if not existed:
                response = get_en_link_and_main(link)
                html_main = response[3]
                dump_html_main(name, link, html_main)
                if not existed:
                    print("New file created. ", end="")
                elif was_empty:
                    print("Overwritten. ", end="")
                print("Size:", os.path.getsize(new_filename), "b.")


if __name__ == "__main__":
    _main()
