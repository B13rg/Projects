#gets a list of subs that don't liek robots
bottiquette = r.get_wiki_page('Bottiquette', 'robots_txt_json')
# see https://www.reddit.com/r/Bottiquette/wiki/robots_txt_json
restricted_subreddits = loads(bottiquette.content_md)
restricted_subreddits = restricted_subreddits["disallowed"] + \
                        restricted_subreddits["posts-only"] + \
                        restricted_subreddits["permission"]