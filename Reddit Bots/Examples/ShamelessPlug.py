import praw

USERNAME = ''
PASSWORD = ''
CLIENT_ID = ''
CLIENT_SECRET = ''

YOUR_SUBREDDIT = ''
TARGET_SUBREDDIT = 'all'

USER_AGENT = 'script:plugs subreddits by common urls:v0.3:written by /u/doug89'

print("Authenticating...")
reddit = praw.Reddit(
    client_id=CLIENT_ID,
    client_secret=CLIENT_SECRET,
    password=PASSWORD,
    user_agent=USER_AGENT,
    username=USERNAME)
print("Authenticaed as {}".format(reddit.user.me()))

while True:
    hot_posts = reddit.subreddit(YOUR_SUBREDDIT).hot()
    links = [post.url for post in hot_posts if not post.is_self]

    for submission in reddit.subreddit(TARGET_SUBREDDIT).new(limit=None):
        same_sub = str(submission.subreddit).lower() == YOUR_SUBREDDIT.lower()
        if submission.saved or same_sub:
            continue
        text = submission.selftext
        has_url = any(url.lower() in text.lower() for url in links)
        if submission.url in links or has_url:
            submission.save()
            reply = submission.reply('/r/{}'.format(YOUR_SUBREDDIT))
            print('http://reddit.com{}'.format(reply.permalink()))

    for comment in reddit.subreddit(TARGET_SUBREDDIT).comments(limit=None):
        same_sub = str(comment.subreddit).lower() == YOUR_SUBREDDIT.lower()
        if comment.saved or same_sub:
            continue
        text = comment.body
        has_url = any(url.lower() in text.lower() for url in links)
        if has_url:
            comment.save()
            reply = comment.reply('/r/{}'.format(YOUR_SUBREDDIT))
            print('http://reddit.com{}'.format(reply.permalink()))