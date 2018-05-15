import praw

bot = praw.Reddit(...)

subreddit = bot.subreddit('firstdoge')
moderators = []

phrase = '!approve'
response = '+/u/sodogetip 5 doge'

def main():
    # Get fresh list of moderators
    for moderator in subreddit.moderator():
        moderators.append(moderator.name)

    while True:
        try:
            for comment in subreddit.stream.comments():
                # Reject if a top-level comment by the moderator
                # Accept if phrase '!approve' is in comment and comment author 
                # is a moderator of the subreddit and no other moderator has replied to it
                if (not comment.is_root and
                    phrase in comment.body.lower() and
                    comment.author.name in moderators):
                        parent = comment.parent()
                        parent.refresh()
                        for reply in parent.replies:
                            if (reply.author.name not in moderators and
                                phrase not in reply.body.lower()):

                                parent.reply(response)


        except Exception as e:
            print(e)
            moderators = []
            for moderator in subreddit.moderator():
                moderators.append(moderator.name)
            continue

if __name__ == '__name__':
    main()