"""
An empty bot to be able to start a new bot fast.
Written by /u/SmBe19
Modified by /u/Leftw
"""

import praw
import time
import settings
import logging.handlers
import logging

# ### USER CONFIGURATION ### #

# The time in seconds the bot should sleep until it checks again.
SLEEP = 60

# ### END USER CONFIGURATION ### #

# ### LOGGING CONFIGURATION ### #
LOG_LEVEL = logging.INFO
LOG_FILENAME = "bot.log"
LOG_FILE_BACKUPCOUNT = 5
LOG_FILE_MAXSIZE = 1024 * 256
# ### END LOGGING CONFIGURATION ### #

# ### LOGGING SETUP ### #
log = logging.getLogger("bot")
log.setLevel(LOG_LEVEL)
log_formatter = logging.Formatter('%(levelname)s: %(message)s')
log_formatter_file = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
log_stderrHandler = logging.StreamHandler()
log_stderrHandler.setFormatter(log_formatter)
log.addHandler(log_stderrHandler)
if LOG_FILENAME is not None:
	log_fileHandler = logging.handlers.RotatingFileHandler(LOG_FILENAME, maxBytes=LOG_FILE_MAXSIZE, backupCount=LOG_FILE_BACKUPCOUNT)
	log_fileHandler.setFormatter(log_formatter_file)
	log.addHandler(log_fileHandler)
# ### END LOGGING SETUP ### #


# ### MAIN PROCEDURE ### #
def run_bot():
	reddit = praw.Reddit(user_agent=settings.REDDIT_USER_AGENT,
	                     client_id=settings.REDDIT_CLIENT_ID,
						 client_secret=settings.REDDIT_CLIENT_SECRET,
						 username=settings.REDDIT_USERNAME,
	                     password=settings.REDDIT_PASSWORD)

	log.info("Start bot for subreddit %s", settings.REDDIT_SUBREDDIT)

	while True:
		try:
			pass

		# Allows the bot to exit on ^C, all other exceptions are ignored
		except KeyboardInterrupt:
			break
		except Exception as e:
			log.error("Exception %s", e, exc_info=True)
		log.info("sleep for %s s", SLEEP)
		time.sleep(SLEEP)
# ### END MAIN PROCEDURE ### #

# ### START BOT ### #
if __name__ == "__main__":
	if not settings.REDDIT_USER_AGENT:
		log.error("missing useragent")
	elif not settings.REDDIT_SUBREDDIT:
		log.error("missing subreddit")
	else:
		run_bot()
# ### END START BOT ### #
