import praw
import sys
import re
from halo import Halo
from config import *


class Colour:
    Green, Red, White, Yellow = '\033[92m', '\033[91m', '\033[0m', '\033[93m'


print(Colour.Yellow + """
╦═╗╔═╗╔╦╗╔═╗╦  ╦╔═╗╔╦╗╔╦╗╦╔╦╗
╠╦╝║╣ ║║║║ ║╚╗╔╝║╣  ║║ ║║║ ║
╩╚═╚═╝╩ ╩╚═╝ ╚╝ ╚═╝═╩╝═╩╝╩ ╩
""")
print(Colour.White + 'Press Ctrl + C to Exit\n')

reddit = praw.Reddit(user_agent=user_agent,
                     client_id=client_id, client_secret=client_secret,
                     username=reddit_user, password=reddit_pass)

try:
    spinner = Halo(text='Running', spinner='dots')
    spinner.start()
    for comment in reddit.subreddit(target_sub).stream.comments():
        if target_keyword in comment.body:
            splits = re.split('(\W)', comment.permalink)
            del splits[-3:]
            msg = 'https://removeddit.com' + ''.join(splits)
            comment.reply(msg)
except Exception as e:
    spinner.stop()
    print(Colour.Red + str(e))
except KeyboardInterrupt:
    spinner.stop()
    sys.exit()