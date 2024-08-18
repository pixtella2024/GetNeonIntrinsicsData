# -*- coding: utf-8 -*-

import argparse
import re
import urllib.robotparser
import urllib.request
import urllib.error
import sys


def parse_url(url: str) -> int:
    result_match_url = re.match(r"(?P<root>https?://.*?)\/.*", url)
    if result_match_url == None:
        return -1
    root_url = result_match_url.group("root")
    # print(f"root_url: {root_url}")

    robots_txt_url: str
    try:
        f_root_url = urllib.request.urlopen(root_url)
        robots_txt_url = root_url + "/robots.txt"
        f_root_url.close()
    except:
        return -1

    rp = urllib.robotparser.RobotFileParser()
    rp.set_url(robots_txt_url)
    rp.read()

    user_agent = "*"
    result_can_fetch = rp.can_fetch(user_agent, url)

    if not result_can_fetch:
        return 0

    result_crawl_delay = rp.crawl_delay(user_agent)

    crawl_delay_sec: int
    if result_crawl_delay is not None:
        crawl_delay_sec = int(result_crawl_delay)
    else:
        crawl_delay_sec = 1

    if crawl_delay_sec < 1:
        crawl_delay_sec = 1

    return crawl_delay_sec


def main():
    parser = argparse.ArgumentParser(description="robots.txt のパーサー")

    parser.add_argument("url", help="パース対象の URL")

    args = parser.parse_args()

    url = args.url

    # print(url)

    result = parse_url(url)
    # print(str(result))

    sys.exit(result)


if __name__ == "__main__":
    main()

