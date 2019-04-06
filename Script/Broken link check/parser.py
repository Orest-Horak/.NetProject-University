# Performs parsing of Xenu HTM report
# Output: tab separated file

import re
import time
import datetime


class BrokenLink:

    def __init__(self):
        self.url = "N/A"
        self.error_code = "N/A"
        self.error = "N/A"
        self.linked_from_pages = []




filename_r = input("Введіть назву htm-звіту Xenu (без розширення)\n") + ".htm"
filename_w = "parsed " + filename_r + str(datetime.datetime.today()).replace(":", ".") + ".txt"

start_time = time.clock()

with open(filename_r, "rt") as r:
    with open(filename_w, "wt") as w:

        w.write('\t'.join([
            'Broken link URL',
            'Error code',
            'Error',
            'Linked from page(s)',
            '\n',
        ]))  # enter captions

        ready = False
        # not ready to parse
        # need to skip some lines
        for line in r:
            # print(line, end='') ###
            if line.startswith("<center>"):
                ready = True
                # we are about to enter the list of broken links descriptions
                # the next line will be the first broken link entry
            elif line.startswith("<pre>empty URL"):
                broken_link = BrokenLink()
                broken_link.url = "empty URL"
            elif line.startswith("<a href"):
                if ready:
                    # lines starting with this pattern
                    # can also be found at the beginning of the file
                    # we are not interested in them
                    broken_link = BrokenLink()
                    broken_link.url = re.search(r'''<a href="(.*?)"''', line).groups()[0]
            elif line.startswith("error code"):
                broken_link.error_code, broken_link.error = re.search(
                    r'''error code: (\d*?) \((.*?)\), linked from page\(s\):''',
                    line).groups()
            elif line.startswith("\t<a href"):
                broken_link.linked_from_pages.append(
                    re.search(r'''\t<a href="(.*?)"''', line).groups()[0]
                )
            elif line.startswith("\n"):
                if ready:
                    # if we are parsing now, not just skipping lines
                    # broken link description finished
                    # dump time
                    for linked_from_page in broken_link.linked_from_pages:
                        w.write('\t'.join([
                            broken_link.url,
                            broken_link.error_code,
                            broken_link.error,
                            linked_from_page,
                            '\n',
                        ]))
                    # delete the object to clear memory
                    # the file is too long
                    del broken_link  # actually it doesn't reduce the time of parsing
            elif re.search(r'''</pre>\d*? broken link\(s\) reported''', line):
                print(line, end='')
                break
                # no more broken link entries
            else:
                pass

timedelta = time.clock() - start_time
print("Parsing has been completed in {:<2.4} seconds".format(str(timedelta)))
