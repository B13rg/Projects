import praw

source = 'murica'
destination = 'muricaspeaks'

reddit = praw.Reddit(
    client_id='',
    client_secret='',
    username='',
    password='',
    user_agent='script:repost from one sub to another:v0.1:written by doug89')

for submission in reddit.subreddit(source).stream.submissions():
    if not submission.is_self:
        reddit.subreddit(destination).submit(submission.title, url=submission.url)