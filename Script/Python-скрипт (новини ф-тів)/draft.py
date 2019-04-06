import datetime
import re

today = datetime.datetime.today()
print(str(today))
print(type(today.month))
link = re.search(
    r"""<a class='read-more' href='(.*?)'>""",
    "article_raw").groups()[0]
print(link)
