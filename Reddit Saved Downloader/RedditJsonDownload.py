import argparse
import json
import logging
import os
import re
import sqlite3
import urllib.parse as parse
import urllib.request as request

import ffmpeg

DIRECTORY = "./Reddit/"
DATABASE = "./reddit.sqlite"
SOURCEFILE = "./feedList.txt"
LOGDIR = "./reddit.log"
DB = None

User_Agent = 'Mozilla/5.0 (Windows NT 6.1; Win64; x64)'
Headers = {'User-Agent': User_Agent}

Image_Extensions = {'gif', 'jpeg', 'jpg', 'png', 'tiff'}



def configureLogging():
    logging.basicConfig(
        filename=LOGDIR,
        format='%(asctime)s %(levelname)-8s %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S',
        level=logging.INFO)


def parseArguments():
    """Setup argument parsing"""

    # Make our global
    global DIRECTORY, DATABASE, SOURCEFILE, LOGDIR, DB
    parser = argparse.ArgumentParser(
        description='Downloads images, videos, and text from reddit json feeds.')
    parser.add_argument(
        '-d', '--directory', help="Directory where posts should be stored.  Default: "+str(DIRECTORY))
    parser.add_argument(
        '-l', '--log', help="Path and filename where the logfile is stored. Default: "+str(LOGDIR))
    parser.add_argument(
        '-s', '--source', help="File where a list of json feeds to download is stored. Default: "+str(SOURCEFILE))
    parser.add_argument('-b', '--database', type=int,
                        help="Path and filename of the sqlite file that saves state.  Default: "+str(DATABASE))
    arguments = parser.parse_args()
    if arguments.log != None:
        LOGDIR = arguments.log
    # Have to configure the logging before we begin logging
    configureLogging()
    # Display arguments passed in for debugging
    logging.info("Received the following arguments: "+str(arguments))

    # Apply our arguments to variables
    if arguments.directory != None:
        DIRECTORY = arguments.directory
    if arguments.source != None:
        SOURCEFILE = arguments.source
    if arguments.database != None:
        DATABASE = arguments.database


def ConnectToDatabase():
    db = sqlite3.connect(DATABASE)
    logging.info("Connected to database "+DATABASE)
    cursor = db.cursor()
    cursor.execute("PRAGMA table_info(saved)")
    if not cursor.fetchone():
        # Create table
        cursor.execute('''
			CREATE TABLE saved(permalink TEXT PRIMARY KEY, title TEXT, url TEXT, subreddit TEXT, nsfw INTEGER)
		''')
        db.commit()
        logging.info("Created table 'saved'")
    return db


def AddPostDB(post):
    cursor = DB.cursor()
    nsfw = 0
    if post['over_18']:
        nsfw = 1
    cursor.execute('''
		INSERT INTO saved(permalink, title, url, subreddit, nsfw)
		VALUES(?, ?, ?, ?, ?)''',
            (post['permalink'], str(post['title']), 
            str(post['url']), str(post['subreddit']), int(nsfw))
    )
    DB.commit()
    logging.info("Added post to database: "+ post['title'])

def AddCommentDB(post):
    nsfw = 0
    if 'permalink' in post:
        key = post['permalink']
    elif 'link_permalink' in  post:
        key = post['list_permalink']
    cursor = DB.cursor()
    cursor.execute('''
		INSERT INTO saved(permalink, title, url, subreddit, nsfw)
		VALUES(?, ?, ?, ?, ?)''',
            (key, str(post['link_title']), 
            str(post['link_url']), str(post['subreddit']), nsfw)
    )
    DB.commit()
    logging.info("Added comment to database: "+ post['link_id'])


def CheckPostDB(post):
    if 'permalink' in post:
        key = post['permalink']
    elif 'link_permalink' in  post:
        key = post['list_permalink']
    cursor = DB.cursor()
    cursor.execute('''SELECT COUNT(*) FROM saved WHERE permalink=?''', (key,))
    result = cursor.fetchone()
    return result[0] != 0


def GetPostFilename(path, filename, extension):
    # Remove illegal characters
    return path + re.sub(r"<|>|\||:|\"|/|\?|\*|\\", " ", filename) + "." + extension


def downloadImage(path, data):
    title = data['title']
    fileLoc = GetPostFilename(path, title, getExtension(data['url']))
    req = request.Request(data['url'])
    req.add_header('User-Agent', User_Agent)
    with request.urlopen(req) as image:
        with open(fileLoc, 'wb') as f:
            f.write(image.read())
            logging.info("Downloaded image from "+ data['url'])
    AddPostDB(data)


def getExtension(url):
    fileParts = parse.urlparse(url).path.split('.')
    if len(fileParts) == 1:
        return ''
    return fileParts[-1]


def hasImageExtension(url):
    return getExtension in Image_Extensions


