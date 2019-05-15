import imgur_downloader
import ComVars
import praw
import os
import urllib.request
import time

from praw.models.reddit.submission import Submission
from praw.models.reddit.comment import Comment

directory="./Reddit Saved Downloader/subreddits/"


reddit = praw.Reddit(user_agent=ComVars.user_agent,
                     client_id=ComVars.client_id,
                     client_secret=ComVars.client_secret,
                     username=ComVars.username,
                     password=ComVars.password)

print(reddit.user.me())

redditor = reddit.redditor(ComVars.username)

index=0
subreddits = {}
total = 0
#index = index + items._list_index

for post in redditor.saved(limit=None):
	if type(post) is Submission:
		title = post.title
		link = 'https://reddit.com' + post.permalink
		url = post.url

	elif type(post) is Comment:
		body = post.body
		continue

	sub = post.subreddit.display_name

	if sub not in subreddits:
		subreddits[sub] = []
	subreddits[sub].append({'title': title, 'url': url, 'link': link})
	total = total + 1

print("Have "+str(total)+" items")

for sub in subreddits:	
	for item in subreddits[sub]:
		if item['url'][-4] != "." and item['url'][-5] != ".":
			continue 
		try:
			newpath = directory+sub
			newfilename = newpath+"/"+item['title']+item['url'][-5:]
			if not os.path.exists(newpath):
				os.makedirs(newpath)
			elif os.path.exists(newfilename):
				continue
			urllib.request.urlretrieve(item['url'], newfilename)
		except:
			continue