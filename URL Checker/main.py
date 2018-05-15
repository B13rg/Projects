#!/usr/bin/python3
import requests
import sys
filename="URList.txt"
if sys.argv[1] == "-s":
    filename=sys.argv[2]
if sys.argv[1] == "-h"
    print("Give this a list of url's, and the one that are")
    print("online will be outputed to Stdout")
    print("-Default input filename: URList.txt")
    print("-Custome file: -s <filename>")
items=list()
text_file = open(filename,"r")
for line in text_file.readlines():
    request=requests.get(line)
    if request.status_code%100==2:
        print(line)