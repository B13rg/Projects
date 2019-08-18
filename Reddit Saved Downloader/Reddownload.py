#import imgur_downloader
import ComVars
import praw
import os
import urllib.request
import time
import sqlite3

from praw.models.reddit.submission import Submission
from praw.models.reddit.comment import Comment

DIRECTORY="./Reddit Saved Downloader/"
DATABASE="./reddit.sqlite"

def ConnectToReddit():
	reddit = praw.Reddit(user_agent=ComVars.user_agent,
								client_id=ComVars.client_id,
								client_secret=ComVars.client_secret,
								username=ComVars.username,
								password=ComVars.password)

	print(reddit.user.me())

	return reddit.redditor(ComVars.username)

def ConnectToDatabase():
	db = sqlite3.connect(DATABASE)
	cursor = db.cursor()
	cursor.execute("PRAGMA table_info(saved)")
	if not cursor.fetchone():
		# Create table
		cursor.execute('''
			CREATE TABLE saved(permalink TEXT PRIMARY KEY, title TEXT, url TEXT, subreddit TEXT, nsfw INTEGER)
		''')
		db.commit()
	return db

def CheckPostDB(db, post):
	cursor = db.cursor()
	cursor.execute('''SELECT COUNT(*) FROM saved WHERE permalink=?''', (post.permalink,))
	result = cursor.fetchone()
	print("Check result: " + str(result))
	return result[0] != 0

def AddPostDB(db, post):
	cursor = db.cursor()
	cursor.execuste('''
		INSERT INTO saved(permalink, title, url, subreddit, nsfw)
		VALUES(?, ?, ?, ?, ?)''', (post.permalink, post.title, post.url, post.subreddit, post.over_18))
	db.commit()	

dbConn=ConnectToDatabase()
redditor = ConnectToReddit()
print("Connected to account")
for post in redditor.saved(limit=None):
	#Skip comments for now
	if type(post) is Comment:
		continue
	print("Checking next post")
	# Check if post has already been downloaded
	if CheckPostDB(dbConn, post):
		continue
	# Lets download it
	sub = post.subreddit.display_name
	try:
		print("Fetching: "+post.title+ " from "+post.url)
		newpath = DIRECTORY
		#Move nsfw into their own directory
		if post.over_18:
			newpath = newpath + 'nsfw/'
		else:
			newpath = newpath + 'subreddits/'
		newpath = newpath+sub
		newfilename = newpath+"/"+post.title+post.url.split("/")[-1].split("?")[0]
		if not os.path.exists(newpath):
			os.makedirs(newpath)
		elif os.path.exists(newfilename):
			continue
		urllib.request.urlretrieve(post.url, newfilename)
		AddPostDB(dbConn,post)
		print("Complete")
	except:
		continue
print("Download Complete")