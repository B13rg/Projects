# Projects

This is a repository to hold all the various projects I do.  Instead of giving each one a repository, I put the different projects I do in here.  It makes it easier to start projects and easier to share!

In no particular order...

## BBC Effects Library Website
### Description
This is a Django project meant to make downloading sound clips from the BBC Sound Effect archive much easier.  There are around 16,000 or 303(ish)GB of sound files, randing from engine noises to audience sounds at a concert.  The current site located at: [http://bbcsfx.acropolis.org.uk/](http://bbcsfx.acropolis.org.uk/) and only features a simple search function which isn't that useful.
### Goals
* Create a simple two page website: home and search results
* Allow searching through Titles, Categories, and CD names
* Allow user to select multiple files and download them all at once

## Reddit Bots
### Description
This project contains various examples of Reddit bots.  Using these you can easily figure out how to build one for a specific purpose.

## URL Checker
### Description
This will be a script that will comb through a list of URL's and compile information about them.  Currently, it only (kind of) checks if it is online.
### Goals
* Grab more information about websites
  * Whois data
  * Protocol versions
  * Screenshot or PDF of the website in question

## Youtube Watched Autoremover
### Description
The purpose of this tool is to remove videos from a youtube "Watch Later" playlist.  Videos can be added very easily, but once they are watched they remain in the list until you remove them.  This would automaticlly run every X minutes or hours and clear out the playlist of watched videos.  That way, the list won't contain videos you've already seen.
### Goals
* Work.
* Choose wether to delete videos or add them to another playlist

## Chip 8 Emulator
### Description
This is an emulator for the chip 8.  It may or may not work, I'm still working on it.  I used this website ([http://www.multigesture.net/articles/how-to-write-an-emulator-chip-8-interpreter/](http://www.multigesture.net/articles/how-to-write-an-emulator-chip-8-interpreter/)) as a guide.  This has the graphics semi-setup, and can be used for other emulator or simple graphic programs.
### Goals
 * Get it working
 * Add Super Chip-8 Opcodes
 * Use functions instead of a switch statement

## Method Timing Graph
### Description
This was a simple example to learn how to make graphs using python and MatPlotLib.  I also wanted to make this type of graph, and decided to try and program a graph instead of using excel.  I was testing the performance speeds between the old and new versions of a method.  The old version executed instructions one by one while the new method uses threads to spped things up.  I collected timings data from testing, and wanted to display it a certain way.

The x values are the old method times while the y values are the new method times.  I plotted it this was because then it would be easier to see which function is usually faster.  The line going across splits the graph into two areas: one where the old method was faster and one where the new method was faster.  It is easy to see at a glance which method was faster out of the 500 trials.  There usually wouldn't be a relationship between the times, but each time the pair of methods were run they used the same source data, and depending on the data amount it would take shorter or longer.

## 6502 Emulator
### Description
This is a emulator for the 6502 written in C#.  Currently, all the opcodes should be complete. More testing and work still needs to take place though.

## Blockchain
### Description
This is an example blockchain I made using C# and an Amazon Dynamo DB.  For simplicity, each block is stored as a row in the DB.  I did this project to better understand how blockchains work.  There is no "work" cost to create a block because I originally planned to use this project to monitor twitter accounts.  Whenever a tweet was made, it would save it in the blockchain.  That way if the user edited or deleted the tweet, we'd still have the original text.  This could of cource be accomplished by a database, but blockchains are "In" right now.

## Blockviewer
### Description
This allows you to view blockchain transactions in real time.  It colors the transactions based on the type and amount.