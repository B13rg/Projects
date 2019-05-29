from django.shortcuts import render
from django.http import HttpRequest, HttpResponse
from ingest.models import Match,Player,Performance,Round,Scoreboard,generateMatch
import json

# Create your views here.
def index(request):
    if request.method == "POST":
        #Decode POST data into json to store in db
        data =  json.loads(request.body.decode('utf-8'))
        summary = generateMatch(data)
        print(str(summary.timestamp) + "\t" + summary.map_name + "\t" + summary.mode + "\t" + str(summary.version))
        print("CT: " + str(summary.score_ct) + "\tT: " + str(summary.score_t))        
    return HttpResponse()
