# у версії .1 файли зберігаються під іменами, які містять інформацію про факультет
# Деякі викладачі працюють і на факультеті, і в коледжі.
# Тому необхідно зберегти дані з обох сторінок - і на сайті ф-ту, і на сайті коледжу


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
            if name[0] == '\ufeff':
                name = name[1:]
            links.append((name, link))
    return links


def get_main(link):
    """Extracts all the content inside <main></main> tag."""
    max_time = 60
    if re.match(r"""http""", link):
        time.sleep(2)  # to avoid overload
        try:
            res = requests.get(link, timeout=max_time)
            # print(res.text)

            # start_time = time.clock()
            # print('|||||||', res.text.replace("\n", ""))

            html_main = re.search(r"""<main class='content-area' role='main'>(.*)</main>""", res.text.replace("\n", "")).groups()[0]
            # html_main = re.search(r"""<main class='content-area' role='main'>(.*)</main>""", html_main.replace("\t", "    ")).groups()[0]
            # tabs are to be removed because they will serve as separators in the output

            # timedelta = time.clock() - start_time
            # if timedelta >= max_time:
            #     print("timeout exceeded")
            # else:
            #     print("request processed in {:<2.4} seconds".format(str(timedelta)))

        except Exception as e:
            # print('$$$$$', e)
            # sometimes the response doesn't have html or connection cannot be established
            return
        return html_main
    return


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

    dirname = "dump_html_main"
    if not os.path.exists('./' + dirname):
        os.makedirs('./' + dirname)
    os.chdir('./' + dirname)

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
            html_main = get_main(link)
            # print("###", html_main)
            dump_html_main(name, link, html_main)
            if not existed:
                print("New file created. ", end="")
            elif was_empty:
                print("Overwritten. ", end="")
            print("Size:", os.path.getsize(new_filename), "b.")


if __name__ == "__main__":
    _main()
