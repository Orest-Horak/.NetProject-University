# Програма, що генерує список викладачів (з посиланнями)
# із набору посилань на факультетські списки персоналу
#
# у вихідному файлі містяться посилання на всіх співробітників факультетів
# Отриманий список можна відфільтрувати вручну:
#     скопіювати в Google Spreadsheets
#     відсортувати за посадою або кафедрою/лабораторією/іншим структурним підрозділом факультету
#     зробити умовне форматування за посадою або кафедрою/лабораторією/іншим структурним підрозділом факультету
#         (це не обов'язково, але полегшить процес фільтрування і допоможе уникнути помилок)
#     видалити посилання на сторінки співробітників, які не потрібно оцінювати


import requests
import re
import time


def extract_links(filename):
    """Extracts the links from the file."""
    links = []
    with open(filename) as f:
        for line in f:
            link = line.strip()
            links.append(link)
    return links


def get_main(link):
    """Extracts all the content inside <main></main> tag."""
    max_time = 60
    if re.match(r"""http""", link):
        time.sleep(2)  # to avoid overload
        # start_time = time.clock()
        res = requests.get(link, timeout=max_time)
        if res.status_code == 200:
            faculty_name_match_obj = re.search(
                r"""<h1 class="site-title">.*?<span>(<em>)?(.*?)(</em>)?</span> <span>(<em>)?(.*?)(</em>)?</span></a></h1>""",
                res.text.replace("\n", ""))
            # print(faculty_name_match_obj.groups(), faculty_name_match_obj.groups()[1])
            faculty_name = "".join([faculty_name_match_obj.groups()[1], " "
            + faculty_name_match_obj.groups()[4]])
            # print(faculty_name)
            # print(type(faculty_name))
            html_main = re.search(
                r"""<main class='content-area' role='main'>(.*)</main>""",
                res.text.replace("\n", "")).groups()[0]
            # timedelta = time.clock() - start_time
            # if timedelta >= max_time:
            #     print("timeout exceeded")
            # else:
            #     print("request processed in {:<2.4} seconds".format(str(timedelta)))
            return faculty_name, html_main
        return None, None


def _main():
    # filename = input(
    #     """Введіть назву файлу (з розширенням), що містить\nпосилання на на факультетські списки персоналу:\n""")
    filename = "faculties_link_list.txt"
    links = extract_links(filename)
    with open('link_list.txt', 'wt', encoding='utf-8') as f:
        f.write("\t".join(['ПІБ ВИКЛАДАЧА', 'ПОСИЛАННЯ НА СТОРІНКУ', 'ПОСАДА', 'ФАКУЛЬТЕТ', 'КАФЕДРА', '\n']))
        position_diversity = set()
        for link in links:
            # print(link) ###
            faculty_name, html_main = get_main(link)
            departments_raw = re.findall(
                r"""<section><h2>.*?</h2><div><table><tbody>.*?</tbody></table></div></section>""",
                html_main, re.MULTILINE)
            # print(len(departments_raw))
            # print(departments_raw)
            for department_raw in departments_raw:
                # print(department_raw)
                department_name = re.search(
                    r"""<h2>(<a href=.*?>)?(.*?)(</a>)?</h2>""",
                    department_raw, re.MULTILINE).groups()[1]
                # print(department_name)
                staff_list_raw = re.findall(
                    r"""<tr>.*?</tr>""",
                    department_raw, re.MULTILINE)
                for staff_member_raw in staff_list_raw:
                    link, name, position = re.search(
                        r"""<td class='name'><span>.*?</span><a href='(.*?)'>(.*?)</a></td><td class='position'>(.*?)</td>""",
                        staff_member_raw).groups()
                    position_diversity.add(position)
                    f.write("\t".join([name, link, position, faculty_name, department_name, '\n']))
                    print("\n".join([name, link, position, faculty_name, department_name, '-'*152]))

    print(sorted(position_diversity))
    print('\n'.join(sorted(position_diversity)))

    # асистент
    # асистент <span class='part'>(сумісник)</span>
    # асистент,  <span class='part'>(сумісник)</span>
    # асистент, асистент <span class='part'>(сумісник)</span>
    # асистент, аспірант <span class='part'>(сумісник)</span>
    # асистент, лаборант <span class='part'>(сумісник)</span>
    # асистент, старший лаборант <span class='part'>(сумісник)</span>
    # аспірант
    # аспірант, асистент <span class='part'>(сумісник)</span>
    # в.о. декана
    # в.о. завідувача кафедри
    # викладач
    # викладач <span class='part'>(сумісник)</span>
    # викладач, кандидат наук
    # головний науковий співробітник
    # головний науковий співробітник, професор <span class='part'>(сумісник)</span>
    # декан
    # директор
    # директор, кандидат наук
    # диспетчер
    # докторант
    # доцент
    # доцент <span class='part'>(сумісник)</span>
    # доцент <span class='part'>(сумісник)</span>, кандидат наук
    # доцент,  <span class='part'>(сумісник)</span>
    # доцент, в.о. завідувача кафедри <span class='part'>(сумісник)</span>
    # доцент, докторант <span class='part'>(сумісник)</span>
    # доцент, кандидат наук
    # доцент, старший науковий співробітник <span class='part'>(сумісник)</span>
    # завідувач
    # завідувач <span class='part'>(сумісник)</span>
    # завідувач кафедри
    # завідувач лабораторії
    # завідувач, професор <span class='part'>(сумісник)</span>
    # заступник декана
    # заступник директора
    # зберігач фондів
    # лаборант
    # лаборант <span class='part'>(сумісник)</span>
    # методист
    # методист 1 категорії
    # молодший науковий співробітник
    # молодший науковий співробітник <span class='part'>(сумісник)</span>
    # молодший науковий співробітник, асистент <span class='part'>(сумісник)</span>
    # науковий співробітник
    # провідний науковий співробітник
    # провідний науковий співробітник, доцент <span class='part'>(сумісник)</span>
    # провідний спеціаліст
    # провідний спеціаліст, асистент <span class='part'>(сумісник)</span>
    # провідний інженер
    # професор
    # професор <span class='part'>(сумісник)</span>
    # професор, доктор наук
    # секретар
    # спеціаліст
    # старший викладач
    # старший викладач <span class='part'>(сумісник)</span>
    # старший диспетчер
    # старший лаборант
    # старший лаборант <span class='part'>(сумісник)</span>
    # старший лаборант,  <span class='part'>(сумісник)</span>
    # старший лаборант, асистент <span class='part'>(сумісник)</span>
    # старший науковий співробітник
    # інженер
    # інженер 1 категорії
    # інженер 2 категорії
    # інженер 2 категорії <span class='part'>(сумісник)</span>
    # інженер <span class='part'>(сумісник)</span>
    # інспектор


if __name__ == "__main__":
    _main()