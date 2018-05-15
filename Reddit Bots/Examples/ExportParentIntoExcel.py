import praw,sys

REDDIT_USERNAME = 'journalingbot' #YOUR USERNAME as string
REDDIT_PASS = '' # YOUR PASSWORD as string
REDDIT_CLIENT_ID = ''
REDDIT_CLIENT_SECRET = ''
user_agent = ("contest_automaton/v0.1 (by /u/Anatoly_Korenchkin)")

r = praw.Reddit(user_agent = user_agent,client_id=REDDIT_CLIENT_ID,client_secret=REDDIT_CLIENT_SECRET,username=REDDIT_USERNAME,password=REDDIT_PASS)

submission = r.submission(url=sys.argv[1])
commentlist= submission.comments.list()

root_comments=[]
[root_comments.append(comment) for comment in commentlist if comment.is_root]

save_file=open('Contest_'+submission.id+'.csv','w')
save_file.write('#,Name,Comment\n')

for count,comment in enumerate(root_comments):
    save_file.write(str(count+1)+','+str(comment.author)+','+str(comment.body)+'\n')