def downloadComment(path, data):
    title = data['link_title']
    fileLoc = GetPostFilename(path, title, "txt")
    with open(fileLoc, 'w') as f:
        f.write(
            data['link_title'] + "\n"
            + data ['link_permalink'] + "\n\n"
            + data['body']
        )
    logging.info("Downloaded text from "+ data['link_url'])
    AddCommentDB(data)

def downloadSelfText(path, data):
    title = data['title']
    fileLoc = GetPostFilename(path, title, "txt")
    with open(fileLoc, 'w') as f:
        f.write(
            data['title'] + "\n"
            + data ['permalink'] + "\n\n"
            + data['selftext']
        )
    logging.info("Downloaded selftext from "+ data['permalink'])
    AddPostDB(data)



def downloadRedditVideo(path, data):
    media_data = data["media"]
    title = data["title"]
    videoURL = media_data["reddit_video"]["fallback_url"]
    audioURL = videoURL.split("DASH_")[0] + "audio"
    logging.info("Downloading video and audio for " + title)
    os.system("curl -s -o video.mp4 {}".format(videoURL))
    os.system("curl -s -o audio.wav {}".format(audioURL))

    videoPath = GetPostFilename(path, title, ".mp4")
    logging.info("Compiling video and audio for " + title)
    video = ffmpeg.input('video.mp4').video
    audio = ffmpeg.input('audio.wav').audio
    (
        ffmpeg
        .output(video, audio, videoPath, vcodec='copy', strict='-2')
        .overwrite_output()
        .run()
    )   
    os.remove("video.mp4")
    os.remove("audio.wav")


def downloadPage(path, data):
    NotImplemented


def t1Comment(path, data):
    path = path + "comments/" + data['subreddit'] + "/"
    # Create folders if this is the first post
    if not os.path.exists(path):
        os.makedirs(path)
    downloadComment(path, data)


def t2Account(path, data):
    NotImplemented


def t3Link(path, data):
    if data['over_18']:
        path = path + "nsfw/"
    path = path + data['subreddit'] + "/"
    # Create folders if this is the first post
    if not os.path.exists(path):
        os.makedirs(path)
    if hasImageExtension(data['url']):
        downloadImage(path, data)
        return
    elif data['is_video']:
        downloadRedditVideo(path, data)
        return
    elif data['is_self']:
        downloadSelfText(path, data)
        return
    elif not data['media']:
        return
    typeHint = data['post_hint']
    if typeHint == 'image':
        downloadImage(path, data)
    elif typeHint == 'rich:video':
        NotImplemented
    elif typeHint == 'link':
        NotImplemented


def t4Message(path, data):
    NotImplemented


def t5Subreddit(path, data):
    NotImplemented


def t6Award(path, data):
    NotImplemented


def downloadPost(rootpath, post):
    # Check if post has already been downloaded
    if CheckPostDB(post['data']):
        logging.info("Post has already been downloaded")
        return
    if post['kind'] == "t1":
        t1Comment(rootpath, post['data'])
    elif post['kind'] == "t2":
        t2Account(rootpath, post['data'])
    elif post['kind'] == "t3":
        t3Link(rootpath, post['data'])
    elif post['kind'] == "t4":
        t4Message(rootpath, post['data'])
    elif post['kind'] == "t5":
        t5Subreddit(rootpath, post['data'])
    elif post['kind'] == "t6":
        t6Award(rootpath, post['data'])


def fetchFeedJson(jsonURL):
    # First open the url
    feed = None
    req = request.Request(jsonURL)
    req.add_header('User-Agent', User_Agent)
    with request.urlopen(req) as url:
        feed = json.loads(url.read().decode())
    if feed == None:
        logging.error("Unable to decode feed: "+jsonURL)
    else:
        logging.info("Loaded feed "+ jsonURL)
    return feed


def downloadFeed(url):
    feed = fetchFeedJson(url)
    # first check for listing of posts
    if feed['kind'] != "Listing":
        logging.info("Downloaded feed doesn't have listing listing element")
        return None
    data = feed['data']
    # if there's another page, grab that too
    next_page = data['after']
    # grab our list of post objects
    listposts = data['children']
    # get username and build download path
    username = parse.parse_qs(parse.urlparse(url).query)['user'][0].strip()
    rootPath = DIRECTORY + username + "/"
    # go through each post and fetch stuff
    for post in listposts:
        downloadPost(rootPath, post)
    return next_page

if __name__ == '__main__':
    parseArguments()
    DB = ConnectToDatabase()
    if not os.path.exists("./tmp"):
        os.mkdir("./tmp")
    for url in open(SOURCEFILE, 'r'):
        urlClean = url.strip()
        nextPage = downloadFeed(urlClean)
        logging.info("Finished feed located at "+ urlClean)
        # Now go through each page of the feed
        while nextPage:
            newURL = urlClean + "&after=" + nextPage
            nextPage = downloadFeed(newURL)
            logging.info("Finished feed located at "+ newURL)
    os.rmdir("./tmp")
    logging.info("Finished; Exiting")
