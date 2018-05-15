import praw

SUBREDDIT_NAME = 'requestabot'
KEYWORDS = ['hunter2', 'lizard']
RESPONSE = 'There is a keyword in your comment!'

USERNAME = ''
PASSWORD = ''
CLIENT_ID = ''
CLIENT_SECRET = ''

USER_AGENT = 'script:reply to keywords in comments:v0.2:written by /u/doug89'

print("Authenticating...")
reddit = praw.Reddit(
    client_id=CLIENT_ID,
    client_secret=CLIENT_SECRET,
    password=PASSWORD,
    user_agent=USER_AGENT,
    username=USERNAME)
print("Authenticaed as {}".format(reddit.user.me()))

print('Starting comment stream...')
for comment in reddit.subreddit(SUBREDDIT_NAME).stream.comments():
    if comment.saved:
        continue
    has_keyword = any(k.lower() in comment.body.lower() for k in KEYWORDS)
    not_self = comment.author != reddit.user.me()
    if has_keyword and not_self:
        comment.save()
        reply = comment.reply(RESPONSE)
        print('http://reddit.com{}'.format(reply.permalink()))