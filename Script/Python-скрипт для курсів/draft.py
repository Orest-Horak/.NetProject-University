import re


faculty_acronym, course_name = re.search(
    r"""http://(.*?).lnu.edu.ua/course/(.*)""",
    "http://philology.lnu.edu.ua/course/zhanrovi-ta-stylovi-osoblyvosti-suchasnoji-rosijskoji-literatury").groups()

print(re.search(
    r"""http://(.*?).lnu.edu.ua/course/(.*)""",
    "http://philology.lnu.edu.ua/course/zhanrovi-ta-stylovi-osoblyvosti-suchasnoji-rosijskoji-literatury").groups())

print(faculty_acronym, course_name)