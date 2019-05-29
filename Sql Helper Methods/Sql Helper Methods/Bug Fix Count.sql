Select COUNT(*) from bugSubmission where bugID in (Select BugID from bug where Submitter=34);

